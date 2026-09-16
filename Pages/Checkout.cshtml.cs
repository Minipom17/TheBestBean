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
        private readonly MercadoPagoService _mercadoPago;
        private readonly PayPalService _payPal;

        public CheckoutModel(CartService cartService, Data.TheBestBeanContext context, MercadoPagoService mercadoPago, PayPalService payPal)
        {
            _cartService = cartService;
            _context = context;
            _mercadoPago = mercadoPago;
            _payPal = payPal;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal CartTotal { get; set; }
        public bool IsWorkshopOnlyCart { get; set; }

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
        public string PaymentMethod { get; set; } = "Yape";

        [BindProperty]
        public string OrderNotes { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            CartItems = _cartService.GetCart(HttpContext.Session);
            CartTotal = _cartService.GetCartTotal(HttpContext.Session);
            IsWorkshopOnlyCart = CartService.IsWorkshopOnlyCart(CartItems);

            if (!CartItems.Any())
            {
                return RedirectToPage("/GreenBeans");
            }

            if (TempData["MpError"] != null)
            {
                ModelState.AddModelError(string.Empty, TempData["MpError"]?.ToString() ?? "Payment failed. Try Yape or try again.");
            }
            if (string.Equals(Request.Query["paypal"].ToString(), "cancel", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "PayPal was cancelled. Your bag is still here.");
            }

            Ga4Ecommerce.SetPageEvent(ViewData, "begin_checkout", Ga4Ecommerce.Payload(CartItems, CartTotal));
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CartItems = _cartService.GetCart(HttpContext.Session);
            CartTotal = _cartService.GetCartTotal(HttpContext.Session);
            IsWorkshopOnlyCart = CartService.IsWorkshopOnlyCart(CartItems);

            if (!CartItems.Any())
            {
                return RedirectToPage("/GreenBeans");
            }

            // Construct FullName from FirstName and LastName if provided
            if (string.IsNullOrEmpty(FullName) && !string.IsNullOrEmpty(FirstName))
            {
                FullName = $"{FirstName} {LastName}".Trim();
            }

            if (string.IsNullOrWhiteSpace(State))
            {
                State = string.IsNullOrWhiteSpace(City) ? "Cusco" : City;
                ModelState.Remove(nameof(State));
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var orderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}";
            var useMercadoPago = PaymentMethod is "Card" or "MercadoPago";
            var usePayPal = PaymentMethod is "PayPal" or "Paypal";
            var payment = usePayPal ? "PayPal" : useMercadoPago ? "MercadoPago" : "Yape";

            var order = new Order
            {
                OrderNumber = orderNumber,
                FullName = FullName,
                Email = Email,
                Phone = Phone,
                Address = Address,
                City = City,
                State = State ?? string.Empty,
                ZipCode = ZipCode,
                DeliveryMethod = DeliveryMethod,
                PaymentMethod = payment,
                PaymentStatus = "Pending",
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

            if (usePayPal)
            {
                if (!_payPal.IsConfigured)
                {
                    ModelState.AddModelError(string.Empty, "PayPal is not connected yet. Use Yape / Plin for now.");
                    return Page();
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var checkoutUrl = await _payPal.CreateCheckoutUrlAsync(order, LocalPricing.Cad(CartTotal), baseUrl);
                if (string.IsNullOrWhiteSpace(checkoutUrl))
                {
                    ModelState.AddModelError(string.Empty, "PayPal did not start. Check that the account can charge CAD, or use Yape.");
                    return Page();
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return Redirect(checkoutUrl);
            }

            if (useMercadoPago)
            {
                if (!_mercadoPago.IsConfigured)
                {
                    ModelState.AddModelError(string.Empty, "Card payments are not connected yet. Use Yape / Plin, or try again in a bit.");
                    return Page();
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var checkoutUrl = await _mercadoPago.CreateCheckoutUrlAsync(order, LocalPricing.YapeSoles(CartTotal), baseUrl);
                if (string.IsNullOrWhiteSpace(checkoutUrl))
                {
                    ModelState.AddModelError(string.Empty, "Mercado Pago did not start. Use Yape / Plin for now.");
                    return Page();
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                _cartService.ClearCart(HttpContext.Session);
                return Redirect(checkoutUrl);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            TempData["OrderNumber"] = orderNumber;
            TempData["CustomerName"] = FullName;
            TempData["CustomerEmail"] = Email;
            TempData["OrderTotal"] = CartTotal.ToString();
            TempData["DeliveryMethod"] = DeliveryMethod;
            TempData["PaymentMethod"] = payment;
            TempData["OrderItems"] = System.Text.Json.JsonSerializer.Serialize(CartItems);

            _cartService.ClearCart(HttpContext.Session);

            return RedirectToPage("/OrderConfirmation");
        }
    }
}
