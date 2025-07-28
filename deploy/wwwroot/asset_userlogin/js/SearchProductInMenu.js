$('#txtSearchBox').on('input', function () {
    var txtsearch = $(this).val();
    console.log(txtsearch);
    if (txtsearch && txtsearch.length > 0) {
        $.ajax({
            url: '/Home/SearchProduct',
            data: { productName: txtsearch },
            success: function (response) {
                $("#content").html(response);
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
            }
        });
    }
    else {
        $.ajax({
            url:'/Home/ReloadMenu',
            success: function (response) {
                $("#content").html(response);
            },
            error: function (xhr, status, error) {
                console.error('Error:', error);
            }
        });
    }
});
