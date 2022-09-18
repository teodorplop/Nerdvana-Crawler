const { google } = require("googleapis");

const auth = new google.auth.GoogleAuth({
  keyFile: "credentials.json",
  scopes: "https://www.googleapis.com/auth/spreadsheets"
});

let client = null;
let googleSheets = null;

const getAuth = function () {
  return auth;
}

const instance = async function () {
  if (client == null) {
    client = await auth.getClient();
    googleSheets = google.sheets({ version: "v4", auth: client });
  }

  return googleSheets;
}

const fetchSubSheets = async function (spreadsheetId, titlePrefix) {
  const getSheetData = await googleSheets.spreadsheets.get({
    auth,
    spreadsheetId
  });

  const filteredSubSheets = getSheetData.data.sheets.filter(s => s.properties.title.startsWith(titlePrefix));

  let subSheets = [];
  for (const filteredSubSheet of filteredSubSheets) {
    let subSheet = {
      name: filteredSubSheet.properties.title,
      subSheetId: filteredSubSheet.properties.sheetId
    };
    subSheets.push(subSheet);
  }

  return subSheets;
}

const isCellNullOrEmpty = function (cell) {
  return !cell.formattedValue || cell.formattedValue.length === 0;
}

const findNextNonEmptyCell = function (cells, startIndex) {
  let index = startIndex;
  while (index < cells.length && isCellNullOrEmpty(cells[index]))
    ++index;

  return index < cells.length ? index : -1;
}

const hasCellHyperlink = function (cell) {
  return !isCellNullOrEmpty(cell) && cell.hyperlink && cell.hyperlink.length > 0;
}

exports.auth = auth;
exports.instance = instance;

exports.fetchSubSheets = fetchSubSheets;

exports.isCellNullOrEmpty = isCellNullOrEmpty;
exports.findNextNonEmptyCell = findNextNonEmptyCell;
exports.hasCellHyperlink = hasCellHyperlink;