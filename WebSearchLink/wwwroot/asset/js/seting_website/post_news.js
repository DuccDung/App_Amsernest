$(document).on('click', '#website-seting', function () {
    $.ajax({
        url: '/Admin/SetingWebsite',
        type: 'POST',
        data: {},
        success: function (data) {
            $('#content-layout-dashboard').html(data);

            tinymce.init({
                selector: '#content_post_new',
                height: 300,
                plugins: 'link image code lists',
                toolbar: 'undo redo | styles | bold italic | alignleft aligncenter alignright | bullist numlist | link image | code'
            });

        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            alert('Failed to load content. Please try again.');
        }
    });
});
$(document).on('click', '#btn_post_new', function () {

    let title = document.getElementById("title_post_new").value.trim();
    let summary = document.getElementById("summary_post_new").value.trim();
    let content = tinymce.get("content_post_new").getContent().trim(); // lấy từ TinyMCE
    let file = document.getElementById("image_post_new").files.length;

    if (!title) {
        alert("Title is required!");
        return;
    }
    if (!summary) {
        alert("Summary is required!");
        return;
    }
    if (!content) {
        alert("Content is required!");
        return;
    }
    if (file === 0) {
        alert("Please upload an image!");
        return;
    }

    // Đồng bộ nội dung từ TinyMCE vào textarea
    if (typeof tinymce !== "undefined") {
        tinymce.triggerSave();
    }

    // Nếu là CKEditor 4
    if (typeof CKEDITOR !== "undefined") {
        for (instance in CKEDITOR.instances) {
            CKEDITOR.instances[instance].updateElement();
        }
    }

    var form = $("#postForm")[0];
    var formData = new FormData(form);

    $.ajax({
        url: '/Admin/PostNews',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.success) {
                alert("Đăng bài thành công!");
            }
            else {
                alert("Đăng bài thất bại: " + res.message);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error:", error);
            alert("Có lỗi xảy ra: " + error);
        }
    });
});


