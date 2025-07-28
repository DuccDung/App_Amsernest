//function addSlot() {
//    const date = document.getElementById("adminSetDate").value;
//    const timeStart = document.getElementById("adminSetTimeStart").value;
//    const timeEnd = document.getElementById("adminSetTimeEnd").value;

//    // Kiểm tra dữ liệu
//    if (!date || !timeStart || !timeEnd) {
//        Swal.fire({
//            title: "Thiếu thông tin",
//            text: "Vui lòng chọn đầy đủ ngày, giờ bắt đầu và kết thúc.",
//            icon: "warning",
//            confirmButtonText: "OK"
//        });
//        return;
//    }

//    // Gửi ajax
//    $.ajax({
//        url: '/WhenToMeet/AddSlot', // Đổi lại nếu controller khác
//        method: 'POST',
//        contentType: 'application/json',
//        data: JSON.stringify({
//            date: date,
//            timeStart: timeStart,
//            timeEnd: timeEnd
//        }),
//        success: function (res) {
//            if (res.success) {
//                Swal.fire({
//                    title: "Thành công",
//                    text: "Đã thêm slot thời gian!",
//                    icon: "success",
//                    confirmButtonText: "OK"
//                });
//                const displayText = `${date} | ${timeStart} - ${timeEnd}`;

//                // Tạo phần tử <li> mới
//                const ul = document.getElementById("adminSlotList");
//                const li = document.createElement("li");
//                li.className = "flex items-center justify-between bg-blue-50 p-3 rounded shadow";

//                li.innerHTML = `
//                    <span class="text-blue-800 font-medium">${displayText}</span>
//                    <button onclick="this.parentElement.remove()" class="text-red-500 hover:text-red-700 font-bold">🗑️</button>
//                `;
//                ul.appendChild(li);
//                // Xóa input sau khi thêm nếu muốn
//                //document.getElementById("adminSetDate").value = '';
//                document.getElementById("adminSetTimeStart").value = '';
//                document.getElementById("adminSetTimeEnd").value = '';
//            } else {
//                Swal.fire({
//                    title: "Lỗi",
//                    text: res.message || "Không thể thêm slot.",
//                    icon: "error",
//                    confirmButtonText: "OK"
//                });
//            }
//        },
//        error: function () {
//            Swal.fire({
//                title: "Lỗi server",
//                text: "Không thể kết nối tới máy chủ.",
//                icon: "error",
//                confirmButtonText: "OK"
//            });
//        }
//    });
//}
//function addSlotInDetail() {
//    const date = document.getElementById("adminSetDateInDetail").value;
//    const timeStart = document.getElementById("adminSetTimeStartInDetail").value;
//    const timeEnd = document.getElementById("adminSetTimeEndInDetail").value;

//    // Kiểm tra dữ liệu
//    if (!date || !timeStart || !timeEnd) {
//        Swal.fire({
//            title: "Thiếu thông tin",
//            text: "Vui lòng chọn đầy đủ ngày, giờ bắt đầu và kết thúc.",
//            icon: "warning",
//            confirmButtonText: "OK"
//        });
//        return;
//    }

//    // Gửi ajax
//    $.ajax({
//        url: '/WhenToMeet/AddSlot', // Đổi lại nếu controller khác
//        method: 'POST',
//        contentType: 'application/json',
//        data: JSON.stringify({
//            date: date,
//            timeStart: timeStart,
//            timeEnd: timeEnd
//        }),
//        success: function (res) {
//            if (res.success) {
//                Swal.fire({
//                    title: "Thành công",
//                    text: "Đã thêm slot thời gian!",
//                    icon: "success",
//                    confirmButtonText: "OK"
//                });
//                const displayText = `${date} | ${timeStart} - ${timeEnd}`;

//                // Tạo phần tử <li> mới
//                const ul = document.getElementById("adminSlotListInDetail");
//                const li = document.createElement("li");
//                li.className = "flex items-center justify-between bg-blue-50 p-3 rounded shadow";

//                li.innerHTML = `
//                    <span class="text-blue-800 font-medium">${displayText}</span>
//                    <button onclick="this.parentElement.remove()" class="text-red-500 hover:text-red-700 font-bold">🗑️</button>
//                `;
//                ul.appendChild(li);
//                // Xóa input sau khi thêm nếu muốn
//                //document.getElementById("adminSetDate").value = '';
//                document.getElementById("adminSetTimeStart").value = '';
//                document.getElementById("adminSetTimeEnd").value = '';
//            } else {
//                Swal.fire({
//                    title: "Lỗi",
//                    text: res.message || "Không thể thêm slot.",
//                    icon: "error",
//                    confirmButtonText: "OK"
//                });
//            }
//        },
//        error: function () {
//            Swal.fire({
//                title: "Lỗi server",
//                text: "Không thể kết nối tới máy chủ.",
//                icon: "error",
//                confirmButtonText: "OK"
//            });
//        }
//    });
//}
