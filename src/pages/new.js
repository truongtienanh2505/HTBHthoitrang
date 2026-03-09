import { mountLayout, productCardHtml, paginationHtml } from "./components.js";
import { listProducts } from "./api.js";
import { $, $$ } from "./utils.js";

// Nạp Header và Footer
await mountLayout({ active: "new" });

const state = { page: 1, pageSize: 12 };

async function render(){
  const grid = $("#grid");
  if (grid) grid.innerHTML = `<div style="text-align:center; width:100%; padding: 40px;">Đang tải sản phẩm mới...</div>`;

  try {
      // Gọi API lấy sản phẩm với tham số sort="new"
      const res = await listProducts({ sort: "new", page: state.page, pageSize: state.pageSize });
      
      if (grid) {
          grid.innerHTML = res.items.map(productCardHtml).join("");
      }

      const pager = $("#pager");
      if (pager) {
          pager.innerHTML = paginationHtml(res);
          $$(".pg", pager).forEach(btn => {
            btn.addEventListener("click", () => {
              const p = Number(btn.getAttribute("data-pg"));
              if (!p || btn.disabled) return;
              state.page = p;
              render();
              window.scrollTo({ top: 0, behavior: "smooth" });
            });
          });
      }
  } catch (err) {
      console.error("Lỗi tải sản phẩm mới:", err);
  }
}

render();