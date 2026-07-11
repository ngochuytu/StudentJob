namespace StudentJob.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("admin")]
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    [HttpGet("quan-ly-ho-so")]
    public IActionResult QuanLyHoSo()
    {
        return View();
    }

    [HttpGet("quan-ly-tai-khoan")]
    public IActionResult QuanLyTaiKhoan()
    {
        return View();
    }
}
