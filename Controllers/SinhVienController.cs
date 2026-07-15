namespace StudentJob.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentJob.Models;
using StudentJob.Repositories;

[Route("sinh-vien")]
[Authorize(Roles = "Sinh viên")]
public class SinhVienController : Controller
{
    private const long KichThuocCVToiDa = 5 * 1024 * 1024;

    private static readonly HashSet<string> DinhDangCVHopLe = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf",
        ".docx",
    };

    private readonly ISinhVienRepository _sinhVienRepository;
    private readonly IBaiTuyenDungRepository _baiTuyenDungRepository;
    private readonly IHoSoUngTuyenRepository _hoSoUngTuyenRepository;
    private readonly ILuuTinRepository _luuTinRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SinhVienController(
        ISinhVienRepository sinhVienRepository,
        IBaiTuyenDungRepository baiTuyenDungRepository,
        IHoSoUngTuyenRepository hoSoUngTuyenRepository,
        ILuuTinRepository luuTinRepository,
        IWebHostEnvironment webHostEnvironment
    )
    {
        _sinhVienRepository = sinhVienRepository;
        _baiTuyenDungRepository = baiTuyenDungRepository;
        _hoSoUngTuyenRepository = hoSoUngTuyenRepository;
        _luuTinRepository = luuTinRepository;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("theo-doi-ho-so")]
    public IActionResult TheoDoiHoSo()
    {
        var sinhVien = GetCurrentSinhVien();

        if (sinhVien == null)
        {
            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        var danhSachHoSo = _hoSoUngTuyenRepository.GetBySinhVienId(sinhVien.PK_IdSinhVien);

        return View(danhSachHoSo);
    }

    [HttpGet("tin-da-luu")]
    public IActionResult TinDaLuu()
    {
        var sinhVien = GetCurrentSinhVien();

        if (sinhVien == null)
        {
            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        var danhSachTinDaLuu = _luuTinRepository.GetBySinhVienId(sinhVien.PK_IdSinhVien);

        return View(danhSachTinDaLuu);
    }

    [HttpPost("api/luu-tin")]
    public IActionResult LuuTin(int baiTuyenDungId)
    {
        var sinhVien = GetCurrentSinhVien();

        if (sinhVien == null)
        {
            return Unauthorized(
                new { success = false, message = "Không xác định được tài khoản sinh viên." }
            );
        }

        var baiTuyenDung = _baiTuyenDungRepository.GetApprovedById(baiTuyenDungId);

        if (baiTuyenDung == null)
        {
            return NotFound(
                new
                {
                    success = false,
                    message = "Tin tuyển dụng không tồn tại hoặc chưa được phê duyệt.",
                }
            );
        }

        bool daLuuTin = _luuTinRepository.IsSaved(
            sinhVien.PK_IdSinhVien,
            baiTuyenDung.PK_IdBaiTuyenDung
        );

        if (daLuuTin)
        {
            return BadRequest(new { success = false, message = "Bạn đã lưu tin tuyển dụng này." });
        }

        try
        {
            var luuTin = new LuuTin
            {
                FK_IdSinhVien = sinhVien.PK_IdSinhVien,
                FK_IdBaiTuyenDung = baiTuyenDung.PK_IdBaiTuyenDung,
                dNgayLuu = DateTime.Now,
            };

            _luuTinRepository.Add(luuTin);

            return Json(
                new
                {
                    success = true,
                    message = "Đã lưu tin tuyển dụng.",
                    data = new { saved = true, jobId = baiTuyenDung.PK_IdBaiTuyenDung },
                }
            );
        }
        catch
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { success = false, message = "Đã xảy ra lỗi khi lưu tin. Vui lòng thử lại." }
            );
        }
    }

    [HttpPost("api/bo-luu-tin")]
    public IActionResult BoLuuTin(int baiTuyenDungId)
    {
        var sinhVien = GetCurrentSinhVien();

        if (sinhVien == null)
        {
            return Unauthorized(
                new { success = false, message = "Không xác định được tài khoản sinh viên." }
            );
        }

        var luuTin = _luuTinRepository.Get(sinhVien.PK_IdSinhVien, baiTuyenDungId);

        if (luuTin == null)
        {
            return NotFound(new { success = false, message = "Tin tuyển dụng này chưa được lưu." });
        }

        try
        {
            _luuTinRepository.Delete(luuTin);

            return Json(
                new
                {
                    success = true,
                    message = "Đã bỏ lưu tin tuyển dụng.",
                    data = new { saved = false, jobId = baiTuyenDungId },
                }
            );
        }
        catch
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { success = false, message = "Đã xảy ra lỗi khi bỏ lưu tin. Vui lòng thử lại." }
            );
        }
    }

    [HttpPost("api/ung-tuyen")]
    public IActionResult UngTuyen(UngTuyenInputModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = GetFirstModelError() });
        }

        var sinhVien = GetCurrentSinhVien();

        if (sinhVien == null)
        {
            return Unauthorized(
                new { success = false, message = "Không xác định được tài khoản sinh viên." }
            );
        }

        var baiTuyenDung = _baiTuyenDungRepository.GetApprovedById(model.FK_IdBaiTuyenDung);

        if (baiTuyenDung == null)
        {
            return NotFound(
                new
                {
                    success = false,
                    message = "Tin tuyển dụng không tồn tại hoặc chưa được phê duyệt.",
                }
            );
        }

        if (baiTuyenDung.dHanNopHoSo < DateOnly.FromDateTime(DateTime.Today))
        {
            return BadRequest(
                new { success = false, message = "Tin tuyển dụng đã hết hạn nhận hồ sơ." }
            );
        }

        bool daUngTuyen = _hoSoUngTuyenRepository.HasApplied(
            sinhVien.PK_IdSinhVien,
            baiTuyenDung.PK_IdBaiTuyenDung
        );

        if (daUngTuyen)
        {
            return BadRequest(new { success = false, message = "Bạn đã ứng tuyển công việc này." });
        }

        string? duongDanCV = null;
        string? duongDanVatLyDaLuu = null;

        try
        {
            if (model.SuDungCVMacDinh)
            {
                var ketQuaCVMacDinh = SaoChepCVMacDinh(sinhVien, baiTuyenDung.PK_IdBaiTuyenDung);

                if (!ketQuaCVMacDinh.ThanhCong)
                {
                    return BadRequest(new { success = false, message = ketQuaCVMacDinh.ThongBao });
                }

                duongDanCV = ketQuaCVMacDinh.DuongDanTuongDoi;

                duongDanVatLyDaLuu = ketQuaCVMacDinh.DuongDanVatLy;
            }
            else
            {
                var ketQuaUpload = LuuCVTaiLen(
                    model.TepCV,
                    sinhVien.PK_IdSinhVien,
                    baiTuyenDung.PK_IdBaiTuyenDung
                );

                if (!ketQuaUpload.ThanhCong)
                {
                    return BadRequest(new { success = false, message = ketQuaUpload.ThongBao });
                }

                duongDanCV = ketQuaUpload.DuongDanTuongDoi;

                duongDanVatLyDaLuu = ketQuaUpload.DuongDanVatLy;
            }

            var hoSoUngTuyen = new HoSoUngTuyen
            {
                FK_IdSinhVien = sinhVien.PK_IdSinhVien,
                FK_IdBaiTuyenDung = baiTuyenDung.PK_IdBaiTuyenDung,
                sDuongDanCV = duongDanCV!,
                sThuXinViec = string.IsNullOrWhiteSpace(model.sThuXinViec)
                    ? null
                    : model.sThuXinViec.Trim(),
                sTrangThaiXetDuyet = "Chờ duyệt",
                sGhiChuPhanHoi = null,
                dThoiGianNopHoSo = DateTime.Now,
            };

            _hoSoUngTuyenRepository.Add(hoSoUngTuyen);

            return Json(
                new
                {
                    success = true,
                    message = "Nộp hồ sơ ứng tuyển thành công.",
                    data = new
                    {
                        id = hoSoUngTuyen.PK_IdHoSoUngTuyen,
                        status = hoSoUngTuyen.sTrangThaiXetDuyet,
                        submittedAt = hoSoUngTuyen.dThoiGianNopHoSo.ToString("dd/MM/yyyy HH:mm"),
                    },
                }
            );
        }
        catch
        {
            if (
                !string.IsNullOrWhiteSpace(duongDanVatLyDaLuu)
                && System.IO.File.Exists(duongDanVatLyDaLuu)
            )
            {
                System.IO.File.Delete(duongDanVatLyDaLuu);
            }

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { success = false, message = "Đã xảy ra lỗi khi lưu hồ sơ. Vui lòng thử lại." }
            );
        }
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

    private KetQuaLuuCV LuuCVTaiLen(IFormFile? tepCV, int sinhVienId, int baiTuyenDungId)
    {
        if (tepCV == null || tepCV.Length == 0)
        {
            return KetQuaLuuCV.ThatBai("Vui lòng chọn CV để tải lên.");
        }

        if (tepCV.Length > KichThuocCVToiDa)
        {
            return KetQuaLuuCV.ThatBai("Dung lượng CV không được vượt quá 5 MB.");
        }

        string extension = Path.GetExtension(tepCV.FileName);

        if (!DinhDangCVHopLe.Contains(extension))
        {
            return KetQuaLuuCV.ThatBai("CV chỉ chấp nhận định dạng PDF hoặc DOCX.");
        }

        string tenThuMuc = $"post_{baiTuyenDungId}";

        string thuMucVatLy = Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads",
            "cvs",
            "applied",
            tenThuMuc
        );

        Directory.CreateDirectory(thuMucVatLy);

        string tenFile = $"cv_{sinhVienId}{extension.ToLowerInvariant()}";

        string duongDanVatLy = Path.Combine(thuMucVatLy, tenFile);

        using var stream = new FileStream(duongDanVatLy, FileMode.Create, FileAccess.Write);

        tepCV.CopyTo(stream);

        string duongDanTuongDoi = $"/uploads/cvs/applied/{tenThuMuc}/{tenFile}";

        return KetQuaLuuCV.ThanhCongVoi(duongDanTuongDoi, duongDanVatLy);
    }

    private KetQuaLuuCV SaoChepCVMacDinh(SinhVien sinhVien, int baiTuyenDungId)
    {
        if (string.IsNullOrWhiteSpace(sinhVien.sDuongDanCVMacDinh))
        {
            return KetQuaLuuCV.ThatBai("Bạn chưa có CV mặc định trong hồ sơ.");
        }

        string duongDanMacDinh = sinhVien
            .sDuongDanCVMacDinh.TrimStart('/')
            .Replace('/', Path.DirectorySeparatorChar);

        string duongDanNguon = Path.Combine(_webHostEnvironment.WebRootPath, duongDanMacDinh);

        if (!System.IO.File.Exists(duongDanNguon))
        {
            return KetQuaLuuCV.ThatBai("Không tìm thấy file CV mặc định. Vui lòng tải lên CV mới.");
        }

        string extension = Path.GetExtension(duongDanNguon);

        if (!DinhDangCVHopLe.Contains(extension))
        {
            return KetQuaLuuCV.ThatBai("CV mặc định không có định dạng hợp lệ.");
        }

        string tenThuMuc = $"post_{baiTuyenDungId}";

        string thuMucDich = Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads",
            "cvs",
            "applied",
            tenThuMuc
        );

        Directory.CreateDirectory(thuMucDich);

        string tenFile = $"cv_{sinhVien.PK_IdSinhVien}{extension.ToLowerInvariant()}";

        string duongDanDich = Path.Combine(thuMucDich, tenFile);

        System.IO.File.Copy(duongDanNguon, duongDanDich, overwrite: true);

        string duongDanTuongDoi = $"/uploads/cvs/applied/{tenThuMuc}/{tenFile}";

        return KetQuaLuuCV.ThanhCongVoi(duongDanTuongDoi, duongDanDich);
    }

    private string GetFirstModelError()
    {
        return ModelState
                .Values.SelectMany(value => value.Errors)
                .Select(error => error.ErrorMessage)
                .FirstOrDefault(message => !string.IsNullOrWhiteSpace(message))
            ?? "Dữ liệu nộp hồ sơ không hợp lệ.";
    }

    private sealed class KetQuaLuuCV
    {
        public bool ThanhCong { get; private init; }

        public string ThongBao { get; private init; } = string.Empty;

        public string? DuongDanTuongDoi { get; private init; }

        public string? DuongDanVatLy { get; private init; }

        public static KetQuaLuuCV ThatBai(string thongBao)
        {
            return new KetQuaLuuCV { ThanhCong = false, ThongBao = thongBao };
        }

        public static KetQuaLuuCV ThanhCongVoi(string duongDanTuongDoi, string duongDanVatLy)
        {
            return new KetQuaLuuCV
            {
                ThanhCong = true,
                DuongDanTuongDoi = duongDanTuongDoi,
                DuongDanVatLy = duongDanVatLy,
            };
        }
    }
}
