import { mountLayout } from "../components.js";
import { listBlog } from "../api.js";
import { $, getQuery } from "../utils.js";

await mountLayout({ active: "blog" });

const { slug } = getQuery();
const data = await getPostBySlug(slug);

if(!data){
  $("#postRoot").innerHTML = `<div class="panel"><div class="panel__head">Không tìm thấy</div><div class="panel__body">Bài viết không tồn tại.</div></div>`;
} else {
  const { post, category } = data;
  $("#postRoot").innerHTML = `
    <div class="breadcrumbs">
      <a href="../index.html">Trang chủ</a><span>›</span><a href="index.html">Blog</a><span>›</span><span>${post.TieuDe}</span>
    </div>

    <div class="panel">
      <div class="panel__head">${post.TieuDe}</div>
      <div class="panel__body">
        <div class="smallhint">${category?.TenChuyenMuc||""} • ${post.XuatBanLuc||""}</div>
        <div class="hr"></div>
        <div class="pre">${post.NoiDung||""}</div>
      </div>
    </div>
  `;
}
