using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using StudentJob.Models;
using StudentJob.Repositories;

namespace StudentJob.Controllers;

[Route("viec-lam")]
public class ViecLamController : Controller
{
    private readonly IBaiTuyenDungRepository _baiTuyenDungRepository;
    private readonly INganhNgheRepository _nganhNgheRepository;
    private readonly IKhuVucRepository _khuVucRepository;

    public ViecLamController(
        IBaiTuyenDungRepository baiTuyenDungRepository,
        INganhNgheRepository nganhNgheRepository,
        IKhuVucRepository khuVucRepository)
    {
        _baiTuyenDungRepository = baiTuyenDungRepository;
        _nganhNgheRepository = nganhNgheRepository;
        _khuVucRepository = khuVucRepository;
    }

    [HttpGet("{id:int}")]
    public IActionResult ChiTiet(int id)
    {
        var job = _baiTuyenDungRepository.GetById(id);
        if (job == null || job.sTrangThaiKiemDuyet != "Đã duyệt")
        {
            return NotFound();
        }

        return View(job);
    }

    private const int SoTinMoiTrang = 8;

    [HttpPost("api/filter")]
    public IActionResult Filter(
        [FromForm] string? keyword,
        [FromForm] string? hinhThuc,
        [FromForm] int? nganhNgheId,
        [FromForm] int? khuVucId,
        [FromForm] int pageIndex = 1)
    {
        var ketQua = _baiTuyenDungRepository.GetByDieuKien(keyword, hinhThuc, nganhNgheId, khuVucId, pageIndex, SoTinMoiTrang);

        return PartialView("_DanhSachViecLamPartial", ketQua);
    }
}
