

document.addEventListener('DOMContentLoaded', function () {
    // Fetch top 3 sellers data
    $.ajax({
        type: 'POST',
        url: '/Admin/GetTopSellers',
        dataType: 'json',
        success: function (data) {
            // Render pyramid chart with the top 3 sellers data
            Highcharts.chart('Top3', {
                chart: {
                    type: 'pyramid3d',
                    options3d: {
                        enabled: true,
                        alpha: 10,
                        depth: 50,
                        viewDistance: 50
                    }
                },
                title: {
                    text: 'Top 3 Selles  '
                },
                plotOptions: {
                    series: {
                        dataLabels: {
                            enabled: true,
                            format: '<b>{point.name}</b> ({point.y:,.0f})',
                            allowOverlap: true,
                            x: 10,
                            y: -5
                        },
                        width: '60%',
                        height: '80%',
                        center: ['50%', '45%']
                    }
                },
                series: [{
                    name: 'Total Quantity Sold',
                    data: data.map(item => [item.name, item.totalQuantitySold])
                }]
            });
        },
        error: function (xhr, status, error) {
            console.error("Error status:", status);
            console.error("Error message:", xhr.responseText);
        }
    });
});