import { mountLayout, productCardHtml } from "../components.js";
import { listCategories, listProducts } from "../api.js";
import { $, money } from "../utils.js";

await mountLayout({ active: "collections" });

const cats = await listCategories();
$("#colCats").innerHTML = cats.map(c=>`
  <a class="card" href="products.html?cat=${encodeURIComponent(c.Slug)}" style="text-decoration:none;color:inherit">
    <div class="card__body">
      <div class="tagrow"><span class="tag">Danh mục</span></div>
      <h3 class="card__title">${c.TenDanhMuc}</h3>
      <div class="smallhint">Xem sản phẩm thuộc danh mục này</div>
    </div>
  </a>
`).join("");

const sale = await listProducts({ sort:"sale", page:1, pageSize:8 });
$("#colPromos").innerHTML = sale.items.map(p=>`
  <a class="card" href="product.html?slug=${encodeURIComponent(p.Slug)}" style="text-decoration:none;color:inherit">
    <div class="card__img"><img alt="${p.TenSanPham}" src="${p.AnhDaiDien||""}"></div>
    <div class="card__body">
      <div class="tagrow">${p._promo ? `<span class="tag sale">${p._promo.LoaiGiamGia}</span>` : ""}</div>
      <h3 class="card__title">${p.TenSanPham}</h3>
      <div class="card__price"><div class="price">${money(p._minPrice)}</div></div>
    </div>
  </a>
`).join("");
