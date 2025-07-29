using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels;
using System.Threading.Tasks;

namespace OrderFlow.Controllers
{
    public class OrderController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<HomeController> logger,IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Index() // see all orders
        {
            var orders = await _orderService.All<Order>().Select(order => new IndexOrderViewModel
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

            if(!await _orderService.CreateOrderAsync(createOrderViewModel,this.GetUserId()))
                return View(createOrderViewModel);

            return RedirectToAction(nameof(Index),"Order");
        }

        [HttpGet]
        public IActionResult Edit(int? id) // edit an existing order
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details(int? id) // view order details
        {
            return View();
        }
    }
}
