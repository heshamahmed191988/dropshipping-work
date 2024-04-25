/*brand js*/
$(document).ready(function () {
    $.ajax({
        type: "POST",
        url: '/Admin/Dashboardcount',
        data: JSON.stringify({}),
        contentType: "application/json",
        dataType: "json",
        success: function (json) {
            console.log("Response from Admin/Dashboardcount:", json);
            if (json && json.length > 0) {
                console.log("Response from Admin/Dashboardcount is not empty");
                var seriesData = [];
                for (var i = 0; i < json.length; i++) {
                    var brandData = json[i];
                    if (brandData.length === 2) {
                        seriesData.push({
                            name: brandData[0],
                            y: parseInt(brandData[1]),
                            drilldown: brandData[0]
                        });
                    }
                }

                Highcharts.chart('productChartContainer', {
                    chart: {
                        type: 'pie'
                    },
                    title: {
                        text: 'Product Distribution by Brand'
                    },
                    series: [{
                        name: 'Brand',
                        colorByPoint: true,
                        data: seriesData
                    }]
                });
            } else {
                console.error("Error: DashBoardcount is empty");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error status:", status);
            console.error("Error message:", xhr.responseText);
        }
    });
});

//$(document).ready(function () {
//    $.ajax({
//        type: "POST",
//        url: '/Admin/Dashboardcount',
//        data: JSON.stringify({}),
//        contentType: "application/json",
//        dataType: "json",
//        success: function (json) {
//            console.log("Response from Admin/Dashboardcount:", json);
//            if (json && json.length > 0) {
//                console.log("Response from Admin/Dashboardcount is not empty");
//                var seriesData = [];

//                for (var i = 0; i < json.length; i++) {
//                    var productData = json[i];
//                    seriesData.push({
//                        name: productData.Name,
//                        y: parseFloat(productData.AverageRating),
//                        drilldown: productData.Name
//                    });
//                }

//                // Create the product chart
//                Highcharts.chart('productChartContainer', {
//                    chart: {
//                        type: 'column'
//                    },
//                    title: {
//                        align: 'left',
//                        text: 'Product Average Review Ratings'
//                    },
//                    xAxis: {
//                        type: 'category'
//                    },
//                    yAxis: {
//                        title: {
//                            text: 'Average Rating'
//                        }
//                    },
//                    legend: {
//                        enabled: false
//                    },
//                    plotOptions: {
//                        series: {
//                            borderWidth: 0,
//                            dataLabels: {
//                                enabled: true,
//                                format: '{point.y:.2f}'
//                            }
//                        }
//                    },
//                    series: [{
//                        name: 'Products',
//                        colorByPoint: true,
//                        data: seriesData
//                    }]
//                });
//            } else {
//                console.error("Error: DashBoardcount is empty");
//            }
//        },
//        error: function (xhr, status, error) {
//            console.error("Error status:", status);
//            console.error("Error message:", xhr.responseText);
//        }
//    });
//});