﻿using System;
using System.Collections.Generic;
using HeThongHoTroVaQuanLyPhongKham.Models;
using Microsoft.EntityFrameworkCore;

namespace HeThongHoTroVaQuanLyPhongKham.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblBenhNhan> TblBenhNhans { get; set; }

    public virtual DbSet<TblDichVuYTe> TblDichVuYTes { get; set; }

    public virtual DbSet<TblDonThuoc> TblDonThuocs { get; set; }

    public virtual DbSet<TblDonThuocThuoc> TblDonThuocThuocs { get; set; }

    public virtual DbSet<TblHoSoYTe> TblHoSoYTes { get; set; }

    public virtual DbSet<TblHoaDon> TblHoaDons { get; set; }

    public virtual DbSet<TblLichHen> TblLichHens { get; set; }

    public virtual DbSet<TblLichSuThayDoi> TblLichSuThayDois { get; set; }

    public virtual DbSet<TblNhanVien> TblNhanViens { get; set; }

    public virtual DbSet<TblPhongKham> TblPhongKhams { get; set; }

    public virtual DbSet<TblPhongKhamNhanVien> TblPhongKhamNhanViens { get; set; }

    public virtual DbSet<TblTaiKhoan> TblTaiKhoans { get; set; }

    public virtual DbSet<TblThuoc> TblThuocs { get; set; }

    public virtual DbSet<TblVaiTro> TblVaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DbConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblBenhNhan>(entity =>
        {
            entity.HasKey(e => e.MaBenhNhan).HasName("pk_tbl_benh_nhan");

            entity.ToTable("tbl_benh_nhan");

            entity.HasIndex(e => e.Email, "uq_tbl_benh_nhan_email").IsUnique();

            entity.HasIndex(e => e.SoDienThoai, "uq_tbl_benh_nhan_soDienThoai").IsUnique();

            entity.Property(e => e.MaBenhNhan).HasColumnName("maBenhNhan");
            entity.Property(e => e.DiaChi).HasColumnName("diaChi");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.GioiTinh).HasColumnName("gioiTinh");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("soDienThoai");
            entity.Property(e => e.Tuoi).HasColumnName("tuoi");
        });

        modelBuilder.Entity<TblDichVuYTe>(entity =>
        {
            entity.HasKey(e => e.MaDichVuYte).HasName("pk_tbl_dich_vu_y_te");

            entity.ToTable("tbl_dich_vu_y_te");

            entity.Property(e => e.MaDichVuYte).HasColumnName("maDichVuYTe");
            entity.Property(e => e.ChiPhi)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("chiPhi");
            entity.Property(e => e.Ten)
                .HasMaxLength(100)
                .HasColumnName("ten");
        });

        modelBuilder.Entity<TblDonThuoc>(entity =>
        {
            entity.HasKey(e => e.MaDonThuoc).HasName("pk_tbl_don_thuoc");

            entity.ToTable("tbl_don_thuoc");

            entity.Property(e => e.MaDonThuoc).HasColumnName("maDonThuoc");
            entity.Property(e => e.LieuLuong).HasColumnName("lieuLuong");
            entity.Property(e => e.MaHoSoYte).HasColumnName("maHoSoYTe");

            entity.HasOne(d => d.MaHoSoYteNavigation).WithMany(p => p.TblDonThuocs)
                .HasForeignKey(d => d.MaHoSoYte)
                .HasConstraintName("fk_tbl_don_thuoc_ho_so_y_te");
        });

        modelBuilder.Entity<TblDonThuocThuoc>(entity =>
        {
            entity.HasKey(e => new { e.MaDonThuoc, e.MaThuoc }).HasName("pk_tbl_don_thuoc_thuoc");

            entity.ToTable("tbl_don_thuoc_thuoc");

            entity.Property(e => e.MaDonThuoc).HasColumnName("maDonThuoc");
            entity.Property(e => e.MaThuoc).HasColumnName("maThuoc");
            entity.Property(e => e.SoLuong).HasColumnName("soLuong");

            entity.HasOne(d => d.MaDonThuocNavigation).WithMany(p => p.TblDonThuocThuocs)
                .HasForeignKey(d => d.MaDonThuoc)
                .HasConstraintName("fk_tbl_don_thuoc_thuoc_don_thuoc");

            entity.HasOne(d => d.MaThuocNavigation).WithMany(p => p.TblDonThuocThuocs)
                .HasForeignKey(d => d.MaThuoc)
                .HasConstraintName("fk_tbl_don_thuoc_thuoc_thuoc");
        });

        modelBuilder.Entity<TblHoSoYTe>(entity =>
        {
            entity.HasKey(e => e.MaHoSoYte).HasName("pk_tbl_ho_so_y_te");

            entity.ToTable("tbl_ho_so_y_te");

            entity.Property(e => e.MaHoSoYte).HasColumnName("maHoSoYTe");
            entity.Property(e => e.ChuanDoan).HasColumnName("chuanDoan");
            entity.Property(e => e.MaBenhNhan).HasColumnName("maBenhNhan");
            entity.Property(e => e.PhuongPhapDieuTri).HasColumnName("phuongPhapDieuTri");

            entity.HasOne(d => d.MaBenhNhanNavigation).WithMany(p => p.TblHoSoYTes)
                .HasForeignKey(d => d.MaBenhNhan)
                .HasConstraintName("fk_tbl_ho_so_y_te_benh_nhan");
        });

        modelBuilder.Entity<TblHoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("pk_tbl_hoa_don");

            entity.ToTable("tbl_hoa_don");

            entity.Property(e => e.MaHoaDon).HasColumnName("maHoaDon");
            entity.Property(e => e.MaLichHen).HasColumnName("maLichHen");
            entity.Property(e => e.NgayThanhToan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ngayThanhToan");
            entity.Property(e => e.TongTien)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("tongTien");
            entity.Property(e => e.TrangThaiThanhToan)
                .HasMaxLength(50)
                .HasDefaultValue("Chưa thanh toán")
                .HasColumnName("trangThaiThanhToan");

            entity.HasOne(d => d.MaLichHenNavigation).WithMany(p => p.TblHoaDons)
                .HasForeignKey(d => d.MaLichHen)
                .HasConstraintName("fk_tbl_hoa_don_lich_hen");
        });

        modelBuilder.Entity<TblLichHen>(entity =>
        {
            entity.HasKey(e => e.MaLichHen).HasName("pk_tbl_lich_hen");

            entity.ToTable("tbl_lich_hen");

            entity.Property(e => e.MaLichHen).HasColumnName("maLichHen");
            entity.Property(e => e.MaBenhNhan).HasColumnName("maBenhNhan");
            entity.Property(e => e.MaDichVuYte).HasColumnName("maDichVuYTe");
            entity.Property(e => e.MaNhanVien).HasColumnName("maNhanVien");
            entity.Property(e => e.MaPhongKham).HasColumnName("maPhongKham");
            entity.Property(e => e.NgayHen)
                .HasColumnType("datetime")
                .HasColumnName("ngayHen");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasColumnName("trangThai");

            entity.HasOne(d => d.MaBenhNhanNavigation).WithMany(p => p.TblLichHens)
                .HasForeignKey(d => d.MaBenhNhan)
                .HasConstraintName("fk_tbl_lich_hen_benh_nhan");

            entity.HasOne(d => d.MaDichVuYteNavigation).WithMany(p => p.TblLichHens)
                .HasForeignKey(d => d.MaDichVuYte)
                .HasConstraintName("fk_tbl_lich_hen_dich_vu_y_te");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.TblLichHens)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("fk_tbl_lich_hen_nhan_vien");

            entity.HasOne(d => d.MaPhongKhamNavigation).WithMany(p => p.TblLichHens)
                .HasForeignKey(d => d.MaPhongKham)
                .HasConstraintName("fk_tbl_lich_hen_phong_kham");
        });

        modelBuilder.Entity<TblLichSuThayDoi>(entity =>
        {
            entity.HasKey(e => e.MaLichSu).HasName("pk_tbl_lich_su_thay_doi");

            entity.ToTable("tbl_lich_su_thay_doi");

            entity.Property(e => e.MaLichSu).HasColumnName("maLichSu");
            entity.Property(e => e.BangLienQuan)
                .HasMaxLength(100)
                .HasColumnName("bangLienQuan");
            entity.Property(e => e.HanhDong)
                .HasMaxLength(100)
                .HasColumnName("hanhDong");
            entity.Property(e => e.MaBanGhi).HasColumnName("maBanGhi");
            entity.Property(e => e.MaNhanVien).HasColumnName("maNhanVien");
            entity.Property(e => e.ThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("thoiGian");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.TblLichSuThayDois)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("fk_tbl_lich_su_thay_doi_nhan_vien");
        });

        modelBuilder.Entity<TblNhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("pk_tbl_nhan_vien");

            entity.ToTable("tbl_nhan_vien");

            entity.HasIndex(e => e.Email, "uq_tbl_nhan_vien_email").IsUnique();

            entity.HasIndex(e => e.SoDienThoai, "uq_tbl_nhan_vien_soDienThoai").IsUnique();

            entity.Property(e => e.MaNhanVien).HasColumnName("maNhanVien");
            entity.Property(e => e.CaLamViec)
                .HasMaxLength(50)
                .HasColumnName("caLamViec");
            entity.Property(e => e.ChuyenMon)
                .HasMaxLength(100)
                .HasColumnName("chuyenMon");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.MaTaiKhoan).HasColumnName("maTaiKhoan");
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("soDienThoai");
            entity.Property(e => e.Ten)
                .HasMaxLength(100)
                .HasColumnName("ten");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithMany(p => p.TblNhanViens)
                .HasForeignKey(d => d.MaTaiKhoan)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_tbl_nhan_vien_tai_khoan");
        });

        modelBuilder.Entity<TblPhongKham>(entity =>
        {
            entity.HasKey(e => e.MaPhongKham).HasName("pk_tbl_phong_kham");

            entity.ToTable("tbl_phong_kham");

            entity.Property(e => e.MaPhongKham).HasColumnName("maPhongKham");
            entity.Property(e => e.Loai)
                .HasMaxLength(100)
                .HasColumnName("loai");
            entity.Property(e => e.SucChua).HasColumnName("sucChua");
        });

        modelBuilder.Entity<TblPhongKhamNhanVien>(entity =>
        {
            entity.HasKey(e => new { e.MaPhongKham, e.MaNhanVien }).HasName("pk_tbl_phong_kham_nhan_vien");

            entity.ToTable("tbl_phong_kham_nhan_vien");

            entity.Property(e => e.MaPhongKham).HasColumnName("maPhongKham");
            entity.Property(e => e.MaNhanVien).HasColumnName("maNhanVien");
            entity.Property(e => e.VaiTro)
                .HasMaxLength(50)
                .HasColumnName("vaiTro");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.TblPhongKhamNhanViens)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("fk_tbl_phong_kham_nhan_vien_nhan_vien");

            entity.HasOne(d => d.MaPhongKhamNavigation).WithMany(p => p.TblPhongKhamNhanViens)
                .HasForeignKey(d => d.MaPhongKham)
                .HasConstraintName("fk_tbl_phong_kham_nhan_vien_phong_kham");
        });

        modelBuilder.Entity<TblTaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("pk_tbl_tai_khoan");

            entity.ToTable("tbl_tai_khoan");

            entity.HasIndex(e => e.TenDangNhap, "uq_tbl_tai_khoan_tenDangNhap").IsUnique();

            entity.Property(e => e.MaTaiKhoan).HasColumnName("maTaiKhoan");
            entity.Property(e => e.MaVaiTro).HasColumnName("maVaiTro");
            entity.Property(e => e.MatKhau)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("matKhau");
            entity.Property(e => e.TenDangNhap)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tenDangNhap");

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.TblTaiKhoans)
                .HasForeignKey(d => d.MaVaiTro)
                .HasConstraintName("fk_tbl_tai_khoan_vai_tro");
        });

        modelBuilder.Entity<TblThuoc>(entity =>
        {
            entity.HasKey(e => e.MaThuoc).HasName("pk_tbl_thuoc");

            entity.ToTable("tbl_thuoc");

            entity.Property(e => e.MaThuoc).HasColumnName("maThuoc");
            entity.Property(e => e.MoTa).HasColumnName("moTa");
            entity.Property(e => e.Ten)
                .HasMaxLength(100)
                .HasColumnName("ten");
        });

        modelBuilder.Entity<TblVaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("pk_tbl_vai_tro");

            entity.ToTable("tbl_vai_tro");

            entity.HasIndex(e => e.Ten, "uq_tbl_vai_tro_ten").IsUnique();

            entity.Property(e => e.MaVaiTro).HasColumnName("maVaiTro");
            entity.Property(e => e.Ten)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ten");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
