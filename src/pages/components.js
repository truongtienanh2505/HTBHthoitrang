import { $ } from "./utils.js";

// Hàm load Header và Footer
export const mountLayout = async ({ active }) => {
    const top = $("#layoutTop");
    if (top) {
        top.innerHTML = `
            <div style="padding: 15px 20px; background: #333; color: white; margin-bottom: 20px; display: flex; gap: 15px;">
                <b>SHOP THỜI TRANG</b>
                <a href="../index.html" style="color: white; text-decoration: none;">Trang chủ</a>
                <a href="profile.html" style="color: white; text-decoration: none;">Tài khoản</a>
            </div>`;
    }

    const bottom = $("#layoutBottom");
    if (bottom) {
        bottom.innerHTML = `
            <div style="padding: 15px; background: #eee; text-align: center; margin-top: 40px; border-top: 1px solid #ccc;">
                © 2026 Bản quyền thuộc về Team
            </div>`;
    }
};

// Hàm vẽ thẻ Sản phẩm (Cho Tuần 2)
export const productCardHtml = (p) => {
    // Ưu tiên đọc biến từ C# gửi xuống, nếu không có thì fallback về biến cũ
    const id = p.MaSanPham || p.id;
    const name = p.TenSanPham || p.name || "Sản phẩm";
    const image = p.AnhDaiDien || p.UrlAnh || p.image || "https://via.placeholder.com/300";
    const price = p.GiaGoc || p.price || 0;

    return `
        <a class="card" href="product.html?id=${id}" style="text-decoration:none; color:inherit; border: 1px solid #eaeaea; padding: 10px; border-radius: 8px; display: block; text-align: center;">
            <div class="card__img"><img alt="${name}" src="${image}" style="max-width: 100%; height: 200px; object-fit: cover; border-radius: 4px;"></div>
            <div class="card__body" style="margin-top: 10px;">
                <h3 class="card__title" style="font-size: 16px; margin-bottom: 5px;">${name}</h3>
                <div class="card__price"><div class="price" style="color: red; font-weight: bold;">${price.toLocaleString('vi-VN')} VNĐ</div></div>
            </div>
        </a>
    `;
};

// Hàm phân trang tạm
export const paginationHtml = (res) => {
    return `<div style="text-align:center; margin-top: 20px; padding: 10px; background: #f9f9f9;">Trang 1</div>`;
};