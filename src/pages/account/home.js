import { mountLayout, productCardHtml } from "../components.js";
import { getBanners, listCategories, listProducts } from "../api.js";
import { $, money, toast } from "../utils.js";

await mountLayout({ active: "home" });

async function render(){
  const banners = await getBanners();
  const cats = await listCategories();
  const prod = await listProducts({ sort: "featured", pageSize: 8, page: 1 });

  // Banner
  const b = banners[0];
  if(b){
    $("#bannerWrap").innerHTML = `
      <section class="banner">
        <div>
          <div class="chip">New Drop ✨</div>
          <h2 class="banner__title">${b.TieuDe || "Ưu đãi hôm nay"}</h2>
          <p class="banner__desc">Demo banner lấy từ bảng <b>BannerTrangChu</b>. Click là qua trang sản phẩm.</p>
          <div class="banner__actions">
            <a class="btn btn--primary" href="${b.UrlLienKet || "products.html"}">Xem ngay</a>
            <a class="btn btn--ghost" href="products.html?sort=sale">Săn sale</a>
          </div>
        </div>
        <div class="banner__img"><img alt="Banner" src="${b.UrlAnh}"/></div>
      </section>
    `;
  }

  // Quick categories
  $("#quickCats").innerHTML = cats.map(c=>`
    <a class="qcat" href="products.html?cat=${encodeURIComponent(c.Slug)}">${c.TenDanhMuc}</a>
  `).join("");

  // Featured
  $("#gridFeatured").innerHTML = prod.items.map(productCardHtml).join("");

  // Best / sale blocks (simple)
  const best = await listProducts({ sort:"featured", pageSize: 8, page: 1, cat: "" });
  const sale = await listProducts({ sort:"sale", pageSize: 8, page: 1 });
  $("#gridBest").innerHTML = best.items.slice(0,8).map(productCardHtml).join("");
  $("#gridSale").innerHTML = sale.items.slice(0,8).map(productCardHtml).join("");

  // Newsletter quick
  $("#btnGotoProducts")?.addEventListener("click", ()=> location.href="products.html");
}
render();
