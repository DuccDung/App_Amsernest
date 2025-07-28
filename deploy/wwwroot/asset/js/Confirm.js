$(document).on("click", ".btn-confirm-report", function () {
    const meetingId = $(this).data("meeting-id");

    $.ajax({
        url: "/Admin/ConfirmReport",
        method: "POST",
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
            alert("Lỗi xác nhận!");
        }
    });
});


$(document).on("click", ".confirm-teacher", function () {
    const teacherId = $(this).data("teacher-id");

    $.ajax({
        url: "/Admin/ConfirmTeacher",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({ teacherId: teacherId }),
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
            alert("Lỗi xác nhận!");
        }
    });
});
//HandleConfirmTeacher
$(document).on("click", "#confirm-submit-Teacher", function () {
    const teacherId = $("#modal-teacher-id").val();
    const scrollTop = $("#teacher-table-containner").scrollTop(); 
    $.ajax({
        url: "/Admin/HandleConfirmTeacher",
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
                    data: JSON.stringify({}),
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
                alert("Lỗi: "+ res.message);
            }
        },
        complete: function () {
            $("#spinner").removeClass("show");
        },
        error: function () {
            alert("Lỗi xác nhận!");
        }
    });
});

//btn-confirm-submit
$(document).on("click", "#btn-confirm-submit", function () {
    const meetingId = $("#report-id-val").val();
    const feedBack = $("#feedback-report").val();
    const scrollTop = $("#container-report").scrollTop();
    $.ajax({
        url: "/Admin/HandleConfirmMeeting",
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
                    data: JSON.stringify({from , to}),
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
        complete: function () {
            $("#spinner").removeClass("show");
        },
        error: function () {
            alert("Lỗi xác nhận!");
        }
    });
});

