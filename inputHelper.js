const readline = require('readline');

const askQuestion = function (query) {
    const rl = readline.createInterface({
        input: process.stdin,
        output: process.stdout
    });

    return new Promise(resolve => rl.question(query, ans => {
        rl.close();
        resolve(ans);
    }));
};

const waitForArraySelectionInput = async function (array, question, includeAll) {
    if (includeAll)
        question += `${0}. All\n`;
    for (let i = 0; i < array.length; ++i)
        question += `${i + 1}. ${array[i].name}\n`;

    const input = await askQuestion(question);
    const selectedIndex = parseInt(input, 10);
    let selectedArray = [];
    if (!isNaN(selectedIndex) && isFinite(selectedIndex) && selectedIndex >= 0 && selectedIndex <= array.length) {
        if (selectedIndex === 0 && includeAll)
            selectedArray = array;
        else
            selectedArray.push(array[selectedIndex - 1]);
    }

    return selectedArray;
};

exports.waitForArraySelectionInput = waitForArraySelectionInput;