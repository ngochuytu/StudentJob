namespace StudentJob.Data;

using Microsoft.EntityFrameworkCore;
using StudentJob.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<VaiTro> DsVaiTro { get; set; }
    public DbSet<TaiKhoan> DsTaiKhoan { get; set; }
    public DbSet<SinhVien> DsSinhVien { get; set; }
    public DbSet<NhaTuyenDung> DsNhaTuyenDung { get; set; }
    public DbSet<NganhNghe> DsNganhNghe { get; set; }
    public DbSet<KhuVuc> DsKhuVuc { get; set; }
    public DbSet<BaiTuyenDung> DsBaiTuyenDung { get; set; }
    public DbSet<HoSoUngTuyen> DsHoSoUngTuyen { get; set; }
    public DbSet<LuuTin> DsLuuTin { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.PK_IdVaiTro);
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.PK_IdTaiKhoan);

            entity.HasIndex(e => e.sEmail)
                  .IsUnique();

            entity.Property(e => e.bTrangThaiHoatDong)
                  .HasDefaultValue(true);

            entity.HasOne(e => e.VaiTro)
                  .WithMany(v => v.DsTaiKhoan)
                  .HasForeignKey(e => e.FK_IdVaiTro)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.PK_IdSinhVien);

            entity.HasOne(e => e.TaiKhoan)
                  .WithOne(t => t.SinhVien)
                  .HasForeignKey<SinhVien>(e => e.FK_IdTaiKhoan)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<NhaTuyenDung>(entity =>
        {
            entity.HasKey(e => e.PK_IdNhaTuyenDung);

            entity.HasOne(e => e.TaiKhoan)
                  .WithOne(t => t.NhaTuyenDung)
                  .HasForeignKey<NhaTuyenDung>(e => e.FK_IdTaiKhoan)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<NganhNghe>(entity =>
        {
            entity.HasKey(e => e.PK_IdNganhNghe);

            entity.HasIndex(e => e.sTenLinhVuc)
                  .IsUnique();
        });

        modelBuilder.Entity<KhuVuc>(entity =>
        {
            entity.HasKey(e => e.PK_IdKhuVuc);

            entity.HasIndex(e => e.sTenKhuVuc)
                  .IsUnique();
        });

        modelBuilder.Entity<BaiTuyenDung>(entity =>
        {
            entity.HasKey(e => e.PK_IdBaiTuyenDung);

            entity.HasOne(e => e.NhaTuyenDung)
                  .WithMany(n => n.DsBaiTuyenDung)
                  .HasForeignKey(e => e.FK_IdNhaTuyenDung)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.NganhNghe)
                  .WithMany(n => n.DsBaiTuyenDung)
                  .HasForeignKey(e => e.FK_IdNganhNghe)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.KhuVuc)
                  .WithMany(k => k.DsBaiTuyenDung)
                  .HasForeignKey(e => e.FK_IdKhuVuc)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HoSoUngTuyen>(entity =>
        {
            entity.HasKey(e => e.PK_IdHoSoUngTuyen);

            entity.HasOne(e => e.SinhVien)
                  .WithMany(s => s.DsHoSoUngTuyen)
                  .HasForeignKey(e => e.FK_IdSinhVien)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.BaiTuyenDung)
                  .WithMany(b => b.DsHoSoUngTuyen)
                  .HasForeignKey(e => e.FK_IdBaiTuyenDung)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LuuTin>(entity =>
            {
            entity.HasKey(e => e.PK_IdLuuTin);

            entity.HasIndex(e => new
            {
                  e.FK_IdSinhVien,
                  e.FK_IdBaiTuyenDung
            })
            .IsUnique();

            entity.HasOne(e => e.SinhVien)
                  .WithMany()
                  .HasForeignKey(e => e.FK_IdSinhVien)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.BaiTuyenDung)
                  .WithMany()
                  .HasForeignKey(e => e.FK_IdBaiTuyenDung)
                  .OnDelete(DeleteBehavior.Restrict);
            });
    }
}
