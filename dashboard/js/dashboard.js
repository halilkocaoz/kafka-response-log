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
        element.timestamp = takeHHMM(element.timestamp);
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

var statusTitle = document.getElementById('statusTitle');

function fetchData() {
    fetch('http://localhost:1923/health/api/products')
        .then(response => {
            if (response.status === 200) {
                response.json()
                    .then(function (data) {
                        seperateData(data);
                        updateChart();
                        statusTitle.innerHTML = "";
                    });
            }
            else if (response.status === 204) {
                statusTitle.innerHTML = "There is no committed data for requests to /api/products in the last hour.";
            }
            setTimeout(fetchData, UpdateTimeoutMs);
        })
        .catch(err => {
            console.log(err);
            statusTitle.innerHTML = "Server could be down";
            setTimeout(fetchData, UpdateTimeoutMs);
        });
}
drawChart();
fetchData();