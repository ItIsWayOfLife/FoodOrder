using System;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IReportService
    {
        List<List<string>> GetReportProvider(int? providerId);
        List<List<string>> GetReportProvider(int? providerId, DateTime? date);
        List<List<string>> GetReportProvider(int? providerId, DateTime? dateWith, DateTime? dateTo);

        List<List<string>> GetReportProviders();
        List<List<string>> GetReportProviders(DateTime? date);
        List<List<string>> GetReportProviders(DateTime? dateWith, DateTime? dateTo);

        List<List<string>> GetReportUser(string userId);
        List<List<string>> GetReportUser(string userId, DateTime? date);
        List<List<string>> GetReportUser(string userId, DateTime? dateWith, DateTime? dateTo);

        List<List<string>> GetReportUsers();
        List<List<string>> GetReportUsers(DateTime? date);
        List<List<string>> GetReportUsers(DateTime? dateWith, DateTime? dateTo);
    }
}
