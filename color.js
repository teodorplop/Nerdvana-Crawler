const color = function(red, green, blue, alpha) { return { red, green, blue, alpha }; }
const green = color(0, 1, 0, 1);
const red = color(1, 0, 0, 1);

const lerp = function(from, to, factor) {
    return {
        red: from.red + (to.red - from.red) * factor,
        green: from.green + (to.green - from.green) * factor,
        blue: from.blue + (to.blue - from.blue) * factor,
        alpha: from.alpha + (to.alpha - from.alpha) * factor
    };
}

exports.color = color;
exports.green = green;
exports.red = red;
exports.lerp = lerp;