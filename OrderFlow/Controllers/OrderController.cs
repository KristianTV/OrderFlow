using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels;
using OrderFlow.ViewModels.Order;
using System.Threading.Tasks;

namespace OrderFlow.Controllers
{
    public class OrderController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<HomeController> logger, IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index() // see all orders
        {
            var orders = await _orderService.All<Order>()
                                            .AsNoTracking()
                                            .Select(order => new IndexOrderViewModel
                                            {
                                                OrderID = order.OrderID,
                                                OrderDate = order.OrderDate,
                                                DeliveryAddress = order.DeliveryAddress,
                                                PickupAddress = order.PickupAddress,
                                                Status = order.Status,
                                                isCanceled = order.isCanceled
                                            }).ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public IActionResult Create() // create a new order
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderViewModel createOrderViewModel) // create a new order
        {
            if (!ModelState.IsValid)
                return View(createOrderViewModel);

            if (!await _orderService.CreateOrderAsync(createOrderViewModel, this.GetUserId()))
                return View(createOrderViewModel);

            return RedirectToAction(nameof(Index), "Order");
        }

        [HttpGet]
        public IActionResult Edit(int? id) // edit an existing order
        {
            return View();
        }

        [HttpGet]
        public IActionResult Detail(string? id) // view order details
        {
            var order = _orderService.All<Order>()
                                     .AsNoTracking()
                                     .Include(o => o.User)
                                     .Include(o => o.Payments)
                                     .Include(o => o.TruckOrder)
                                     .Include(o => o.TruckOrder!.Truck)
                                     .Where(o => o.OrderID.Equals(Guid.Parse(id)))
                                     .Select(o => new DetailsOrderViewModel
                                     {
                                         OrderID = o.OrderID,
                                         UserName = o.User!.UserName!,
                                         OrderDate = o.OrderDate,
                                         DeliveryDate = o.DeliveryDate,
                                         DeliveryAddress = o.DeliveryAddress,
                                         PickupAddress = o.PickupAddress,
                                         DeliveryInstructions = o.DeliveryInstructions,
                                         Status = o.Status,
                                         isCanceled = o.isCanceled,
                                         TruckLicensePlate = o.TruckOrder!.Truck.LicensePlate,
                                         Payments = o.Payments.ToList(),
                                         TotalPrice = o.Payments.ToList().Sum(p => p.Amount)
                                     }).SingleOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(string? id) // cancel an order
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!await _orderService.CancelOrderAsync(Guid.Parse(id), this.GetUserId()))
            {
                return BadRequest("Failed to cancel the order.");
            }

            return RedirectToAction(nameof(Index), "Order");
        }
    }
}
