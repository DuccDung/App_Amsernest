//btn-remove-report

$(document).on("click", ".btn-remove-report", function () {
    const meetingId = $(this).data("meeting-id");

    $.ajax({
        url: "/Admin/RemoveMeeting",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ meetingId: meetingId }),
        beforeSend: function () {
            $("#spinner").addClass("show");
        },
        success: function (res) {
            $("#content-modal-confirm-report").html(res);
        },
        complete: function () {
            $("#spinner").removeClass("show");
        },
        error: function () {
            alert("Lỗi lấy thông tin chi tiết báo cáo!");
        }
    });
});

$(document).on("click", ".btn-remove-report-teacher", function () {
    const meetingId = $(this).data("meeting-id");

    $.ajax({
        url: "/Admin/RemoveMeetingTeacher",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ meetingId: meetingId }),
        beforeSend: function () {
            $("#spinner").addClass("show");
        },
        success: function (res) {
            $("#content-modal-confirm-report").html(res);
        },
        complete: function () {
            $("#spinner").removeClass("show");
        },
        error: function () {
            alert("Lỗi lấy thông tin chi tiết báo cáo!");
        }
    });
});



$(document).on("click", "#remove-submit-meeting-teacher", function () {
    const meetingId = $("#report-id-val-remove").val();
    const scrollTop = $("#container-report").scrollTop();
    $.ajax({
        url: "/Admin/HandleRemoveMeeting",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({ meetingId: meetingId }),
        beforeSend: function () {
            $("#spinner").addClass("show");
        },
        success: function (res) {
            if (res.success) {
                $("#content-modal-confirm-report").html("");
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
            }
            else {
                alert("Lỗi: " + res.message);
            }
        },
        error: function (xhr, status, error) {
            $("#spinner").removeClass("show");
            alert("Lỗi xác nhận: ", error);
        }
    });
});

$(document).on("click", "#remove-submit-meeting", function () {
    const meetingId = $("#report-id-val-remove").val();
    const feedBack = $("#feedback-report").val();
    const scrollTop = $("#container-report").scrollTop();
    $.ajax({
        url: "/Admin/HandleRemoveMeeting",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({ meetingId: meetingId, feedBack: feedBack }),
        beforeSend: function () {
            $("#spinner").addClass("show");
        },
        success: function (res) {
            if (res.success) {
                $("#content-modal-confirm-report").html("");
                const from = $("#from").val();
                const to = $("#to").val();
                $.ajax({
                    url: "/Admin/ProccessReportChoose",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({ from, to }),
                    beforeSend: function () {
                        $("#spinner").addClass("show");
                    },
                    success: function (data) {
                        $("#dashBoard__table-report").html(data);
                        $("#container-report").scrollTop(scrollTop);
                    },
                    complete: function () {
                        $("#spinner").removeClass("show");
                    },
                    error: function (xhr, status, error) {
                        $("#spinner").removeClass("show");
                        console.error("Error fetching today's summary report:", error);
                    }
                })
            }
            else {
                alert("Lỗi: " + res.message);
            }
        },
        error: function (xhr, status, error) {
            $("#spinner").removeClass("show");
            alert("Lỗi xác nhận: ", error);
        }
    });
});

$(document).on("click", ".btn-remove-teacher", function () {
    const teacherId = $(this).data("teacher-id");

    $.ajax({
        url: "/Admin/RemoveTeacher",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ teacherId }),
        beforeSend: function () {
            $("#spinner").addClass("show");
        },
        success: function (res) {
            $("#content-modal-confirm-report").html(res);
        },
        complete: function () {
            $("#spinner").removeClass("show");
        },
        error: function () {
            alert("Lỗi lấy thông tin chi tiết báo cáo!");
        }
    });
});


$(document).on("click", "#remove-teacher-submit-meeting", function () {
    const teacherId = $("#teacher-id-val-remove").val();
    const scrollTop = $("#teacher-table-containner").scrollTop();
    $.ajax({
        url: "/Admin/HandleRemoveTeacher",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({ teacherId: teacherId }),
        beforeSend: function () {
            $("#spinner").addClass("show");
        },
        success: function (res) {
            if (res.success) {
                $("#content-modal-confirm-report").html("");
                $.ajax({
                    url: "/Admin/TecherPage",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({ }),
                    beforeSend: function () {
                        $("#spinner").addClass("show");
                    },
                    success: function (data) {
                        $("#content-layout-dashboard").html(data);
                        $("#teacher-table-containner").scrollTop(scrollTop);
                    },
                    complete: function () {
                        $("#spinner").removeClass("show");
                    },
                    error: function (xhr, status, error) {
                        $("#spinner").removeClass("show");
                        console.error("Error fetching today's summary report:", error);
                    }
                })
            }
            else {
                alert("Lỗi: " + res.message);
            }
        },
        complete: function () {
            $("#spinner").removeClass("show");
        },
        error: function (xhr, status, error) {
            alert("Lỗi xác nhận: ", error);
        }
    });
});