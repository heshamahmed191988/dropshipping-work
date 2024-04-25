$(document).ready(function () {
    $.ajax({
        type: "POST",
        url: '/Admin/UserDashbord',
        dataType: "json",
        success: function (data) {
            console.log("Response from Admin/UserDashbord:", data);
            $('#userCount').text(data.totalUsers);
        },
        error: function (xhr, status, error) {
            console.error("Error status:", status);
            console.error("Error message:", xhr.responseText);
        }
    });
});