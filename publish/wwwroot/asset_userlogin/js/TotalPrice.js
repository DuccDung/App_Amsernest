window.onload = function () {
    document.querySelector('.product-detail').classList.add('active');
};

function updateTotalPrice() {
    const price = parseFloat(document.getElementById('order-detail-price').value);
    const quantity = parseInt(document.getElementById('quantity').value);
    const totalPrice = price * quantity;

    document.getElementById('Productdetail-price').innerText =
        `(${totalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })})`;
    document.getElementById('order-detail-total-price').value = totalPrice;
}

document.getElementById('button-plus').addEventListener('click', function () {
    let quantityInput = document.getElementById('quantity');
    let currentValue = parseInt(quantityInput.value);
    quantityInput.value = currentValue + 1;

    updateTotalPrice();
});

document.getElementById('button-minus').addEventListener('click', function () {
    let quantityInput = document.getElementById('quantity');
    let currentValue = parseInt(quantityInput.value);
    if (currentValue > 1) {
        quantityInput.value = currentValue - 1;
        updateTotalPrice();
    }
});

document.getElementById('quantity').addEventListener('input', updateTotalPrice);
