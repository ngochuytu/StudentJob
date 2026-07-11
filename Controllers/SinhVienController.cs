namespace StudentJob.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("sinh-vien")]
[Authorize(Roles = "Sinh viên")]
public class SinhVienController : Controller
{
    [HttpGet("theo-doi-ho-so")]
    public IActionResult TheoDoiHoSo()
    {
        return View();
    }
}
