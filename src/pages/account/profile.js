// URL Backend vừa tạo
const API_URL = "http://localhost:3000/api";

// ... (Phần code cũ render form giữ nguyên) ...

// Thay thế đoạn xử lý click nút Lưu (#btnSaveAddr)
$("#btnSaveAddr").addEventListener("click", async () => {
    // 1. Lấy dữ liệu từ form
    const payload = {
        contactName: $("#addrName").value,
        contactPhone: $("#addrPhone").value,
        province: $("#addrCity").value,
        district: $("#addrDist").value,
        ward: "", // Nếu form bạn chưa có ô này thì để trống
        addressLine: $("#addrDetail").value,
        isDefault: $("#addrDefault").checked
    };

    // 2. Gửi về Backend (POST)
    try {
        const res = await fetch(`${API_URL}/user-addresses`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        const data = await res.json();
        
        if (res.ok) {
            toast("Thêm địa chỉ thành công! ✅");
            // Load lại danh sách địa chỉ (Bạn cần viết hàm fetch GET để lấy lại data mới)
            loadAddresses(); 
        } else {
            toast("Lỗi: " + data.message);
        }
    } catch (e) {
        console.error(e);
        toast("Không kết nối được server ❌");
    }
});