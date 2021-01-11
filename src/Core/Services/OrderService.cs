using Core.DTO;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Services
{
    public class OrderService : IOrderService
    {
        private IUnitOfWork Database { get; set; }

        public OrderService(IUnitOfWork uow)
        {
            Database = uow;
        }

        private decimal FullPriceOrder(IEnumerable<OrderDishes> orderDishes)
        {
            decimal fullPrice = 0;

            foreach (var cartDish in orderDishes)
            {
                fullPrice += cartDish.Count * cartDish.Dish.Price;
            }

            return fullPrice;
        }

        public OrderDTO Create(string applicationUserId)
        {
            if (applicationUserId == null)
                throw new ValidationException("User id not set", "");

            var cart = Database.Cart.Find(p => p.ApplicationUserId == applicationUserId).FirstOrDefault();

            if (cart == null)
                throw new ValidationException("Cart not found", "");

            var cartDishes = Database.CartDishes.Find(p => p.CartId == cart.Id).ToList();

            if (cartDishes == null || cartDishes.Count() == 0)
                throw new ValidationException("Cart is empty", "");

            // create order
            Order order = new Order() { ApplicationUserId = applicationUserId, DateOrder = DateTime.Now };

            Database.Orders.Create(order);
            Database.Save();

            foreach (var cartDish in cartDishes)
            {
                Database.OrderDishes.Create(new OrderDishes()
                {
                    Count = cartDish.Count,
                    DishId = cartDish.DishId,
                    OrderId = order.Id
                });
            }

            Database.Save();

            OrderDTO orderDTO = new OrderDTO()
            {
                CountDish = cartDishes.Sum(p => p.Count),
                DateOrder = order.DateOrder,
                FullPrice = FullPriceOrder(Database.OrderDishes.Find(p => p.OrderId == order.Id)),
                Id = order.Id
            };

            return orderDTO;
        }

        public IEnumerable<OrderDTO> GetOrders(string applicationUserId)
        {
            if (applicationUserId == null)
                throw new ValidationException("User id not set", "");

            var orders = Database.Orders.Find(p => p.ApplicationUserId == applicationUserId);

            var ordersDTOs = new List<OrderDTO>();

            foreach (var order in orders)
            {
                ordersDTOs.Add(new OrderDTO()
                {
                    CountDish = Database.OrderDishes.Find(p => p.OrderId == order.Id).Sum(p => p.Count),
                    Id = order.Id,
                    DateOrder = order.DateOrder,
                    FullPrice = FullPriceOrder(Database.OrderDishes.Find(p => p.OrderId == order.Id))
                });
            }

            return ordersDTOs;
        }

        public IEnumerable<OrderDishesDTO> GetOrderDishes(string applicationUserId, int? orderId)
        {
            if (applicationUserId == null)
                throw new ValidationException("User id not set", "");

            if (orderId == null)
                throw new ValidationException("Order not selected", "");

            var orderDishes = Database.OrderDishes.Find(p => p.OrderId == orderId).Where(p => p.Order.ApplicationUserId == applicationUserId);

            var orderDishesDTOs = new List<OrderDishesDTO>();

            foreach (var orderDish in orderDishes)
            {
                orderDishesDTOs.Add(new OrderDishesDTO()
                {
                    Count = orderDish.Count,
                    DishId = orderDish.DishId,
                    Id = orderDish.Id,
                    Info = orderDish.Dish.Info,
                    OrderId = orderDish.OrderId,
                    Name = orderDish.Dish.Name,
                    Path = orderDish.Dish.Path,
                    Price = orderDish.Dish.Price,
                    Weight = orderDish.Dish.Weight
                });
            }

            return orderDishesDTOs;
        }
    }
}
