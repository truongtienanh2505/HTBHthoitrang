// Dữ liệu giả lập Tracking Đơn hàng
const mockOrderHistory = {
    "DH0099": {
        // Trạng thái hiện tại: 1 (Chờ xác nhận), 2 (Đã xác nhận), 3 (Đang giao), 4 (Thành công)
        currentState: 3, 
        logs: [
            { time: "06/03/2026 10:00", status: "Chờ Xác Nhận", note: "Hệ thống ghi nhận đơn hàng mới." },
            { time: "06/03/2026 14:30", status: "Đã Xác Nhận", note: "Shop đã đóng gói xong." },
            { time: "07/03/2026 08:15", status: "Đang Giao", note: "Shipper đang lấy hàng đi giao." }
        ]
    }
};

function trackOrder() {
    const orderId = document.getElementById('orderId').value.trim();
    const resultDiv = document.getElementById('trackingResult');
    
    if (!orderId) {
        alert("Vui lòng nhập mã đơn hàng!");
        return;
    }

    const orderData = mockOrderHistory[orderId];

    if (orderData) {
        resultDiv.style.display = "block";
        document.getElementById('displayOrderId').innerText = orderId;
        
        // Cập nhật UI State Machine (Thanh tiến trình)
        updateStateMachineUI(orderData.currentState);

        // Render Bảng lịch sử
        const tbody = document.getElementById('historyBody');
        tbody.innerHTML = '';
        orderData.logs.forEach(log => {
            tbody.innerHTML += `
                <tr>
                    <td>${log.time}</td>
                    <td style="font-weight: bold; color: #2980b9;">${log.status}</td>
                    <td>${log.note}</td>
                </tr>
            `;
        });
    } else {
        resultDiv.style.display = "none";
        alert("Không tìm thấy mã đơn hàng này trên hệ thống!");
    }
}

function updateStateMachineUI(currentState) {
    // Reset tất cả các bước
    for(let i=1; i<=4; i++) {
        document.getElementById(`step${i}`).classList.remove('active');
    }

    // Active các bước từ 1 đến trạng thái hiện tại
    for(let i=1; i<=currentState; i++) {
        document.getElementById(`step${i}`).classList.add('active');
    }

    // Kéo dài thanh gạch ngang màu xanh
    // State 1 = 0%, State 2 = 33%, State 3 = 66%, State 4 = 100%
    const progressWidth = ((currentState - 1) / 3) * 100;
    document.getElementById('progressLine').style.width = `${progressWidth}%`;
}