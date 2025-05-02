using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TechSavvy.Models;
using TechSavvy.Models.ViewModels;
using TechSavvy.Repository;

namespace TechSavvy.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        public ProductController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(int Id)
        {

            ProductModel productById = _dataContext.Products.Include(p => p.Ratings).FirstOrDefault(p => p.Id == Id);
            if (productById == null) RedirectToAction("Index");
            var relatedProduct = await _dataContext.Products
                .Where(p => p.CategoryId == productById.CategoryId && p.Id != productById.Id)
                .Take(4)
                .ToListAsync();
            var viewModel = new ProductDetailsViewModel
            {
                ProductDetails = productById,

            };
            ViewBag.RelatedProducts = relatedProduct;
            return View(viewModel);
        }
        public async Task<IActionResult> Search(string searchItem)
        {
            // Xử lý khi searchItem là null hoặc rỗng
            if (string.IsNullOrEmpty(searchItem))
            {
                return View(new List<ProductModel>());
            }
            var products = await _dataContext.Products.Where(p => p.Name.Contains(searchItem.ToLower()) || p.Description.Contains(searchItem)).ToListAsync();
            ViewBag.Keyword = searchItem;
            return View(products);
        }
        public async Task<IActionResult> CommentProduct(RatingModel rating)
        {
            return View();
        }
    }
}