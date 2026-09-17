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
        private readonly PayPalService _payPal;
        private readonly CulqiService _culqi;
        private readonly BookingCalendarService _calendar;
        private readonly CoffeeFreshnessService _freshness;

        public CheckoutModel(CartService cartService, Data.TheBestBeanContext context, PayPalService payPal, CulqiService culqi, BookingCalendarService calendar, CoffeeFreshnessService freshness)
        {
            _cartService = cartService;
            _context = context;
            _payPal = payPal;
            _culqi = culqi;
            _calendar = calendar;
            _freshness = freshness;
        }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal CartTotal { get; set; }
        public bool IsWorkshopOnlyCart { get; set; }
        public bool IsPayPalTestCart { get; set; }
        public bool HasExperienceItems { get; set; }
        public bool HasCoffeeItems { get; set; }
        public CoffeeLabBoard LabBoard { get; set; } = new();
        public bool CulqiReady => _culqi.IsConfigured;
        public string CulqiPublicKey => _culqi.PublicKey;
        public string CulqiRsaId => _culqi.RsaId;
        public string CulqiRsaPublicKey => _culqi.RsaPublicKey;
        public int CulqiAmountCentimos => (int)(CulqiTotalSoles * 100m);
        public decimal CulqiTotalSoles => LocalPricing.CulqiSoles(CartTotal);
        public decimal CulqiSurchargeSoles => CulqiTotalSoles - LocalPricing.YapeSoles(CartTotal);
        public decimal VisaSurchargeUsd => LocalPricing.CardSurcharge(CartTotal, "USD");
        public decimal PayPalUsdTotal => LocalPricing.WithCardSurcharge(CartTotal, "USD");
        public decimal PayPalCadSubtotal => LocalPricing.Cad(CartTotal);
        public decimal PayPalCadSurcharge => LocalPricing.CardSurcharge(PayPalCadSubtotal, "CAD");
        public decimal PayPalCadTotal => LocalPricing.WithCardSurcharge(PayPalCadSubtotal, "CAD");

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
        public string? OrderNotes { get; set; }

        [BindProperty]
        public string? CulqiToken { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCartAsync();

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

        public async Task<IActionResult> OnPostAddExtra(string sku)
        {
            await LoadCartAsync();
            if (!HasExperienceItems)
            {
                return RedirectToPage();
            }

            var addOn = ExperienceAddOns.FindBySku(sku);
            if (addOn == null)
            {
                return RedirectToPage();
            }

            _cartService.AddToCart(HttpContext.Session, new CartItem
            {
                ProductId = addOn.Id,
                ProductName = addOn.Name,
                ProductType = ExperienceAddOns.ProductType,
                Price = addOn.Usd,
                Quantity = 1,
                Description = addOn.Description
            });

            return RedirectToPage();
        }

        public IActionResult OnPostRemoveExtra(int productId)
        {
            _cartService.RemoveFromCart(HttpContext.Session, productId, ExperienceAddOns.ProductType);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadCartAsync();

            if (!CartItems.Any())
            {
                return RedirectToPage("/GreenBeans");
            }

            if (string.IsNullOrEmpty(FullName) && !string.IsNullOrEmpty(FirstName))
            {
                FullName = $"{FirstName} {LastName}".Trim();
            }

            ModelState.Remove(nameof(CulqiToken));
            ModelState.Remove(nameof(OrderNotes));
            ModelState.Remove(nameof(DeliveryMethod));
            if (string.IsNullOrWhiteSpace(DeliveryMethod)) DeliveryMethod = "ship";

            if (CartService.SkipsShipping(CartItems))
            {
                if (string.IsNullOrWhiteSpace(Address)) Address = IsPayPalTestCart ? "PayPal $1 test" : "Cusco experience";
                if (string.IsNullOrWhiteSpace(City)) City = "Cusco";
                if (string.IsNullOrWhiteSpace(ZipCode)) ZipCode = "08000";
                ModelState.Remove(nameof(Address));
                ModelState.Remove(nameof(City));
                ModelState.Remove(nameof(ZipCode));
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
            var usePayPalUsd = PayPalService.IsUsd(PaymentMethod);
            var usePayPal = PayPalService.IsPayPalMethod(PaymentMethod) || usePayPalUsd;
            var useCulqi = CulqiService.IsCulqiMethod(PaymentMethod);
            var payment = usePayPalUsd ? "PayPal-USD" : usePayPal ? "PayPal-CAD" : useCulqi ? "Culqi" : "Yape";
            var cardSurcharge = usePayPalUsd ? VisaSurchargeUsd : usePayPal ? PayPalCadSurcharge : useCulqi ? CulqiSurchargeSoles : 0m;

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
                OrderNotes = OrderNotes ?? string.Empty,
                TotalAmount = CartTotal,
                CardSurcharge = cardSurcharge,
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
                    UnitPrice = item.Price,
                    SlotId = item.SlotId
                });
            }

            if (useCulqi)
            {
                if (!_culqi.IsConfigured)
                {
                    ModelState.AddModelError(string.Empty, "Culqi is not connected yet. Use Yape / Plin for now.");
                    return Page();
                }

                var charge = await _culqi.ChargeAsync(CulqiToken, CulqiAmountCentimos, Email, orderNumber);
                if (!charge.Ok)
                {
                    ModelState.AddModelError(string.Empty, charge.Error);
                    return Page();
                }

                order.CulqiChargeId = charge.ChargeId;
                order.PaymentStatus = "Paid";
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await ReserveSlotsAsync();
                return FinishLocalOrder(order);
            }

            if (usePayPal)
            {
                if (!_payPal.IsConfigured)
                {
                    ModelState.AddModelError(string.Empty, "PayPal is not connected yet. Use Yape / Plin for now.");
                    return Page();
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var currency = usePayPalUsd ? "USD" : "CAD";
                var amount = usePayPalUsd ? PayPalUsdTotal : PayPalCadTotal;
                var checkoutUrl = await _payPal.CreateCheckoutUrlAsync(order, amount, currency, baseUrl);
                if (string.IsNullOrWhiteSpace(checkoutUrl))
                {
                    ModelState.AddModelError(string.Empty, $"PayPal did not start. Check that the account can charge {currency}, or use Yape.");
                    return Page();
                }

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                await ReserveSlotsAsync();
                return Redirect(checkoutUrl);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            await ReserveSlotsAsync();
            return FinishLocalOrder(order);
        }

        private async Task ReserveSlotsAsync()
        {
            var holds = CartItems.Where(i => i.SlotId > 0).Select(i => (i.SlotId, i.Quantity)).ToList();
            if (holds.Count > 0)
            {
                await _calendar.ReserveAsync(holds);
            }
        }

        private IActionResult FinishLocalOrder(Order order)
        {
            TempData["OrderNumber"] = order.OrderNumber;
            TempData["CustomerName"] = order.FullName;
            TempData["CustomerEmail"] = order.Email;
            TempData["OrderTotal"] = order.TotalAmount.ToString();
            TempData["DeliveryMethod"] = order.DeliveryMethod;
            TempData["PaymentMethod"] = order.PaymentMethod;
            TempData["OrderItems"] = System.Text.Json.JsonSerializer.Serialize(CartItems);

            _cartService.ClearCart(HttpContext.Session);
            return RedirectToPage("/OrderConfirmation");
        }

        private async Task LoadCartAsync()
        {
            _cartService.ClampExperienceGuests(HttpContext.Session);
            CartItems = _cartService.GetCart(HttpContext.Session);
            CartTotal = _cartService.GetCartTotal(HttpContext.Session);
            IsWorkshopOnlyCart = CartService.IsWorkshopOnlyCart(CartItems);
            IsPayPalTestCart = CartService.IsPayPalTestCart(CartItems);
            HasExperienceItems = CartService.HasExperienceItems(CartItems);
            HasCoffeeItems = CartService.HasCoffeeItems(CartItems);
            if (HasCoffeeItems)
            {
                LabBoard = await _freshness.BoardForCartAsync(CartItems);
            }
        }
    }
}
