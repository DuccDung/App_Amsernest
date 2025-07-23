$(document).on("click", "#btn-restore-meeting", function () {
    const uuid = $(this).data("uuid");
    $.ajax({
        url: "/Admin/RestoreMeeting",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({ meetingId: uuid }),
        beforeSend: function () {
            $("#spinner").addClass("show");
        },
        success: function (res) {
            if (res.success) {
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
            }
        },
        error: function () {
            $("#spinner").removeClass("show");
            alert("Lỗi khôi phục cuộc họp!");
        }
    });
});
//overlay-delete-meeting

$(document).on("click", ".overlay-delete-meeting", function () {
    const teacherId = $("#seclectTeacherIdInSearch").val();
    const from = $("#from-in-meetingtoday").val();
    const to = $("#to-in-meetingtoday").val();

    $.ajax({
        url: "/Admin/SearchMeetingByTeacher",
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
            $("#dashBoard__table-report-today").html(data);
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