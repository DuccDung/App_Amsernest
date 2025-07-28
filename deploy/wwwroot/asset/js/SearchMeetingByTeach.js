$(document).on("click", "#btnSearch-report-by-teacher", function () {
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

$(document).on("click", "#btnSearch-report-by-teacherư-in-summary", function () {
    const from = $("#from-in-meetingtoday-summary").val();
    const to = $("#to-in-meetingtoday-summary").val();
    $.ajax({
        url: "/Admin/HandleSammaryReport",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({
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
    });
})

$(document).on("click", "#export-excel-report", function () {
    const teacherId = $("#seclectTeacherIdInSearch").val();
    const from = $("#from-in-meetingtoday").val();
    const to = $("#to-in-meetingtoday").val();

    $("#spinner").addClass("show");

    fetch("/Admin/ExportToExcel", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            teacherId: teacherId,
            from: from,
            to: to
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Tải file thất bại");
            }
            return response.blob();
        })
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = url;
            a.download = `MeetingReports_${new Date().toISOString().slice(0, 19).replaceAll(':', '')}.xlsx`;
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        })
        .catch(error => {
            alert("Lỗi tải file: " + error.message);
        })
        .finally(() => {
            $("#spinner").removeClass("show");
        });
});

$(document).on("click", "#export-sumary-excel-report", function () {
    const from = $("#from-in-meetingtoday-summary").val();
    const to = $("#to-in-meetingtoday-summary").val();

    $("#spinner").addClass("show");

    fetch("/Admin/ExportToExcel", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            from: from,
            to: to
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Tải file thất bại");
            }
            return response.blob();
        })
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = url;
            a.download = `MeetingReports_${new Date().toISOString().slice(0, 19).replaceAll(':', '')}.xlsx`;
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(url);
        })
        .catch(error => {
            alert("Lỗi tải file: " + error.message);
        })
        .finally(() => {
            $("#spinner").removeClass("show");
        });
});
