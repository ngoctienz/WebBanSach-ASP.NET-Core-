using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebBanSach.Models;
using WebBanSach.Models.DTOs;
using WebBanSach.Services;

namespace WebBanSach.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookServices _bookService;
        private readonly IValidator<DonDatHangRequest> _validator;

        public HomeController(IBookServices bookService, IValidator<DonDatHangRequest> validator)
        {
            _bookService = bookService;
            _validator = validator;
        }

        public async Task<IActionResult> Index()
        {
            var book = await _bookService.GetBookAsync();
            var author = book != null && !string.IsNullOrEmpty(book.MaTacGia)
                ? await _bookService.GetAuthorAsync(book.MaTacGia)
                : null;
            var reviews = await _bookService.GetReviewsAsync();

            ViewData["Book"] = book;
            ViewData["Author"] = author;
            ViewData["Reviews"] = reviews;

            return View();
        }

        [HttpPost]
        [Route("Home/DatHang")]
        [Route("DatHang")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatHang([FromBody] DonDatHangRequest request)
        {
            // 1. Kiểm tra dữ liệu ở Server bằng FluentValidation
            var validationResult = await _validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(new DonDatHangResponse
                {
                    Tatus = 400,
                    message = validationResult.Errors.First().ErrorMessage,
                    Eror = validationResult.Errors.Select(e => e.ErrorMessage).ToList(),
                    ThanhCong = false,
                    ThongBao = validationResult.Errors.First().ErrorMessage
                });
            }

            // 2. Chuyển toàn bộ nghiệp vụ đặt hàng cho BookService xử lý
            var response = await _bookService.DatHangAsync(request);
            return Ok(response);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
