import { mountLayout, productCardHtml, paginationHtml } from "./components.js";
import { listCategories, listProducts } from "./api.js";
import { $, $$, getQuery, setQuery, toast } from "./utils.js";

// Nạp Header và Footer
await mountLayout({ active: "products" });

// 1. Quản lý trạng thái từ URL
const q = getQuery();
const state = {
  search: q.search || "",
  cat: q.cat || "",
  sort: q.sort || "new",
  minPrice: q.minPrice || "",
  maxPrice: q.maxPrice || "",
  page: Number(q.page || 1),
  pageSize: 12
};

function syncUrl(){
  setQuery({
    search: state.search,
    cat: state.cat,
    sort: state.sort,
    minPrice: state.minPrice,
    maxPrice: state.maxPrice,
    page: state.page
  });
}

// 2. Hiển thị bộ lọc
async function renderFilters(){
  const cats = await listCategories();
  const catSelect = $("#catSelect");
  if (catSelect) {
      const options = ['<option value="">Tất cả danh mục</option>']
        .concat(cats.map(c => `<option value="${c.Slug}">${c.TenDanhMuc}</option>`));
      catSelect.innerHTML = options.join("");
      catSelect.value = state.cat;
  }

  if ($("#searchBox")) $("#searchBox").value = state.search;
  if ($("#sortSelect2")) $("#sortSelect2").value = state.sort;
  if ($("#minPrice")) $("#minPrice").value = state.minPrice;
  if ($("#maxPrice")) $("#maxPrice").value = state.maxPrice;
}

// 3. Hiển thị danh sách sản phẩm lấy từ C#
async function renderList(){
  const grid = $("#grid");
  if (grid) grid.innerHTML = `<div style="text-align:center; width:100%; padding: 40px;">Đang tải dữ liệu...</div>`;

  try {
      const res = await listProducts(state);

      if ($("#resultCount")) {
          $("#resultCount").textContent = `${(res.total || 0).toLocaleString("vi-VN")} sản phẩm`;
      }
      
      if (grid) {
          if (res.total === 0) {
              grid.innerHTML = `<div style="text-align:center; width:100%; padding: 40px; color:red;">Không tìm thấy sản phẩm nào!</div>`;
          } else {
              grid.innerHTML = res.items.map(productCardHtml).join("");
          }
      }

      if ($("#pager")) {
          $("#pager").innerHTML = paginationHtml(res);
          $$(".pg", $("#pager")).forEach(btn => {
            btn.addEventListener("click", (e) => {
              e.preventDefault();
              const p = Number(btn.getAttribute("data-pg"));
              if (!p || btn.classList.contains("active")) return;
              state.page = p;
              syncUrl();
              renderList();
              window.scrollTo({ top: 0, behavior: "smooth" });
            });
          });
      }
  } catch (err) {
      console.error("Lỗi:", err);
  }
}

// 4. Gắn sự kiện nút bấm
function bind(){
  $("#btnApply")?.addEventListener("click", () => {
    state.search = $("#searchBox")?.value.trim() || "";
    state.cat = $("#catSelect")?.value || "";
    state.sort = $("#sortSelect2")?.value || "new";
    state.minPrice = $("#minPrice")?.value.trim() || "";
    state.maxPrice = $("#maxPrice")?.value.trim() || "";
    state.page = 1;
    syncUrl();
    renderList();
  });

  $("#btnReset")?.addEventListener("click", () => {
    state.search = "";
    state.cat = "";
    state.sort = "new";
    state.minPrice = "";
    state.maxPrice = "";
    state.page = 1;
    syncUrl();
    renderFilters();
    renderList();
    if (typeof toast === "function") toast("Đã đặt lại bộ lọc ✨");
  });
}

// Khởi chạy
async function init() {
    await renderFilters();
    bind();
    await renderList();
}
init();