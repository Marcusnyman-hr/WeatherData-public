using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WeatherData.Web.Data;
using WeatherData.Web.Models;

namespace WeatherData.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WeatherContext _context;
        public HomeController(ILogger<HomeController> logger, WeatherContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ReadCSV()
        {
            var noInsertedRows = DataAccess.ReadDataFile(_context);
            ViewBag.InsertedRows = noInsertedRows;
            return View();
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Documentation()
        {
            return View();
        }
        public IActionResult ProjectInformation()
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
