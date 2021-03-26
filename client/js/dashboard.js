const UpdateTimeoutMs = 2000;

var getData = [];
var postData = [];
var putData = [];
var deleteData = [];

function seperateData(data) {
    getData = [];
    postData = [];
    putData = [];
    deleteData = [];
    data.forEach(element => {
        element.timestamp = convertUnixToHourMinute(element.timestamp);
        if (element.method === "GET") {
            getData.push(element);
        } else if (element.method === "POST") {
            postData.push(element);
        } else if (element.method === "PUT") {
            putData.push(element);
        }
        else {
            deleteData.push(element);
        }
    });
}

function fetchData() {
    fetch('http://localhost:1923/health/api/products')
        .then(response => {
            if (response.status === 200) {
                response.json()
                    .then(function (data) {
                        seperateData(data);
                        updateChart();
                        setTimeout(fetchData, UpdateTimeoutMs);
                    });
            }
            else if (response.status === 204) {
                //alert("There is no committed data for requests to /api/products in the last hour.");
                setTimeout(fetchData, UpdateTimeoutMs);
            }
        })
        .catch(err => console.log(err));
}
drawChart();
fetchData();