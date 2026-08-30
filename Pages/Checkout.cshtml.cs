using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheBestBean.Models;
using TheBestBean.Services;
using System.ComponentModel.DataAnnotations;

namespace TheBestBean.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly CartService _cartService;
        private readonly Data.TheBestBeanContext _context;

        public CheckoutModel(CartService cartService, Data.TheBestBeanContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal CartTotal { get; set; }

        [BindProperty]
        public string FirstName { get; set; } = string.Empty;

        [BindProperty]
        public string LastName { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "State is required")]
        public string State { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "ZIP code is required")]
        public string ZipCode { get; set; } = string.Empty;

        [BindProperty]
        public string DeliveryMethod { get; set; } = "ship";

        [BindProperty]
        public string OrderNotes { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            CartItems = _cartService.GetCart(HttpContext.Session);
            CartTotal = _cartService.GetCartTotal(HttpContext.Session);

            if (!CartItems.Any())
            {
                return RedirectToPage("/GreenBeans");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            CartItems = _cartService.GetCart(HttpContext.Session);
            CartTotal = _cartService.GetCartTotal(HttpContext.Session);

            if (!CartItems.Any())
            {
                return RedirectToPage("/GreenBeans");
            }

            // Construct FullName from FirstName and LastName if provided
            if (string.IsNullOrEmpty(FullName) && !string.IsNullOrEmpty(FirstName))
            {
                FullName = $"{FirstName} {LastName}".Trim();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var orderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}";

            var order = new Order
            {
                OrderNumber = orderNumber,
                FullName = FullName,
                Email = Email,
                Phone = Phone,
                Address = Address,
                City = City,
                State = State,
                ZipCode = ZipCode,
                DeliveryMethod = DeliveryMethod,
                OrderNotes = OrderNotes,
                TotalAmount = CartTotal,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var item in CartItems)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductType = item.ProductType,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                });
            }

            _context.Orders.Add(order);
            _context.SaveChanges();

            // Store order information in TempData for confirmation page
            TempData["OrderNumber"] = orderNumber;
            TempData["CustomerName"] = FullName;
            TempData["CustomerEmail"] = Email;
            TempData["OrderTotal"] = CartTotal.ToString(); // TempData can't reliably store decimal sometimes
            TempData["DeliveryMethod"] = DeliveryMethod;
            TempData["OrderItems"] = System.Text.Json.JsonSerializer.Serialize(CartItems);

            // Clear the cart
            _cartService.ClearCart(HttpContext.Session);

            return RedirectToPage("/OrderConfirmation");
        }
    }
}
