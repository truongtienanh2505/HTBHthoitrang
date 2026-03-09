import { mountLayout } from "../components.js";
import { listBlog } from "../api.js";
import { $, getQuery } from "../utils.js";

await mountLayout({ active: "blog" });

const { cat } = getQuery();
const data = await listBlog({ cat });

$("#blogRoot").innerHTML = `
  <div class="breadcrumbs">
    <a href="../index.html">Trang chủ</a><span>›</span><span>Blog</span>
  </div>

  <div class="page">
    <div class="panel">
      <div class="panel__head">Chuyên mục</div>
      <div class="panel__body">
        <a class="menu__item" href="index.html">Tất cả</a>
        ${data.categories.map(c=>`
          <a class="menu__item" href="index.html?cat=${encodeURIComponent(c.Slug)}">${c.TenChuyenMuc}</a>
        `).join("")}
      </div>
    </div>

    <div class="panel">
      <div class="panel__head">Bài viết (${data.posts.length})</div>
      <div class="panel__body">
        <div class="blog">
          ${data.posts.map(p=>`
            <article class="blogcard">
              <div class="blogcard__img"><img alt="img" src="${p.AnhDaiDien}" style="width:100%;height:100%;object-fit:cover;display:block"/></div>
              <div class="blogcard__body">
                <div class="blogcard__tag">${(data.categories.find(x=>x.MaChuyenMuc===p.MaChuyenMuc)?.TenChuyenMuc)||""}</div>
                <h3>${p.TieuDe}</h3>
                <p>${p.TomTat||""}</p>
                <a class="link" href="post.html?slug=${encodeURIComponent(p.Slug)}">Đọc tiếp</a>
              </div>
            </article>
          `).join("")}
        </div>
      </div>
    </div>
  </div>
`;
