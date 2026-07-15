namespace StudentJob.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using StudentJob.Models;
using StudentJob.Repositories;

[Route("viec-lam")]
public class ViecLamController : Controller
{
    private readonly IBaiTuyenDungRepository _baiTuyenDungRepository;
    private readonly INganhNgheRepository _nganhNgheRepository;
    private readonly IKhuVucRepository _khuVucRepository;
    private readonly ISinhVienRepository _sinhVienRepository;
    private readonly IHoSoUngTuyenRepository _hoSoUngTuyenRepository;
    private readonly ILuuTinRepository _luuTinRepository;

    public ViecLamController(
        IBaiTuyenDungRepository baiTuyenDungRepository,
        INganhNgheRepository nganhNgheRepository,
        IKhuVucRepository khuVucRepository,
        ISinhVienRepository sinhVienRepository,
        IHoSoUngTuyenRepository hoSoUngTuyenRepository,
        ILuuTinRepository luuTinRepository
    )
    {
        _baiTuyenDungRepository = baiTuyenDungRepository;
        _nganhNgheRepository = nganhNgheRepository;
        _khuVucRepository = khuVucRepository;
        _sinhVienRepository = sinhVienRepository;
        _hoSoUngTuyenRepository = hoSoUngTuyenRepository;
        _luuTinRepository = luuTinRepository;
    }

    [HttpGet("{id:int}")]
    public IActionResult ChiTiet(int id)
    {
        var job = _baiTuyenDungRepository.GetById(id);

        if (job == null)
        {
            return NotFound();
        }

        if (job.sTrangThaiKiemDuyet != "Đã duyệt")
        {
            TempData["ErrorMessage"] =
                "Tin tuyển dụng này không còn khả dụng hoặc chưa được phê duyệt.";

            return RedirectToAction("Index", "Home");
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
                    job.PK_IdBaiTuyenDung
                );

                coCVMacDinh = !string.IsNullOrWhiteSpace(sinhVien.sDuongDanCVMacDinh);

                daLuuTin = _luuTinRepository.IsSaved(sinhVien.PK_IdSinhVien, job.PK_IdBaiTuyenDung);
            }
        }

        var viewModel = new ChiTietBaiTuyenDungViewModel
        {
            BaiTuyenDung = job,
            DaDangNhap = daDangNhap,
            LaSinhVien = laSinhVien,
            DaUngTuyen = daUngTuyen,
            CoCVMacDinh = coCVMacDinh,
            DaLuuTin = daLuuTin,

            DaHetHan = job.dHanNopHoSo < DateOnly.FromDateTime(DateTime.Today),

            UngTuyen = new UngTuyenInputModel
            {
                FK_IdBaiTuyenDung = job.PK_IdBaiTuyenDung,

                SuDungCVMacDinh = coCVMacDinh,
            },
        };

        return View(viewModel);
    }

    [HttpPost("api/filter")]
    public IActionResult Filter(
        [FromForm] string? keyword,
        [FromForm] string? hinhThuc,
        [FromForm] int? nganhNgheId,
        [FromForm] int? khuVucId
    )
    {
        var filteredJobs = _baiTuyenDungRepository.GetByDieuKien(
            keyword,
            hinhThuc,
            nganhNgheId,
            khuVucId
        );

        return PartialView("_DanhSachViecLamPartial", filteredJobs);
    }

    private SinhVien? GetCurrentSinhVien()
    {
        string? taiKhoanIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(taiKhoanIdClaim, out int taiKhoanId))
        {
            return null;
        }

        return _sinhVienRepository.GetByTaiKhoanId(taiKhoanId);
    }
}
