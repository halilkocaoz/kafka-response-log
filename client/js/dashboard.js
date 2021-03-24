function nowHourMinute() {
    return moment.unix(Date.now() / 1000).format("kk:mm");
}
function oneHourAgoHourMinute() {
    return moment.unix((Date.now() - (1000 * 60 * 60)) / 1000).format("kk:mm")
}
function convertUnixToHourMinute(timestamp) {
    return moment.unix(timestamp).format("kk:mm");
}
function chartLabels() {
    var chartLabels = [];
    var agoTime = oneHourAgoHourMinute();
    var nowTime = nowHourMinute();
    var nowMinute = nowTime.slice(-2);

    for (let index = nowMinute; index <= 59; index++) {
        chartLabels.push(agoTime.substring(0, 2) + ":" + index);
    }

    chartLabels.push(nowTime.substring(0, 2) + ":00")

    for (let index = 1; index <= nowMinute - 1; index++) {
        if (index < 10) {
            chartLabels.push(nowTime.substring(0, 2) + ":0" + index);
        }
        else {
            chartLabels.push(nowTime.substring(0, 2) + ":" + index);
        }
    }
    return chartLabels;
}