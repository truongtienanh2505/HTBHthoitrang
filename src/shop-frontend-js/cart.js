// Dữ liệu giả lập (Mock Data) vì chưa nối C#
let cart = [
    { id: 1, name: "Áo Thun Cổ Tròn Basic", price: 150000, qty: 2, img: "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=200" },
    { id: 2, name: "Quần Jean Nam Cao Cấp", price: 350000, qty: 1, img: "https://images.unsplash.com/photo-1542272604-787c3835535d?w=200" }
];

// Định dạng tiền tệ
const formatPrice = (price) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(price);

// 1. Vẽ UI Giỏ hàng
function renderCart() {
    const container = document.getElementById('cartItemsContainer');
    const totalEl = document.getElementById('totalPrice');
    container.innerHTML = '';
    let total = 0;

    if (cart.length === 0) {
        container.innerHTML = '<p style="text-align:center; color:#888;">Giỏ hàng của bạn đang trống.</p>';
        totalEl.innerText = '0 ₫';
        return;
    }

    cart.forEach((item, index) => {
        total += item.price * item.qty;
        
        container.innerHTML += `
            <div class="cart-item">
                <div class="item-info">
                    <img src="${item.img}" alt="${item.name}">
                    <div>
                        <h4 style="margin: 0 0 5px 0;">${item.name}</h4>
                        <div style="color: #7f8c8d; font-size: 14px;">${formatPrice(item.price)} / sp</div>
                    </div>
                </div>
                <div class="qty-controls">
                    <button class="qty-btn" onclick="updateQty(${index}, -1)">-</button>
                    <span style="width: 30px; text-align: center; font-weight: bold;">${item.qty}</span>
                    <button class="qty-btn" onclick="updateQty(${index}, 1)">+</button>
                </div>
                <div class="item-price">${formatPrice(item.price * item.qty)}</div>
                <button class="remove-btn" onclick="removeItem(${index})">X</button>
            </div>
        `;
    });

    totalEl.innerText = formatPrice(total);
}

// 2. Logic Tăng / Giảm số lượng
function updateQty(index, change) {
    if (cart[index].qty + change > 0) {
        cart[index].qty += change;
        renderCart();
    }
}

// 3. Logic Xóa sản phẩm
function removeItem(index) {
    cart.splice(index, 1);
    renderCart();
}

// 4. Logic UI Checkout
function handleCheckout(event) {
    event.preventDefault();
    if (cart.length === 0) {
        alert("Giỏ hàng trống! Vui lòng chọn sản phẩm trước khi đặt hàng.");
        return;
    }

    // Lấy thông tin
    const name = document.getElementById('custName').value;
    
    // Giả lập gọi API thành công
    alert(`🎉 Đặt hàng thành công!\nCảm ơn ${name} đã mua sắm.\nMã đơn hàng của bạn là: #DH0099`);
    
    // Xóa giỏ hàng và render lại
    cart = [];
    renderCart();
    
    // Chuyển sang trang Tracking
    window.location.href = "tracking.html";
}

// Chạy lần đầu
window.onload = renderCart;