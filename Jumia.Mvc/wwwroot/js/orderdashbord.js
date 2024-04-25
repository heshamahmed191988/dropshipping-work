$(document).ready(function () {
    $.ajax({
        type: "POST",
        url: '/Admin/OrderDashbord',
        dataType: "json",
        success: function (data) {
            console.log("Response from Admin/OrderDashbord:", data);
            $('#orderCount').text(data.totalOrders);
        },
        error: function (xhr, status, error) {
            console.error("Error status:", status);
            console.error("Error message:", xhr.responseText);
        }
    });
});