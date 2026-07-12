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

    [HttpGet("")]
    public IActionResult Index()
    {
        var jobs = _baiTuyenDungRepository.GetApprovedJobs();
        ViewBag.NganhNghe = _nganhNgheRepository.GetAll();
        ViewBag.KhuVuc = _khuVucRepository.GetAll();
        return View(jobs);
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
}
