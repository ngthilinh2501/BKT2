# Vợt Thủ Phố Núi - Pickleball Club Management

Hệ thống quản lý câu lạc bộ Pickleball "Vợt Thủ Phố Núi", hỗ trợ quản lý thành viên, đặt sân, tài chính và tổ chức các giải đấu/kèo đấu một cách chuyên nghiệp.

## 🌟 Tính Năng Chính

### 1. 👥 Quản Lý Hội Viên (Members)
- Hồ sơ chi tiết với cấp độ Rank (DUPR)
- Lịch sử thi đấu và thống kê thắng/thua
- Tự động đánh giá và cập nhật Rank sau mỗi trận đấu Ranked

### 2. 🏟️ Đặt Sân (Bookings)
- Đặt sân trực tuyến dễ dàng
- Kiểm tra lịch trống của sân theo thời gian thực
- Tự động tạo hồ sơ hội viên khi đặt sân lần đầu

### 3. 🏆 Thi Đấu & Kết Quả (Matches)
- Ghi nhận kết quả trận đấu (Đơn/Đôi)
- Hỗ trợ tính điểm Ranked hoặc Giao hữu
- **Lịch sử thi đấu**: Xem lại chi tiết các trận đã đấu, đối thủ và kết quả thắng/thua
- Tự động cập nhật Rank cho người chiến thắng

### 4. 💰 Quản Lý Tài Chính (Treasury)
- Quản lý thu/chi minh bạch
- Phân loại danh mục thu chi (Tiền sân, Nước uống, Giải thưởng...)
- Báo cáo số dư quỹ CLB

### 5. 📰 Tin Tức & Thông Báo
- Hệ thống bảng tin ghim các thông báo quan trọng
- Giao diện tin tức trực quan

## 🛠️ Cập Nhật Gần Đây

- **Giao diện (UI/UX)**: 
  - Cập nhật Footer với Light Theme hiện đại, gradient Emerald-Blue.
  - Tối ưu hóa hiển thị bảng tin và các cards.
- **Tính năng Matches**:
  - Sửa lỗi tạo trận đấu Đôi (Validation 4 người chơi).
  - Cải thiện trang `Lịch sử thi đấu` (Hiển thị tên đối thủ, trạng thái Thắng/Thua).
  - Tự động điều hướng về trang lịch sử sau khi ghi kết quả.
- **Fix lỗi Logic**:
  - Tự động tạo Member Profile cho Admin/User mới khi truy cập các trang chức năng.
  - Sửa lỗi hiển thị form ghi nhận kết quả cho thể thức Đơn/Đôi.

## 🚀 Cài Đặt & Chạy Ứng Dụng

1. Clone repository:
```bash
git clone https://github.com/ngthilinh2501/BKT2.git
```

2. Cấu hình Database trong `appsettings.json`.

3. Chạy ứng dụng:
```bash
dotnet run
```

4. Truy cập: `https://localhost:5001` hoặc `http://localhost:5000`

## 👨‍💻 Tác Giả

Project BKT2 - Phát triển bởi [Nguyễn Thị Linh]
