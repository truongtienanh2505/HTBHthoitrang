// Chọn phần tử DOM nhanh (Giống jQuery)
export const $ = document.querySelector.bind(document);
export const $$ = document.querySelectorAll.bind(document);

// Hàm format tiền tệ VNĐ
export const money = (amount) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount || 0);
};

// Hàm lấy tham số trên thanh URL (Ví dụ lấy id từ product.html?id=1)
export const getQuery = () => {
    const params = new URLSearchParams(window.location.search);
    return Object.fromEntries(params.entries());
};
export const setQuery = (obj) => {
    const params = new URLSearchParams();
    for (const [key, value] of Object.entries(obj)) {
      if (value !== undefined && value !== null && value !== "") {
        params.set(key, value);
      }
    }
    const newUrl = `${window.location.pathname}?${params.toString()}`;
    window.history.pushState({}, "", newUrl);
  };
// Hàm hiển thị thông báo góc màn hình
export const toast = (message) => {
    // Tạm thời dùng alert để chắc chắn không bị lỗi giao diện
    alert(message); 
};