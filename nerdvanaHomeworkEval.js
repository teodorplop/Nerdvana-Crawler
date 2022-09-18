const nerdvanaSheet = require('./nerdvanaSheet');
const inputHelper = require('./inputHelper');
const crawler = require('./crawler');

const lecturesQuestion = "Select lecture to evaluate:\n";
const homeworksQuestion = "Select homework to evaluate:\n";
const studentsQuestion = "Select students to evaluate:\n";

const evaluate = async function (spreadsheet) {
	await nerdvanaSheet.init(spreadsheet);

	const studentsInfoMap = await nerdvanaSheet.fetchStudentsInfoMap();
	
	let lectures = await nerdvanaSheet.fetchLectureSheets();
	lectures = await inputHelper.waitForArraySelectionInput(lectures, lecturesQuestion, false);
	if (!lectures || lectures.length === 0) {
		console.log("[Error] Invalid input.");
		return;
	}
	
	for (const lecture of lectures) {
		let homeworks = await nerdvanaSheet.fetchLectureHomeworks(lecture.name);
		if (homeworks.length > 1) {
			homeworks = await inputHelper.waitForArraySelectionInput(homeworks, homeworksQuestion, true);
			if (!homeworks || homeworks.length === 0) {
				console.log("[Error] Invalid input.");
				return;
			}
		}
		
		let students = await nerdvanaSheet.fetchLectureStudents(lecture.name);
		students = await inputHelper.waitForArraySelectionInput(students, studentsQuestion, true);
		
		for (let student of students) {
			const studentInfo = studentsInfoMap[student.name];
			if (studentInfo === null) {
				console.log(`[Error] No info for student ${student.name}`);
			} else {
				student.homeworks = JSON.parse(JSON.stringify(homeworks));
				student.totalScore = 0;
				for (let homework of student.homeworks) {
					homework.totalScore = 0;
					for (let task of homework.tasks) {
						try {
							const score = await crawler.extractScore(task.URL, studentInfo);
							task.score = score;
							task.solutionsURL = crawler.constructSolutionsUrl(task.URL, studentInfo);

							const parsedScore = parseInt(score, 10);
							if (!isNaN(parsedScore))
								homework.totalScore += parsedScore;
						} catch (error) {
							console.log(`[Error] Failed to extract score for ${student.name}`);
            }
					}
					
					student.totalScore += homework.totalScore;
				}
			}
		}

		await nerdvanaSheet.updateHomeworks(lecture.subSheetId, students);
	}
}

const updateTotal = async function(spreadsheet) {
	await nerdvanaSheet.init(spreadsheet);
	
	let homeworks = await nerdvanaSheet.fetchHomeworkSheets();
	homeworks = await inputHelper.waitForArraySelectionInput(homeworks, homeworksQuestion, true);
	
	for (const homework of homeworks) {
		const students = await nerdvanaSheet.fetchLectureStudents(homework.name);
		for (let student of students)
			student.totalScore = 0;

		let maxScore = 0;
		const lectures = await nerdvanaSheet.fetchLectureSheets();
		for (const lecture of lectures) {
			const homeworkResults = await nerdvanaSheet.fetchLectureHomeworkResults(lecture.name, homework.name);

			for (const homeworkStudent of homeworkResults.students) {
				let student = students.find(s => s.name === homeworkStudent.name);
				if (student)
					student.totalScore += homeworkStudent.score;
			}

			maxScore += homeworkResults.maxScore;
		}

		for (let student of students)
			student.maxScore = maxScore;

		await nerdvanaSheet.updateHomeworkTotal(homework.subSheetId, students);
	}
}

exports.evaluate = evaluate;
exports.updateTotal = updateTotal;