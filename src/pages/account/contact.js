import { mountLayout } from "../components.js";
import { $, toast } from "../utils.js";

await mountLayout({ active: "contact" });

$("#formContact").addEventListener("submit", (e)=>{
  e.preventDefault();
  const name = $("#ctName").value.trim();
  const email = $("#ctEmail").value.trim();
  const msg = $("#ctMsg").value.trim();
  if(!name || !email || !msg) return toast("Điền đủ thông tin đã 😄");
  toast("Đã gửi (demo) ✉️");
  $("#ctName").value = "";
  $("#ctEmail").value = "";
  $("#ctMsg").value = "";
});
