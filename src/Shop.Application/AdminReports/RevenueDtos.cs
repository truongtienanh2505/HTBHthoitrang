namespace Shop.Application.AdminReports;

public sealed record RevenueByDayDto(
    DateOnly Ngay,
    int SoDon,
    decimal TongTien,
    decimal TongGiam,
    decimal TongShip,
    decimal DoanhThu
);