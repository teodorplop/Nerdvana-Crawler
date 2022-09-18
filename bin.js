const fileSystem = require('fs');
const homeworkEval = require('./nerdvanaHomeworkEval');
const inputHelper = require('./inputHelper');

const evaluateHomework = async function (spreadsheetId) {
  await homeworkEval.evaluate(spreadsheetId);
}
const updateTotal = async function (spreadsheetId) {
  await homeworkEval.updateTotal(spreadsheetId);
}
const exit = async function (spreadsheetId) { }

const spreadsheetQuestion = "Select spreadsheet:\n";
const spreadsheetOptions = JSON.parse(fileSystem.readFileSync("spreadsheets.json"));

const actionQuestion = "Select action:\n";
const actionOptions = [
  { name: "Evaluate homework", action: evaluateHomework },
  { name: "Update total", action: updateTotal },
  { name: "Exit", action: exit }
];

const runApp = async function () {
  const spreadsheet = await inputHelper.waitForArraySelectionInput(spreadsheetOptions, spreadsheetQuestion, false);
  if (!spreadsheet || spreadsheet.length == 0) {
    console.log("[Error] Invalid input.");
    return;
  }

  const action = await inputHelper.waitForArraySelectionInput(actionOptions, actionQuestion, false);
  if (!action || action.length === 0) {
    console.log("[Error] Invalid input.");
    return;
  }

  await action[0].action(spreadsheet[0].id);
}

runApp().then(r => { console.log("[Log] Done!") });