using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HäggesPizzeria.Data;
using HäggesPizzeria.Models;
using Microsoft.EntityFrameworkCore;

namespace HäggesPizzeria.Services
{
    public class OrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Order GetOrderWithOrderedDishes(int orderId)
        {
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderedDishes)
                .ThenInclude(od => od.OrderedDishIngredients)
                .ThenInclude(odi => odi.Ingredient)
                .Single(o => o.OrderId == orderId);
        }

        public async Task<ICollection<Order>> GetAllOrdersWithOrderedDishes()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderedDishes)
                .ThenInclude(od => od.OrderedDishIngredients)
                .ThenInclude(odi => odi.Ingredient).ToListAsync();
        }

        public async Task<ICollection<Order>> GetNonCompletedOrdersWithOrderedDishes()
        {
            return await _context.Orders
                .Where(o => !o.IsComplete)
                .Include(o => o.User)
                .Include(o => o.OrderedDishes)
                .ThenInclude(od => od.OrderedDishIngredients)
                .ThenInclude(odi => odi.Ingredient).ToListAsync();
        }

        public void SetOrderComplete(int orderId)
        {
            var order = _context.Orders.SingleOrDefault(o => o.OrderId == orderId);
            order.IsComplete = true;
            _context.Update(order);
            _context.SaveChanges();
        }

        public void SaveOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void SaveOrderedDishes(ICollection<OrderedDish> orderedDishes)
        {
            var order = _context.Orders.OrderByDescending(o => o.OrderId).FirstOrDefault();
            order.OrderDate = DateTime.Now;
            order.TotalPrice = orderedDishes.Sum(od => od.Price);
            order.User = _context.Users.SingleOrDefault(u => u.Email == order.Email);
            CreateOrderedDishes(orderedDishes, order);
        }

        public void CreateOrderedDishes(ICollection<OrderedDish> orderedDishes, Order order)
        {
            foreach (var orderedDish in orderedDishes)
            {
                _context.OrderedDishes.Add(orderedDish);
                _context.SaveChanges();

                var newOrderedDish = _context.OrderedDishes.OrderByDescending(od => od.OrderedDishId).FirstOrDefault();
                newOrderedDish.Order = order;

                _context.OrderedDishIngredients.AddRange(orderedDishes.Select(od => new OrderedDishIngredient { OrderedDishId = newOrderedDish.OrderedDishId, IngredientId = od.OrderedDishId }).ToList());
                _context.SaveChanges();
            }
        }
    }
}
