$(document).ready(function () {
    $.ajax({
        type: "POST",
        url: '/Admin/ProductCountDashbord',
        dataType: "json",
        success: function (data) {
            console.log("Response from Admin/ProductCountDashbord:", data);
            $('#productCount').text(data.productCount);
        },
        error: function (xhr, status, error) {
            console.error("Error status:", status);
            console.error("Error message:", xhr.responseText);
        }
    });
});