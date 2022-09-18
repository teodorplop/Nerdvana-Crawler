const got = require('got');

const noSolutionStrings = ["nici o solutie", "nici o soluție"];
const compileOkayStrings = ["evaluare completa: ", "evaluare completă: "];
const delayBetweenRequests = 1000;

let lastRequestTimestamp = 0;

// Extracts username from user object, for task URL platform
const extractUsername = function(taskUrl, user) {
	return taskUrl.includes("varena") ? user.varena : user.iarena;
};
// Extracts task name from task URL
const extractTaskName = function(taskUrl) {
	taskUrl = taskUrl.replace(/\/$/, '');
	return taskUrl.substring(taskUrl.lastIndexOf('/') + 1);
};
// Extracts root of url string
const extractRootUrl = function(taskUrl) {
	const pathArray = taskUrl.split('/');
	const protocol = pathArray[0];
	const host = pathArray[2];
	return protocol + '//' + host;
};

// Returns URL to solutions for a task and a given user
const constructSolutionsUrl = function (taskUrl, user) {
	let username = extractUsername(taskUrl, user);
	if (!username)
		throw new Error(`Failed to extract username a user.`);

	return `${extractRootUrl(taskUrl)}/monitor?user=${username}&task=${extractTaskName(taskUrl)}`;
};

// Retrieves highest score from an URL
const extractScoreFromBody = function(responseBody) {
	responseBody = responseBody.toLowerCase();
	
	if (noSolutionStrings.findIndex((noSolutionString) => responseBody.indexOf(noSolutionString) !== -1) !== -1)
		return 'X';
	
	const compileOkayString = compileOkayStrings.find((compileOkayString) => responseBody.indexOf(compileOkayString) !== -1);
	let maxScore = 0;
	if (compileOkayString && compileOkayString.length > 0) {
		let index = responseBody.indexOf(compileOkayString);
		while (index !== -1) {
			index += compileOkayString.length;

			let score = 0;
			while (responseBody[index] >= '0' && responseBody[index] <= '9') {
				score = score * 10 + (responseBody[index] - '0');
				++index;
			}
			maxScore = Math.max(maxScore, score);

			index = responseBody.indexOf(compileOkayString, index);
		}
	}
	
	return maxScore.toString();
};

const extractScore = async function(taskUrl, user) {
	const delay = delayBetweenRequests - (Date.now() - lastRequestTimestamp);
	if (delay > 0)
		await new Promise(resolve => setTimeout(resolve, delay));
	
	const solutionsUrl = constructSolutionsUrl(taskUrl, user);
	try {
		console.log(`[Log] Sending request ${solutionsUrl}`);
		const response = await got(solutionsUrl);
		console.log(`[Log] Success!`);
		return extractScoreFromBody(response.body);
	} catch (error) {
		console.log(`[Error] ${error.response.body}`);
	} finally {
		lastRequestTimestamp = Date.now();
	}
	return 'X';
}

exports.extractScore = extractScore;
exports.constructSolutionsUrl = constructSolutionsUrl;