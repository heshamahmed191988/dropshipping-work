
$(document).ready(function () {
    $.ajax({
        type: "POST",
        url: '/Admin/DashBoardreview',
        data: JSON.stringify({}),
        contentType: "application/json",
        dataType: "json",
        success: function (json) {
            console.log("Response from Admin/DashBoardreview:", json);
            if (json && json.length > 0) {
                console.log("Response from Admin/DashBoardreview is not empty");
                var seriesData = [];

                for (var i = 0; i < json.length; i++) {
                    var productData = json[i];
                    seriesData.push({
                        name: productData[0],
                        y: parseFloat(productData[1]),
                        drilldown: productData[0]
                    });
                }

                Highcharts.chart('reviewChartContainer', {
                    chart: {
                        type: 'column'
                    },
                    title: {
                        align: 'left',
                        text: 'Product Average Review Ratings'
                    },
                    xAxis: {
                        type: 'category'
                    },
                    yAxis: {
                        title: {
                            text: 'Review Count'
                        }
                    },
                    legend: {
                        enabled: false
                    },
                    plotOptions: {
                        series: {
                            borderWidth: 0,
                            dataLabels: {
                                enabled: true,
                                format: '{point.y:.0f}'
                            }

                        }
                    },
                    series: [{
                        name: 'Products',
                        colorByPoint: true,
                        data: seriesData
                    }]
                });
            } else {
                console.error("Error: DashBoardreview is empty");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error status:", status);
            console.error("Error message:", xhr.responseText);
        }
    });
});