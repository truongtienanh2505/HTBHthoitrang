const express = require('express');
const sql = require('mssql');
const cors = require('cors');

const app = express();
app.use(express.json());
app.use(cors());

// 1. CẤU HÌNH KẾT NỐI (Điền trực tiếp giống file setup lúc nãy)
const config = {
    user: 'sa',                      // User mặc định
    password: 'nhapmatkhaudungvoday',    // <--- THAY PASS SQL CỦA BẠN VÀO ĐÂY !!!
    server: 'localhost',
    database: 'HeThongBanHangThoiTrangDB', // Tên DB chuẩn
    options: { encrypt: false, trustServerCertificate: true }
};

// Kết nối SQL
sql.connect(config)
    .then(() => console.log('✅ Server đã kết nối Database thành công!'))
    .catch(err => console.error('❌ Lỗi kết nối:', err));

// ==========================================
// API M2: QUẢN LÝ ĐỊA CHỈ (Chuẩn Tiếng Việt)
// ==========================================

// 1. Lấy danh sách địa chỉ
app.get('/api/user-addresses', async (req, res) => {
    try {
        // Lấy địa chỉ của User ID = 1
        const result = await sql.query`
            SELECT MaDiaChi, TenNguoiNhan, SoDienThoai, TinhThanh, QuanHuyen, PhuongXa, DiaChiChiTiet, MacDinh 
            FROM DiaChi 
            WHERE MaNguoiDung = 1 
            ORDER BY MacDinh DESC`;
        
        // Chuyển đổi tên cột từ Tiếng Việt (DB) -> Tiếng Anh (Frontend)
        const data = result.recordset.map(item => ({
            id: item.MaDiaChi,
            contactName: item.TenNguoiNhan,
            contactPhone: item.SoDienThoai,
            province: item.TinhThanh,
            district: item.QuanHuyen,
            ward: item.PhuongXa,
            addressLine: item.DiaChiChiTiet,
            isDefault: item.MacDinh
        }));
        
        res.json(data);
    } catch (err) { res.status(500).json({ error: err.message }); }
});

// 2. Thêm địa chỉ mới (Transaction xử lý Mặc định)
app.post('/api/user-addresses', async (req, res) => {
    const transaction = new sql.Transaction();
    try {
        const { contactName, contactPhone, addressLine, province, district, ward, isDefault } = req.body;
        const userId = 1; // Giả lập User ID = 1

        await transaction.begin();
        const request = new sql.Request(transaction);

        // a. Nếu chọn mặc định -> Reset các cái cũ về 0
        if (isDefault) {
            await request.query`UPDATE DiaChi SET MacDinh = 0 WHERE MaNguoiDung = ${userId}`;
        }

        // b. Nếu là địa chỉ đầu tiên -> Bắt buộc là Mặc định
        const check = await request.query`SELECT COUNT(*) as C FROM DiaChi WHERE MaNguoiDung = ${userId}`;
        const finalDefault = (check.recordset[0].C === 0) ? true : isDefault;

        // c. Thêm mới (Dùng cột Tiếng Việt)
        await request.input('MaNguoiDung', sql.Int, userId)
                     .input('TenNguoiNhan', sql.NVarChar, contactName)
                     .input('SoDienThoai', sql.VarChar, contactPhone)
                     .input('TinhThanh', sql.NVarChar, province)
                     .input('QuanHuyen', sql.NVarChar, district)
                     .input('PhuongXa', sql.NVarChar, ward)
                     .input('DiaChiChiTiet', sql.NVarChar, addressLine)
                     .input('MacDinh', sql.Bit, finalDefault)
                     .query`
                        INSERT INTO DiaChi (MaNguoiDung, TenNguoiNhan, SoDienThoai, TinhThanh, QuanHuyen, PhuongXa, DiaChiChiTiet, MacDinh)
                        VALUES (@MaNguoiDung, @TenNguoiNhan, @SoDienThoai, @TinhThanh, @QuanHuyen, @PhuongXa, @DiaChiChiTiet, @MacDinh)
                     `;

        await transaction.commit();
        res.json({ success: true, message: "Thêm địa chỉ thành công!" });

    } catch (err) {
        if (transaction._aborted === false) await transaction.rollback();
        // Lỗi vi phạm Index (đã có địa chỉ mặc định mà cố tình thêm cái nữa sai logic)
        if (err.number === 2601) return res.status(409).json({ message: "Lỗi hệ thống: Đã có địa chỉ mặc định!" });
        res.status(500).json({ error: err.message });
    }
});

// 3. Đổi địa chỉ mặc định
app.put('/api/user-addresses/set-default/:id', async (req, res) => {
    const transaction = new sql.Transaction();
    try {
        const addressId = req.params.id;
        const userId = 1;

        await transaction.begin();
        const request = new sql.Request(transaction);

        // Reset tất cả về 0
        await request.query`UPDATE DiaChi SET MacDinh = 0 WHERE MaNguoiDung = ${userId}`;
        // Set cái được chọn lên 1
        await request.query`UPDATE DiaChi SET MacDinh = 1 WHERE MaDiaChi = ${addressId} AND MaNguoiDung = ${userId}`;

        await transaction.commit();
        res.json({ success: true });
    } catch (err) {
        if (transaction._aborted === false) await transaction.rollback();
        res.status(500).json({ error: err.message });
    }
});

app.listen(3000, () => console.log('🚀 Server đang chạy tại http://localhost:3000'));