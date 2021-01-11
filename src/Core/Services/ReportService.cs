using Core.DTO;
using Core.Exceptions;
using Core.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Services
{
    public class ReportService : IReportService
    {
        private IUnitOfWork Database { get; set; }

        private readonly UserManager<ApplicationUser> _userManager;

        public ReportService(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
        {
            Database = uow;
            _userManager = userManager;
        }

        #region for provider

        public List<List<string>> GetReportProvider(int? providerId)
        {
            if (providerId == null)
                throw new ValidationException("Provider id not set", "");

            return GetReportProviderList(providerId.Value, null, null);
        }

        public List<List<string>> GetReportProvider(int? providerId, DateTime? date)
        {
            if (providerId == null)
                throw new ValidationException("Provider id not set", "");

            if (date == null)
                throw new ValidationException("Date not set", "");

            return GetReportProviderList(providerId.Value, date, null);
        }

        public List<List<string>> GetReportProvider(int? providerId, DateTime? dateWith, DateTime? dateTo)
        {
            if (providerId == null)
                throw new ValidationException("Provider id not set", "");

            if (dateWith == null || dateTo == null)
                throw new ValidationException("Date not set", "");

            return GetReportProviderList(providerId.Value, dateWith, dateTo);
        }

        public List<List<string>> GetReportProviders()
        {
            return GetReportProvidersList(null, null);
        }

        public List<List<string>> GetReportProviders(DateTime? date)
        {
            return GetReportProvidersList(date, null);
        }

        public List<List<string>> GetReportProviders(DateTime? dateWith, DateTime? dateTo)
        {
            return GetReportProvidersList(dateWith, dateTo);
        }


        private List<ReportProviderDTO> GetReportProviderListDTO(int providerId, DateTime? dateWith, DateTime? dateTo)
        {
            var catalogs = Database.Catalog.Find(p => p.ProviderId == providerId).ToList();

            var dishes = Database.Dish.GetAll().ToList();
            var orderDishes = Database.OrderDishes.GetAll().ToList();
            var order = Database.Orders.GetAll().ToList();

            var result = from c in catalogs
                         join d in dishes on c.Id equals d.CatalogId
                         join oD in orderDishes on d.Id equals oD.DishId
                         join o in order on oD.OrderId equals o.Id
                         select new { Id = o.Id, Count = oD.Count, OrderId = oD.OrderId, DateOrder = o.DateOrder };

            List<ReportProviderDTO> reportProviderDTOs = new List<ReportProviderDTO>();

            foreach (var rs in result)
            {
                if (reportProviderDTOs.Find(p => p.Id == rs.Id) == null)
                {
                    reportProviderDTOs.Add(
                        new ReportProviderDTO()
                        {
                            Id = rs.Id,
                            CountOrderDishes = GetCountForProvider(rs.Id, providerId),
                            DateOrder = rs.DateOrder,
                            FullPrice = GetPriceForProvider(rs.Id, providerId)
                        });
                }
            }

            if (dateWith != null && dateTo == null)
            {
                reportProviderDTOs = reportProviderDTOs.Where(p => p.DateOrder.ToString("dd.MM.yyyy") == dateWith.Value.ToString("dd.MM.yyyy")).ToList();
            }
            else if (dateWith != null && dateTo != null)
            {
                reportProviderDTOs = reportProviderDTOs.Where(p => p.DateOrder.Date >= dateWith.Value.Date &&
                 p.DateOrder.Date <= dateTo.Value.Date).ToList();
            }


            return reportProviderDTOs;
        }

        private List<List<string>> GetReportProviderList(int providerId, DateTime? dateWith, DateTime? dateTo)
        {
            List<ReportProviderDTO> reportProviderDTOs = GetReportProviderListDTO(providerId, dateWith, dateTo);

            List<List<string>> reportList = new List<List<string>>();

            List<string> header = new List<string>() { "Order number","Date and time of order", "Number of dishes", "Price" };
            reportList.Add(header);

            foreach (var providerReportDTO in reportProviderDTOs.OrderBy(p => p.DateOrder))
            {
                List<string> columReport = new List<string>();

                columReport.Add(providerReportDTO.Id.ToString());
                columReport.Add(providerReportDTO.DateOrder.ToString());
                columReport.Add(providerReportDTO.CountOrderDishes.ToString());
                columReport.Add(providerReportDTO.FullPrice.ToString());

                reportList.Add(columReport);
            }

            return reportList;
        }

        private List<ReportProvidersDTO> GetReportProvidersListDTO(DateTime? dateWith, DateTime? dateTo)
        {
            var providers = Database.Provider.GetAll().ToList();

            var providersDTOs = new List<ReportProvidersDTO>();

            foreach (var pr in providers)
            {
                List<ReportProviderDTO> prov = new List<ReportProviderDTO>();

                if (dateWith != null && dateTo == null)
                {
                    prov = GetReportProviderListDTO(pr.Id, dateWith, null);
                }
                else if (dateWith != null && dateTo != null)
                {
                    prov = GetReportProviderListDTO(pr.Id, dateWith, dateTo);
                }
                else
                {
                    prov = GetReportProviderListDTO(pr.Id, null, null);
                }

                providersDTOs.Add(
                    new ReportProvidersDTO()
                    {
                        Id = pr.Id,
                        Name = pr.Name,
                        CountOrder = prov.Count(),
                        CountOrderDishes = prov.Sum(p => p.CountOrderDishes),
                        FullPrice = prov.Sum(p => p.FullPrice)
                    });
            }

            return providersDTOs;
        }

        private List<List<string>> GetReportProvidersList(DateTime? dateWith, DateTime? dateTo)
        {
            List<ReportProvidersDTO> reportProvidersDTOs = GetReportProvidersListDTO(dateWith, dateTo);

            List<List<string>> reportList = new List<List<string>>();

            List<string> header = new List<string>() { "Supplier number", "Provider", "Number of orders", "Number of ordered dishes", "Orders for the amount" };
            reportList.Add(header);

            foreach (var providerReportDTO in reportProvidersDTOs.OrderBy(p => p.Name))
            {
                List<string> columReport = new List<string>();

                columReport.Add(providerReportDTO.Id.ToString());
                columReport.Add(providerReportDTO.Name.ToString());
                columReport.Add(providerReportDTO.CountOrder.ToString());
                columReport.Add(providerReportDTO.CountOrderDishes.ToString());
                columReport.Add(providerReportDTO.FullPrice.ToString());

                reportList.Add(columReport);
            }

            return reportList;
        }

        private decimal GetPriceForProvider(int orderId, int providerId)
        {
            var catalogs = Database.Catalog.Find(p => p.ProviderId == providerId).ToList();
            var dishes = Database.Dish.GetAll().ToList();
            var orderDishes = Database.OrderDishes.Find(p => p.OrderId == orderId).ToList();

            var result = from c in catalogs
                         join d in dishes on c.Id equals d.CatalogId
                         join oD in orderDishes on d.Id equals oD.DishId
                         select new { Count = oD.Count, d.Price };

            decimal fullPrice = 0;

            foreach (var pr in result)
            {
                fullPrice += pr.Count * pr.Price;
            }

            return fullPrice;
        }

        private int GetCountForProvider(int orderId, int providerId)
        {
            var catalogs = Database.Catalog.Find(p => p.ProviderId == providerId).ToList();
            var dishes = Database.Dish.GetAll().ToList();
            var orderDishes = Database.OrderDishes.Find(p => p.OrderId == orderId).ToList();

            var result = from c in catalogs
                         join d in dishes on c.Id equals d.CatalogId
                         join oD in orderDishes on d.Id equals oD.DishId
                         select new { Count = oD.Count };

            int count = 0;

            foreach (var pr in result)
            {
                count += pr.Count;
            }

            return count;
        }

        #endregion

        #region for user

        public List<List<string>> GetReportUser(string userId)
        {
            if (userId == null)
                throw new ValidationException("User id not set", "");

            return GetReportUserList(userId, null, null);
        }

        public List<List<string>> GetReportUser(string userId, DateTime? date)
        {
            if (userId == null)
                throw new ValidationException("User id not set", "");

            if (date == null)
                throw new ValidationException("Date not set", "");

            return GetReportUserList(userId, date, null);
        }

        public List<List<string>> GetReportUser(string userId, DateTime? dateWith, DateTime? dateTo)
        {
            if (userId == null)
                throw new ValidationException("User id not set", "");

            if (dateWith == null || dateTo == null)
                throw new ValidationException("Date not set", "");

            return GetReportUserList(userId, dateWith, dateTo);
        }

        public List<List<string>> GetReportUsers()
        {
            return GetReportUsersList(null, null);
        }

        public List<List<string>> GetReportUsers(DateTime? date)
        {
            return GetReportUsersList(date, null);
        }

        public List<List<string>> GetReportUsers(DateTime? dateWith, DateTime? dateTo)
        {
            return GetReportUsersList(dateWith, dateTo);
        }

        private List<ReportUsersDTO> GetReportUsersListDTO(DateTime? dateWith, DateTime? dateTo)
        {
            var users = _userManager.Users.ToList();

            var usersDTOs = new List<ReportUsersDTO>();

            foreach (var u in users)
            {
                List<ReportUserDTO> user = new List<ReportUserDTO>();

                if (dateWith != null && dateTo == null)
                {
                    user = GetReportUserListDTO(u.Id, dateWith, null);
                }
                else if (dateWith != null && dateTo != null)
                {
                    user = GetReportUserListDTO(u.Id, dateWith, dateTo);
                }
                else
                {
                    user = GetReportUserListDTO(u.Id, null, null);
                }

                usersDTOs.Add(
                    new ReportUsersDTO()
                    {
                        Id = u.Id,
                        Email = u.Email,
                        LFP = $"{u.Lastname} {u.Firstname} {u.Patronomic}",
                        CountOrder = user.Count(),
                        CountOrderDishes = user.Sum(p => p.CountOrderDishes),
                        FullPrice = user.Sum(p => p.FullPrice)
                    }); ;
            }

            return usersDTOs;
        }

        private List<List<string>> GetReportUsersList(DateTime? dateWith, DateTime? dateTo)
        {
            List<ReportUsersDTO> reportUsersDTOs = GetReportUsersListDTO(dateWith, dateTo);

            List<List<string>> reportList = new List<List<string>>();

            List<string> header = new List<string>() { "Id", "Email", "Full name", "Number of orders", "Number of dishes", "Total cost" };
            reportList.Add(header);

            foreach (var providerReportDTO in reportUsersDTOs.OrderBy(p => p.Email))
            {
                List<string> columReport = new List<string>();

                columReport.Add(providerReportDTO.Id.ToString());
                columReport.Add(providerReportDTO.Email.ToString());
                columReport.Add(providerReportDTO.LFP.ToString());
                columReport.Add(providerReportDTO.CountOrder.ToString());
                columReport.Add(providerReportDTO.CountOrderDishes.ToString());
                columReport.Add(providerReportDTO.FullPrice.ToString());

                reportList.Add(columReport);
            }

            return reportList;
        }

        private List<ReportUserDTO> GetReportUserListDTO(string userId, DateTime? dateWith, DateTime? dateTo)
        {
            var orders = Database.Orders.Find(p => p.ApplicationUserId == userId).ToList();

            var ordersDishes = Database.OrderDishes.GetAll().ToList();
            var dishes = Database.Dish.GetAll().ToList();

            var result = from o in orders
                         join oD in ordersDishes on o.Id equals oD.OrderId
                         join d in dishes on oD.DishId equals d.Id
                         select new { Id = o.Id, Count = oD.Count, OrderId = oD.OrderId, DateOrder = o.DateOrder };

            List<ReportUserDTO> reportUserDTOs = new List<ReportUserDTO>();

            foreach (var rs in result)
            {
                if (reportUserDTOs.Find(p => p.Id == rs.Id) == null)
                {
                    reportUserDTOs.Add(
                        new ReportUserDTO()
                        {
                            Id = rs.Id,
                            CountOrderDishes = GetCountForUser(rs.Id, userId),
                            DateOrder = rs.DateOrder,
                            FullPrice = GetPriceForUser(rs.Id, userId)
                        });
                }
            }

            if (dateWith != null && dateTo == null)
            {
                reportUserDTOs = reportUserDTOs.Where(p => p.DateOrder.ToString("dd.MM.yyyy") == dateWith.Value.ToString("dd.MM.yyyy")).ToList();
            }
            else if (dateWith != null && dateTo != null)
            {
                reportUserDTOs = reportUserDTOs.Where(p => p.DateOrder.Date >= dateWith.Value.Date &&
                 p.DateOrder.Date <= dateTo.Value.Date).ToList();
            }

            return reportUserDTOs.OrderBy(p => p.DateOrder).ToList();
        }

        private List<List<string>> GetReportUserList(string userId, DateTime? dateWith, DateTime? dateTo)
        {
            List<ReportUserDTO> reportProviderDTOs = GetReportUserListDTO(userId, dateWith, dateTo);

            List<List<string>> reportList = new List<List<string>>();

            List<string> header = new List<string>() { "Order number", "Date and time of order", "Number of dishes", "Price" };
            reportList.Add(header);

            foreach (var providerReportDTO in reportProviderDTOs.OrderBy(p => p.DateOrder))
            {
                List<string> columReport = new List<string>();

                columReport.Add(providerReportDTO.Id.ToString());
                columReport.Add(providerReportDTO.DateOrder.ToString());
                columReport.Add(providerReportDTO.CountOrderDishes.ToString());
                columReport.Add(providerReportDTO.FullPrice.ToString());

                reportList.Add(columReport);
            }

            return reportList;
        }

        private decimal GetPriceForUser(int orderId, string userId)
        {
            var order = Database.Orders.Find(p => p.ApplicationUserId == userId).ToList();
            var orderDishes = Database.OrderDishes.Find(p => p.OrderId == orderId).ToList();
            var dishes = Database.Dish.GetAll().ToList();

            var result = from o in order
                         join oD in orderDishes on o.Id equals oD.OrderId
                         join d in dishes on oD.DishId equals d.Id
                         select new { Count = oD.Count, d.Price };

            decimal fullPrice = 0;

            foreach (var pr in result)
            {
                fullPrice += pr.Count * pr.Price;
            }

            return fullPrice;
        }

        private int GetCountForUser(int orderId, string userId)
        {
            var order = Database.Orders.Find(p => p.ApplicationUserId == userId).ToList();
            var orderDishes = Database.OrderDishes.Find(p => p.OrderId == orderId).ToList();
            var dishes = Database.Dish.GetAll().ToList();

            var result = from o in order
                         join oD in orderDishes on o.Id equals oD.OrderId
                         join d in dishes on oD.DishId equals d.Id
                         select new { Count = oD.Count };

            int count = 0;

            foreach (var pr in result)
            {
                count += pr.Count;
            }

            return count;
        }

        #endregion
    }
}
