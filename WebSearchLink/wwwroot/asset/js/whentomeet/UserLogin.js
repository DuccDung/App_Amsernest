document.getElementById('startButton').addEventListener('click', function () {
    const userName = document.getElementById('userName').value.trim();

    if (!userName) {
        alert("Vui lòng nhập tên!");
        return;
    }

    const data = { userName: userName };


    $.ajax({
        url: '/WhenToMeet/InitCookie',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        beforeSend: function () {
            $('.overlay-spinner').show(); // Show spinner before sending request

        },
        success: function (responseData) {
            console.log("Response Data:", responseData);
            $('.overlay-spinner').hide(); // Hide spinner
            if (responseData.userId) {
                window.location.href = responseData.redirectUrl;
            }
        },
        complete: function () {
            $('.overlay-spinner').hide(); // Hide spinner after completion
        },
        error: function (xhr, status, error) {
            $('.overlay-spinner').hide(); // Hide spinner
            console.error('Error:', error);
            alert('Đã xảy ra lỗi, vui lòng thử lại.');
        }
    });

});


