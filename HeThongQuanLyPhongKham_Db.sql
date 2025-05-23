﻿SET DATEFORMAT DMY;
GO

-- 1. TẠO CƠ SỞ DỮ LIỆU
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'HeThongQuanLyPhongKham_Db')
    CREATE DATABASE HeThongQuanLyPhongKham_Db;
GO
USE HeThongQuanLyPhongKham_Db;
GO

-- 2. TẠO CÁC BẢNG

-- Bảng VaiTro
CREATE TABLE tbl_vai_tro (
    maVaiTro    INT             NOT NULL    IDENTITY(1,1),
    ten         VARCHAR(100)    NOT NULL,
    CONSTRAINT pk_tbl_vai_tro PRIMARY KEY (maVaiTro),
    CONSTRAINT uq_tbl_vai_tro_ten UNIQUE (ten)
);
GO

-- Bảng TaiKhoan
CREATE TABLE tbl_tai_khoan (
    maTaiKhoan      INT             NOT NULL    IDENTITY(1,1),
	maVaiTro		INT             NULL,
    tenDangNhap     VARCHAR(50)     NOT NULL,
    matKhau         VARCHAR(255)    NOT NULL,
    CONSTRAINT pk_tbl_tai_khoan PRIMARY KEY (maTaiKhoan),
    CONSTRAINT uq_tbl_tai_khoan_tenDangNhap UNIQUE (tenDangNhap)
);
GO

-- Bảng NhanVien
CREATE TABLE tbl_nhan_vien (
    maNhanVien      INT             NOT NULL    IDENTITY(1,1),
    maTaiKhoan      INT             NOT NULL,
    ten             NVARCHAR(100)   NOT NULL,
    soDienThoai     VARCHAR(15)     NOT NULL,
	caLamViec       NVARCHAR(50)    NULL,
    chuyenMon       NVARCHAR(100)   NOT NULL,
    CONSTRAINT pk_tbl_nhan_vien PRIMARY KEY (maNhanVien),
    CONSTRAINT ck_tbl_nhan_vien_soDienThoai CHECK (LEN(soDienThoai) >= 10),
	CONSTRAINT uq_tbl_nhan_vien_taikhoan UNIQUE (maTaiKhoan)
);
GO

-- Bảng PhongKham
CREATE TABLE tbl_phong_kham (
    maPhongKham        INT             NOT NULL IDENTITY(1,1),
    loai               NVARCHAR(100)   NOT NULL,
    sucChua            INT             NOT NULL,
    CONSTRAINT pk_tbl_phong_kham PRIMARY KEY (maPhongKham),
    CONSTRAINT ck_tbl_phong_kham_sucChua CHECK (sucChua > 0)
);
GO

-- Bảng PhongKham_NhanVien
CREATE TABLE tbl_phong_kham_nhan_vien (
	maPhongKhamNhanVien	INT				IDENTITY(1,1),
    maPhongKham			INT             NOT NULL,
    maNhanVien			INT             NOT NULL,
    vaiTro				NVARCHAR(50)    NOT NULL,
    CONSTRAINT pk_tbl_phong_kham_nhan_vien PRIMARY KEY (maPhongKhamNhanVien)
);
GO

-- Bảng LichHen
CREATE TABLE tbl_lich_hen (
    maLichHen       INT             NOT NULL IDENTITY(1,1),
    maBenhNhan      INT             NOT NULL,
    maNhanVien      INT             NULL,
    maDichVuYTe     INT             NULL,
    maPhongKham     INT             NULL,
    ngayHen         DATETIME        NOT NULL,
    trangThai       NVARCHAR(50)    NOT NULL,
    CONSTRAINT pk_tbl_lich_hen PRIMARY KEY (maLichHen),
);
GO

-- Bảng BenhNhan
CREATE TABLE tbl_benh_nhan (
    maBenhNhan			INT				NOT NULL IDENTITY(1,1),
	maTaiKhoan			INT				NOT NULL,
	ten					NVARCHAR(100)	NULL,
    tuoi				INT				NULL,
    gioiTinh			BIT				NULL,
    diaChi				NVARCHAR(1000)  NULL,
    soDienThoai			VARCHAR(15)		NULL,
    CONSTRAINT pk_tbl_benh_nhan PRIMARY KEY (maBenhNhan),
    CONSTRAINT ck_tbl_benh_nhan_tuoi CHECK (tuoi >= 0),
    CONSTRAINT ck_tbl_benh_nhan_gioiTinh CHECK (gioiTinh IN (0, 1)),
	CONSTRAINT uq_tbl_benh_nhan_taikhoan UNIQUE (maTaiKhoan)
);
GO

-- Bảng HoSoYTe
CREATE TABLE tbl_ho_so_y_te (
    maHoSoYTe			INT             NOT NULL    IDENTITY(1,1),
    maBenhNhan			INT             NOT NULL,
    chuanDoan			NVARCHAR(1000)   NULL,
    phuongPhapDieuTri	NVARCHAR(1000)   NULL,
	lichSuBenh			NVARCHAR(1000)	NULL
    CONSTRAINT pk_tbl_ho_so_y_te PRIMARY KEY (maHoSoYTe)
);
GO

-- Bảng TrieuChung
CREATE TABLE tbl_trieu_chung (
    MaTrieuChung     INT             NOT NULL IDENTITY(1,1),
    MaHoSoYTe        INT             NOT NULL,
    TenTrieuChung    NVARCHAR(200)   NOT NULL,
    MoTa             NVARCHAR(500)   NULL,
    ThoiGianXuatHien DATETIME        NULL,
    CONSTRAINT pk_tbl_trieu_chung PRIMARY KEY (MaTrieuChung)
);
GO

-- Bảng KetQuaXetNghiem
CREATE TABLE tbl_ket_qua_xet_nghiem (
    MaKetQua        INT             NOT NULL IDENTITY(1,1),
    MaHoSoYTe       INT             NOT NULL,
    TenXetNghiem    NVARCHAR(200)   NOT NULL,
    KetQua          NVARCHAR(500)   NULL,
    NgayXetNghiem   DATETIME        NOT NULL,
    CONSTRAINT pk_tbl_ket_qua_xet_nghiem PRIMARY KEY (MaKetQua)
);
GO

-- Bảng DonThuoc
CREATE TABLE tbl_don_thuoc (
    maDonThuoc      INT             NOT NULL    IDENTITY(1,1),
	maHoSoYTe		INT				NOT NULL,
	maHoaDon        INT				NULL,
    ngayKeDon       DATETIME        NOT NULL    DEFAULT GETDATE(),
    CONSTRAINT pk_tbl_don_thuoc PRIMARY KEY (maDonThuoc)
);
GO

-- Bảng DonThuoc_Thuoc
CREATE TABLE tbl_don_thuoc_chi_tiet (
	maDonThuocChiTiet	INT				NOT NULL IDENTITY(1,1),
    maDonThuoc			INT             NOT NULL,
    maThuoc				INT             NOT NULL,
    soLuong				INT             NOT NULL,           -- Tổng số lượng thuốc cấp phát
    cachDung			NVARCHAR(200)   NOT NULL,           -- Cách dùng (liều lượng + tần suất + thời gian)
	LieuLuong			NVARCHAR(50)    NULL,
    TanSuat				NVARCHAR(50)    NULL,
    ThanhTien           DECIMAL(15,2)   NOT NULL,
    CONSTRAINT pk_tbl_don_thuoc_chi_tiet PRIMARY KEY (maDonThuocChiTiet, maDonThuoc, maThuoc),
    CONSTRAINT chk_soLuong CHECK (soLuong > 0)
);
GO

-- Bảng Thuoc
CREATE TABLE tbl_thuoc (
    maThuoc         INT             NOT NULL    IDENTITY(1,1),
    ten             NVARCHAR(100)   NOT NULL,
    moTa            NVARCHAR(500)   NOT NULL,
    donVi           NVARCHAR(20)    NOT NULL    DEFAULT N'viên', -- Đơn vị mặc định của thuốc
	ChongChiDinh    NVARCHAR(1000)  NULL,
    TuongTacThuoc   NVARCHAR(1000)  NULL,
    donGia          DECIMAL(15,2)   NOT NULL,
    CONSTRAINT pk_tbl_thuoc PRIMARY KEY (maThuoc)
);
GO

-- Bảng KetQuaDieuTri
CREATE TABLE tbl_ket_qua_dieu_tri (
    MaKetQuaDieuTri INT             NOT NULL IDENTITY(1,1),
    MaHoSoYTe       INT             NOT NULL,
    MaDonThuoc      INT             NOT NULL,
    HieuQua         NVARCHAR(500)   NULL,
    TacDungPhu      NVARCHAR(500)   NULL,
    NgayDanhGia     DATETIME        NOT NULL,
    CONSTRAINT pk_tbl_ket_qua_dieu_tri PRIMARY KEY (MaKetQuaDieuTri)
);
GO

-- Bảng HoaDon
CREATE TABLE tbl_hoa_don (
    maHoaDon               INT             NOT NULL		IDENTITY(1,1),
    maLichHen              INT             NULL,
    tongTien               DECIMAL(15, 2)  NOT NULL,
    ngayThanhToan          DATETIME        NULL			DEFAULT GETDATE(),
    trangThaiThanhToan     NVARCHAR(50)    NOT NULL		DEFAULT N'Chưa thanh toán',
    CONSTRAINT pk_tbl_hoa_don PRIMARY KEY (maHoaDon),
    CONSTRAINT ck_tbl_hoa_don_tongTien CHECK (tongTien >= 0)
);
GO

-- Bảng DichVuYTe
CREATE TABLE tbl_dich_vu_y_te (
    maDichVuYTe        INT             NOT NULL    IDENTITY(1,1),
    ten                NVARCHAR(100)   NOT NULL,
    chiPhi             DECIMAL(15, 2)  NOT NULL,
    CONSTRAINT pk_tbl_dich_vu_y_te PRIMARY KEY (maDichVuYTe),
    CONSTRAINT ck_tbl_dich_vu_y_te_chiPhi CHECK (chiPhi >= 0)
);
GO

-- Bảng LichSuThayDoi
CREATE TABLE tbl_lich_su_thay_doi (
    maLichSu        INT             NOT NULL    IDENTITY(1,1),
    maNhanVien      INT             NOT NULL,
    maBanGhi        INT             NOT NULL,
    thoiGian        DATETIME        NOT NULL    DEFAULT GETDATE(),
    bangLienQuan    NVARCHAR(100)   NOT NULL,
    hanhDong        NVARCHAR(100)   NOT NULL,
    CONSTRAINT pk_tbl_lich_su_thay_doi PRIMARY KEY (maLichSu)
);
GO

-- 3. TẠO KHÓA NGOẠI

-- Khóa ngoại cho tbl_nhan_vien
ALTER TABLE tbl_nhan_vien
	ADD CONSTRAINT fk_tbl_nhan_vien_tai_khoan FOREIGN KEY (maTaiKhoan) REFERENCES tbl_tai_khoan (maTaiKhoan)
		ON DELETE CASCADE
			ON UPDATE CASCADE;
GO

-- Khóa ngoại cho tbl_trieu_chung
ALTER TABLE tbl_trieu_chung
    ADD CONSTRAINT fk_tbl_trieu_chung_ho_so_y_te FOREIGN KEY (MaHoSoYTe) REFERENCES tbl_ho_so_y_te (maHoSoYTe)
		ON DELETE CASCADE
			ON UPDATE CASCADE;
GO

-- Khóa ngoại cho tbl_ket_qua_xet_nghiem
ALTER TABLE tbl_ket_qua_xet_nghiem
    ADD CONSTRAINT fk_tbl_ket_qua_xet_nghiem_ho_so_y_te FOREIGN KEY (MaHoSoYTe) REFERENCES tbl_ho_so_y_te (maHoSoYTe)
		ON DELETE CASCADE
			ON UPDATE CASCADE;
GO

-- Khóa ngoại cho tbl_tai_khoan
ALTER TABLE tbl_tai_khoan
	ADD CONSTRAINT fk_tbl_tai_khoan_vai_tro FOREIGN KEY (maVaiTro) REFERENCES tbl_vai_tro (maVaiTro)
		ON DELETE CASCADE
			ON UPDATE CASCADE;
GO

-- Khóa ngoại cho tbl_benh_nhan
ALTER TABLE tbl_benh_nhan
	ADD CONSTRAINT fk_tbl_benh_nhan_tai_khoan FOREIGN KEY (maTaiKhoan) REFERENCES tbl_tai_khoan (maTaiKhoan)
		ON DELETE NO ACTION
			ON UPDATE NO ACTION;
GO

-- Khóa ngoại cho tbl_phong_kham_nhan_vien
ALTER TABLE tbl_phong_kham_nhan_vien
	ADD CONSTRAINT fk_tbl_phong_kham_nhan_vien_phong_kham FOREIGN KEY (maPhongKham) REFERENCES tbl_phong_kham (maPhongKham)
		ON DELETE CASCADE
			ON UPDATE CASCADE,
	CONSTRAINT fk_tbl_phong_kham_nhan_vien_nhan_vien FOREIGN KEY (maNhanVien) REFERENCES tbl_nhan_vien (maNhanVien)
		ON DELETE CASCADE
			ON UPDATE CASCADE;
GO

-- Khóa ngoại cho tbl_lich_hen
ALTER TABLE tbl_lich_hen
	ADD CONSTRAINT fk_tbl_lich_hen_benh_nhan FOREIGN KEY (maBenhNhan) REFERENCES tbl_benh_nhan (maBenhNhan)
		ON DELETE CASCADE
			ON UPDATE CASCADE,
	CONSTRAINT fk_tbl_lich_hen_nhan_vien FOREIGN KEY (maNhanVien) REFERENCES tbl_nhan_vien (maNhanVien)
		ON DELETE CASCADE
			ON UPDATE CASCADE,
	CONSTRAINT fk_tbl_lich_hen_dich_vu_y_te FOREIGN KEY (maDichVuYTe) REFERENCES tbl_dich_vu_y_te (maDichVuYTe)
		ON DELETE SET NULL
			ON UPDATE CASCADE,
	CONSTRAINT fk_tbl_lich_hen_phong_kham FOREIGN KEY (maPhongKham) REFERENCES tbl_phong_kham (maPhongKham)
		ON DELETE CASCADE
			ON UPDATE CASCADE;
GO

-- Khóa ngoại cho tbl_ho_so_y_te
ALTER TABLE tbl_ho_so_y_te
	ADD CONSTRAINT fk_tbl_ho_so_y_te_benh_nhan FOREIGN KEY (maBenhNhan) REFERENCES tbl_benh_nhan (maBenhNhan)
		ON DELETE CASCADE
			ON UPDATE CASCADE;
GO

-- Khóa ngoại cho tbl_don_thuoc_chi_tiet
ALTER TABLE tbl_don_thuoc_chi_tiet
	ADD CONSTRAINT fk_tbl_don_thuoc_chi_tiet_don_thuoc FOREIGN KEY (maDonThuoc) REFERENCES tbl_don_thuoc (maDonThuoc)
		ON DELETE CASCADE
			ON UPDATE CASCADE,
	CONSTRAINT fk_tbl_don_thuoc_chi_tiet_thuoc FOREIGN KEY (maThuoc) REFERENCES tbl_thuoc (maThuoc)
		ON DELETE CASCADE
			ON UPDATE CASCADE;
GO

-- Khóa ngoại cho tbl_don_thuoc
ALTER TABLE tbl_don_thuoc
	ADD CONSTRAINT fk_tbl_don_thuoc_ho_so_y_te FOREIGN KEY (maHoSoYTe) REFERENCES tbl_ho_so_y_te (maHoSoYTe)
		ON DELETE CASCADE
			ON UPDATE CASCADE,
    CONSTRAINT fk_tbl_don_thuoc_hoa_don FOREIGN KEY (maHoaDon) REFERENCES tbl_hoa_don (maHoaDon)
        ON DELETE SET NULL
            ON UPDATE CASCADE;
GO

-- Khóa ngoại cho tbl_hoa_don
ALTER TABLE tbl_hoa_don
	ADD CONSTRAINT fk_tbl_hoa_don_lich_hen FOREIGN KEY (maLichHen) REFERENCES tbl_lich_hen (maLichHen)
		ON DELETE NO ACTION
			ON UPDATE NO ACTION;
GO

-- Khóa ngoại cho tbl_lich_su_thay_doi
ALTER TABLE tbl_lich_su_thay_doi
	ADD CONSTRAINT fk_tbl_lich_su_thay_doi_nhan_vien FOREIGN KEY (maNhanVien) REFERENCES tbl_nhan_vien (maNhanVien)
		ON DELETE CASCADE
			ON UPDATE CASCADE;
GO

-- Khóa ngoại cho tbl_ket_qua_dieu_tri
ALTER TABLE tbl_ket_qua_dieu_tri
    ADD CONSTRAINT fk_tbl_ket_qua_dieu_tri_ho_so_y_te FOREIGN KEY (MaHoSoYTe) REFERENCES tbl_ho_so_y_te (maHoSoYTe)
		ON DELETE NO ACTION
			ON UPDATE NO ACTION,
    CONSTRAINT fk_tbl_ket_qua_dieu_tri_don_thuoc FOREIGN KEY (MaDonThuoc) REFERENCES tbl_don_thuoc (maDonThuoc)
		ON DELETE NO ACTION
			ON UPDATE NO ACTION;
GO

-- 4. CHÈN DỮ LIỆU MẪU
-- Chèn dữ liệu vào tbl_vai_tro
INSERT INTO tbl_vai_tro (ten) VALUES
    ('QuanLy'),
    ('BacSi'),
    ('YTa'),
    ('LeTan'),
    ('KyThuatVienXetNghiem'),
    ('DuocSi'),
    ('KeToan'),
    ('BenhNhan'),
    ('TroLyBacSy'),
    ('NhanVienHanhChinh');
GO

-- Chèn dữ liệu vào tbl_tai_khoan
INSERT INTO tbl_tai_khoan (maVaiTro, tenDangNhap, matKhau) VALUES
    (1, 'admin', '$2a$11$tefBETGZk1g2HGBIKWtMOu6gnsC9Pk/1BkakgDKBjavu4iXxjPMgK'),	-- QuanLy -- pass: admin1111
    (2, 'bsnguyen', '$2a$11$3AiR4BJPF.oYRN6kcYHBxeQnrpOdk6iyhYgVLf89PWIWjnUcv0Xwq'),	-- BacSi -- pass: bsnguyen1111
    (2, 'bsmatdo', 'eye2023'),                -- BacSi
    (2, 'bstimngo', 'heart123'),              -- BacSi
    (2, 'bsnhivu', 'kid2023'),                -- BacSi
    (3, 'ytatran', '$2a$11$kyYa0L9pJdyC9t3XHrZDZ.sMwc5D/XIoL9OLkmsBBujlS40MDH.Nm'),		-- YTa --  pass: ytatran001
    (4, 'letanpham', '$2a$11$A6ZtPS3YLQAuIfpplADyLu2C3tLIZjZ7NhXhYnuZkFhxGjRk6Bnui'),   -- LeTan -- pass: letanpham001
    (5, 'ktvhoang', '$2a$11$ksNlbMg8cWmEQ3RIq9InZehw7Gmo7o4/u35gjoe/GQJQbWQUuVDPi'),    -- KyThuatVienXetNghiem	-- pass: ktvhoang001
    (6, 'duocsiphan001', '$2a$11$7CcjA8ONrr0Z7aJAj3Pp7eLH5Tbbz.hb32OveWv3imQf.OWXA3bSK'),-- DuocSi	-- pass: duocsiphan001
    (7, 'ketoantruong', '$2a$11$AWZAIMOmEQMSD3v3le3Ecuow1UkqaOqljqtqB694AbOVGEoPso9Wm'),-- KeToan	-- pass: ketoantruong001
    (9, 'trolybui', '$2a$11$wxI3CAXYNJlwXXLukvjkQuf9VBDrDEEN7dbktzNjuP2f.Xt2DxUZq'),    -- TroLyBacSy	-- pass: trolybui001
    (10, 'hanhchinhnguyen', '$2a$11$iCR.hgsaPLOnLsrmexD8.upGYJkQ8k02BSDnY5/16wCHaaixZV75y'),
	(8, 'benhnhan1', 'benhnhan1'),
    (8, 'benhnhan2', 'benhnhan2'),
    (8, 'benhnhan3', 'benhnhan3'),
    (8, 'benhnhan4', 'benhnhan4'),
    (8, 'benhnhan5', 'benhnhan5'),
    (8, 'benhnhan6', 'benhnhan6'),
    (8, 'benhnhan7', 'benhnhan7'),
    (8, 'benhnhan8', 'benhnhan8'),
    (8, 'benhnhan9', 'benhnhan9'),
    (8, 'benhnhan10', 'benhnhan10'),
    (8, 'benhnhan11', 'benhnhan11'),
    (8, 'benhnhan12', 'benhnhan12'),
    (8, 'benhnhan13', 'benhnhan13'),
    (8, 'benhnhan14', 'benhnhan14'),
    (8, 'benhnhan15', 'benhnhan15'),
	(8, 'benhnhan16', 'benhnhan16'),
    (8, 'benhnhan17', 'benhnhan17');
GO

-- Chèn dữ liệu vào tbl_nhan_vien
INSERT INTO tbl_nhan_vien (maTaiKhoan, ten, soDienThoai, caLamViec, chuyenMon) VALUES
    (1, N'Lê Thị D', '0934567890', N'Toàn thời gian', N'Quản lý'),            -- QuanLy
    (2, N'Nguyễn Văn A', '0901234567', N'Sáng', N'Nội khoa'),             -- BacSi
    (3, N'Đỗ Thị I', '0989012345', N'Chiều', N'Chuyên khoa mắt'),             -- BacSi
    (4, N'Ngô Văn J', '0990123456', N'Sáng', N'Chuyên khoa tim'),            -- BacSi
    (5, N'Vũ Thị K', '0902345678', N'Chiều', N'Chuyên khoa nhi'),             -- BacSi
    (6, N'Trần Thị B', '0912345678', N'Chiều', N'Hỗ trợ y tế'),             -- YTa
    (7, N'Phạm Văn C', '0923456789', N'Toàn thời gian', N'Lễ tân'),         -- LeTan
    (8, N'Hoàng Văn E', '0945678901', N'Sáng', N'Xét nghiệm'),             -- KyThuatVienXetNghiem
    (9, N'Phan Thị F', '0956789012', N'Chiều', N'Dược học'),                -- DuocSi
    (10, N'Trương Thị H', '0978901234', N'Sáng', N'Kế toán'),             -- KeToan
    (11, N'Bùi Thị M', '0924567890', N'Sáng', N'Trợ lý y tế'),               -- TroLyBacSy
    (12, N'Nguyễn Thị O', '0946789012', N'Toàn thời gian', N'Hành chính'); -- NhanVienHanhChinh
GO

-- Chèn dữ liệu vào tbl_phong_kham
INSERT INTO tbl_phong_kham (loai, sucChua) VALUES
    (N'Phòng khám nội khoa', 10),
    (N'Phòng khám mắt', 5),
    (N'Phòng khám tim mạch', 8),
    (N'Phòng khám nhi', 12),
    (N'Phòng xét nghiệm', 6),
    (N'Phòng cấp cứu', 4),
    (N'Phòng siêu âm', 5),
    (N'Phòng X-quang', 3),
    (N'Phòng tư vấn', 8),
    (N'Phòng tiêm chủng', 10),
    (N'Phòng vật lý trị liệu', 7),
    (N'Phòng khám tai mũi họng', 6),
    (N'Phòng khám da liễu', 5),
    (N'Phòng khám tổng quát', 15);
GO

-- Chèn dữ liệu vào tbl_phong_kham_nhan_vien
INSERT INTO tbl_phong_kham_nhan_vien (maPhongKham, maNhanVien, vaiTro) VALUES
    (1, 2, N'Bác sĩ chính'),     -- Nguyễn Văn A (Nội khoa)
    (2, 3, N'Bác sĩ chuyên khoa'), -- Đỗ Thị I (Mắt)
    (3, 4, N'Bác sĩ chính'),     -- Ngô Văn J (Tim mạch)
    (4, 5, N'Bác sĩ chính'),     -- Vũ Thị K (Nhi)
    (5, 8, N'Kỹ thuật viên'),    -- Hoàng Văn E (Xét nghiệm)
    (1, 6, N'Y tá hỗ trợ'),      -- Trần Thị B (Hỗ trợ y tế)
    (10, 11, N'Trợ lý bác sĩ'),  -- Bùi Thị M (Trợ lý y tế)
    (6, 8, N'Kỹ thuật viên'),    -- Hoàng Văn E (Cấp cứu)
    (7, 8, N'Kỹ thuật viên'),    -- Hoàng Văn E (Siêu âm)
    (9, 7, N'Lễ tân'),           -- Phạm Văn C (Lễ tân)
    (10, 6, N'Y tá tiêm chủng'), -- Trần Thị B (Tiêm chủng)
    (11, 11, N'Trợ lý'),         -- Bùi Thị M (Vật lý trị liệu)
    (12, 2, N'Bác sĩ khám'),     -- Nguyễn Văn A (Tai mũi họng)
    (13, 3, N'Bác sĩ khám'),     -- Đỗ Thị I (Da liễu)
    (14, 2, N'Bác sĩ tổng quát'); -- Nguyễn Văn A (Tổng quát)
GO

-- Chèn dữ liệu vào tbl_dich_vu_y_te
INSERT INTO tbl_dich_vu_y_te (ten, chiPhi) VALUES
    (N'Khám nội khoa', 150000),
    (N'Khám mắt', 200000),
    (N'Khám tim mạch', 250000),
    (N'Khám nhi', 180000),
    (N'Xét nghiệm máu', 300000),
    (N'Siêu âm', 250000),
    (N'X-quang', 200000),
    (N'Tiêm chủng', 100000),
    (N'Vật lý trị liệu', 150000),
    (N'Khám tai mũi họng', 180000),
    (N'Khám da liễu', 200000),
    (N'Khám tổng quát', 300000),
    (N'Cấp cứu', 400000);
GO

-- Chèn dữ liệu vào tbl_benh_nhan
INSERT INTO tbl_benh_nhan (maTaiKhoan, ten, tuoi, gioiTinh, diaChi, soDienThoai) VALUES
    (13, N'Nguyễn Văn An', 30, 1, N'123 Đường Láng, Đống Đa, Hà Nội', '0912345678'),
    (14, N'Trần Thị Bình', 45, 0, N'45 Lê Lợi, Quận 1, TP.HCM', '0923456789'),
    (15, N'Lê Minh Châu', 25, 1, N'67 Trần Phú, Hải Châu, Đà Nẵng', '0934567890'),
    (16, N'Phạm Thị Dung', 60, 0, N'89 Nguyễn Huệ, TP Huế, Thừa Thiên Huế', '0945678901'),
    (17, N'Hoàng Văn Em', 10, 1, N'12 Hùng Vương, Lộc Thọ, Nha Trang', '0956789012'),
    (18, N'Vũ Thị Hoa', 35, 0, N'34 Điện Biên Phủ, Ba Đình, Hà Nội', '0967890123'),
    (19, N'Đỗ Văn Hùng', 50, 1, N'56 Phạm Ngũ Lão, Quận 1, TP.HCM', '0978901234'),
    (20, N'Bùi Thị Kim', 28, 0, N'78 Lý Thường Kiệt, Ngũ Hành Sơn, Đà Nẵng', '0989012345'),
    (21, N'Ngô Văn Long', 70, 1, N'90 Hai Bà Trưng, TP Huế, Thừa Thiên Huế', '0990123456'),
    (22, N'Tạ Thị Mai', 15, 0, N'23 Nguyễn Trãi, Phước Tiến, Nha Trang', '0911234567'),
    (23, N'Phan Văn Nam', 40, 1, N'45 Lê Đại Hành, Hai Bà Trưng, Hà Nội', '0922345678'),
    (24, N'Lý Thị Oanh', 55, 0, N'67 Nguyễn Thị Minh Khai, Quận 3, TP.HCM', '0933456789'),
    (25, N'Mai Văn Phong', 33, 1, N'89 Bùi Thị Xuân, Thanh Khê, Đà Nẵng', '0944567890'),
    (26, N'Đặng Thị Quyên', 22, 0, N'12 Trần Hưng Đạo, Phú Hòa, Huế', '0955678901'),
    (27, N'Hà Văn Sơn', 65, 1, N'34 Lê Hồng Phong, Vĩnh Nghiêm, Nha Trang', '0966789012');
GO

-- Chèn dữ liệu vào tbl_lich_hen
INSERT INTO tbl_lich_hen (maBenhNhan, maNhanVien, maDichVuYTe, maPhongKham, ngayHen, trangThai) VALUES
    -- Trường hợp 1: Chưa phân công (maNhanVien = NULL), Chờ xác nhận
    (1, NULL, 1, 1, '07-12-2025 08:00', N'Chờ xác nhận'),   -- Khám nội khoa, chưa phân công bác sĩ

    -- Trường hợp 2: Đã phân công, Chờ xác nhận
    (2, 3, 2, 2, '07-12-2025 09:00', N'Chờ xác nhận'),      -- Khám mắt, bác sĩ Đỗ Thị I

    -- Trường hợp 3: Đã phân công, Đã xác nhận
    (3, 4, 3, 3, '08-12-2025 10:00', N'Đã xác nhận'),       -- Khám tim mạch, bác sĩ Ngô Văn J

    -- Trường hợp 4: Đã phân công, Đã hoàn thành
    (4, 5, 4, 4, '08-12-2025 14:00', N'Đã hoàn thành'),     -- Khám nhi, bác sĩ Vũ Thị K

    -- Trường hợp 5: Đã phân công, Đã hủy
    (5, 8, 5, 5, '09-12-2025 08:30', N'Đã hủy'),            -- Xét nghiệm máu, kỹ thuật viên Hoàng Văn E

    -- Trường hợp 6: Chưa phân công, Chờ xác nhận
    (6, NULL, 6, 7, '09-12-2025 15:00', N'Chờ xác nhận'),   -- Siêu âm, chưa phân công

    -- Trường hợp 7: Đã phân công, Đã xác nhận
    (7, 8, 7, 8, '10-12-2025 09:30', N'Đã xác nhận'),       -- X-quang, kỹ thuật viên Hoàng Văn E

    -- Trường hợp 8: Đã phân công, Đã hoàn thành
    (8, 6, 8, 10, '10-12-2025 13:00', N'Đã hoàn thành'),    -- Tiêm chủng, y tá Trần Thị B

    -- Trường hợp 9: Đã phân công, Đã hủy
    (9, 11, 9, 11, '11-12-2025 10:00', N'Đã hủy'),          -- Vật lý trị liệu, trợ lý Bùi Thị M

    -- Trường hợp 10: Chưa phân công, Chờ xác nhận
    (10, NULL, 10, 12, '11-12-2025 14:30', N'Chờ xác nhận'), -- Khám tai mũi họng, chưa phân công

    -- Trường hợp 11: Đã phân công, Đã xác nhận
    (11, 3, 11, 13, '12-12-2025 08:00', N'Đã xác nhận'),    -- Khám da liễu, bác sĩ Đỗ Thị I

    -- Trường hợp 12: Đã phân công, Đã hoàn thành
    (12, 2, 12, 14, '12-12-2025 15:00', N'Đã hoàn thành'),  -- Khám tổng quát, bác sĩ Nguyễn Văn A

    -- Trường hợp 13: Đã phân công, Chờ xác nhận
    (13, 7, 10, 12, '13-12-2025 09:00', N'Chờ xác nhận'),   -- Khám tai mũi họng, lễ tân Phạm Văn C

    -- Trường hợp 14: Chưa phân công, Đã hủy
    (14, NULL, 12, 14, '13-12-2025 14:00', N'Đã hủy'),      -- Khám tổng quát, chưa phân công

    -- Trường hợp 15: Đã phân công, Đã xác nhận
    (15, 2, 13, 6, '14-12-2025 10:30', N'Đã xác nhận');     -- Cấp cứu, bác sĩ Nguyễn Văn A
GO

-- Chèn dữ liệu vào tbl_ho_so_y_te
INSERT INTO tbl_ho_so_y_te (maBenhNhan, chuanDoan, phuongPhapDieuTri, LichSuBenh) VALUES
    (1, N'Viêm họng cấp', N'Uống kháng sinh và giảm đau', N'Không có bệnh lý nền'),
    (2, N'Cận thị', N'Đeo kính điều chỉnh', N'Không có bệnh lý nền'),
    (3, N'Tăng huyết áp', N'Uống thuốc hạ áp, theo dõi', N'Tiểu đường type 2'),
    (4, N'Sốt xuất huyết', N'Truyền dịch, nghỉ ngơi', N'Không có bệnh lý nền'),
    (5, N'Thiếu máu', N'Bổ sung sắt, vitamin', N'Không có bệnh lý nền'),
    (6, N'Viêm dạ dày', N'Thuốc giảm axit, kiêng đồ chua', N'Viêm loét dạ dày cũ'),
    (7, N'Gãy xương cẳng tay', N'Bó bột, nghỉ ngơi', N'Không có bệnh lý nền'),
    (8, N'Viêm da dị ứng', N'Bôi kem chống viêm', N'Dị ứng da mãn tính'),
    (9, N'Viêm thận cấp', N'Kháng sinh, theo dõi chức năng thận', N'Không có bệnh lý nền'),
    (10, N'Sốt virus', N'Nghỉ ngơi, hạ sốt', N'Không có bệnh lý nền'),
    (11, N'Viêm xoang mạn', N'Rửa xoang, kháng sinh', N'Không có bệnh lý nền'),
    (12, N'Đau lưng cơ học', N'Vật lý trị liệu, giảm đau', N'Không có bệnh lý nền'),
    (13, N'Tiểu đường type 2', N'Thuốc hạ đường huyết, chế độ ăn', N'Béo phì'),
    (14, N'Chàm (eczema)', N'Bôi kem dưỡng ẩm, tránh kích ứng', N'Không có bệnh lý nền'),
    (15, N'Suy tim nhẹ', N'Thuốc hỗ trợ tim mạch, nghỉ ngơi', N'Tăng huyết áp cũ');
GO

-- Chèn dữ liệu vào tbl_trieu_chung
INSERT INTO tbl_trieu_chung (MaHoSoYTe, TenTrieuChung, MoTa, ThoiGianXuatHien) VALUES
    (1, N'Đau họng', N'Đau rát khi nuốt, 3 ngày', '16-12-2025 08:00:00'),
    (2, N'Mờ mắt', N'Mờ khi đọc sách, 1 tuần', '15-12-2025 09:00:00'),
    (3, N'Chóng mặt', N'Chóng mặt khi đứng lên, 2 ngày', '17-12-2025 07:00:00'),
    (4, N'Sốt cao', N'39°C, mệt mỏi, 2 ngày', '16-12-2025 10:00:00'),
    (5, N'Mệt mỏi', N'Mệt kéo dài, da nhợt, 5 ngày', '14-12-2025 08:00:00'),
    (6, N'Đau thượng vị', N'Đau âm ỉ sau ăn, 3 ngày', '15-12-2025 11:00:00'),
    (7, N'Đau tay', N'Đau sau té ngã, sưng nhẹ', '13-12-2025 14:00:00'),
    (8, N'Ngứa da', N'Ngứa toàn thân, nổi mẩn đỏ', '18-12-2025 09:00:00'),
    (9, N'Đau lưng dưới', N'Đau khi đứng lâu, 4 ngày', '19-12-2025 08:00:00'),
    (10, N'Hắt hơi', N'Hắt hơi nhiều, chảy nước mũi', '17-12-2025 10:00:00'),
    (11, N'Nghẹt mũi', N'Nghẹt mũi kéo dài, 1 tuần', '16-12-2025 07:00:00'),
    (12, N'Đau lưng', N'Đau khi cúi, 3 ngày', '18-12-2025 08:00:00'),
    (13, N'Khát nước', N'Khát nước nhiều, tiểu nhiều', '15-12-2025 09:00:00'),
    (14, N'Ngứa da tay', N'Ngứa kèm khô da, 5 ngày', '17-12-2025 10:00:00'),
    (15, N'Khó thở', N'Khó thở khi nằm, 2 ngày', '16-12-2025 11:00:00');
GO

-- Chèn dữ liệu vào tbl_ket_qua_xet_nghiem
INSERT INTO tbl_ket_qua_xet_nghiem (MaHoSoYTe, TenXetNghiem, KetQua, NgayXetNghiem) VALUES
    (1, N'Xét nghiệm máu', N'WBC: 9.5K/mm³', '17-12-2025 14:00:00'),
    (2, N'Kiểm tra thị lực', N'Tầm nhìn: 0.5', '16-12-2025 10:00:00'),
    (3, N'Đo huyết áp', N'150/95 mmHg', '18-12-2025 09:00:00'),
    (4, N'Xét nghiệm máu', N'Dengue NS1 dương tính', '17-12-2025 11:00:00'),
    (5, N'Xét nghiệm máu', N'Hb: 10g/dL, Ferritin: 20ng/mL', '15-12-2025 13:00:00'),
    (6, N'Nội soi dạ dày', N'Viêm niêm mạc nhẹ', '16-12-2025 15:00:00'),
    (7, N'X-quang tay', N'Gãy xương quay', '14-12-2025 10:00:00'),
    (8, N'Test dị ứng da', N'Dương tính với bụi nhà', '19-12-2025 11:00:00'),
    (9, N'Xét nghiệm nước tiểu', N'Protein: 2+, Creatinine: 1.5mg/dL', '20-12-2025 09:00:00'),
    (10, N'Xét nghiệm máu', N'CRP: 15mg/L', '18-12-2025 10:00:00'),
    (11, N'Chụp CT xoang', N'Viêm xoang hàm', '17-12-2025 14:00:00'),
    (12, N'X-quang cột sống', N'Thoái hóa nhẹ', '19-12-2025 13:00:00'),
    (13, N'Đường huyết', N'Glucose: 180mg/dL', '16-12-2025 08:00:00'),
    (14, N'Test da liễu', N'Dương tính với hóa chất', '18-12-2025 11:00:00'),
    (15, N'Điện tâm đồ', N'Sóng T bất thường', '17-12-2025 15:00:00');
GO

-- Chèn dữ liệu vào tbl_hoa_don
INSERT INTO tbl_hoa_don (maLichHen, tongTien, ngayThanhToan, trangThaiThanhToan) VALUES
    (1, 150000, '19-12-2025 09:00', N'Đã thanh toán'),
    (2, 200000, NULL, N'Chưa thanh toán'),
    (3, 250000, '19-12-2025 11:00', N'Đã thanh toán'),
    (4, 180000, '18-12-2025 15:00', N'Đã thanh toán'),
    (5, 300000, NULL, N'Chưa thanh toán'),
    (6, 250000, '17-12-2025 16:00', N'Đã thanh toán'),
    (7, 200000, '15-12-2025 10:30', N'Đã thanh toán'),
    (8, 100000, '20-12-2025 14:00', N'Đã thanh toán'),
    (9, 150000, NULL, N'Chưa thanh toán'),
    (10, 180000, '19-12-2025 15:30', N'Đã thanh toán'),
    (11, 200000, '18-12-2025 09:00', N'Đã thanh toán'),
    (12, 300000, '20-12-2025 16:00', N'Đã thanh toán'),
    (13, 180000, NULL, N'Chưa thanh toán'),
    (14, 300000, '19-12-2025 15:00', N'Đã thanh toán'),
    (15, 400000, '18-12-2025 11:30', N'Đã thanh toán');
GO

-- Chèn dữ liệu vào tbl_don_thuoc
INSERT INTO tbl_don_thuoc (maHoSoYTe, maHoaDon, ngayKeDon) VALUES
    (1, 1, '18-12-2025 15:00:00'),
    (2, 2, '16-12-2025 11:00:00'),
    (3, 3, '18-12-2025 10:00:00'),
    (4, 4, '17-12-2025 12:00:00'),
    (5, 5, '15-12-2025 14:00:00'),
    (6, 6, '16-12-2025 16:00:00'),
    (7, 7, '14-12-2025 11:00:00'),
    (8, 8, '19-12-2025 12:00:00'),
    (9, 9, '20-12-2025 10:00:00'),
    (10, 10, '18-12-2025 11:00:00'),
    (11, 11, '17-12-2025 15:00:00'),
    (12, 12, '19-12-2025 14:00:00'),
    (13, 13, '16-12-2025 09:00:00'),
    (14, 14, '18-12-2025 12:00:00'),
    (15, 15, '17-12-2025 16:00:00');
GO

-- Chèn dữ liệu vào tbl_thuoc
INSERT INTO tbl_thuoc (ten, moTa, donVi, ChongChiDinh, TuongTacThuoc, donGia) VALUES
    (N'Amoxicillin', N'Kháng sinh điều trị nhiễm khuẩn', N'viên', N'Không dùng cho dị ứng penicillin', N'Không dùng với Methotrexat', 5000),
    (N'Paracetamol', N'Giảm đau, hạ sốt', N'viên', N'Không dùng cho suy gan', N'Không dùng với rượu', 2000),
    (N'Amlodipine', N'Hạ huyết áp', N'viên', N'Không dùng cho sốc', N'Không dùng với Simvastatin', 3000),
    (N'Sắt viên', N'Bổ sung sắt cho thiếu máu', N'viên', N'Không dùng cho bệnh thalassemia', N'Không dùng với kháng sinh', 1500),
    (N'Omeprazole', N'Giảm axit dạ dày', N'viên', N'Không dùng cho suy gan nặng', N'Không dùng với Clopidogrel', 4000),
    (N'Hydrocortisone', N'Kem bôi chống viêm', N'tuýp', N'Không dùng cho nhiễm trùng da', N'Không dùng với thuốc khác bôi ngoài da', 25000),
    (N'Natri Clorid', N'Dung dịch truyền bổ sung nước', N'chai', N'Không dùng cho phù phổi', N'Không dùng với Kali-sparing diuretics', 10000),
    (N'Aspirin', N'Giảm đau, phòng nhồi máu', N'viên', N'Không dùng cho loét dạ dày', N'Không dùng với Warfarin', 1000),
    (N'Vitamin C', N'Tăng sức đề kháng', N'viên', N'Không dùng cho sỏi thận', N'Không dùng với Aspirin liều cao', 500),
    (N'Cefalexin', N'Kháng sinh phổ rộng', N'viên', N'Không dùng cho dị ứng cephalosporin', N'Không dùng với Probenecid', 6000),
    (N'Ibuprofen', N'Giảm đau, kháng viêm', N'viên', N'Không dùng cho loét dạ dày hoặc suy thận', N'Không dùng với Aspirin liều cao', 3000),
    (N'Loratadine', N'Chống dị ứng, giảm ngứa', N'viên', N'Không dùng cho bệnh nhân suy gan nặng', N'Không dùng với Ketoconazole', 2500),
    (N'Dexamethasone', N'Chống viêm, dị ứng', N'viên', N'Không dùng cho nhiễm trùng toàn thân', N'Không dùng với thuốc chống đông máu', 4000),
    (N'Atorvastatin', N'Hạ cholesterol máu', N'viên', N'Không dùng cho bệnh gan tiến triển', N'Không dùng với Cyclosporine', 7000),
    (N'Metformin', N'Hạ đường huyết', N'viên', N'Không dùng cho suy thận nặng', N'Không dùng với rượu', 2000);
GO

-- Chèn dữ liệu vào tbl_don_thuoc_chi_tiet
INSERT INTO tbl_don_thuoc_chi_tiet (maDonThuoc, maThuoc, soLuong, cachDung, LieuLuong, TanSuat, ThanhTien) VALUES
    (1, 1, 15, N'Uống sau ăn', N'500mg', N'3 lần/ngày', 75000.00),       -- Amoxicillin
    (2, 9, 30, N'Uống sáng', N'500mg', N'1 lần/ngày', 15000.00),         -- Vitamin C
    (3, 3, 30, N'Uống tối', N'5mg', N'1 lần/ngày', 90000.00),            -- Amlodipine
    (4, 7, 3, N'Truyền tĩnh mạch', N'500ml', N'1 lần/ngày', 30000.00),   -- Natri Clorid
    (5, 4, 30, N'Uống sau ăn', N'100mg', N'1 lần/ngày', 45000.00),       -- Sắt viên
    (6, 5, 14, N'Uống trước ăn', N'20mg', N'2 lần/ngày', 56000.00),      -- Omeprazole
    (7, 2, 10, N'Uống khi đau', N'500mg', N'4-6 giờ/lần', 20000.00),     -- Paracetamol
    (8, 6, 1, N'Bôi ngoài da', N'1%', N'2 lần/ngày', 25000.00),          -- Hydrocortisone
    (9, 10, 14, N'Uống sau ăn', N'250mg', N'2 lần/ngày', 84000.00),      -- Cefalexin
    (10, 2, 10, N'Uống khi sốt', N'500mg', N'4-6 giờ/lần', 20000.00),    -- Paracetamol
    (11, 1, 10, N'Uống sau ăn', N'500mg', N'3 lần/ngày', 50000.00),      -- Amoxicillin
    (12, 11, 10, N'Uống khi đau', N'200mg', N'3 lần/ngày', 30000.00),    -- Ibuprofen
    (13, 8, 30, N'Uống sáng', N'100mg', N'1 lần/ngày', 30000.00),        -- Aspirin
    (14, 12, 10, N'Uống tối', N'10mg', N'1 lần/ngày', 25000.00),         -- Loratadine
    (15, 14, 30, N'Uống tối', N'10mg', N'1 lần/ngày', 210000.00);        -- Atorvastatin
GO

-- Chèn dữ liệu vào tbl_lich_su_thay_doi
INSERT INTO tbl_lich_su_thay_doi (maNhanVien, maBanGhi, thoiGian, bangLienQuan, hanhDong) VALUES
    (2, 1, '06-12-2025 08:00', N'tbl_lich_hen', N'Tạo lịch hẹn'),    -- Nguyễn Văn A
    (3, 2, '06-12-2025 09:00', N'tbl_lich_hen', N'Tạo lịch hẹn'),    -- Đỗ Thị I
    (4, 3, '06-12-2025 10:00', N'tbl_lich_hen', N'Tạo lịch hẹn'),    -- Ngô Văn J
    (5, 4, '06-12-2025 11:00', N'tbl_lich_hen', N'Cập nhật trạng thái'), -- Vũ Thị K
    (8, 5, '06-12-2025 12:00', N'tbl_lich_hen', N'Tạo lịch hẹn'),    -- Hoàng Văn E
    (2, 1, '07-12-2025 09:00', N'tbl_hoa_don', N'Thanh toán hóa đơn'), -- Nguyễn Văn A
    (7, 2, '07-12-2025 10:00', N'tbl_hoa_don', N'Tạo hóa đơn'),      -- Phạm Văn C
    (4, 3, '08-12-2025 11:00', N'tbl_hoa_don', N'Thanh toán hóa đơn'), -- Ngô Văn J
    (5, 4, '08-12-2025 15:00', N'tbl_hoa_don', N'Thanh toán hóa đơn'), -- Vũ Thị K
    (8, 5, '09-12-2025 09:30', N'tbl_hoa_don', N'Tạo hóa đơn'),      -- Hoàng Văn E
    (2, 6, '09-12-2025 16:00', N'tbl_hoa_don', N'Thanh toán hóa đơn'), -- Nguyễn Văn A
    (8, 7, '10-12-2025 10:30', N'tbl_hoa_don', N'Thanh toán hóa đơn'), -- Hoàng Văn E
    (6, 8, '10-12-2025 14:00', N'tbl_hoa_don', N'Thanh toán hóa đơn'), -- Trần Thị B
    (11, 9, '11-12-2025 11:00', N'tbl_hoa_don', N'Tạo hóa đơn'),      -- Bùi Thị M
    (2, 10, '11-12-2025 15:30', N'tbl_hoa_don', N'Thanh toán hóa đơn'); -- Nguyễn Văn A
GO
-- Chèn dữ liệu vào tbl_trieu_chung
INSERT INTO tbl_trieu_chung (MaHoSoYTe, TenTrieuChung, MoTa, ThoiGianXuatHien) VALUES
    (1, N'Đau họng', N'Đau nhẹ, kéo dài 2 ngày', '18-12-2025'),
    (2, N'Mờ mắt', N'Mờ mắt khi đọc sách', '18-12-2025'),
    (3, N'Đau đầu', N'Đau đầu nhẹ, kèm chóng mặt', '17-12-2025'),
    (4, N'Sốt', N'Sốt 38°C, mệt mỏi', '16-12-2025'),
    (5, N'Mệt mỏi', N'Mệt mỏi kéo dài', '15-12-2025'),
    (6, N'Đau dạ dày', N'Đau vùng thượng vị', '14-12-2025'),
    (7, N'Đau tay', N'Đau tay trái sau té ngã', '13-12-2025'),
    (8, N'Ngứa da', N'Ngứa da toàn thân', '20-12-2025'),
    (9, N'Đau lưng', N'Đau lưng dưới', '27-12-2025'),
    (10, N'Hắt hơi', N'Hắt hơi liên tục', '19-12-2025'),
    (11, N'Đau đầu', N'Đau đầu nhẹ', '19-12-2025'),
    (12, N'Đau lưng', N'Đau lưng nhẹ', '28-12-2025'),
    (13, N'Khát nước', N'Khát nước nhiều', '19-12-2025'),
    (14, N'Ngứa da', N'Ngứa da vùng tay', '07-12-2025'),
    (15, N'Đau ngực', N'Đau ngực trái', '19-12-2025');
GO

-- Chèn dữ liệu vào tbl_ket_qua_xet_nghiem
INSERT INTO tbl_ket_qua_xet_nghiem (MaHoSoYTe, TenXetNghiem, KetQua, NgayXetNghiem) VALUES
    (1, N'Xét nghiệm máu', N'Hb: 12g/dL', '20-12-2025'),
    (2, N'Kiểm tra mắt', N'Tầm nhìn: 0.5', '19-12-2025'),
    (3, N'Xét nghiệm huyết áp', N'150/90 mmHg', '18-12-2025'),
    (4, N'Xét nghiệm máu', N'WBC: 12000/mm3', '17-12-2025'),
    (5, N'Xét nghiệm máu', N'Iron: 50 µg/dL', '16-12-2025'),
    (6, N'Nội soi dạ dày', N'Viêm nhẹ', '15-12-2025'),
    (7, N'X-quang tay', N'Gãy xương', '14-12-2025'),
    (8, N'Test dị ứng', N'Dương tính', '20-12-2025'),
    (9, N'Xét nghiệm nước tiểu', N'Protein: 2+', '28-12-2025'),
    (10, N'Xét nghiệm máu', N'WBC: 8000/mm3', '20-12-2025'),
    (11, N'X-quang xoang', N'Viêm xoang', '20-12-2025'),
    (12, N'X-quang lưng', N'Không bất thường', '29-12-2025'),
    (13, N'Xét nghiệm đường huyết', N'Glucose: 180 mg/dL', '20-12-2025'),
    (14, N'Test dị ứng', N'Dương tính', '08-12-2025'),
    (15, N'Điện tâm đồ', N'Bất thường', '20-12-2025');
GO

-- Chèn dữ liệu vào tbl_ket_qua_dieu_tri
INSERT INTO tbl_ket_qua_dieu_tri (MaHoSoYTe, MaDonThuoc, HieuQua, TacDungPhu, NgayDanhGia) VALUES
    (1, 1, N'Hiệu quả tốt', N'Buồn nôn nhẹ', '23-12-2025 09:00:00'),
    (2, 2, N'Cải thiện', N'Không có', '21-12-2025 10:00:00'),
    (3, 3, N'Cải thiện', N'Nhức đầu nhẹ', '23-12-2025 08:00:00'),
    (4, 4, N'Hiệu quả tốt', N'Không có', '22-12-2025 11:00:00'),
    (5, 5, N'Cải thiện', N'Táo bón', '20-12-2025 13:00:00'),
    (6, 6, N'Hiệu quả tốt', N'Không có', '21-12-2025 14:00:00'),
    (7, 7, N'Hiệu quả tốt', N'Không có', '19-12-2025 09:00:00'),
    (8, 8, N'Cải thiện', N'Khô da nhẹ', '24-12-2025 10:00:00'),
    (9, 9, N'Hiệu quả tốt', N'Không có', '25-12-2025 11:00:00'),
    (10, 10, N'Hiệu quả tốt', N'Không có', '23-12-2025 12:00:00'),
    (11, 11, N'Cải thiện', N'Buồn ngủ', '22-12-2025 13:00:00'),
    (12, 12, N'Hiệu quả tốt', N'Không có', '24-12-2025 14:00:00'),
    (13, 13, N'Cải thiện', N'Không có', '21-12-2025 09:00:00'),
    (14, 14, N'Hiệu quả tốt', N'Không có', '23-12-2025 10:00:00'),
    (15, 15, N'Cải thiện', N'Nhịp tim nhanh', '22-12-2025 15:00:00');
GO