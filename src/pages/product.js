import { mountLayout } from "./components.js";
import { getProductDetail, getProductReviews, submitReview } from "./api.js";
import { $, $$, money, getQuery, toast } from "./utils.js";

await mountLayout({ active: "" });

// 1. Lấy ID sản phẩm từ URL (VD: product.html?id=1)
const { id } = getQuery();
const productId = id; // Dùng chung cho Review

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
    $("#title").innerText = product.TenSanPham;
    $("#priceNow").innerText = money(product.GiaGoc);
    $("#sku").innerText = product.Slug || "--";
    $("#stock").innerText = "Còn hàng"; 
    $("#paneDesc").innerHTML = product.MoTa || "Đang cập nhật mô tả...";
    
    if (images && images.length > 0) {
        $("#galleryMain").innerHTML = `<img src="${images[0].UrlAnh}" alt="main" style="width:100%; border-radius:8px" />`;
    }

    const colors = [...new Map(variants.map(v => [v.MaMauSac, { id: v.MaMauSac, name: v.TenMau, hex: v.MaHex }])).values()];
    const sizes = [...new Map(variants.map(v => [v.MaKichCo, { id: v.MaKichCo, name: v.TenKichCo }])).values()];

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
    const qtyInput = $("#qtyInput");
    $("#btnMinus")?.addEventListener("click", () => {
        let val = parseInt(qtyInput.value) || 1;
        if (val > 1) qtyInput.value = val - 1;
    });
    $("#btnPlus")?.addEventListener("click", () => {
        let val = parseInt(qtyInput.value) || 1;
        qtyInput.value = val + 1;
    });

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

    $("#btnAdd")?.addEventListener("click", () => {
        if (currentVariants.length > 0 && (!selectedColor || !selectedSize)) {
            return toast("Vui lòng chọn đầy đủ Màu sắc và Kích cỡ!");
        }
        toast(`Đã thêm ${$("#qtyInput").value} sản phẩm vào giỏ hàng 🛒`);
    });
}

function updatePrice() {
    if (!selectedColor || !selectedSize) return;
    const variant = currentVariants.find(v => v.MaMauSac === selectedColor && v.MaKichCo === selectedSize);
    
    if (variant) {
        const finalPrice = currentProduct.GiaGoc + (variant.DieuChinhGia || 0);
        $("#priceNow").innerText = money(finalPrice);
        $("#stock").innerText = `Còn ${variant.SoLuongTon || 0} sản phẩm`;
    } else {
        $("#stock").innerText = "Hết hàng phiên bản này";
    }
}

// ==========================================
// XỬ LÝ TAB MÔ TẢ & ĐÁNH GIÁ (TUẦN 4)
// ==========================================
$$(".tab").forEach(btn => {
    btn.addEventListener("click", (e) => {
        $$(".tab").forEach(b => b.style.fontWeight = "normal");
        $$(".pane").forEach(p => p.style.display = "none");
        
        e.target.style.fontWeight = "bold";
        const tabId = e.target.getAttribute("data-tab");
        
        if (tabId === "desc") $("#tabDesc").style.display = "block";
        if (tabId === "reviews") {
            $("#tabReviews").style.display = "block";
            loadReviews(); 
        }
    });
});

async function loadReviews() {
    if (!productId) return;
    const reviews = await getProductReviews(productId);
    const listEl = $("#reviewList");
    
    if (reviews.length === 0) {
        listEl.innerHTML = "<div class='smallhint'>Chưa có đánh giá nào. Hãy là người đầu tiên!</div>";
        return;
    }

    listEl.innerHTML = reviews.map(r => `
        <div style="border-bottom: 1px solid #eee; padding-bottom: 10px; margin-bottom: 10px;">
            <div style="display: flex; justify-content: space-between;">
                <b>${r.ReviewerName}</b>
                <span style="color: #f39c12;">${'★'.repeat(r.Rating)}${'☆'.repeat(5 - r.Rating)}</span>
            </div>
            <div style="margin-top: 5px;">${r.Content}</div>
            <div class="smallhint" style="margin-top: 5px; font-size: 12px;">${new Date(r.CreatedAt).toLocaleString('vi-VN')}</div>
        </div>
    `).join('');
}

$("#btnSubmitReview")?.addEventListener("click", async () => {
    if (!productId) return toast("Lỗi: Không xác định được sản phẩm!");

    const payload = {
        ProductId: parseInt(productId),
        Rating: parseInt($("#reviewStar").value),
        Content: $("#reviewContent").value.trim()
    };

    if (!payload.Content) return toast("Vui lòng nhập nội dung đánh giá!");
    if (payload.Rating < 1 || payload.Rating > 5) return toast("Số sao phải từ 1 đến 5!");

    const res = await submitReview(payload);
    
    if (res.success) {
        toast("Đánh giá thành công! ✅");
        $("#reviewContent").value = ""; 
        $("#reviewStar").value = "5";
        loadReviews(); 
    } else {
        toast("Lỗi: " + res.message + " ❌");
    }
});

init();