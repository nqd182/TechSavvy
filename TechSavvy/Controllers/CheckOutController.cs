﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechSavvy.Models;
using TechSavvy.Repository;

namespace TechSavvy.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly DataContext _dataContext;
        public CheckOutController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IActionResult> Checkout()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email); // Claims là những thông tin về người dùng
            if (userEmail == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var orderCode = Guid.NewGuid().ToString();
                var orderItem = new OrderModel();
                orderItem.OrderCode = orderCode;
                orderItem.UserName = userEmail;
                orderItem.Status = 1;
                orderItem.CreatedDate = DateTime.Now;
                _dataContext.Add(orderItem);
                _dataContext.SaveChanges();
                List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
                foreach(var cart in cartItems)
                {
                    var orderdetail = new OrderDetails();
                    orderdetail.OrderCode = orderCode;
                    orderdetail.ProductId = cart.ProductId;
                    orderdetail.Price = cart.Price;
                    orderdetail.Quantity = cart.Quantity;
                    _dataContext.Add(orderdetail);
                    _dataContext.SaveChanges();
                }
                HttpContext.Session.Remove("Cart");
                TempData["success"] = "Thanh toán thành công vui lòng chờ duyệt đơn hàng";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
