using System;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using AboutMe.Data;
using AboutMe.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AboutMe.Controllers
{
    public class HomeController(
        ILogger<HomeController> logger,
        MyDb context,
        IConfiguration config) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly MyDb _context = context;
        private readonly IConfiguration _config = config;

        public async Task<IActionResult> Index()
        {
            var counts = await _context.Counts.FirstOrDefaultAsync(c => c.Id == 1);
            if (counts == null)
            {
                counts = new Counts { LikeCount = 0, CommentCount = 0, ShareCount = 0};
                await _context.Counts.AddAsync(counts);
                await _context.SaveChangesAsync();
            }

            return View(counts);
        }


        [HttpGet]
        public async Task<IActionResult> GetCommentCount()
        {
            var counts = await _context.Counts.FirstOrDefaultAsync(c => c.Id == 1);

            if (counts == null)
            {
                return Json(new { commentcount = 0 });
            }

            return Json(new { commentcount =  counts.CommentCount });
        }

        [HttpPost]
        public async Task<IActionResult> AddLike()
        {
            var counts = await _context.Counts.FirstOrDefaultAsync(c => c.Id == 1);
            if (counts == null)
            {
                counts = new Counts { LikeCount = 1 };
                await _context.Counts.AddAsync(counts);
            }
            else
            {
                counts.LikeCount += 1;
            }
            await _context.SaveChangesAsync();

            return Json(new { likecount = counts.LikeCount });
        }  /// Like bosilganida bazadagi LikeCount jadvalidagi id = 1 ni update qilopti (+1)

        [HttpPost]
        public async Task<IActionResult> RemoveLike()
        {
            var counts = await _context.Counts.FirstOrDefaultAsync(c => c.Id == 1);
            if (counts == null) return NotFound();

            if (counts.LikeCount > 0)
                counts.LikeCount -= 1;

            await _context.SaveChangesAsync();

            return Json(new { likecount = counts.LikeCount });
        }  /// Like  bosilganida bazadagi LikeCount jadvalidagi id = 1ni update qiladi (-1)

        [HttpPost]
        public async Task<IActionResult> ShareCount()
        {
            var counts = await _context.Counts.FirstOrDefaultAsync(c => c.Id == 1);
            if (counts == null)
            {
                counts = new Counts { ShareCount = 1 };
                await _context.Counts.AddAsync(counts);
            }
            else
            {
                counts.ShareCount += 1;
            }
            await _context.SaveChangesAsync();

            return Json(new { countshare = counts.ShareCount });
        }  /// Share qilaytgonida bazadagi ShareCount jadvalidagi id = 1ni update qiladi (+1)

        [HttpPost]
        public async Task<IActionResult> SendToTelegram([FromBody] MessageDto model)
        {
            string? botToken = _config["Telegram:Token"];
            string? chatId = _config["Telegram:ChatId"];

            var payload = new
            {
                chat_id = chatId,
                text = model.Text
            };

            using var client = new HttpClient();
            var content = JsonContent.Create(payload);
            var response = await client.PostAsync($"https://api.telegram.org/bot{botToken}/sendMessage", content);

            if (response.IsSuccessStatusCode)
            {
                return Ok("Message sent successfully");
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Telegram API error: {Response}", responseBody);
                return StatusCode(500, "Failed to send message.");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public class MessageDto
        {
            public string? Text { get; set; }
        }
    }
}