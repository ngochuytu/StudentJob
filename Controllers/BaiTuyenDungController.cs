namespace StudentJob.Controllers;

using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using StudentJob.Models;
using StudentJob.Repositories;

[Route("jobs")]
public class BaiTuyenDungController : Controller
{
    private readonly IBaiTuyenDungRepository _baiTuyenDungRepository;
    private readonly ISinhVienRepository _sinhVienRepository;
    private readonly IHoSoUngTuyenRepository _hoSoUngTuyenRepository;
    private readonly ILuuTinRepository _luuTinRepository;

    public BaiTuyenDungController(
        IBaiTuyenDungRepository baiTuyenDungRepository,
        ISinhVienRepository sinhVienRepository,
        IHoSoUngTuyenRepository hoSoUngTuyenRepository,
        ILuuTinRepository luuTinRepository)
    {
        _baiTuyenDungRepository = baiTuyenDungRepository;
        _sinhVienRepository = sinhVienRepository;
        _hoSoUngTuyenRepository = hoSoUngTuyenRepository;
        _luuTinRepository = luuTinRepository;
    }

    [HttpGet("{id:int}/{slug?}")]
    public IActionResult ChiTiet(int id, string? slug)
    {
        var baiTuyenDung = _baiTuyenDungRepository.GetApprovedById(id);

        if (baiTuyenDung == null)
        {
            return NotFound();
        }

        string canonicalSlug = CreateSlug(baiTuyenDung.sTieuDeCongViec);

        if (!string.Equals(
                slug,
                canonicalSlug,
                StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToActionPermanent(
                nameof(ChiTiet),
                new
                {
                    id = baiTuyenDung.PK_IdBaiTuyenDung,
                    slug = canonicalSlug
                });
        }

        bool daDangNhap = User.Identity?.IsAuthenticated == true;
        bool laSinhVien = User.IsInRole("Sinh viên");
        bool daUngTuyen = false;
        bool coCVMacDinh = false;
        bool daLuuTin = false;

        if (daDangNhap && laSinhVien)
        {
            var sinhVien = GetCurrentSinhVien();

            if (sinhVien != null)
            {
                daUngTuyen = _hoSoUngTuyenRepository.HasApplied(
                    sinhVien.PK_IdSinhVien,
                    baiTuyenDung.PK_IdBaiTuyenDung);

                coCVMacDinh =
                    !string.IsNullOrWhiteSpace(
                        sinhVien.sDuongDanCVMacDinh);

                daLuuTin = _luuTinRepository.IsSaved(
                    sinhVien.PK_IdSinhVien,
                    baiTuyenDung.PK_IdBaiTuyenDung);
            }
        }

        var viewModel = new ChiTietBaiTuyenDungViewModel
        {
            BaiTuyenDung = baiTuyenDung,
            DaDangNhap = daDangNhap,
            LaSinhVien = laSinhVien,
            DaUngTuyen = daUngTuyen,
            CoCVMacDinh = coCVMacDinh,
            DaLuuTin = daLuuTin,
            DaHetHan =
                baiTuyenDung.dHanNopHoSo <
                DateOnly.FromDateTime(DateTime.Today),

            UngTuyen = new UngTuyenInputModel
            {
                FK_IdBaiTuyenDung =
                    baiTuyenDung.PK_IdBaiTuyenDung,

                SuDungCVMacDinh = coCVMacDinh
            }
        };

        return View(viewModel);
    }

    private SinhVien? GetCurrentSinhVien()
    {
        string? taiKhoanIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(
                taiKhoanIdClaim,
                out int taiKhoanId))
        {
            return null;
        }

        return _sinhVienRepository
            .GetByTaiKhoanId(taiKhoanId);
    }

    private static string CreateSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "viec-lam";
        }

        string normalized = value
            .Normalize(NormalizationForm.FormD);

        var slugBuilder = new StringBuilder();

        foreach (char character in normalized)
        {
            UnicodeCategory category =
                CharUnicodeInfo.GetUnicodeCategory(character);

            if (category != UnicodeCategory.NonSpacingMark)
            {
                slugBuilder.Append(character);
            }
        }

        string slug = slugBuilder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .ToLowerInvariant();

        slug = slug
            .Replace('đ', 'd')
            .Replace('Đ', 'd');

        slug = Regex.Replace(
            slug,
            @"[^a-z0-9\s-]",
            string.Empty);

        slug = Regex.Replace(
            slug,
            @"[\s-]+",
            "-");

        return slug.Trim('-');
    }
}