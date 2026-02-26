import { mountLayout } from "../components.js";
import { getProductDetail } from "../api.js";
import { $, $$, money, getQuery, toast } from "../utils.js";

await mountLayout({ active: "" });

// 1. Lấy ID sản phẩm từ URL (VD: product.html?id=1)
const { id } = getQuery();

let currentProduct = null;
let currentVariants = [];
let selectedColor = null;
let selectedSize = null;

async function init() {
    if (!id) return $("#pdpRoot").innerHTML = "Không tìm thấy mã sản phẩm.";
    
    try {
        const data = await getProductDetail(id);
        currentProduct = data.product;
        currentVariants = data.variants || [];
        
        renderUI(data);
        attachEvents();
    } catch (err) {
        $("#pdpRoot").innerHTML = `<div class="panel__body">Lỗi: ${err.message}</div>`;
    }
}

function renderUI({ product, variants, images }) {
    // Render thông tin cơ bản
    $("#title").innerText = product.TenSanPham;
    $("#priceNow").innerText = money(product.GiaGoc);
    $("#sku").innerText = product.Slug || "--";
    $("#stock").innerText = "Còn hàng"; // Có thể tính tổng tồn kho từ variants sau
    $("#paneDesc").innerHTML = product.MoTa || "Đang cập nhật mô tả...";
    
    // Render Ảnh
    if (images && images.length > 0) {
        $("#galleryMain").innerHTML = `<img src="${images[0].UrlAnh}" alt="main" style="width:100%; border-radius:8px" />`;
    }

    // Render Nút chọn Màu (Lọc ra các màu không trùng lặp)
    const colors = [...new Map(variants.map(v => [v.MaMauSac, { id: v.MaMauSac, name: v.TenMau, hex: v.MaHex }])).values()];
    
    // Render Nút chọn Size (Lọc ra các size không trùng lặp)
    const sizes = [...new Map(variants.map(v => [v.MaKichCo, { id: v.MaKichCo, name: v.TenKichCo }])).values()];

    // Tìm thẻ chứa Màu và Size (Dựa theo HTML của bạn)
    // Giả sử HTML của bạn có 2 div kế tiếp chữ "Màu" và "Kích cỡ", ta sẽ render động vào đó:
    const labels = $$('.label');
    let colorContainer = null;
    let sizeContainer = null;
    
    labels.forEach(lbl => {
        if (lbl.innerText.includes('Màu')) colorContainer = lbl.nextElementSibling;
        if (lbl.innerText.includes('Kích cỡ')) sizeContainer = lbl.nextElementSibling;
    });

    if (colorContainer) {
        colorContainer.innerHTML = colors.map(c => `
            <button class="btn btn--ghost variant-color" data-id="${c.id}" style="margin-right:8px; border-color:${c.hex}">
                ${c.name}
            </button>
        `).join('');
    }

    if (sizeContainer) {
        sizeContainer.innerHTML = sizes.map(s => `
            <button class="btn btn--ghost variant-size" data-id="${s.id}" style="margin-right:8px;">
                ${s.name}
            </button>
        `).join('');
    }
}

function attachEvents() {
    // Sự kiện tăng giảm số lượng
    const qtyInput = $("#qtyInput");
    $("#btnMinus")?.addEventListener("click", () => {
        let val = parseInt(qtyInput.value) || 1;
        if (val > 1) qtyInput.value = val - 1;
    });
    $("#btnPlus")?.addEventListener("click", () => {
        let val = parseInt(qtyInput.value) || 1;
        qtyInput.value = val + 1;
    });

    // Sự kiện chọn Màu
    $$(".variant-color").forEach(btn => {
        btn.addEventListener("click", (e) => {
            $$(".variant-color").forEach(b => b.classList.remove("btn--primary"));
            $$(".variant-color").forEach(b => b.classList.add("btn--ghost"));
            btn.classList.remove("btn--ghost");
            btn.classList.add("btn--primary");
            
            selectedColor = parseInt(btn.dataset.id);
            updatePrice();
        });
    });

    // Sự kiện chọn Size
    $$(".variant-size").forEach(btn => {
        btn.addEventListener("click", (e) => {
            $$(".variant-size").forEach(b => b.classList.remove("btn--primary"));
            $$(".variant-size").forEach(b => b.classList.add("btn--ghost"));
            btn.classList.remove("btn--ghost");
            btn.classList.add("btn--primary");
            
            selectedSize = parseInt(btn.dataset.id);
            updatePrice();
        });
    });

    // Nút Thêm vào giỏ
    $("#btnAdd")?.addEventListener("click", () => {
        if (currentVariants.length > 0 && (!selectedColor || !selectedSize)) {
            return toast("Vui lòng chọn đầy đủ Màu sắc và Kích cỡ!");
        }
        toast(`Đã thêm ${$("#qtyInput").value} sản phẩm vào giỏ hàng 🛒`);
    });
}

function updatePrice() {
    if (!selectedColor || !selectedSize) return;
    
    // Tìm biến thể khớp với Màu và Size đã chọn
    const variant = currentVariants.find(v => v.MaMauSac === selectedColor && v.MaKichCo === selectedSize);
    
    if (variant) {
        // Nếu biến thể có cài đặt giá riêng (hoặc cộng thêm tiền)
        const finalPrice = currentProduct.GiaGoc + (variant.DieuChinhGia || 0);
        $("#priceNow").innerText = money(finalPrice);
        $("#stock").innerText = `Còn ${variant.SoLuongTon || 0} sản phẩm`;
    } else {
        $("#stock").innerText = "Hết hàng phiên bản này";
    }
}

init();