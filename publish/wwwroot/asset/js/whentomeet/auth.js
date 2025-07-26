let container = document.getElementById('container');

function toggle() {
    container.classList.toggle('sign-in');
    container.classList.toggle('sign-up');
}
setTimeout(() => {
    container.classList.add('sign-in');
}, 200);

function validateSignUp(event) {
    const username = document.getElementById('signUpUsernameWTM').value.trim();
    const email = document.getElementById('signUpEmailWTM').value.trim();
    const password = document.getElementById('signUpPasswordWTM').value;
    const confirm = document.getElementById('signUpConfirmWTM').value;

    if (!username || username.length < 3) {
        alert("Vui lòng nhập tên đăng ký (ít nhất 3 ký tự)!");
        return false;
    }

    if (!email) {
        alert("Vui lòng nhập địa chỉ email!");
        return false;
    }
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        alert("Địa chỉ email không hợp lệ!");
        return false;
    }

    if (!password || password.length < 6) {
        alert("Vui lòng nhập mật khẩu (ít nhất 6 ký tự)!");
        return false;
    }

    if (password !== confirm) {
        alert("Mật khẩu và xác nhận mật khẩu không khớp!");
        return false;
    }

    return true;
}


function validateSignIn(event) {
    const email = document.getElementById('signInEmailWTM').value.trim();
    const password = document.getElementById('signInPasswordWTM').value;

    if (password.length < 6) {
        alert("Vui lòng nhập đúng tên đăng nhập và mật khẩu!");
        return false;
    }
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
        alert("Địa chỉ email không hợp lệ!");
        return false;
    }
    return true;
}
$(document).on("click", "#sign-up-wtm", function (event) {
    const UserName = $("#signUpUsernameWTM").val();
    const Email = $("#signUpEmailWTM").val();
    const Password = $("#signUpPasswordWTM").val();
    if (validateSignUp(event) == true) {
        $.ajax({
            url: "/WhenToMeet/HandleRegister",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify({ UserName, Email, Password }),
            beforeSend: function () {
                $("#spinner").addClass("show");
            },
            success: function (res) {
                if (res.success) {
                    Swal.fire({
                        title: 'Thông báo',
                        text: 'Đăng ký thành công hãy đăng nhập!',
                        icon: 'success',
                        confirmButtonText: 'OK'
                    });

                    toggle();
                } else {
                    alert(res.message);
                }
            },
            complete: function () {
                $("#spinner").removeClass("show");
            },
        });
    }

});
$(document).on("click", "#sign-in-wtm", function (event) {
    const Email = $("#signInEmailWTM").val();
    const Password = $("#signInPasswordWTM").val();
    if (validateSignIn(event) == true) {
        $.ajax({
            url: "/WhenToMeet/HandleLogin",
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify({ Email, Password }),
            beforeSend: function () {
                Swal.fire({
                    title: 'Đang đăng nhập...',
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });
            },
            success: function (res) {
                if (res.redirectUrl) {
                    Swal.close();
                    window.location.href = res.redirectUrl;
                    return;
                }
                if (!res.success) {
                    Swal.fire({
                        title: 'Lỗi!',
                        text: 'Đăng nhập không thành công. Vui lòng thử lại.',
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            },
            complete: function () {
                $("#spinner").removeClass("show");
            },
        });
    }

});