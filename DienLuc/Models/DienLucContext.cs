using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DienLuc.Models;

public partial class DienLucContext : DbContext
{
    public DienLucContext()
    {
    }

    public DienLucContext(DbContextOptions<DienLucContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietSddien> ChiTietSddiens { get; set; }

    public virtual DbSet<DongHoDien> DongHoDiens { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThongBao> ThongBaos { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MSI\\SQLSEVER2022DEV;Database=DienLuc;trusted_connection=yes;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietSddien>(entity =>
        {
            entity.HasKey(e => new { e.MaDh, e.NgayLayThongTin }).HasName("PK_ChiTietSDDien_1");

            entity.ToTable("ChiTietSDDien");

            entity.HasIndex(e => e.Id, "IX_ChiTietSDDien").IsUnique();

            entity.Property(e => e.MaDh)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MaDH");
            entity.Property(e => e.NgayLayThongTin).HasColumnType("datetime");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.MaNv)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MaNV");

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.ChiTietSddiens)
                .HasForeignKey(d => d.MaDh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietSDDien_DongHoDien1");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.ChiTietSddiens)
                .HasForeignKey(d => d.MaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTietSDDien_NhanVien");
        });

        modelBuilder.Entity<DongHoDien>(entity =>
        {
            entity.HasKey(e => e.MaDh).HasName("PK_DongHoDien_1");

            entity.ToTable("DongHoDien");

            entity.Property(e => e.MaDh)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MaDH");
            entity.Property(e => e.LoaiDhd)
                .HasMaxLength(50)
                .HasColumnName("LoaiDHD");
            entity.Property(e => e.MaKh)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DongHoDiens)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK_DongHoDien_KhachHang");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHd);

            entity.ToTable("HoaDon");

            entity.HasIndex(e => e.MaChiTietSddien, "IX_HoaDon").IsUnique();

            entity.Property(e => e.MaHd)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MaHD");
            entity.Property(e => e.HinhThucThanhToan).HasMaxLength(50);
            entity.Property(e => e.MaChiTietSddien).HasColumnName("MaChiTietSDDien");
            entity.Property(e => e.MaKh)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.MaNv)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MaNV");
            entity.Property(e => e.NgayDenHan).HasColumnType("datetime");
            entity.Property(e => e.NgayTao).HasColumnType("datetime");
            entity.Property(e => e.NgayThanhToan)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Thue).HasColumnType("decimal(10, 0)");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaChiTietSddienNavigation).WithOne(p => p.HoaDon)
                .HasPrincipalKey<ChiTietSddien>(p => p.Id)
                .HasForeignKey<HoaDon>(d => d.MaChiTietSddien)
                .HasConstraintName("FK_HoaDon_ChiTietSDDien");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK_HoaDon_KhachHang");

            entity.HasOne(d => d.MaNvNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaNv)
                .HasConstraintName("FK_HoaDon_NhanVien2");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKh);

            entity.ToTable("KhachHang");

            entity.HasIndex(e => e.Matk, "IX_KhachHang").IsUnique();

            entity.Property(e => e.MaKh)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MaKH");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .HasColumnName("CCCD");
            entity.Property(e => e.DiaChi)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Ho).HasMaxLength(25);
            entity.Property(e => e.MaThue)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.MaVaiTro)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Matk)
                .HasMaxLength(25)
                .IsFixedLength()
                .HasColumnName("MATK");
            entity.Property(e => e.SoDt)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("SoDT");
            entity.Property(e => e.Ten).HasMaxLength(25);

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.MaVaiTro)
                .HasConstraintName("FK_KhachHang_VaiTro");

            entity.HasOne(d => d.MatkNavigation).WithOne(p => p.KhachHang)
                .HasForeignKey<KhachHang>(d => d.Matk)
                .HasConstraintName("FK_KhachHang_TaiKhoan");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNv);

            entity.ToTable("NhanVien");

            entity.HasIndex(e => e.Matk, "IX_NhanVien").IsUnique();

            entity.Property(e => e.MaNv)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MaNV");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsFixedLength()
                .HasColumnName("CCCD");
            entity.Property(e => e.DiaChi).HasMaxLength(55);
            entity.Property(e => e.Ho).HasMaxLength(25);
            entity.Property(e => e.Matk)
                .HasMaxLength(25)
                .IsFixedLength()
                .HasColumnName("MATK");
            entity.Property(e => e.SoDt)
                .HasMaxLength(11)
                .HasColumnName("SoDT");
            entity.Property(e => e.Ten).HasMaxLength(25);

            entity.HasOne(d => d.MatkNavigation).WithOne(p => p.NhanVien)
                .HasForeignKey<NhanVien>(d => d.Matk)
                .HasConstraintName("FK_NhanVien_TaiKhoan1");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTk);

            entity.ToTable("TaiKhoan");

            entity.Property(e => e.MaTk)
                .HasMaxLength(25)
                .IsFixedLength()
                .HasColumnName("MaTK");
            entity.Property(e => e.LoaiTk)
                .HasMaxLength(25)
                .HasColumnName("LoaiTK");
            entity.Property(e => e.Mk)
                .HasMaxLength(25)
                .HasColumnName("MK");
        });

        modelBuilder.Entity<ThongBao>(entity =>
        {
            entity.ToTable("ThongBao");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.MaHd)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MaHD");

            entity.HasOne(d => d.MaHdNavigation).WithMany(p => p.ThongBaos)
                .HasForeignKey(d => d.MaHd)
                .HasConstraintName("FK_ThongBao_HoaDon");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.ToTable("VaiTro");

            entity.HasIndex(e => e.ViTri, "IX_VaiTro").IsUnique();

            entity.Property(e => e.Id)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ViTri).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
