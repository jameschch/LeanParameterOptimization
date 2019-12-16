function solve(p1, p2) {

    var heights = [1.47, 1.50, 1.52, 1.55, 1.57, 1.60, 1.63, 1.65, 1.68, 1.70, 1.73, 1.75, 1.78, 1.80, 1.83];
    var weights = [52.21, 53.12, 54.48, 55.84, 57.20, 58.57, 59.93, 61.29, 63.11, 64.47, 66.28, 68.10, 69.92, 72.19, 74.46];

    var cost = 0.0;

    for (i = 0; i < heights.Length; i++) {

        cost += (p1 * heights[i] - weights[i]) * (p2 * heights[i] - weights[i]);
    }
    
    return cost;
}