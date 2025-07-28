
$(document).on("click", "#btnSearch-report", function () {
    $.ajax({
        url: "/Admin/ProccessReportChoose",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            from: $("#from").val(), to: $("#to").val()
        }),
        beforeSend: function () {
            $("#spinner").addClass("show");
        },
        success: function (res) {
            $("#dashBoard__table-report").html(res);
        },
        complete: function () {
            $("#spinner").removeClass("show");
        },
        error: function () {
            alert("Lỗi gửi!");
        }
    })
})

$(document).ready(function () {
    const today = new Date().toISOString().split('T')[0];
    $('#from').val(today);
    $('#to').val(today);
});

$(document).ready(function () {
    const today = new Date().toISOString().split('T')[0];
    $('#from-in-meetingtoday').val(today);
    $('#to-in-meetingtoday').val(today);
});
