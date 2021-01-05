using Core.Constants;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Interfaces;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoggerService _loggerService;

        private const string CONTROLLER_NAME = "home";

        public HomeController(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public IActionResult Index()
        {
            _loggerService.LogInformation(CONTROLLER_NAME + LoggerConstants.ACTION_INDEX, LoggerConstants.TYPE_GET, "index", GetCurrentUserId());

            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            _loggerService.LogInformation(CONTROLLER_NAME + "/privacy", LoggerConstants.TYPE_GET, "privacy", GetCurrentUserId());

            return View();
        }

        [HttpGet]
        public IActionResult AboutDelivery()
        {
            _loggerService.LogInformation(CONTROLLER_NAME + "/aboutdelivery", LoggerConstants.TYPE_GET, "aboutdelivery", GetCurrentUserId());

            return View();
        }

        [HttpGet]
        public IActionResult About()
        {
            _loggerService.LogInformation(CONTROLLER_NAME + "/about", LoggerConstants.TYPE_GET, "about", GetCurrentUserId());

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public IActionResult Error(string requestId)
        {
            _loggerService.LogInformation(CONTROLLER_NAME + $"/error/{requestId}", LoggerConstants.TYPE_GET, $"error {requestId}", GetCurrentUserId());

            return View(new ErrorViewModel() { RequestId = requestId });
        }

        private string GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                return User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            else
            {
                return null;
            }
        }
    }
}
