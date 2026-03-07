# PRN232 TechStore

Backend ASP.NET Core Web API (.NET 8) cho hệ thống TechStore.

## Tài khoản test sẵn (seed trong `TechStoreDB.sql`)

Sau khi chạy script DB, bạn có thể đăng nhập nhanh bằng 2 tài khoản sau:

- **Admin**
  - Email: `admin@techstore.local`
  - Password: `123456789`
  - Role: `admin`

- **Staff**
  - Email: `staff@techstore.local`
  - Password: `123456789`
  - Role: `staff`

> Lưu ý: mật khẩu trong DB được hash theo đúng logic BE: `SHA256 + Base64`.

## Cách dùng nhanh

1. Chạy `TechStoreDB.sql` để tạo DB + seed data.
2. Chạy project `TechStoreWebAPI`.
3. Mở Swagger tại `/swagger`.
4. Gọi `POST /api/Auth/login` với 1 trong 2 tài khoản trên để lấy JWT.
5. Dùng JWT với nút **Authorize** trong Swagger (`Bearer <token>`).

## Endpoint login mẫu

`POST /api/Auth/login`

```json
{
  "email": "admin@techstore.local",
  "password": "123456789"
}
```

Hoặc

```json
{
  "email": "staff@techstore.local",
  "password": "123456789"
}
```

## Ghi chú

- Script `TechStoreDB.sql` là idempotent, có thể chạy lại nhiều lần.
- Seed products/images/specs đã có sẵn để FE test Home / Products / ProductDetail / Cart / Checkout / BuildPC.
