CREATE DATABASE StudentJobDB;
GO
USE StudentJobDB;
GO

CREATE TABLE tbl_VaiTro (
    PK_IdVaiTro INT IDENTITY(1,1),
    sTenVaiTro NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_tbl_VaiTro PRIMARY KEY (PK_IdVaiTro),
    CONSTRAINT CHK_TenVaiTro CHECK (sTenVaiTro IN (N'Admin', N'Sinh viên', N'Nhà tuyển dụng'))
);
GO

CREATE TABLE tbl_TaiKhoan (
    PK_IdTaiKhoan INT IDENTITY(1,1),
    sEmail VARCHAR(100) NOT NULL,
    sMatKhau VARCHAR(255) NOT NULL,
    sSoDienThoai VARCHAR(20) NOT NULL,
    FK_IdVaiTro INT NOT NULL,
    bTrangThaiHoatDong BIT NOT NULL DEFAULT 1,
    dNgayTaoTaiKhoan DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_tbl_TaiKhoan PRIMARY KEY (PK_IdTaiKhoan),
    CONSTRAINT UQ_Email UNIQUE (sEmail),
    CONSTRAINT FK_tbl_TaiKhoan_VaiTro FOREIGN KEY (FK_IdVaiTro) REFERENCES tbl_VaiTro(PK_IdVaiTro)
);
GO

CREATE TABLE tbl_SinhVien (
    PK_IdSinhVien INT IDENTITY(1,1),
    FK_IdTaiKhoan INT NOT NULL,
    sHoTen NVARCHAR(100) NOT NULL,
    sChuyenNganhHoc NVARCHAR(100) NOT NULL,
    sDuongDanCVMacDinh VARCHAR(255) NULL,
    CONSTRAINT PK_tbl_SinhVien PRIMARY KEY (PK_IdSinhVien),
    CONSTRAINT UQ_SinhVien_TaiKhoan UNIQUE (FK_IdTaiKhoan),
    CONSTRAINT FK_tbl_SinhVien_TaiKhoan FOREIGN KEY (FK_IdTaiKhoan) REFERENCES tbl_TaiKhoan(PK_IdTaiKhoan) ON DELETE CASCADE
);
GO

CREATE TABLE tbl_NhaTuyenDung (
    PK_IdNhaTuyenDung INT IDENTITY(1,1),
    FK_IdTaiKhoan INT NOT NULL,
    sTenDoanhNghiep NVARCHAR(150) NOT NULL,
    sDuongDanAnhLogo VARCHAR(255) NULL,
    sDiaChiVanPhong NVARCHAR(200) NOT NULL,
    sMoTaTongQuan NVARCHAR(MAX) NULL,
    CONSTRAINT PK_tbl_NhaTuyenDung PRIMARY KEY (PK_IdNhaTuyenDung),
    CONSTRAINT UQ_NhaTuyenDung_TaiKhoan UNIQUE (FK_IdTaiKhoan),
    CONSTRAINT FK_tbl_NhaTuyenDung_TaiKhoan FOREIGN KEY (FK_IdTaiKhoan) REFERENCES tbl_TaiKhoan(PK_IdTaiKhoan) ON DELETE CASCADE
);
GO

CREATE TABLE tbl_NganhNghe (
    PK_IdNganhNghe INT IDENTITY(1,1),
    sTenLinhVuc NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_tbl_NganhNghe PRIMARY KEY (PK_IdNganhNghe),
    CONSTRAINT UQ_TenLinhVuc UNIQUE (sTenLinhVuc)
);
GO

CREATE TABLE tbl_KhuVuc (
    PK_IdKhuVuc INT IDENTITY(1,1),
    sTenKhuVuc NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_tbl_KhuVuc PRIMARY KEY (PK_IdKhuVuc),
    CONSTRAINT UQ_TenKhuVuc UNIQUE (sTenKhuVuc)
);
GO

CREATE TABLE tbl_BaiTuyenDung (
    PK_IdBaiTuyenDung INT IDENTITY(1,1),
    FK_IdNhaTuyenDung INT NOT NULL,
    sTieuDeCongViec NVARCHAR(150) NOT NULL,
    sHinhThucLamViec NVARCHAR(50) NOT NULL,
    sMoTaCongViec NVARCHAR(MAX) NOT NULL,
    sCaLam NVARCHAR(200) NOT NULL,
    sMucLuong NVARCHAR(50) NOT NULL,
    FK_IdNganhNghe INT NOT NULL,
    FK_IdKhuVuc INT NOT NULL,
    dHanNopHoSo DATE NOT NULL,
    sTrangThaiKiemDuyet NVARCHAR(50) NOT NULL DEFAULT N'Chờ duyệt',
    dNgayTaoBai DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_tbl_BaiTuyenDung PRIMARY KEY (PK_IdBaiTuyenDung),
    CONSTRAINT FK_tbl_BaiTuyenDung_NTD FOREIGN KEY (FK_IdNhaTuyenDung) REFERENCES tbl_NhaTuyenDung(PK_IdNhaTuyenDung),
    CONSTRAINT FK_tbl_BaiTuyenDung_Nganh FOREIGN KEY (FK_IdNganhNghe) REFERENCES tbl_NganhNghe(PK_IdNganhNghe),
    CONSTRAINT FK_tbl_BaiTuyenDung_KhuVuc FOREIGN KEY (FK_IdKhuVuc) REFERENCES tbl_KhuVuc(PK_IdKhuVuc),
    CONSTRAINT CHK_HanNopHoSo CHECK (dHanNopHoSo >= CAST(dNgayTaoBai AS DATE))
);
GO

CREATE TABLE tbl_HoSoUngTuyen (
    PK_IdHoSoUngTuyen INT IDENTITY(1,1),
    FK_IdSinhVien INT NOT NULL,
    FK_IdBaiTuyenDung INT NOT NULL,
    sDuongDanCV VARCHAR(255) NOT NULL,
    sThuXinViec NVARCHAR(MAX) NULL,
    sTrangThaiXetDuyet NVARCHAR(50) NOT NULL DEFAULT N'Chờ duyệt',
    sGhiChuPhanHoi NVARCHAR(200) NULL,
    dThoiGianNopHoSo DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_tbl_HoSoUngTuyen PRIMARY KEY (PK_IdHoSoUngTuyen),
    CONSTRAINT UQ_SinhVien_BaiTuyenDung UNIQUE (FK_IdSinhVien, FK_IdBaiTuyenDung), -- Chống nộp trùng lặp
    CONSTRAINT FK_tbl_HoSoUngTuyen_SV FOREIGN KEY (FK_IdSinhVien) REFERENCES tbl_SinhVien(PK_IdSinhVien),
    CONSTRAINT FK_tbl_HoSoUngTuyen_Bai FOREIGN KEY (FK_IdBaiTuyenDung) REFERENCES tbl_BaiTuyenDung(PK_IdBaiTuyenDung) ON DELETE CASCADE
);
GO


---------------------- DATA ----------------------------------
INSERT INTO tbl_VaiTro (sTenVaiTro) VALUES (N'Admin'), (N'Sinh viên'), (N'Nhà tuyển dụng');
GO

INSERT INTO tbl_NganhNghe (sTenLinhVuc) VALUES 
(N'Công nghệ thông tin'), (N'Marketing / Truyền thông'), (N'Dịch vụ F&B / Nhà hàng'), (N'Giao nhận / Logistics'), (N'Gia sư / Giáo dục');
INSERT INTO tbl_KhuVuc (sTenKhuVuc) VALUES 
(N'Hoàn Kiếm, Hà Nội'), (N'Cầu Giấy, Hà Nội'), (N'Đống Đa, Hà Nội'), (N'Hai Bà Trưng, Hà Nội'), (N'Quận 1, TP. Hồ Chí Minh'), (N'Quận 3, TP. Hồ Chí Minh');
GO

INSERT INTO tbl_TaiKhoan (sEmail, sMatKhau, sSoDienThoai, FK_IdVaiTro, bTrangThaiHoatDong, dNgayTaoTaiKhoan) VALUES
('vuquanganh@gmail.com', '12345678', '0912345678', 2, 1, GETDATE()), 
('phamthanhhuy@gmail.com', '12345678', '0923456789', 2, 1, GETDATE()),
('tungochuy@gmail.com', '12345678', '0934567890', 2, 1, GETDATE()), 
('trantrongnghia@gmail.com', '12345678', '0945678901', 2, 1, GETDATE()); 
GO

-- sDuongDanCVMacDinh lưu ở /uploads/cvs/default/cv_${PK_IdSinhVien}.pdf

INSERT INTO tbl_SinhVien (FK_IdTaiKhoan, sHoTen, sChuyenNganhHoc, sDuongDanCVMacDinh) VALUES
(1, N'Vũ Quang Anh', N'Công nghệ thông tin - Lớp 2310A03', '/uploads/cvs/default/cv_1.pdf'),
(2, N'Phạm Thành Huy', N'Công nghệ thông tin - Lớp 2310A02', '/uploads/cvs/default/cv_2.pdf'),
(3, N'Từ Ngọc Huy', N'Công nghệ thông tin - Lớp 1910A04', '/uploads/cvs/default/cv_3.pdf'),
(4, N'Trần Trọng Nghĩa', N'Công nghệ thông tin - Lớp 2310A03', '/uploads/cvs/default/cv_4.pdf');
GO

INSERT INTO tbl_TaiKhoan (sEmail, sMatKhau, sSoDienThoai, FK_IdVaiTro) VALUES
('admin@gmail.com', '12345678', '0243380234', 1);
GO

INSERT INTO tbl_TaiKhoan (sEmail, sMatKhau, sSoDienThoai, FK_IdVaiTro) VALUES
('hr.techcorp@gmail.com', '12345678', '0247300123', 3),
('recruitment.highlands@cafe.com', '12345678', '0287300456', 3);
GO

INSERT INTO tbl_NhaTuyenDung (FK_IdTaiKhoan, sTenDoanhNghiep, sDuongDanAnhLogo, sDiaChiVanPhong, sMoTaTongQuan) VALUES
(6, N'Công ty Cổ phần Công nghệ TechCorp', '/uploads/logos/techcorp.png', N'Số 12 Duy Tân, Cầu Giấy, Hà Nội', N'Đơn vị phát triển các giải pháp phần mềm và ứng dụng di động hàng đầu.'),
(7, N'Chuỗi Cửa hàng Cà phê Highlands Coffee', '/uploads/logos/highlands.png', N'135 Nguyễn Huệ, Quận 1, TP. Hồ Chí Minh', N'Thương hiệu chuỗi cửa hàng cà phê và đồ ăn nhanh đại chúng lớn tại Việt Nam.');
GO

INSERT INTO tbl_BaiTuyenDung (FK_IdNhaTuyenDung, sTieuDeCongViec, sHinhThucLamViec, sMoTaCongViec, sCaLam, sMucLuong, FK_IdNganhNghe, FK_IdKhuVuc, dHanNopHoSo, sTrangThaiKiemDuyet) VALUES
(1, N'Thực tập sinh Lập trình viên ASP.NET Core', N'Thực tập', N'Tham gia phát triển dự án web nội bộ, xây dựng API và thiết kế CSDL hệ thống.', N'Sáng: 08:00 - 12:00 hoặc Chiều: 13:30 - 17:30 (Tối thiểu 4 buổi/tuần)', N'3.000.000đ - 5.000.000đ', 1, 1, '2026-08-31', N'Đã duyệt'),
(1, N'Cộng tác viên Content Marketing Part-time', N'Bán thời gian', N'Viết bài chuẩn SEO cho fanpage và website công nghệ, lên kịch bản video ngắn.', N'Thời gian linh hoạt theo lịch học của sinh viên', N'2.000.000đ - 4.000.000đ', 2, 1, '2026-08-15', N'Đã duyệt'),
(2, N'Nhân viên phục vụ quầy ca tối cửa hàng', N'Bán thời gian', N'Đón tiếp khách hàng, nhận order, pha chế đồ uống cơ bản và dọn dẹp vệ sinh quầy.', N'Ca tối: 18:00 - 22:30 hoặc Ca đêm: 22:00 - 06:00', N'22.000đ - 25.000đ/giờ', 3, 4, '2026-09-10', N'Đã duyệt'),
(2, N'Nhân viên ứng tuyển thử nghiệm tính năng', N'Bán thời gian', N'Bài viết này được tạo ra ở trạng thái chờ để Admin đăng nhập vào hệ thống test tính năng phê duyệt.', N'Ca tự do', N'Thỏa thuận', 1, 2, '2026-07-30', N'Chờ duyệt');
GO

-- sDuongDanCV khi apply lưu ở /uploads/cvs/applied/post_${FK_IdBaiTuyenDung}/cv_${FK_IdSinhVien}.pdf

INSERT INTO tbl_HoSoUngTuyen (FK_IdSinhVien, FK_IdBaiTuyenDung, sDuongDanCV, sThuXinViec, sTrangThaiXetDuyet, sGhiChuPhanHoi, dThoiGianNopHoSo) VALUES
(1, 1, '/uploads/cvs/applied/post_1/cv_1.pdf', N'Em là sinh viên lớp 2310A03, rất mong muốn được ứng tuyển học hỏi vị trí .NET của công ty.', N'Hẹn phỏng vấn', N'Mời em tham gia phỏng vấn online qua Teams vào lúc 09:30 ngày 20/07/2026.', GETDATE()),
(3, 1, '/uploads/cvs/applied/post_1/cv_3.pdf', N'Em đã học qua môn Lập trình Web nâng cao và muốn trải nghiệm thực tế công việc.', N'Chờ duyệt', NULL, GETDATE()),
(2, 3, '/uploads/cvs/applied/post_3/cv_2.pdf', N'Em muốn ứng tuyển làm ca tối để kiếm thêm thu nhập trang trải chi phí học tập.', N'Chờ duyệt', NULL, GETDATE());
GO