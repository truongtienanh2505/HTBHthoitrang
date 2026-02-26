export const getProductDetail = async (id) => {
    const res = await fetch(`http://localhost:5000/api/products/${id}`);
    if (!res.ok) throw new Error("Không tìm thấy sản phẩm");
    return res.json();
};