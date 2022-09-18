const googleSheets = require('./googleSheets');
const color = require("./color");

const studentsSubSheet = "Studenți";
const lectureSubSheetPrefix = "Cursul";
const homeworkSubSheetPrefix = "Temă";

const nameHeader = "Nume";
const varenaHeader = "Varena";
const iarenaHeader = "Infoarena";
const totalHeader = "Total";
const scoreHeader = "Scor";

const minScoreColor = color.red;
const maxScoreColor = color.green;

const auth = googleSheets.auth;
let sheets = null;

let spreadsheetId = null;
const init = async function (spreadsheet) {
  sheets = await googleSheets.instance();
  spreadsheetId = spreadsheet;
}

const fetchLectureSheets = async function () {
  return await googleSheets.fetchSubSheets(spreadsheetId, lectureSubSheetPrefix);
}

const fetchLectureStudents = async function (lectureSubSheet) {
  const getRows = await sheets.spreadsheets.values.get({
    auth,
    spreadsheetId,
    range: lectureSubSheet
  });
  const rows = getRows.data.values;
  const nameColumnIndex = rows[0].indexOf(nameHeader);

  let students = [];
  for (let i = 1; i < rows.length; ++i) {
    const name = rows[i][nameColumnIndex];
    if (name && name.length > 0) {
      students.push({
        name: name,
        rowIndex: i
      });
    }
  }

  return students;
}

const fetchLectureHomeworks = async function (lectureSubSheet) {
  const getHeaderRows = await sheets.spreadsheets.get({
    auth,
    spreadsheetId,
    includeGridData: true,
    ranges: `${lectureSubSheet}!1:2`
  });
  const headerRows = getHeaderRows.data.sheets[0].data[0].rowData;

  let homeworks = [];
  let homeworkColumn = googleSheets.findNextNonEmptyCell(headerRows[1].values, 0);
  while (homeworkColumn !== -1) {
    let nextHomeworkColumn = googleSheets.findNextNonEmptyCell(headerRows[0].values, homeworkColumn + 1);
    if (nextHomeworkColumn === -1)
      nextHomeworkColumn = headerRows[0].values.length;

    let homework = {};
    homework.name = headerRows[0].values[homeworkColumn].formattedValue;
    homework.tasks = [];
    homework.totalScoreColumnIndex = -1;
    for (; homeworkColumn < nextHomeworkColumn; ++homeworkColumn) {
      const cell = headerRows[1].values[homeworkColumn];
      if (googleSheets.hasCellHyperlink(cell)) {
        homework.tasks.push({
          URL: cell.hyperlink,
          columnIndex: homeworkColumn
        });
      } else if (!googleSheets.isCellNullOrEmpty(cell) && cell.formattedValue.includes(totalHeader)) {
        homework.totalScoreColumnIndex = homeworkColumn;
      }
    }
    homeworks.push(homework);

    homeworkColumn = googleSheets.findNextNonEmptyCell(headerRows[0].values, homeworkColumn);
  }

  return homeworks;
}

const fetchHomeworkSheets = async function () {
  return await googleSheets.fetchSubSheets(spreadsheetId, homeworkSubSheetPrefix);
}

const fetchLectureHomeworkResults = async function (lectureSubSheet, homeworkName) {
  const getRows = await sheets.spreadsheets.values.get({
    auth,
    spreadsheetId,
    range: lectureSubSheet
  });
  const rows = getRows.data.values;
  const nameColumnIndex = rows[0].indexOf(nameHeader);
  const homeworkColumnIndex = rows[0].findIndex(r => r && r.length > 0 && homeworkName.includes(r));

  let homeworkResults = {};
  homeworkResults.students = [];
  homeworkResults.maxScore = 0;

  if (homeworkColumnIndex !== -1) {
    const totalColumnIndex = rows[1].indexOf(totalHeader, homeworkColumnIndex);
    homeworkResults.maxScore = (totalColumnIndex - homeworkColumnIndex) * 100;

    for (let rowIndex = 2; rowIndex < rows.length; ++rowIndex) {
      homeworkResults.students.push({
        name: rows[rowIndex][nameColumnIndex],
        score: parseInt(rows[rowIndex][totalColumnIndex])
      });
    }
  }

  return homeworkResults;
}

const fetchStudentsInfoMap = async function () {
  const getRows = await sheets.spreadsheets.values.get({
    auth,
    spreadsheetId,
    range: studentsSubSheet
  });
  const rows = getRows.data.values;
  const nameColumnIndex = rows[0].indexOf(nameHeader);
  const varenaColumnIndex = rows[0].indexOf(varenaHeader);
  const iarenaColumnIndex = rows[0].indexOf(iarenaHeader);

  let studentsMap = {};
  for (let i = 1; i < rows.length; ++i)
    studentsMap[rows[i][nameColumnIndex]] = {
      varena: rows[i][varenaColumnIndex],
      iarena: rows[i][iarenaColumnIndex]
    };

  return studentsMap;
}

const getColor = function (percentage) {
  return color.lerp(minScoreColor, maxScoreColor, percentage);
}

const getUpdateHomeworkGridStart = function (students, subSheetId) {
  let rowIndex = students[0].rowIndex;
  let columnIndex = students[0].homeworks[0].tasks[0].columnIndex;
  for (const task of students[0].homeworks[0].tasks)
    columnIndex = Math.min(columnIndex, task.columnIndex);

  return {
    sheetId: subSheetId,
    columnIndex,
    rowIndex
  };
}

const getUpdateHomeworkRows = function (students) {
  let rows = [];
  for (const student of students) {
    let columns = [];
    for (const homework of student.homeworks) {
      for (const task of homework.tasks)
        columns.push({
          userEnteredValue: { formulaValue: `=HYPERLINK("${task.solutionsURL}", "${task.score}")` }
        });

      if (homework.totalScoreColumnIndex !== -1) {
        const maxScore = homework.tasks.length * 100;
        const percentage = homework.totalScore / maxScore;
        columns.push({
          userEnteredFormat: { backgroundColor: getColor(percentage) },
          userEnteredValue: { numberValue: homework.totalScore }
        });
      }
    }

    rows.push({ values: columns });
  }

  return rows;
}

const updateHomeworks = async function (subSheetId, students) {
  const requestBody = {
    requests: [
      {
        updateCells: {
          fields: "userEnteredValue,userEnteredFormat.backgroundColor",
          start: getUpdateHomeworkGridStart(students, subSheetId),
          rows: getUpdateHomeworkRows(students)
        }
      }
    ]
  };
  await sheets.spreadsheets.batchUpdate({
    spreadsheetId,
    requestBody
  });
}

const getUpdateHomeworkTotalGridStart = function (students, subSheetId) {
  let rowIndex = students[0].rowIndex;
  let columnIndex = 1;

  return {
    sheetId: subSheetId,
    columnIndex,
    rowIndex
  };
}

const getUpdateHomeworkTotalRows = function (students) {
  let rows = [];
  for (const student of students) {
    const percentage = student.totalScore / student.maxScore;

    let columns = [];
    columns.push({
      userEnteredValue: { numberValue: student.totalScore }
    });
    columns.push({
      userEnteredValue: { numberValue: student.maxScore }
    });
    columns.push({
      userEnteredFormat: { backgroundColor: getColor(percentage) },
      userEnteredValue: { numberValue: Math.round(percentage * 100) }
    });

    rows.push({ values: columns });
  }

  return rows;
}

const updateHomeworkTotal = async function (subSheetId, students) {
  const requestBody = {
    requests: [
      {
        updateCells: {
          fields: "userEnteredValue,userEnteredFormat.backgroundColor",
          start: getUpdateHomeworkTotalGridStart(students, subSheetId),
          rows: getUpdateHomeworkTotalRows(students)
        }
      }
    ]
  };
  await sheets.spreadsheets.batchUpdate({
    spreadsheetId,
    requestBody
  });
}

exports.init = init;

exports.fetchStudentsInfoMap = fetchStudentsInfoMap;

exports.fetchHomeworkSheets = fetchHomeworkSheets;
exports.fetchLectureSheets = fetchLectureSheets;
exports.fetchLectureStudents = fetchLectureStudents;
exports.fetchLectureHomeworks = fetchLectureHomeworks;
exports.fetchLectureHomeworkResults = fetchLectureHomeworkResults;

exports.updateHomeworks = updateHomeworks;
exports.updateHomeworkTotal = updateHomeworkTotal;