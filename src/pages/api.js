// Khai báo đúng cổng của C#
const API_BASE_URL = "http://localhost:5157/api";

// ==========================================
// API TUẦN 1: Lấy thông tin User
// ==========================================
export const getMe = async () => {
    try {
        const res = await fetch(`${API_BASE_URL}/users/me`, {
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
    const res = await fetch(`${API_BASE_URL}/products/${id}`);
    if (!res.ok) throw new Error("Không tìm thấy sản phẩm");
    return res.json();
};

// ==========================================
// API TUẦN 4: BLOG TIN TỨC
// ==========================================
export const listBlog = async ({ cat } = {}) => {
    let url = `${API_BASE_URL}/articles`;
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
        const res = await fetch(`${API_BASE_URL}/articles/${slug}`);
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
export const getProductReviews = async (productId) => {
    try {
        const res = await fetch(`${API_BASE_URL}/reviews/product/${productId}`);
        if (!res.ok) return [];
        return await res.json();
    } catch (err) {
        console.error("Lỗi lấy đánh giá:", err);
        return [];
    }
};

export const submitReview = async (payload) => {
    try {
        const res = await fetch(`${API_BASE_URL}/reviews`, {
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

// ==========================================
// CÁC HÀM DÀNH CHO TRANG CHỦ & DANH SÁCH
// ==========================================

// 1. Hàm lấy Banner
export async function getBanners() {
    try {
        const res = await fetch(`${API_BASE_URL}/banners`);
        if (!res.ok) return [];
        return await res.json();
    } catch (err) {
        console.error("Lỗi tải Banner:", err);
        return [];
    }
}

// 2. Hàm lấy Danh Mục
export async function listCategories() {
    try {
        const res = await fetch(`${API_BASE_URL}/categories`); 
        if (!res.ok) return [];
        return await res.json();
    } catch (err) {
        console.error("Lỗi tải Danh mục:", err);
        return [];
    }
}

// 3. Hàm lấy Danh sách Sản phẩm (Bản tối ưu lọc tham số rỗng)
export async function listProducts(params = {}) {
    const url = new URL(`${API_BASE_URL}/products`);
    
    // Chỉ thêm vào URL những tham số có giá trị thật sự để tránh lỗi 400
    Object.keys(params).forEach(key => {
        const val = params[key];
        if (val !== undefined && val !== null && val !== "") {
            url.searchParams.append(key, val);
        }
    });

    try {
        const res = await fetch(url);
        if (!res.ok) return { total: 0, items: [] };
        return await res.json();
    } catch (err) {
        console.error("Lỗi tải Danh sách sản phẩm:", err);
        return { total: 0, items: [] };
    }
}