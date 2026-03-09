const API_URL = 'http://localhost:5157/api/Auth';

// 1. XỬ LÝ ĐĂNG KÝ
async function handleRegister(event) {
    event.preventDefault(); // Ngăn form tự động reload trang

    const fullName = document.getElementById('fullName').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    try {
        const response = await fetch(`${API_URL}/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ fullName, email, password })
        });

        const data = await response.json();
        
        if (response.ok) {
            alert(data.message || "Đăng ký thành công!");
            window.location.href = 'login.html'; // Chuyển sang trang đăng nhập
        } else {
            alert("Lỗi: " + (data.message || "Đăng ký thất bại"));
        }
    } catch (error) {
        alert("Lỗi kết nối đến máy chủ C#!");
        console.error(error);
    }
}

// 2. XỬ LÝ ĐĂNG NHẬP THƯỜNG
async function handleBasicLogin(event) {
    event.preventDefault();

    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    try {
        const response = await fetch(`${API_URL}/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });

        const data = await response.json();

        if (response.ok) {
            localStorage.setItem('token', data.token); // Lưu token vào trình duyệt
            alert("Đăng nhập hệ thống thành công!");
            // window.location.href = 'index.html'; // Mở khóa dòng này khi có trang chủ
        } else {
            alert("Lỗi: " + (data.message || "Sai email hoặc mật khẩu"));
        }
    } catch (error) {
        alert("Lỗi kết nối đến máy chủ C#!");
        console.error(error);
    }
}

// 3. XỬ LÝ ĐĂNG NHẬP GOOGLE (Hàm này được Google tự động gọi)
async function handleGoogleResponse(response) {
    try {
        const res = await fetch(`${API_URL}/google-login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ token: response.credential })
        });

        const data = await res.json();

        if (res.ok) {
            localStorage.setItem('token', data.token);
            alert("Đăng nhập bằng Google thành công!");
        } else {
            alert("Đăng nhập Google thất bại: " + data.message);
        }
    } catch (error) {
        alert("Lỗi kết nối đến máy chủ C#!");
        console.error(error);
    }
}