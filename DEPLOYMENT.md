# Hướng Dẫn Deploy (Triển Khai) Lên Server

Hướng dẫn này sẽ giúp bạn đưa ứng dụng **Vợt Thủ Phố Núi** lên một máy chủ ảo (VPS - Virtual Private Server) chạy Linux (Ubuntu/Debian) sử dụng Docker.

## 1. Chuẩn Bị
- **Một VPS**: Mua từ các nhà cung cấp như DigitalOcean, Vultr, Azure, AWS, hoặc Google Cloud.
- **Hệ điều hành**: Khuyên dùng **Ubuntu 22.04 LTS** hoặc mới hơn.
- **Domain (Tùy chọn)**: Nếu muốn truy cập bằng tên miền (ví dụ: `votthuphonui.com`).

## 2. Cài Đặt Môi Trường Trên Server
Kết nối SSH vào server của bạn và chạy các lệnh sau để cài đặt Docker:

```bash
# Cập nhật hệ thống
sudo apt update && sudo apt upgrade -y

# Cài đặt Docker và Docker Compose Plugin
sudo apt install -y docker.io docker-compose-v2

# Khởi động Docker
sudo systemctl enable --now docker
```

## 3. Tải Mã Nguồn
Bạn cần đưa code lên server. Cách đơn giản nhất là dùng Git:

```bash
# Clone source code về server
git clone https://github.com/ngthilinh2501/BKT2.git
cd BKT2
```

## 4. Cấu Hình Cho Production (Quan Trọng)
Trước khi chạy, hãy chỉnh sửa file `docker-compose.yml` để bảo mật hơn.
Mở file bằng nano: `nano docker-compose.yml`

Sửa các phần sau:
1. **Password**: Đổi `YourStrong@Password` thành mật khẩu khó đoán hơn.
2. **Environment**: Đổi `Development` thành `Production`.

Ví dụ file `docker-compose.yml` cho Production:

```yaml
services:
  app:
    # ...
    environment:
      - ASPNETCORE_ENVIRONMENT=Production # Đổi thành Production
      - ConnectionStrings__DefaultConnection=Server=db;Database=PCM_357;User Id=sa;Password=MAT_KHAU_BAO_MAT_CUA_BAN;TrustServerCertificate=True;
    # ...

  db:
    # ...
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=MAT_KHAU_BAO_MAT_CUA_BAN # Phải khớp với password ở trên
    # ...
```

## 5. Chạy Ứng Dụng
Tại thư mục dự án (trên server), chạy lệnh:

```bash
sudo docker compose up -d --build
```
*Lưu ý: Trên một số phiên bản cũ cũ là `docker-compose` (có gạch nối), bản mới là `docker compose` (dấu cách).*

## 6. Kiểm Tra
Truy cập IP của server trên trình duyệt: `http://IP_CUA_SERVER:5000`

## 7. Các Lệnh Thường Dùng
- **Xem log ứng dụng**: `sudo docker compose logs -f app`
- **Cập nhật code mới**:
  ```bash
  git pull
  sudo docker compose up -d --build
  ```
- **Backup dữ liệu**: Copy thư mục được mount trong volume (hoặc export dữ liệu từ container db).

## Nâng Cao (Tùy chọn)
Để chạy ở cổng 80 (không cần nhập :5000) và có HTTPS:
1. Sửa `ports` của service `app` thành `"80:8080"`.
2. Hoặc cài đặt **Nginx** làm Reverse Proxy và cài chứng chỉ SSL miễn phí bằng **Certbot**.
