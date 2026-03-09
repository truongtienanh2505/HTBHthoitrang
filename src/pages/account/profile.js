import { mountLayout } from "../components.js";
import { getMe } from "../api.js";
import { $, $$, toast } from "../utils.js";

// CHÚ Ý: Cổng 7127 là cổng Backend C# của nhóm bạn
const API_URL = "https://localhost:7127/api";

await mountLayout({ active: "" });

// 1. Lấy thông tin user (Từ API C#)
const me = await getMe();

if (!me) {
  $("#profileRoot").innerHTML = `
    <div class="panel">
      <div class="panel__head">Tài khoản</div>
      <div class="panel__body">Cần đăng nhập. <a class="link" href="../login.html">Đăng nhập</a></div>
    </div>`;
} else {
  render(me);
  loadAddresses(); // Tải danh sách địa chỉ M2
}

// 2. Render Giao diện (Profile + Sổ địa chỉ)
function render(me) {
  $("#profileRoot").innerHTML = `
    <div class="breadcrumbs">
      <a href="../index.html">Trang chủ</a><span>›</span><span>Tài khoản</span>
    </div>

    <div class="page">
      <div style="flex: 1; display: flex; flex-direction: column; gap: 20px;">
          
          <div class="panel">
            <div class="panel__head">Hồ sơ cá nhân</div>
            <div class="panel__body">
              <div class="field"><label class="label">Họ tên</label><input class="input" id="name" value="${me.HoTen||""}"></div>
              <div class="row">
                <div class="field"><label class="label">Email</label><input class="input" id="email" value="${me.Email||""}" disabled></div>
                <div class="field"><label class="label">SĐT</label><input class="input" id="phone" value="${me.SoDienThoai||""}"></div>
              </div>
              <button class="btn btn--primary" id="btnSaveProfile">Lưu Hồ Sơ</button>
            </div>
          </div>

          <div class="panel">
            <div class="panel__head">Sổ địa chỉ</div>
            <div class="panel__body">
                <div id="addressList" style="margin-bottom: 20px;">Đang tải...</div>
                <div class="hr"></div>
                <h3 style="margin-top: 20px; font-size: 16px;">+ Thêm địa chỉ mới</h3>
                
                <div class="row">
                    <div class="field"><label class="label">Tên người nhận</label><input class="input" id="addrName" placeholder="Ví dụ: Nguyễn Văn A"></div>
                    <div class="field"><label class="label">Số điện thoại</label><input class="input" id="addrPhone" placeholder="Ví dụ: 090123..."></div>
                </div>
                <div class="row">
                    <div class="field"><label class="label">Tỉnh/Thành phố</label><input class="input" id="addrCity" placeholder="Ví dụ: TP.HCM"></div>
                    <div class="field"><label class="label">Quận/Huyện</label><input class="input" id="addrDist" placeholder="Ví dụ: Quận 1"></div>
                </div>
                <div class="field">
                    <label class="label">Địa chỉ chi tiết (Số nhà, tên đường)</label>
                    <input class="input" id="addrDetail" placeholder="Ví dụ: 123 Đường Lê Lợi...">
                </div>
                <div class="field" style="display: flex; align-items: center; gap: 8px;">
                    <input type="checkbox" id="addrDefault" style="width: 16px; height: 16px;"> 
                    <label for="addrDefault" style="cursor: pointer;">Đặt làm địa chỉ mặc định</label>
                </div>
                <button class="btn btn--primary" id="btnSaveAddr" style="margin-top: 10px;">Lưu địa chỉ</button>
            </div>
          </div>

      </div>

      <div class="panel" style="width: 250px; flex-shrink: 0;">
        <div class="panel__head">Menu</div>
        <div class="panel__body">
          <a class="menu__item" href="orders.html">Đơn hàng</a>
          <a class="menu__item" href="../wishlist.html">Wishlist</a>
          <a class="menu__item" href="notifications.html">Thông báo</a>
        </div>
      </div>
    </div>
  `;

  // Sự kiện nút
  $("#btnSaveProfile").addEventListener("click", () => {
    toast("Chức năng đang cập nhật API C# ✅");
  });

  $("#btnSaveAddr").addEventListener("click", async () => {
    const payload = {
        contactName: $("#addrName").value,
        contactPhone: $("#addrPhone").value,
        province: $("#addrCity").value,
        district: $("#addrDist").value,
        ward: "", 
        addressLine: $("#addrDetail").value,
        isDefault: $("#addrDefault").checked
    };

    if(!payload.contactName || !payload.contactPhone || !payload.addressLine) {
        return toast("Vui lòng nhập đủ thông tin bắt buộc!");
    }

    try {
        const res = await fetch(`${API_URL}/user-addresses`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });
        const data = await res.json();
        
        if (res.ok) {
            toast("Thêm địa chỉ thành công! ✅");
            // Xóa rỗng form
            $("#addrName").value = ""; $("#addrPhone").value = "";
            $("#addrCity").value = ""; $("#addrDist").value = "";
            $("#addrDetail").value = ""; $("#addrDefault").checked = false;
            loadAddresses(); 
        } else {
            toast("Lỗi: " + data.message);
        }
    } catch (e) {
        console.error(e);
        toast("Không kết nối được server ❌");
    }
  });
}

// 3. Tải danh sách địa chỉ
async function loadAddresses() {
    try {
        const res = await fetch(`${API_URL}/user-addresses`);
        if (!res.ok) throw new Error("Lỗi tải địa chỉ");
        const list = await res.json();

        if (list.length === 0) {
            $("#addressList").innerHTML = "<div class='smallhint'>Chưa có địa chỉ nào trong sổ.</div>";
            return;
        }

        $("#addressList").innerHTML = list.map(addr => `
            <div style="border: 1px solid var(--line); padding: 12px; border-radius: 8px; margin-bottom: 10px;">
                <div style="display: flex; justify-content: space-between; margin-bottom: 4px;">
                    <b style="font-size: 15px;">${addr.contactName} | ${addr.contactPhone}</b>
                    ${addr.isDefault ? `<span class="tag" style="background: var(--primary); color: white;">Mặc định</span>` : ``}
                </div>
                <div class="smallhint">${addr.addressLine}, ${addr.district}, ${addr.province}</div>
            </div>
        `).join('');
    } catch (error) {
        $("#addressList").innerHTML = "<div class='smallhint' style='color: red;'>Lỗi kết nối hoặc chưa có dữ liệu.</div>";
    }
}