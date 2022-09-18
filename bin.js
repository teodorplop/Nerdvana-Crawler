const homeworkEval = require('./nerdvanaHomeworkEval');
const inputHelper = require('./inputHelper');

const spreadsheetId = "1pwMv0GdnO180U-7234ttAbe_hDXsyiZweHz3nY_ii6o";

const updateTotal = async function() {
    await homeworkEval.updateTotal(spreadsheetId);
}
const evaluateHomework = async function() {
    await homeworkEval.evaluate(spreadsheetId);
}
const exit = async function() {}

const selectOptionQuestion = "Select option:\n";
const options = [
    { name: "Evaluate homework", action: evaluateHomework },
    { name: "Update total", action: updateTotal },
    { name: "Exit", action: exit }
];

const runApp = async function() {
    const selection = await inputHelper.waitForArraySelectionInput(options, selectOptionQuestion, false);

    if (!selection || selection.length === 0) {
        console.log("[Error] Invalid input.");
        return;
    }
    
    await selection[0].action();
}

runApp().then(r => { console.log("[Log] Done!") });