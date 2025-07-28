let lastCreatedWTMId = null;

$(document).ready(function () {
    $('#form-create-wtm').on('submit', function (e) {
        e.preventDefault();

        const data = {
            title: $('#title-add-wtm').val(),
            description: $('#description-add-wtm').val(),
            createdBy: 1,
            createdAt: $('#createdAt-add-wtm').val()
        };

        $.ajax({
            url: '/WhenToMeet/HandleAddWTM',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (res) {
                lastCreatedWTMId = res.id;

                $('#createModal').modal('hide');
                $('#form-create-wtm')[0].reset();

                const newRow = `
                    <tr>
                        <td>${res.id}</td>
                        <td class="fw-semibold text-dark">${res.title}</td>
                        <td>${res.email || 'Không rõ'}</td>
                        <td>${res.create_at?.split('T')[0]}</td>
                        <td>0</td>
                        <td>0</td>
                        <td>
                            <a href="#" class="btn btn-sm btn-outline-primary">Chi tiết</a>
                            <a href="#" class="btn btn-sm btn-outline-danger ms-2">Xóa</a>
                        </td>
                    </tr>
                `;
                $('table tbody').prepend(newRow);

                setTimeout(() => { $('#slotModal').modal('show'); }, 400);
            },
            error: function (xhr) {
                alert('Đã xảy ra lỗi: ' + xhr.responseText);
            }
        });
    });
});

// Thêm slot thời gian vào danh sách
function addSlot() {
    const date = $('#adminSetDate').val();
    const start = $('#adminSetTimeStart').val();
    const end = $('#adminSetTimeEnd').val();

    if (!date || !start || !end) {
        alert("Vui lòng nhập đầy đủ thông tin slot.");
        return;
    }

    $.ajax({
        url: '/WhenToMeet/AddSlot',
        type: 'POST',
        data: {
            meetId: lastCreatedWTMId,
            date: date,
            timeStart: start,
            timeEnd: end
        },
        success: function (res) {
            if (res.success) {
                const li = `
                    <li class="list-group-item d-flex justify-content-between align-items-center">
                        <span>${date} | ${start} - ${end}</span>
                        <button class="btn btn-sm btn-outline-danger" onclick="this.closest('li').remove()">🗑️ Xoá</button>
                    </li>
                `;
                $('#adminSlotList').prepend(li);
            } else {
                alert("Lỗi khi lưu slot.");
            }
        },
        error: function (xhr) {
            alert("Lỗi server: " + xhr.responseText);
        }
    });

    // Reset input sau khi gửi
    $('#adminSetDate').val('');
    $('#adminSetTimeStart').val('');
    $('#adminSetTimeEnd').val('');
}

