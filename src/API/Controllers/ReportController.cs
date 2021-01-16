using API.Interfaces;
using API.Reports;
using Core.Constants;
using Core.Exceptions;
using Core.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IProviderService _providerService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserHelper _userHelper;
        public ReportController(IReportService reportService,
              IProviderService providerService,
               IWebHostEnvironment webHostEnvironment,
               UserManager<ApplicationUser> userManager,
                IUserHelper userHelper)
        {
            _reportService = reportService;
            _providerService = providerService;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _userHelper = userHelper;
        }

        [HttpGet("provider/{providerId}/{dateWith?}/{dateTo?}")]
        public IActionResult GetReportProvider(int providerId, DateTime? dateWith = null, DateTime? dateTo = null)
        {
            var provider = _providerService.GetProvider(providerId);

            if (provider == null)
                return NotFound("Provider not found");

            List<List<string>> reportList;

            string title = string.Empty;

            try
            {
                if (dateWith != null && dateTo == null)
                {
                    reportList = _reportService.GetReportProvider(providerId, dateWith.Value);
                    title = $"{ReportConstants.REPORT_BY_PROVIDER} ({provider.Name}) {ReportConstants.PER} {dateWith.Value.ToString("dd.MM.yyyy")}";

                    //   _loggerService.LogInformation(CONTROLLER_NAME + $"/getreportprovider/{providerId}", LoggerConstants.TYPE_GET, $"get report provider id: {providerId} dateWith: {dateWith}", GetCurrentUserId());
                }
                else if (dateWith != null && dateTo != null)
                {
                    reportList = _reportService.GetReportProvider(providerId, dateWith.Value, dateTo.Value);
                    title = $"{ReportConstants.REPORT_BY_PROVIDER} ({provider.Name}) {ReportConstants.WITH} {dateWith.Value.ToString("dd.MM.yyyy")} \n {ReportConstants.BY} {dateTo.Value.ToString("dd.MM.yyyy")}";

                    // _loggerService.LogInformation(CONTROLLER_NAME + $"/getreportprovider/{providerId}", LoggerConstants.TYPE_GET, $"get report provider id: {providerId} dateWith: {dateWith} dateTo: {dateTo}", GetCurrentUserId());
                }
                else
                {
                    reportList = _reportService.GetReportProvider(providerId);
                    title = $"{ReportConstants.REPORT_BY_PROVIDER} ({provider.Name}) {ReportConstants.FOR_ALL_TIME} ";

                    //   _loggerService.LogInformation(CONTROLLER_NAME + $"/getreportprovider/{providerId}", LoggerConstants.TYPE_GET, $"get report provider id: {providerId}", GetCurrentUserId());
                }
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            ReportPDF reportProvider = new ReportPDF(_webHostEnvironment);

            return File(reportProvider.Report(reportList, title), "application/pdf");
        }

        [HttpGet("providers/{dateWith?}/{dateTo?}")]
        public IActionResult GetReportProviders(DateTime? dateWith = null, DateTime? dateTo = null)
        {
            List<List<string>> reportProvidersDTOs;

            string title = string.Empty;

            try
            {
                if (dateWith != null && dateTo == null)
                {
                    reportProvidersDTOs = _reportService.GetReportProviders(dateWith.Value);
                    title = $"{ReportConstants.REPORT_BY_PROVIDERS_PER} {dateWith.Value.ToString("dd.MM.yyyy")}";

                    //   _loggerService.LogInformation(CONTROLLER_NAME + $"/getreportproviders", LoggerConstants.TYPE_GET, $"get report providers dateWith: {dateWith}", GetCurrentUserId());
                }
                else if (dateWith != null && dateTo != null)
                {
                    reportProvidersDTOs = _reportService.GetReportProviders(dateWith.Value, dateTo.Value);
                    title = $"{ReportConstants.REPORT_BY_PROVIDERS_WITH} {dateWith.Value.ToString("dd.MM.yyyy")} \n {ReportConstants.BY} {dateTo.Value.ToString("dd.MM.yyyy")}";

                    //    _loggerService.LogInformation(CONTROLLER_NAME + $"/getreportproviders", LoggerConstants.TYPE_GET, $"get report providers dateWith: {dateWith} dateTo: {dateTo}", GetCurrentUserId());
                }
                else
                {
                    reportProvidersDTOs = _reportService.GetReportProviders();
                    title = ReportConstants.REPORT_BY_PROVIDERS_FOR_ALL_TIME;

                    //     _loggerService.LogInformation(CONTROLLER_NAME + $"/getreportproviders", LoggerConstants.TYPE_GET, $"get report providers", GetCurrentUserId());
                }
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            ReportPDF reportProviders = new ReportPDF(_webHostEnvironment);

            return File(reportProviders.Report(reportProvidersDTOs, title), "application/pdf");
        }

        [HttpGet("user/{userId}/{dateWith?}/{dateTo?}")]
        public async Task<IActionResult> GetReportUser(string userId, DateTime? dateWith = null, DateTime? dateTo = null)
        {
            var user = await _userHelper.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            List<List<string>> reportUserDTOs = null;

            string title = string.Empty;

            try
            {
                if (dateWith != null && dateTo == null)
                {
                    reportUserDTOs = _reportService.GetReportUser(userId, dateWith.Value);
                    title = $"{ReportConstants.REPORT_BY_USER} ({user.Email}) {ReportConstants.PER} {dateWith.Value.ToString("dd.MM.yyyy")}";

                   // _loggerService.LogInformation(CONTROLLER_NAME + $"/getreportuser/{userId}", LoggerConstants.TYPE_GET, $"get report user id: {userId} dateWith: {dateWith}", GetCurrentUserId());
                }
                else if (dateWith != null && dateTo != null)
                {
                    reportUserDTOs = _reportService.GetReportUser(userId, dateWith.Value, dateTo.Value);
                    title = $"{ReportConstants.REPORT_BY_USER} ({user.Email}) {ReportConstants.WITH} {dateWith.Value.ToString("dd.MM.yyyy")} \n {ReportConstants.BY} {dateTo.Value.ToString("dd.MM.yyyy")}";

                   // _loggerService.LogInformation(CONTROLLER_NAME + $"/getreportuser/{userId}", LoggerConstants.TYPE_GET, $"get report user id: {userId} dateWith: {dateWith} dateTo: {dateTo}", GetCurrentUserId());
                }
                else
                {
                    reportUserDTOs = _reportService.GetReportUser(userId);
                    title = $"{ReportConstants.REPORT_BY_USER} ({user.Id}) {ReportConstants.FOR_ALL_TIME}";

                 //   _loggerService.LogInformation(CONTROLLER_NAME + $"/getreportuser/{userId}", LoggerConstants.TYPE_GET, $"get report user id: {userId}", GetCurrentUserId());
                }
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            ReportPDF reportUser = new ReportPDF(_webHostEnvironment);

            return File(reportUser.Report(reportUserDTOs, title), "application/pdf");
        }

        [HttpGet("users/{dateWith?}/{dateTo?}")]
        public IActionResult GetReportUsers(DateTime? dateWith = null, DateTime? dateTo = null)
        {
            List<List<string>> reportUsersDTOs = null;
            string title = string.Empty;

            try
            {
                if (dateWith != null && dateTo == null)
                {
                    reportUsersDTOs = _reportService.GetReportUsers(dateWith.Value);
                    title = $"{ReportConstants.REPORT_BY_USERS_PER} {dateWith.Value.ToString("dd.MM.yyyy")}";

                    //  _loggerService.LogInformation(CONTROLLER_NAME + $"/getreportusers", LoggerConstants.TYPE_GET, $"get report users dateWith: {dateWith}", GetCurrentUserId());
                }
                else if (dateWith != null && dateTo != null)
                {
                    reportUsersDTOs = _reportService.GetReportUsers(dateWith.Value, dateTo.Value);
                    title = $"{ReportConstants.REPORT_BY_USERS_WITH} {dateWith.Value.ToString("dd.MM.yyyy")} \n {ReportConstants.BY} {dateTo.Value.ToString("dd.MM.yyyy")}";

                    // _loggerService.LogInformation(CONTROLLER_NAME + $"/getreportusers", LoggerConstants.TYPE_GET, $"get report users dateWith: {dateWith} dateTo: {dateTo}", GetCurrentUserId());
                }
                else
                {
                    reportUsersDTOs = _reportService.GetReportUsers();
                    title = ReportConstants.REPORT_BY_USERS_FOR_ALL_TIME;

                    // _loggerService.LogInformation(CONTROLLER_NAME + $"/getreportusers", LoggerConstants.TYPE_GET, $"get report users", GetCurrentUserId());
                }
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }

            ReportPDF reportUsers = new ReportPDF(_webHostEnvironment);

            return File(reportUsers.Report(reportUsersDTOs, title), "application/pdf");
        }
    }
}
