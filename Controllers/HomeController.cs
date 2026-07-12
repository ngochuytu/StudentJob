using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudentJob.Models;
using StudentJob.Repositories;

namespace StudentJob.Controllers;

[Route("")]
public class HomeController : Controller
{
    private readonly IBaiTuyenDungRepository _baiTuyenDungRepository;
    private readonly INganhNgheRepository _nganhNgheRepository;
    private readonly IKhuVucRepository _khuVucRepository;

    public HomeController(
        IBaiTuyenDungRepository baiTuyenDungRepository,
        INganhNgheRepository nganhNgheRepository,
        IKhuVucRepository khuVucRepository)
    {
        _baiTuyenDungRepository = baiTuyenDungRepository;
        _nganhNgheRepository = nganhNgheRepository;
        _khuVucRepository = khuVucRepository;
    }

    private const int SoTinMoiTrang = 8;

    [HttpGet("")]
    public IActionResult Index()
    {
        var ketQua = _baiTuyenDungRepository.GetByDieuKien(null, null, null, null, 1, SoTinMoiTrang);
        ViewBag.NganhNghe = _nganhNgheRepository.GetAll();
        ViewBag.KhuVuc = _khuVucRepository.GetAll();
        return View(ketQua);
    }

    [HttpGet("privacy")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [HttpGet("error")]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }

    [HttpGet("loi/403")]
    public IActionResult TruyCapBiTuChoi()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View();
    }
}
