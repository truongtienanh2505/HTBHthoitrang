import { mountLayout, productCardHtml, paginationHtml } from "../components.js";
import { listProducts } from "../api.js";
import { $, $$ } from "../utils.js";

await mountLayout({ active: "best" });

const state = { page: 1, pageSize: 12 };

async function render(){
  const res = await listProducts({ sort:"best", page: state.page, pageSize: state.pageSize });
  $("#grid").innerHTML = res.items.map(productCardHtml).join("");
  $("#pager").innerHTML = paginationHtml(res);
  $$(".pg", $("#pager")).forEach(btn=>{
    btn.addEventListener("click", ()=>{
      const p = Number(btn.getAttribute("data-pg"));
      if(!p || btn.disabled) return;
      state.page = p;
      render();
      window.scrollTo({ top:0, behavior:"smooth" });
    });
  });
}
render();
