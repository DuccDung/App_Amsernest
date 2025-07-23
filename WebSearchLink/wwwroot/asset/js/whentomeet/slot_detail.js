 
$(".btn-detail-time-slot").click(function (e) {
    e.preventDefault();
    var id = $(this).data("id");
    // Gửi AJAX lấy chi tiết
    $.ajax({
        url: '/WhenToMeet/WTM_Detail', //Controller/Action
        type: 'GET',
        data: { wtmId: id },
        success: function (data) {
            $('#dynamicContentBody').html(data);
            var modal = new bootstrap.Modal(document.getElementById('dynamicContentModal'));
            modal.show();
        },
        error: function () {
            alert("Đã xảy ra lỗi khi lấy dữ liệu!");
        }
    });
});
