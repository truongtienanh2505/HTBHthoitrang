# HTBHthoitrang
Headers:
- X-Dev-UserId: số nguyên > 0
- X-Dev-Role: Admin hoặc User
- (optional) X-Dev-Name

Ví dụ:
curl -H "X-Dev-UserId: 1" -H "X-Dev-Role: Admin" "http://localhost:5157/api/admin/reports/revenue-by-day?from=2026-02-01&to=2026-03-01"