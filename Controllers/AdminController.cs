namespace StudentJob.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentJob.Repositories;
using StudentJob.Models;
using System.Security.Claims;

[Route("admin")]
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ITaiKhoanRepository _taiKhoanRepository;
    private readonly IBaiTuyenDungRepository _baiTuyenDungRepository;
    private readonly IHoSoUngTuyenRepository _hoSoUngTuyenRepository;

    public AdminController(
        ITaiKhoanRepository taiKhoanRepository,
        IBaiTuyenDungRepository baiTuyenDungRepository,
        IHoSoUngTuyenRepository hoSoUngTuyenRepository)
    {
        _taiKhoanRepository = taiKhoanRepository;
        _baiTuyenDungRepository = baiTuyenDungRepository;
        _hoSoUngTuyenRepository = hoSoUngTuyenRepository;
    }

    [HttpGet("quan-ly-tin")]
    public IActionResult QuanLyTin()
    {
        var jobs = _baiTuyenDungRepository.GetAll();

        ViewBag.TotalUsers = _taiKhoanRepository.GetAll().Count;
        ViewBag.TotalJobs = jobs.Count;
        ViewBag.TotalApplications = _hoSoUngTuyenRepository.GetAll().Count;

        return View(jobs);
    }

    [HttpPost("duyet-tin")]
    public IActionResult DuyetTin(int id, string status)
    {
        var job = _baiTuyenDungRepository.GetById(id);
        if (job == null)
        {
            return Json(new { success = false, message = "Không tìm thấy tin tuyển dụng" });
        }

        if (status != "Đã duyệt" && status != "Từ chối" && status != "Chờ duyệt")
        {
            return Json(new { success = false, message = "Trạng thái không hợp lệ" });
        }

        job.sTrangThaiKiemDuyet = status;
        _baiTuyenDungRepository.Update(job);

        string message = status == "Đã duyệt" 
            ? "Phê duyệt tin tuyển dụng thành công" 
            : "Từ chối tin tuyển dụng thành công";

        return Json(new {
            success = true,
            message = message,
            data = new { status = status }
        });
    }

    [HttpGet("quan-ly-tai-khoan")]
    public IActionResult QuanLyTaiKhoan(string? query, string? role, string? status)
    {
        var accounts = _taiKhoanRepository.GetAll();
        var jobs = _baiTuyenDungRepository.GetAll();
        var applications = _hoSoUngTuyenRepository.GetAll();

        // if (!string.IsNullOrWhiteSpace(query))
        // {
        //     var q = query.ToLower().Trim();
        //     accounts = accounts.Where(t => 
        //         t.sEmail.ToLower().Contains(q) || 
        //         t.sSoDienThoai.Contains(q) ||
        //         (t.VaiTro.sTenVaiTro == "Sinh viên" && t.SinhVien != null && t.SinhVien.sHoTen.ToLower().Contains(q)) ||
        //         (t.VaiTro.sTenVaiTro == "Nhà tuyển dụng" && t.NhaTuyenDung != null && t.NhaTuyenDung.sTenDoanhNghiep.ToLower().Contains(q))
        //     ).ToList();
        // }

        // if (!string.IsNullOrWhiteSpace(role))
        // {
        //     accounts = accounts.Where(t => t.VaiTro.sTenVaiTro == role).ToList();
        // }

        // if (!string.IsNullOrWhiteSpace(status))
        // {
        //     bool isActive = status == "Hoạt động";
        //     accounts = accounts.Where(t => t.bTrangThaiHoatDong == isActive).ToList();
        // }

        // if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        // {
        //     return PartialView("_DanhSachTaiKhoanPartial", accounts);
        // }

        

        ViewBag.TotalUsers = _taiKhoanRepository.GetAll().Count;
        ViewBag.TotalJobs = jobs.Count;
        ViewBag.TotalApplications = applications.Count;

        return View(accounts);
    }

    [HttpPost("khoa-tai-khoan")]
    public IActionResult KhoaTaiKhoan(int id, bool active)
    {
        var taiKhoan = _taiKhoanRepository.GetById(id);
        if (taiKhoan == null)
        {
            return Json(new { success = false, message = "Không tìm thấy tài khoản." });
        }

        var currentUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (currentUserIdClaim != null && int.TryParse(currentUserIdClaim, out int currentUserId) && currentUserId == id)
        {
            return Json(new { success = false, message = "Bạn không thể tự khóa tài khoản của chính mình" });
        }

        _taiKhoanRepository.UpdateTrangThai(id, active);

        return Json(new {
            success = true,
            message = active ? "Mở khóa tài khoản thành công." : "Khóa tài khoản thành công.",
            data = new { active = active }
        });
    }
}
