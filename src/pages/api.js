// Khai báo đúng cổng 7127 của C#
const API_URL = "https://localhost:7127/api";

// ==========================================
// API TUẦN 2: Lấy chi tiết sản phẩm
// ==========================================
export const getProductDetail = async (id) => {
    const res = await fetch(`${API_URL}/products/${id}`);
    if (!res.ok) throw new Error("Không tìm thấy sản phẩm");
    return res.json();
};

// ==========================================
// API TUẦN 1: Lấy thông tin User cho trang Profile
// ==========================================
export const getMe = async () => {
    // [TODO CHO NHÓM]: Sau này làm Đăng nhập thì gắn JWT Token vào header ở đây
    try {
        const res = await fetch(`${API_URL}/users/me`, {
            method: "GET",
            headers: { "Content-Type": "application/json" }
        });
        
        if (!res.ok) return null;
        return await res.json();
    } catch (err) {
        console.error("Lỗi lấy thông tin User:", err);
        return null;
    }
};