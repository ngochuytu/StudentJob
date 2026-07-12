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

        ViewBag.TotalJobs = jobs.Count;
        ViewBag.ApprovedJobs = jobs.Count(j => j.sTrangThaiKiemDuyet == "Đã duyệt");
        ViewBag.PendingJobs = jobs.Count(j => j.sTrangThaiKiemDuyet == "Chờ duyệt");
        ViewBag.RejectedJobs = jobs.Count(j => j.sTrangThaiKiemDuyet == "Từ chối");

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

        ViewBag.TotalUsers = accounts.Count;
        ViewBag.StudentUsers = accounts.Count(t => t.VaiTro.sTenVaiTro == "Sinh viên");
        ViewBag.RecruiterUsers = accounts.Count(t => t.VaiTro.sTenVaiTro == "Nhà tuyển dụng");
        ViewBag.AdminUsers = accounts.Count(t => t.VaiTro.sTenVaiTro == "Admin");

        return View(accounts);
    }

    [HttpGet("thong-ke")]
    public IActionResult ThongKe()
    {
        var accounts = _taiKhoanRepository.GetAll();
        var jobs = _baiTuyenDungRepository.GetAll();
        var applications = _hoSoUngTuyenRepository.GetAll();

        ViewBag.TotalUsers = accounts.Count;
        ViewBag.StudentUsers = accounts.Count(t => t.VaiTro.sTenVaiTro == "Sinh viên");
        ViewBag.RecruiterUsers = accounts.Count(t => t.VaiTro.sTenVaiTro == "Nhà tuyển dụng");
        ViewBag.AdminUsers = accounts.Count(t => t.VaiTro.sTenVaiTro == "Admin");

        ViewBag.TotalJobs = jobs.Count;
        ViewBag.ApprovedJobs = jobs.Count(j => j.sTrangThaiKiemDuyet == "Đã duyệt");
        ViewBag.PendingJobs = jobs.Count(j => j.sTrangThaiKiemDuyet == "Chờ duyệt");
        ViewBag.RejectedJobs = jobs.Count(j => j.sTrangThaiKiemDuyet == "Từ chối");

        ViewBag.TotalApplications = applications.Count;
        ViewBag.PendingApplications = applications.Count(a => a.sTrangThaiXetDuyet == "Chờ duyệt");
        ViewBag.ShortlistedApplications = applications.Count(a => a.sTrangThaiXetDuyet == "Hẹn phỏng vấn");
        ViewBag.RejectedApplications = applications.Count(a => a.sTrangThaiXetDuyet == "Từ chối");

        return View();
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
