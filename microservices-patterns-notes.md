# SRS (Ngắn gọn) – Website

**Phạm vi mô tả:** chức năng thương mại điện tử bán linh kiện/PC/laptop & tiện ích “Xây dựng cấu hình”.

## 1. Giới thiệu
Website bán lẻ sản phẩm công nghệ (PC, laptop, linh kiện, phụ kiện, gaming gear, phần mềm) kèm các nội dung hỗ trợ như blog/tin tức, chính sách (giao nhận, thanh toán, bảo hành/đổi trả) và trang liên hệ.

### 1.1 Mục tiêu
- Cho phép khách hàng tìm kiếm – chọn mua – thanh toán sản phẩm online nhanh chóng.
- Cung cấp thông tin chính sách minh bạch (giao/nhận hàng, thanh toán, bảo hành/đổi trả).
- Hỗ trợ người dùng tự “xây dựng cấu hình” PC theo nhu cầu (Build PC).

### 1.2 Thuật ngữ/viết tắt
- **SRS**: Software Requirements Specification – đặc tả yêu cầu phần mềm.
- **SKU/Mã SP**: Mã sản phẩm.
- **COD**: Thanh toán khi nhận hàng.
- **Gateway**: Cổng thanh toán (ví dụ Alepay).

## 2. Mô tả tổng quan hệ thống

### 2.1 Bối cảnh & phạm vi
- Hệ thống web thương mại điện tử (frontend + backend + CSDL) cho bán hàng online.
- Tích hợp đăng nhập/tài khoản người dùng; giỏ hàng; đặt hàng; thanh toán.
- Tích hợp các trang nội dung: giới thiệu, blog, chính sách, liên hệ.
- Có trang/tiện ích Build PC (xây dựng cấu hình) để chọn linh kiện theo nhóm.

### 2.2 Nhóm người dùng
- Khách vãng lai (Guest).
- Khách hàng có tài khoản (Registered Customer).
- Nhân viên bán hàng/CSKH (Sales/Support).
- Kho – giao nhận (Warehouse/Fulfillment).
- Quản trị hệ thống (Admin).

### 2.3 Giả định & phụ thuộc
- Có tích hợp cổng thanh toán (thẻ/QR/ví điện tử) và/hoặc chuyển khoản, COD.
- Có đơn vị vận chuyển xử lý giao hàng toàn quốc.
- Hệ thống email/SMS/notification phục vụ xác nhận đơn hàng và cập nhật trạng thái (nếu có).

## 3. Phân tích Actors (tác nhân)

### 3.1 Danh sách actors chính
- A1. Khách vãng lai (Guest)
- A2. Khách hàng đã đăng ký/đăng nhập (Customer)
- A3. Quản trị viên (Admin)
- A4. Nhân viên bán hàng (Staff)
- A5. Cổng thanh toán (Payment Gateway)
- A6. Đơn vị vận chuyển (Delivery Partner)

### 3.2 Mô tả trách nhiệm theo actor

| Actor | Mục tiêu/Trách nhiệm | Tương tác chính |
|---|---|---|
| A1. Guest | Xem sản phẩm, tìm kiếm, xem chính sách; có thể thêm giỏ và đặt hàng như khách. | Danh mục/chi tiết SP, tìm kiếm, giỏ hàng, checkout (nếu cho phép). |
| A2. Customer | Mua hàng, theo dõi đơn, quản lý thông tin cá nhân/địa chỉ. | Đăng nhập/đăng ký, giỏ hàng, checkout, lịch sử đơn, build PC. |
| A3. Admin | Quản trị danh mục, sản phẩm, giá, tồn kho, đơn hàng, nội dung trang. | Backoffice: CRUD sản phẩm/danh mục, quản lý đơn & người dùng. |
| A4. Staff | Tư vấn, xử lý đơn, tiếp nhận phản ánh, hỗ trợ đổi trả/bảo hành. | Dashboard đơn, ticket hỗ trợ, cập nhật trạng thái, ghi chú. |
| A6. Delivery Partner | Vận chuyển, giao/thu COD, hoàn hàng khi thất bại. | Nhận đơn, cập nhật trạng thái giao hàng, đối soát COD. |
| A7. Payment Gateway | Xử lý thanh toán online, trả kết quả giao dịch. | Redirect/Callback, đối soát giao dịch, hoàn/huỷ. |

## 4. Ma trận Actors - Features

| Mã | Tính năng | Guest | Member | Admin | Staff | Delivery Partner |
|---|---|---|---|---|---|---|
|  | **QUẢN LÝ VÀ HIỂN THỊ SẢN PHẨM** |  |  |  |  |  |
| F01 | Xem danh sách sản phẩm theo danh mục | ✓ | ✓ | ✓ | ✓ |  |
| F02 | Xem chi tiết sản phẩm (hình ảnh, giá, mô tả) | ✓ | ✓ | ✓ | ✓ |  |
| F03 | Tìm kiếm sản phẩm theo tên | ✓ | ✓ | ✓ | ✓ |  |
| F04 | Lọc sản phẩm theo giá, loại hoa | ✓ | ✓ | ✓ | ✓ |  |
| F05 | Thêm/sửa/xóa sản phẩm |  |  | ✓ | ✓ |  |
| F06 | Quản lý danh mục sản phẩm |  |  | ✓ | ✓ |  |
|  | **GIỎ HÀNG VÀ ĐẶT HÀNG** |  |  |  |  |  |
| F07 | Thêm sản phẩm vào giỏ hàng | ✓ | ✓ |  |  |  |
| F08 | Cập nhật số lượng/xóa sản phẩm trong giỏ | ✓ | ✓ |  |  |  |
| F09 | Chọn địa điểm giao hàng | ✓ | ✓ |  |  |  |
| F10 | Nhập thông tin người nhận và lời nhắn | ✓ | ✓ |  |  |  |
| F11 | Thanh toán đơn hàng (COD, thẻ, ví điện tử) | ✓ | ✓ |  |  |  |
| F12 | Tra cứu trạng thái đơn hàng | ✓ | ✓ | ✓ | ✓ | ✓ |
| F13 | Quản lý đơn hàng (xác nhận, cập nhật) |  |  | ✓ | ✓ | ✓ |
|  | **QUẢN LÝ TÀI KHOẢN** |  |  |  |  |  |
| F14 | Đăng ký tài khoản mới | ✓ |  |  |  |  |
| F15 | Đăng nhập/đăng xuất |  | ✓ | ✓ |  |  |
| F16 | Xem và cập nhật thông tin cá nhân |  | ✓ | ✓ |  |  |
| F17 | Xem lịch sử đơn hàng |  | ✓ | ✓ |  |  |
| F18 | Xem điểm thành viên và ưu đãi |  | ✓ |  |  |  |
| F19 | Quản lý khách hàng và thành viên |  |  | ✓ | ✓ |  |
|  | **NỘI DUNG VÀ KHUYẾN MÃI** |  |  |  |  |  |
| F20 | Xem chương trình khuyến mãi | ✓ | ✓ | ✓ |  |  |
| F21 | Quản lý chương trình khuyến mãi |  |  | ✓ | ✓ |  |
|  | **Build PC - XÂY DỰNG CẤU HÌNH** |  |  |  |  |  |
| F22 | Chọn linh kiện theo từng nhóm (mainboard, CPU, RAM, SSD/HDD, PSU, VGA, case…). | ✓ | ✓ |  |  |  |
| F23 | Cho phép ‘Xây dựng lại’/reset cấu hình; xuất/ghi cấu hình (tối thiểu hiển thị để người dùng chụp/lưu). | ✓ | ✓ |  |  |  |

## 5. Yêu cầu phi chức năng (Non-functional Requirements)

### 5.1 Hiệu năng & khả dụng
- **NFR-01**: Thời gian tải trang danh mục/chi tiết sản phẩm ở mức chấp nhận được khi có nhiều ảnh và biến thể.
- **NFR-02**: Hệ thống chịu tải cho các đợt khuyến mãi; có cơ chế cache/CDN cho nội dung tĩnh.
- **NFR-03**: Tính sẵn sàng cao; có backup dữ liệu định kỳ (đơn hàng, khách hàng, sản phẩm).

### 5.2 Bảo mật
- **NFR-04**: Bảo vệ phiên đăng nhập (HTTPS, cookie an toàn, chống CSRF).
- **NFR-05**: Mã hóa/ẩn thông tin nhạy cảm; phân quyền Admin/Staff rõ ràng.
- **NFR-06**: Tuân thủ quy trình xử lý thanh toán theo tiêu chuẩn của cổng thanh toán; không lưu trữ thông tin thẻ nhạy cảm trên hệ thống.

### 5.3 Tính dùng được & tương thích
- **NFR-07**: UI/UX thân thiện, hỗ trợ mobile & desktop (responsive).
- **NFR-08**: Tương thích các trình duyệt phổ biến (Chrome/Edge/Firefox/Safari).
- **NFR-09**: Tìm kiếm và lọc sản phẩm rõ ràng; hỗ trợ tiếng Việt.

### 5.4 Nhật ký & giám sát
- **NFR-10**: Ghi log các sự kiện quan trọng: tạo đơn, thanh toán, đổi trạng thái, lỗi hệ thống.
- **NFR-11**: Có cơ chế cảnh báo khi lỗi thanh toán/callback thất bại hoặc tồn kho âm.

## 6. Yêu cầu dữ liệu (Data Requirements)
- **D-01**: Sản phẩm: tên, SKU, giá, tồn kho, mô tả, ảnh, thương hiệu, thuộc tính/biến thể, chính sách bảo hành.
- **D-02**: Khách hàng: thông tin liên hệ, địa chỉ, lịch sử đơn hàng.
- **D-03**: Đơn hàng: mã đơn, danh sách item, phí, tổng tiền, trạng thái, phương thức thanh toán, tracking vận chuyển.
- **D-04**: Nội dung: bài blog, trang chính sách, thông tin showroom/liên hệ.

## 7. Use Cases chính (tóm tắt)
1. UC-01: Duyệt danh mục & xem chi tiết sản phẩm.
2. UC-02: Tìm kiếm và lọc sản phẩm.
3. UC-03: Thêm giỏ hàng và cập nhật giỏ.
4. UC-04: Checkout và tạo đơn hàng.
5. UC-05: Thanh toán (COD/Chuyển khoản/Online gateway).
6. UC-06: Đăng ký/Đăng nhập (bao gồm Google/Facebook).
7. UC-07: Theo dõi trạng thái đơn hàng.
8. UC-08: Xây dựng cấu hình PC (Build PC).
9. UC-09: Gửi liên hệ/nhận hỗ trợ.
10. UC-10: Yêu cầu bảo hành/đổi trả (theo chính sách).

## 8. Ngoài phạm vi (Out of Scope)
- Quản trị ERP nội bộ nâng cao (kế toán, nhân sự) nếu không tích hợp sẵn.
- Tối ưu tương thích linh kiện chuyên sâu (BIOS version, VRM, clearance case…) nếu không có dữ liệu kỹ thuật chi tiết.
- Chương trình khách hàng thân thiết/điểm thưởng (nếu website chưa triển khai).

## 9. Tham chiếu (trang trên website)
- Trang chủ/danh mục sản phẩm (tinhocngoisao.com).
- Build PC – Xây dựng cấu hình (pages/xay-dung-cau-hinh).
- Chính sách giao nhận & kiểm hàng (pages/chinh-sach-giao-nhan-hang-va-kiem-hang).
- Thanh toán (pages/chinh-sach-thanh-toan; pages/thanh-toan-tai-khoan).
- Bảo hành & đổi trả (pages/bao-hanh).
- Điều khoản dịch vụ (pages/dieu-khoan-dich-vu).
- Liên hệ (pages/lien-he).
