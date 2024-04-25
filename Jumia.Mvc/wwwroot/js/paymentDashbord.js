$(document).ready(function () {
    $.ajax({
        type: "POST",
        url: '/Admin/PaymentDashbord',
        dataType: "json",
        success: function (data) {
            console.log("Response from Admin/PaymentDashbord:", data);
            $('#paymentCount').text(data.totalPayment);
        },
        error: function (xhr, status, error) {
            console.error("Error status:", status);
            console.error("Error message:", xhr.responseText);
        }
    });
});