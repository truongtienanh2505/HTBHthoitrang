// Khai báo đúng cổng 7127 của C#
const API_URL = "http://localhost:5157/api";
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
// ==========================================
// API TUẦN 2+3: Lấy chi tiết sản phẩm
// ==========================================
export const getProductDetail = async (id) => {
    const res = await fetch(`${API_URL}/products/${id}`);
    if (!res.ok) throw new Error("Không tìm thấy sản phẩm");
    return res.json();
};
// ==========================================
// API TUẦN 4: BLOG TIN TỨC
// ==========================================

export const listBlog = async ({ cat } = {}) => {
    let url = `${API_URL}/articles`;
    if (cat) url += `?cat=${cat}`;
    
    try {
        const res = await fetch(url);
        if (!res.ok) return { categories: [], posts: [] };
        return await res.json();
    } catch (error) {
        console.error("Lỗi lấy danh sách blog:", error);
        return { categories: [], posts: [] };
    }
};

export const getPostBySlug = async (slug) => {
    try {
        const res = await fetch(`${API_URL}/articles/${slug}`);
        if (!res.ok) return null;
        return await res.json();
    } catch (error) {
        console.error("Lỗi lấy chi tiết bài viết:", error);
        return null;
    }
};
// ==========================================
// API TUẦN 4: ĐÁNH GIÁ SẢN PHẨM
// ==========================================

// Lấy danh sách đánh giá của 1 sản phẩm
export const getProductReviews = async (productId) => {
    try {
        const res = await fetch(`${API_URL}/reviews/product/${productId}`);
        if (!res.ok) return [];
        return await res.json();
    } catch (err) {
        console.error("Lỗi lấy đánh giá:", err);
        return [];
    }
};

// Gửi đánh giá mới
export const submitReview = async (payload) => {
    try {
        const res = await fetch(`${API_URL}/reviews`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });
        const data = await res.json();
        return { success: res.ok, message: data.message };
    } catch (err) {
        console.error("Lỗi gửi đánh giá:", err);
        return { success: false, message: "Lỗi kết nối server" };
    }
};