$(document).on("click", "#btn-report-trash-info", function () {
    const teacherId = $("#seclectTeacherIdInSearch").val();
    const from = $("#from-in-meetingtoday").val();
    const to = $("#to-in-meetingtoday").val();

    $.ajax({
        url: "/Admin/DeletedMeeting",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
            teacherId: teacherId,
            from: from,
            to: to
        }),
        beforeSend: function () {
            $("#spinner").addClass("show");
        },
        success: function (data) {
            $("#content-modal-confirm-report").html(data);
        },
        complete: function () {
            $("#spinner").removeClass("show");
        },
        error: function (xhr, status, error) {
            $("#spinner").removeClass("show");
            alert("Error fetching today's meetings:", error);
        }
    });
})