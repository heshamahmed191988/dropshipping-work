document.addEventListener('DOMContentLoaded', function () {
    const chart = Highcharts.chart('sellesChartContainer', {
        title: {
            text: 'Number of Orders by Year'
        },
        xAxis: {
            type: 'category'
        },
        yAxis: {
            title: {
                text: 'Number of Orders'
            }
        },
        series: [{
            type: 'column',
            name: 'Orders',
            data: []
        }]
    });

    $.ajax({
        type: 'POST',
        url: '/Admin/SellesbyYear',
        dataType: 'json',
        success: function (data) {
            chart.series[0].setData(data);
        },
        error: function (xhr, status, error) {
            console.error("Error status:", status);
            console.error("Error message:", xhr.responseText);
        }
    });
});
