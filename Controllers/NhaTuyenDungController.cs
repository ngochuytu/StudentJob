namespace StudentJob.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("nha-tuyen-dung")]
[Authorize(Roles = "Nhà tuyển dụng")]
public class NhaTuyenDungController : Controller
{
    [HttpGet("quan-ly-tin")]
    public IActionResult QuanLyTin()
    {
        return View();
    }
}
