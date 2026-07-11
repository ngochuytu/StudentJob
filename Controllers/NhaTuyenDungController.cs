namespace StudentJob.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentJob.Models;
using StudentJob.Repositories;

[Route("nha-tuyen-dung")]
[Authorize(Roles = "Nhà tuyển dụng")]
public class NhaTuyenDungController : Controller
{
    private readonly INhaTuyenDungRepository _nhaTuyenDungRepository;
    private readonly IBaiTuyenDungRepository _baiTuyenDungRepository;
    private readonly IHoSoUngTuyenRepository _hoSoUngTuyenRepository;
    private readonly INganhNgheRepository _nganhNgheRepository;
    private readonly IKhuVucRepository _khuVucRepository;

    public NhaTuyenDungController(
        INhaTuyenDungRepository nhaTuyenDungRepository,
        IBaiTuyenDungRepository baiTuyenDungRepository,
        IHoSoUngTuyenRepository hoSoUngTuyenRepository,
        INganhNgheRepository nganhNgheRepository,
        IKhuVucRepository khuVucRepository)
    {
        _nhaTuyenDungRepository = nhaTuyenDungRepository;
        _baiTuyenDungRepository = baiTuyenDungRepository;
        _hoSoUngTuyenRepository = hoSoUngTuyenRepository;
        _nganhNgheRepository = nganhNgheRepository;
        _khuVucRepository = khuVucRepository;
    }

    [HttpGet("quan-ly-tin")]
    public IActionResult QuanLyTin()
    {
        var nhaTuyenDung = GetCurrentNhaTuyenDung();

        if (nhaTuyenDung == null)
        {
            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        var danhSachTin = _baiTuyenDungRepository
            .GetByNhaTuyenDungId(nhaTuyenDung.PK_IdNhaTuyenDung);

        ViewBag.TenDoanhNghiep = nhaTuyenDung.sTenDoanhNghiep;

        return View(danhSachTin);
    }

    [HttpGet("tao-tin")]
    public IActionResult TaoTin()
    {
        var nhaTuyenDung = GetCurrentNhaTuyenDung();

        if (nhaTuyenDung == null)
        {
            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        LoadDanhMuc();

        return View(new BaiTuyenDungInputModel
        {
            dHanNopHoSo = DateTime.Today.AddDays(7)
        });
    }

    [HttpPost("tao-tin")]
    [ValidateAntiForgeryToken]
    public IActionResult TaoTin(BaiTuyenDungInputModel model)
    {
        var nhaTuyenDung = GetCurrentNhaTuyenDung();

        if (nhaTuyenDung == null)
        {
            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        ValidateHanNopHoSo(model);

        if (!ModelState.IsValid)
        {
            LoadDanhMuc();
            return View(model);
        }

        var baiTuyenDung = new BaiTuyenDung
        {
            FK_IdNhaTuyenDung = nhaTuyenDung.PK_IdNhaTuyenDung,
            sTieuDeCongViec = model.sTieuDeCongViec.Trim(),
            sHinhThucLamViec = model.sHinhThucLamViec.Trim(),
            sMoTaCongViec = model.sMoTaCongViec.Trim(),
            sCaLam = string.IsNullOrWhiteSpace(model.sCaLam)
                ? null
                : model.sCaLam.Trim(),
            sMucLuong = model.sMucLuong.Trim(),
            FK_IdNganhNghe = model.FK_IdNganhNghe!.Value,
            FK_IdKhuVuc = model.FK_IdKhuVuc!.Value,
            dHanNopHoSo = DateOnly.FromDateTime(model.dHanNopHoSo!.Value),
            sTrangThaiKiemDuyet = "Chờ duyệt",
            dNgayTaoBai = DateTime.Now
        };

        _baiTuyenDungRepository.Add(baiTuyenDung);

        TempData["SuccessMessage"] =
            "Tạo tin tuyển dụng thành công. Tin đang chờ quản trị viên phê duyệt.";

        return RedirectToAction(nameof(QuanLyTin));
    }

    [HttpGet("chinh-sua/{id:int}")]
    public IActionResult ChinhSuaTin(int id)
    {
        var nhaTuyenDung = GetCurrentNhaTuyenDung();

        if (nhaTuyenDung == null)
        {
            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        var baiTuyenDung = _baiTuyenDungRepository.GetById(id);

        if (baiTuyenDung == null)
        {
            return NotFound();
        }

        if (baiTuyenDung.FK_IdNhaTuyenDung !=
            nhaTuyenDung.PK_IdNhaTuyenDung)
        {
            return Forbid();
        }

        var model = new BaiTuyenDungInputModel
        {
            sTieuDeCongViec = baiTuyenDung.sTieuDeCongViec,
            sHinhThucLamViec = baiTuyenDung.sHinhThucLamViec,
            sMoTaCongViec = baiTuyenDung.sMoTaCongViec,
            sCaLam = baiTuyenDung.sCaLam,
            sMucLuong = baiTuyenDung.sMucLuong,
            FK_IdNganhNghe = baiTuyenDung.FK_IdNganhNghe,
            FK_IdKhuVuc = baiTuyenDung.FK_IdKhuVuc,
            dHanNopHoSo = baiTuyenDung.dHanNopHoSo.ToDateTime(
                TimeOnly.MinValue)
        };

        ViewBag.BaiTuyenDungId = baiTuyenDung.PK_IdBaiTuyenDung;
        ViewBag.TrangThaiHienTai = baiTuyenDung.sTrangThaiKiemDuyet;

        LoadDanhMuc();

        return View(model);
    }

    [HttpPost("chinh-sua/{id:int}")]
    [ValidateAntiForgeryToken]
    public IActionResult ChinhSuaTin(
        int id,
        BaiTuyenDungInputModel model)
    {
        var nhaTuyenDung = GetCurrentNhaTuyenDung();

        if (nhaTuyenDung == null)
        {
            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        var baiTuyenDung = _baiTuyenDungRepository.GetById(id);

        if (baiTuyenDung == null)
        {
            return NotFound();
        }

        if (baiTuyenDung.FK_IdNhaTuyenDung !=
            nhaTuyenDung.PK_IdNhaTuyenDung)
        {
            return Forbid();
        }

        ValidateHanNopHoSo(model);

        if (!ModelState.IsValid)
        {
            ViewBag.BaiTuyenDungId = id;
            ViewBag.TrangThaiHienTai =
                baiTuyenDung.sTrangThaiKiemDuyet;

            LoadDanhMuc();

            return View(model);
        }

        baiTuyenDung.sTieuDeCongViec =
            model.sTieuDeCongViec.Trim();

        baiTuyenDung.sHinhThucLamViec =
            model.sHinhThucLamViec.Trim();

        baiTuyenDung.sMoTaCongViec =
            model.sMoTaCongViec.Trim();

        baiTuyenDung.sCaLam =
            string.IsNullOrWhiteSpace(model.sCaLam)
                ? null
                : model.sCaLam.Trim();

        baiTuyenDung.sMucLuong =
            model.sMucLuong.Trim();

        baiTuyenDung.FK_IdNganhNghe =
            model.FK_IdNganhNghe!.Value;

        baiTuyenDung.FK_IdKhuVuc =
            model.FK_IdKhuVuc!.Value;

        baiTuyenDung.dHanNopHoSo =
            DateOnly.FromDateTime(model.dHanNopHoSo!.Value);

        baiTuyenDung.sTrangThaiKiemDuyet = "Chờ duyệt";

        _baiTuyenDungRepository.Update(baiTuyenDung);

        TempData["SuccessMessage"] =
            "Cập nhật tin tuyển dụng thành công. Tin đã được chuyển về trạng thái chờ duyệt.";

        return RedirectToAction(nameof(QuanLyTin));
    }

    [HttpPost("api/xoa-tin/{id:int}")]
    [ValidateAntiForgeryToken]
    public IActionResult XoaTin(int id)
    {
        var nhaTuyenDung = GetCurrentNhaTuyenDung();

        if (nhaTuyenDung == null)
        {
            return Unauthorized(new
            {
                success = false,
                message = "Phiên đăng nhập không hợp lệ."
            });
        }

        var baiTuyenDung = _baiTuyenDungRepository.GetById(id);

        if (baiTuyenDung == null)
        {
            return NotFound(new
            {
                success = false,
                message = "Không tìm thấy tin tuyển dụng."
            });
        }

        if (baiTuyenDung.FK_IdNhaTuyenDung !=
            nhaTuyenDung.PK_IdNhaTuyenDung)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                success = false,
                message = "Bạn không có quyền xóa tin tuyển dụng này."
            });
        }

        if (_hoSoUngTuyenRepository.HasApplicationsForJob(id))
        {
            return BadRequest(new
            {
                success = false,
                message =
                    "Không thể xóa tin đã có sinh viên ứng tuyển. Bạn nên giữ tin để bảo toàn lịch sử hồ sơ."
            });
        }

        _baiTuyenDungRepository.Delete(baiTuyenDung);

        return Json(new
        {
            success = true,
            message = "Xóa tin tuyển dụng thành công.",
            data = new
            {
                id = baiTuyenDung.PK_IdBaiTuyenDung
            }
        });
    }

    private NhaTuyenDung? GetCurrentNhaTuyenDung()
    {
        string? taiKhoanIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(taiKhoanIdClaim, out int taiKhoanId))
        {
            return null;
        }

        return _nhaTuyenDungRepository
            .GetByTaiKhoanId(taiKhoanId);
    }

    private void LoadDanhMuc()
    {
        ViewBag.DsNganhNghe = _nganhNgheRepository.GetAll();
        ViewBag.DsKhuVuc = _khuVucRepository.GetAll();
    }

    private void ValidateHanNopHoSo(
        BaiTuyenDungInputModel model)
    {
        if (!model.dHanNopHoSo.HasValue)
        {
            return;
        }

        if (model.dHanNopHoSo.Value.Date < DateTime.Today)
        {
            ModelState.AddModelError(
                nameof(model.dHanNopHoSo),
                "Hạn nộp hồ sơ không được nhỏ hơn ngày hiện tại.");
        }
    }
}
