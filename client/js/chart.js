var chart = null;
var chartLabels = [];

function drawChart() {
    if (chart != null) {
        chart.destroy();
    }
    var chartData = {
        labels: createChartLabel(),
        datasets: [
            { label: "GET", backgroundColor: '#99ffcc75', borderColor: '#99ffcc', data: getData, fill: false },
            { label: "POST", backgroundColor: '#cc880075', borderColor: '#cc8800', data: postData, fill: false },
            { label: "PUT", backgroundColor: '#00006675', borderColor: '#000066', data: putData, fill: false },
            { label: "DELETE", backgroundColor: '#cc000075', borderColor: '#cc0000', data: deleteData, fill: false }
        ]
    };
    var chartOptions = {
        responsive: true,
        animation: false,
        title: { display: true, text: 'api/products', fontSize: 25 },
        tooltips: { mode: 'index', intersect: false },
        hover: { mode: 'nearest', intersect: true },
        scales: { yAxes: [{ display: true, scaleLabel: { display: true, labelString: 'Elapsed time to response (milliseconds)', fontSize: 22, } }] }
    };
    var chartConfig = { type: 'line', data: chartData, options: chartOptions };
    var canvas = document.getElementById("canvas").getContext('2d');
    chart = new Chart(canvas, chartConfig);
}

function updateChart() {
    chart.data.labels = createChartLabel();
    chart.data.datasets[0].data = getAsChartData(getData);
    chart.data.datasets[1].data = getAsChartData(postData);
    chart.data.datasets[2].data = getAsChartData(putData);
    chart.data.datasets[3].data = getAsChartData(deleteData);
    chart.update();
}

function groupAverageByTimestamp(data) {
    var newData = [];
    for (let firstIndex = 0; firstIndex < data.length; firstIndex++) {
        const firstElement = data[firstIndex];
        var totalItemCountInSameTimestamp = 1;
        var totalElapsedTime = firstElement.elapsedTime;

        let searchIndex;
        var reachingTheEndCount = 0;

        for (searchIndex = firstIndex + 1; searchIndex < data.length; searchIndex++) {
            const searchElement = data[searchIndex];
            var inSameTimestamp = firstElement.timestamp === searchElement.timestamp;

            if (inSameTimestamp) {
                totalItemCountInSameTimestamp++;
                totalElapsedTime += searchElement.elapsedTime;
            }
            else {
                firstIndex = searchIndex - 1;
                break;
            }
        }

        if (searchIndex === data.length) {
            reachingTheEndCount++;
        }

        if (reachingTheEndCount >= 2) {
            break;
        } else {
            firstElement.elapsedTime = totalElapsedTime / totalItemCountInSameTimestamp;
            newData.push(firstElement);
        }
    }

    return newData;
}

function getAsChartData(data) {
    let chartData = new Int32Array(60);
    data = groupAverageByTimestamp(data);
    data.forEach(element => {
        chartData[chartLabels.indexOf(element.timestamp)] = element.elapsedTime;
    });
    return chartData;
}

function addZero(val) {
    if (val < 10) {
        val = "0" + val;
    }
    return val;
}

function takeHHMM(timestampSecond) {
    var d = new Date(timestampSecond * 1000);
    return addZero(d.getHours()) + ":" + addZero(d.getMinutes());
}

function createChartLabel() {
    chartLabels = [];
    var nowDate = new Date();
    nowDate.setSeconds(0);
    nowDate.setMilliseconds(0);
    var oneHourAgo = nowDate.getTime() - (1000 * 60 * 59);

    for (let time = oneHourAgo; time <= nowDate.getTime(); time += 60000) {
        chartLabels.push(takeHHMM(time / 1000));
    }

    return chartLabels;
}