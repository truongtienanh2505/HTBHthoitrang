// Đảm bảo cổng này giống với cổng API C# của bạn đang chạy (hiện tại là 5157)
const API_URL = 'http://localhost:5157/api/Search/search';

// Bắt sự kiện nhấn Enter ở ô tìm kiếm
function handleKeyPress(event) {
    if (event.key === 'Enter') {
        executeSearch();
    }
}

// Hàm chính: Lấy dữ liệu và gọi API
async function executeSearch() {
    const container = document.getElementById('productResults');
    const resultText = document.getElementById('resultText');
    container.innerHTML = '<div class="loading">Đang tìm kiếm sản phẩm...</div>';

    // 1. Lấy dữ liệu từ giao diện
    const keyword = document.getElementById('keyword').value.trim();
    const minPrice = document.getElementById('minPrice').value;
    const maxPrice = document.getElementById('maxPrice').value;
    const color = document.getElementById('colorSelect').value;
    const size = document.getElementById('sizeSelect').value;

    // 2. Tạo URL parameters (Query String)
    let queryParams = new URLSearchParams();
    if (keyword) queryParams.append('Keyword', keyword);
    if (minPrice) queryParams.append('MinPrice', minPrice);
    if (maxPrice) queryParams.append('MaxPrice', maxPrice);
    if (color) queryParams.append('MaMauSac', color);
    if (size) queryParams.append('MaKichCo', size);

    try {
        // 3. Gọi API Backend C#
        const response = await fetch(`${API_URL}?${queryParams.toString()}`);
        
        if (!response.ok) {
            throw new Error('Lỗi mạng hoặc Server C# chưa bật');
        }

        const result = await response.json();
        
        // Cập nhật câu thông báo
        resultText.innerText = keyword 
            ? `Kết quả tìm kiếm cho: "${keyword}" (${result.totalItems} sản phẩm)`
            : `Tất cả sản phẩm (${result.totalItems} sản phẩm)`;

        // 4. Vẽ dữ liệu ra màn hình
        renderProducts(result.data);

    } catch (error) {
        console.error("Lỗi:", error);
        container.innerHTML = `<div class="loading" style="color: #e74c3c;">Không thể kết nối tới Server. Vui lòng kiểm tra lại Backend!</div>`;
    }
}

// Hàm phụ: Vẽ HTML cho từng sản phẩm
function renderProducts(products) {
    const container = document.getElementById('productResults');
    container.innerHTML = ''; // Xóa chữ "Đang tìm kiếm..."

    if (!products || products.length === 0) {
        container.innerHTML = '<div class="loading">Không tìm thấy sản phẩm nào phù hợp với bộ lọc của bạn.</div>';
        return;
    }

    products.forEach(p => {
        // Format tiền tệ chuẩn Việt Nam
        const priceFormatted = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(p.giaGoc);
        // Nếu không có ảnh thì dùng ảnh mặc định
        const imgUrl = p.anhDaiDien || 'https://images.unsplash.com/photo-1523381210434-271e8be1f52b?ixlib=rb-4.0.3&auto=format&fit=crop&w=500&q=60';

        const card = document.createElement('div');
        card.className = 'product-card';
        card.innerHTML = `
            <img src="${imgUrl}" alt="${p.tenSanPham}">
            <h4 class="product-title" title="${p.tenSanPham}">${p.tenSanPham}</h4>
            <div class="product-price">${priceFormatted}</div>
            <button class="btn-view">Xem Chi Tiết</button>
        `;
        container.appendChild(card);
    });
}

// Chạy tìm kiếm mặc định ngay khi vừa load trang xong
window.onload = () => {
    executeSearch();
};