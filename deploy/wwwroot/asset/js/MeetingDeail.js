$(document).on("click", ".btn-detail-report", function () {
    const meetingId = $(this).data("meeting-id");

    $.ajax({
        url: "/Admin/MeetingDetail",
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

$(document).on("click", "#btn-detail-send", function () {
    const meetingId = $("#detail-meeting-id").val();
    const typeMeeting = $("#floatingSelectType").val();
    const teacherId = $("#floatingSelectTeacher").val();
    const scrollTop = $("#container-report").scrollTop();
    $.ajax({
        url: "/Admin/HandleDetailMeeting",
        method: "POST",
        contentType: "application/json",
        data: JSON.stringify({ teacherId, meetingId, typeMeeting }),
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
        complete: function () {
            $("#spinner").removeClass("show");
        },
        error: function (xhr, status, error) {
            alert("Vui lòng chọn đủ thông tin ! ", error);
        }
    });
});