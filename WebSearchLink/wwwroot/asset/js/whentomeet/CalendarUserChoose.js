const calendarGrid = document.getElementById("calendarGrid");
const monthYearLabel = document.getElementById("monthYearLabel");
const timeSlotList = document.getElementById("time-slot-list");
const userId_current = $("#userId_choose_time").val(); // Simulated user ID
const wtmId_current = $("#wtmId_choose_time").val(); // Simulated wtm ID
const adminId_current = $("#adminId_choose_time").val(); // Simulated wtm ID
// Simulate ASP.NET Core API endpoints
const API_BASE = '/api/timeslots';

let currentMonth = new Date().getMonth();
let currentYear = new Date().getFullYear();
let bookingData = {}; // Cache booking data

async function fetchBookingData(year, month) {
    try {
        const response = await fetch('/WhenToMeet/WTM_BookingData', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                whenToMeetId: wtmId_current
            })
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log('Booking Data:', data);
        return data;

    } catch (error) {
        console.error('Error fetching booking data:', error);
        return {};
    }
}

// Book a time slot
//async function bookTimeSlot(timeSlotId, selectedDate, selectedTime) {
//    try {
//        // Simulate successful booking
//        alert(`Đã đặt lịch thành công!\nNgày: ${selectedDate}\nGiờ: ${selectedTime}`);

//        // Refresh data
//        await loadBookingData();
//        renderCalendar(currentMonth, currentYear);

//    } catch (error) {
//        console.error('Error booking time slot:', error);
//        alert('Có lỗi xảy ra khi đặt lịch!');
//    }
//}
async function bookTimeSlot(timeSlotId, selectedDate, selectedTime) {
    try {
        // Dữ liệu cần gửi
        const requestData = {
            user_id: userId_current,
            timeslot_id: timeSlotId,
        };

        // Gửi yêu cầu AJAX tới API
        $.ajax({
            url: '/WhenToMeet/BookTimeSlot',  
            method: 'POST',            
            contentType: 'application/json',
            data: JSON.stringify(requestData),  
            success: function (response) {
                alert(`Đã đặt lịch thành công!\nNgày: ${selectedDate}\nGiờ: ${selectedTime}`);
                loadBookingData();
                renderSlots(selectedDate); 
                renderCalendar(currentMonth, currentYear);
            },
            error: function (xhr, status, error) {
                console.error('Error booking time slot:', error);
                alert('Có lỗi xảy ra khi đặt lịch!');
            }
        });
    } catch (error) {
        console.error('Error booking time slot:', error);
        alert('Có lỗi xảy ra khi đặt lịch!');
    }
}


// Load booking data for current month
async function loadBookingData() {
    bookingData = await fetchBookingData(currentYear, currentMonth);
}


async function getDateStatus(dateStr, userId, whenToMeetId) {
    try {
        const response = await fetch('/WhenToMeet/UserDateSlotStatus', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                date: dateStr,
                userId: userId,
                whenToMeetId: whenToMeetId
            })
        });

        if (!response.ok) throw new Error(`HTTP ${response.status}`);

        const result = await response.json();
        const { totalSlots, userSelectedSlotIds } = result;

        const bookedCount = userSelectedSlotIds.length;

        if (bookedCount === 0 && totalSlots > 0) return 'no_selected';
        if (bookedCount === totalSlots && totalSlots > 0) return 'fully-booked';
        if (bookedCount > 0 && bookedCount < totalSlots) return 'has-bookings';
        return 'available';

    } catch (error) {
        console.error("Lỗi khi kiểm tra trạng thái ngày:", error);
        return 'available';
    }
}



// Render available time slots for selected date
//function renderSlots(dateStr) {
//    const slots = bookingData[dateStr] || [];

//    if (slots.length === 0) {
//        timeSlotList.innerHTML = '<div class="no-slots">Không có slot nào cho ngày này</div>';
//        return;
//    }

//    timeSlotList.innerHTML = '';

//    slots.forEach(slot => {
//        const btn = document.createElement("button");
//        btn.textContent = slot.start_time + " - " + slot.end_time;
//        btn.disabled = slot.is_booked;

//        if (slot.is_booked) {
//            btn.textContent += ' (Đã đặt)';
//        } else {
//            btn.onclick = () => bookTimeSlot(slot.id, dateStr, slot.start_time);
//        }

//        timeSlotList.appendChild(btn);
//    });
//}
function renderSlots(dateStr) {
    const slots = bookingData[dateStr] || [];

    if (slots.length === 0) {
        timeSlotList.innerHTML = '<div class="no-slots">Không có slot nào cho ngày này</div>';
        return;
    }

    timeSlotList.innerHTML = '';

    slots.forEach(slot => {
        const btn = document.createElement("button");
        btn.textContent = slot.start_time + " - " + slot.end_time;
        btn.disabled = slot.is_booked;

        // Gửi yêu cầu AJAX để kiểm tra trạng thái đã đặt hay chưa
        $.ajax({
            url: '/WhenToMeet/CheckUserTimeSlot', // Endpoint kiểm tra
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ UserId: userId_current, TimeSlotId: slot.id }),
            success: function (response) {
                if (response.isBooked) {
                    btn.textContent += ' (Đã đặt)';
                    btn.onclick = () => cancelBooking(slot.id, dateStr); // Hủy đặt
                } else {
                    btn.onclick = () => bookTimeSlot(slot.id, dateStr, slot.start_time); // Đặt
                }
                timeSlotList.appendChild(btn);
            },
            error: function (xhr, status, error) {
                console.error('Error checking time slot:', error);
                alert('Có lỗi khi kiểm tra thời gian slot.');
            }
        });
    });
}

// Hủy đặt chỗ
function cancelBooking(timeSlotId, dateStr) {
    $.ajax({
        url: '/WhenToMeet/CancelBooking', // Endpoint hủy đặt
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ UserId: userId_current, TimeSlotId: timeSlotId }),
        success: function (response) {
            if (response.success) {
                alert('Đã hủy đặt thành công!');
                loadBookingData();
                renderSlots(dateStr); 
                renderCalendar(currentMonth, currentYear);
            } else {
                alert('Có lỗi xảy ra khi hủy đặt.');
            }
        },
        error: function (xhr, status, error) {
            console.error('Error cancelling booking:', error);
            alert('Có lỗi khi hủy đặt.');
        }
    });
}


// Render calendar with booking status
async function renderCalendar(month, year) {
    // Load booking data first
    await loadBookingData();

    calendarGrid.innerHTML = "";

    // Day names
    const days = ["MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN"];
    days.forEach(d => {
        const dayName = document.createElement("div");
        dayName.textContent = d;
        dayName.classList.add("day-name");
        calendarGrid.appendChild(dayName);
    });

    const firstDay = new Date(year, month, 1);
    let startDay = (firstDay.getDay() + 6) % 7;
    const daysInMonth = new Date(year, month + 1, 0).getDate();

    monthYearLabel.textContent = `${firstDay.toLocaleString("en-US", { month: "long" })} ${year}`;

    // Empty cells for days before month starts
    for (let i = 0; i < startDay; i++) {
        const empty = document.createElement("div");
        calendarGrid.appendChild(empty);
    }

    // Create day buttons
    for (let day = 1; day <= daysInMonth; day++) {
        const btn = document.createElement("button");
        btn.className = "day";
        btn.textContent = day;

        const fullDate = `${year}-${String(month + 1).padStart(2, "0")}-${String(day).padStart(2, "0")}`;
        const dateStatus = await getDateStatus(fullDate, userId_current, wtmId_current); 
        console.log(`Ngày ${fullDate}: trạng thái =`, dateStatus);

        // Add status class
        if (dateStatus !== 'available') {
            btn.classList.add(dateStatus);
        }

        btn.onclick = function () {
            document.querySelectorAll(".day.selected").forEach(el => el.classList.remove("selected"));
            this.classList.add("selected");
            renderSlots(fullDate);
        };

        calendarGrid.appendChild(btn);
    }
}

// Navigation event handlers
document.getElementById("prevMonth").onclick = async () => {
    if (currentMonth === 0) {
        currentMonth = 11;
        currentYear--;
    } else {
        currentMonth--;
    }
    await renderCalendar(currentMonth, currentYear);
};

document.getElementById("nextMonth").onclick = async () => {
    if (currentMonth === 11) {
        currentMonth = 0;
        currentYear++;
    } else {
        currentMonth++;
    }
    await renderCalendar(currentMonth, currentYear);
};

// Initialize calendar
renderCalendar(currentMonth, currentYear);