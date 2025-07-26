function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}
const userIdsignalR = getCookie("userId");

const menuHubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/HubServer", {
        withCredentials: true
    })
    .configureLogging(signalR.LogLevel.Information)
    .build();

let cntOrderDetail = Number(document.querySelector('#foodter__cnt-orderDetail').value);
const foodterText = document.querySelector('#foodter__content-cnt');

menuHubConnection.on("UpdateIncreasedOrderDetailView", (message) => {
    cntOrderDetail+=1;
    foodterText.innerHTML = "Xem giỏ hàng(" + cntOrderDetail + ")";
});

menuHubConnection.on("UpdateDecreasedOrderDetailView", (message) => {
    cntOrderDetail -= 1;
    foodterText.innerHTML = "Xem giỏ hàng(" + cntOrderDetail + ")";
});

menuHubConnection.on("UpdateRemoveOrderDetailView", (message) => {
    if (cntOrderDetail <= 1) {
        document.querySelector('.foodter').remove();
    }
    else {
        cntOrderDetail -= 1;
        foodterText.innerHTML = "Xem giỏ hàng(" + cntOrderDetail + ")";
    }
});

// =================================================================================
// handle addition and subtraction order detail in page menu
$(".increase-btn").on('click', function (event) {
    event.preventDefault();
    var quantityInput = $(this).closest('form').find('.quantityInput');
    var quantity = parseInt(quantityInput.val());
    var productId = $(this).data('product-id');
    var orderId = $(this).closest('form').find('input[name="orderId"]').val();

    $.ajax({
        url: '/Home/Increase',
        type: 'POST',
        data: {
            quantity: quantity,
            productId: productId,
            orderId: orderId
        },
        beforeSend: function () {
            $(".overlay-spinner").addClass("show");
        },
        success: function (response) {
            if (response.error) {
                console.error(response.error);
            } else {
                quantityInput.val(response.newQuantity);

                if (menuHubConnection.state === signalR.HubConnectionState.Disconnected) {
                    menuHubConnection.start().then(() => {
                        menuHubConnection.invoke("SendMessageInMenuIncreasedProduct", userIdsignalR, productId + "")
                    })
                        .catch(err => console.error(" Hub start error:", err));
                }
                else {
                    menuHubConnection.invoke("SendMessageInMenuIncreasedProduct", userIdsignalR, productId + "")
                }
            }
        },
        complete: function () {
            $(".overlay-spinner").addClass("hide");
        },
        error: function (xhr, status, error) {
            console.error('Có lỗi xảy ra:', error);
        }
    });
});

$(".decrease-btn").on('click', function (event) {
    event.preventDefault();

    var quantityInput = $(this).closest('form').find('.quantityInput');
    var quantity = parseInt(quantityInput.val());
    var productId = $(this).data('product-id');
    var orderId = $(this).closest('form').find('input[name="orderId"]').val();
    if (quantity <= 1) {
        $.ajax({
            url: '/Home/GetProductPartial',
            type: 'POST',
            data: {
                productId: productId
            },
            beforeSend: function () {
                $(".overlay-spinner").addClass("show");
            },
            success: function (response) {
                if (response.error) {
                    console.error(response.error);
                } else {
                    $(`#product-${productId}`).html(response);

                    if (menuHubConnection.state === signalR.HubConnectionState.Disconnected) {
                        menuHubConnection.start().then(() => {
                            menuHubConnection.invoke("SendMessageInMenuRemoveOrderDetail", userIdsignalR, productId + "")
                        })
                            .catch(err => console.error(" Hub start error:", err));
                    }
                    else {
                        menuHubConnection.invoke("SendMessageInMenuRemoveOrderDetail", userIdsignalR, productId + "")
                    }
                }
            },
            complete: function () {
                $(".overlay-spinner").addClass("hide");
            },
            error: function (xhr, status, error) {
                console.error('Có lỗi xảy ra:', error);
            }
        });
    }
    else {
        $.ajax({
            url: '/Home/Reduce',
            type: 'POST',
            data: {
                quantity: quantity,
                productId: productId,
                orderId: orderId
            },
            beforeSend: function () {
                $(".overlay-spinner").addClass("show");
            },
            success: function (response) {
                if (response.error) {
                    console.error(response.error);
                } else {
                    quantityInput.val(response.newQuantity);

                    if (menuHubConnection.state === signalR.HubConnectionState.Disconnected) {
                        menuHubConnection.start().then(() => {
                            menuHubConnection.invoke("SendMessageInMenuDecreasedProduct", userIdsignalR, productId + "")
                        })
                            .catch(err => console.error(" Hub start error:", err));
                    }
                    else {
                        menuHubConnection.invoke("SendMessageInMenuDecreasedProduct", userIdsignalR, productId + "")
                    }
                }
            },
            complete: function () {
                $(".overlay-spinner").addClass("hide");
            },
            error: function (xhr, status, error) {
                console.error('Có lỗi xảy ra:', error);
            }
        });
    }
});

$(".btn-redirect-cart").on("click", function () {
    $.ajax({
        url: '/Home/RedirectCart',
        type: 'POST',
        beforeSend: function () {
            $(".overlay-spinner").addClass("show");
        },
        success: function (response) {
            if (response.error) {
                console.error(response.error);
            } else {
                window.location.href = response.redirectUrl;
            }
        },
        complete: function () {
            $(".overlay-spinner").addClass("hide");
        },
        error: function (xhr, status, error) {
            console.error('Có lỗi xảy ra:', error);
        }
    })
})