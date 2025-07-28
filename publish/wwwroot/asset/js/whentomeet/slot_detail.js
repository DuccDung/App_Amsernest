 
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
$(".btn-remove-time-slot").on('click', function (e) {
    e.preventDefault();
    var id = $(this).data("id");
    $.ajax({
        url: '/WhenToMeet/HandleDeleteWTM', 
        type: 'GET',
        data: { Id: id },
        success: function (data) {
            if (data.success) {
                alert("Đã xoá thành công slot thời gian!");
                location.reload();
            } else {
                alert("Lỗi khi xoá slot: " + data.message);
            }
        },
        error: function () {
            alert("Đã xảy ra lỗi khi lấy dữ liệu!");
        }
    });
});