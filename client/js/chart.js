function drawChart(getData, postData, putData, deleteData, canvasId) {
    // Dummy
    getData = [113, 199, 219, 279, 285, 398, 414, 443, 541, 671, 702, 746, 865, 1004, 1036, 1058, 1063, 1192, 1227, 1264, 1280, 1321, 1347, 1354, 1366, 1391, 1473, 1490, 1548, 1603, 1859, 1893, 1922, 1933, 1939, 2020, 2030, 2115, 2133, 2155, 2194, 2196, 2212, 2239, 2347, 2365, 2377, 2466, 2475, 2542, 2551, 2552, 2593, 2613, 2658, 2749, 2754, 2836, 2900, 2912];
    postData = [121, 170, 180, 219, 235, 262, 369, 380, 478, 548, 596, 615, 645, 669, 688, 866, 923, 967, 1009, 1012, 1036, 1113, 1169, 1234, 1287, 1315, 1320, 1359, 1421, 1441, 1519, 1543, 1590, 1664, 1679, 1744, 1823, 1835, 1919, 1973, 2005, 2099, 2121, 2123, 2177, 2263, 2312, 2340, 2446, 2477, 2563, 2564, 2616, 2617, 2619, 2648, 2649, 2681, 2851, 2891];
    putData = [134, 138, 243, 248, 254, 329, 342, 352, 407, 413, 531, 534, 577, 615, 689, 700, 705, 722, 883, 923, 969, 1014, 1120, 1151, 1168, 1188, 1215, 1319, 1331, 1384, 1502, 1527, 1549, 1552, 1561, 1613, 1750, 1799, 1805, 1807, 1867, 2034, 2088, 2159, 2170, 2197, 2304, 2360, 2392, 2443, 2550, 2582, 2619, 2714, 2754, 2786, 2793, 2834, 2900, 2937];
    deleteData = [135, 168, 177, 207, 265, 282, 398, 399, 513, 565, 571, 654, 745, 816, 824, 872, 874, 896, 904, 1030, 1046, 1049, 1110, 1135, 1142, 1204, 1206, 1270, 1280, 1351, 1412, 1419, 1468, 1539, 1569, 1652, 1659, 1775, 1808, 1887, 1994, 2041, 2089, 2104, 2105, 2193, 2198, 2297, 2329, 2392, 2407, 2544, 2561, 2591, 2655, 2732, 2737, 2747, 2748, 2924];

    var chartData = {
        labels: chartLabels(),
        datasets: [
            { label: "GET", backgroundColor: '#99ffcc', borderColor: '#99ffcc', data: getData, fill: false },
            { label: "POST", backgroundColor: '#cc8800', borderColor: '#cc8800', data: postData, fill: false },
            { label: "PUT", backgroundColor: '#000066', borderColor: '#000066', data: putData, fill: false },
            { label: "DELETE", backgroundColor: '#cc0000', borderColor: '#cc0000', data: deleteData, fill: false }
        ]
    };

    var chartOptions = {
        responsive: true,
        title: { display: true, text: 'api/products' },
        tooltips: { mode: 'index', intersect: false },
        hover: { mode: 'nearest', intersect: true },
        scales: { yAxes: [{ display: true, scaleLabel: { display: true, labelString: 'Elapsed time to response (milliseconds)' } }] }
    };

    var chart = { type: 'line', data: chartData, options: chartOptions };
    var canvas = document.getElementById(canvasId).getContext('2d');
    window.myLine = new Chart(canvas, chart);
}