$(document).on("click", "#meeting-tody-left-menu", function () {
    $.ajax({
        url: "/Admin/HandleLeftMenuMeetingToDay",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({}),
        beforeSend: function () {
            $("#spinner").addClass("show");
        },
        success: function (data) {
            $("#content-layout-dashboard").html(data);
        },
        complete: function () {
            $("#spinner").removeClass("show");
        },
        error: function (xhr, status, error) {
            $("#spinner").removeClass("show");
            console.error("Error fetching today's meetings:", error);
        }
    });
});

$(document).on("click", "#summary-tody-left-menu", function () {
    $.ajax({
        url: "/Admin/SummaryReportToDay",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({}),
        beforeSend: function () {
            $("#spinner").addClass("show");
        },
        success: function (data) {
            $("#content-layout-dashboard").html(data);
        },
        complete: function () {
            $("#spinner").removeClass("show");
        },
        error: function (xhr, status, error) {
            $("#spinner").removeClass("show");
            console.error("Error fetching today's summary report:", error);
        }
    })
})

$(document).on("click", "#TeacherPage-info", function () {
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
        },
        complete: function () {
            $("#spinner").removeClass("show");
        },
        error: function (xhr, status, error) {
            $("#spinner").removeClass("show");
            console.error("Error fetching today's summary report:", error);
        }
    })
})


