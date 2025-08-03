using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Areas.Admin.Controllers
{
    public class OrderController : BaseAdminController
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ILogger<OrderController> logger, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _orderService = orderService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(bool? hideCompleted, string? searchId = null, string? statusFilter = null, string? sortOrder = null)
        {
            var orders = _orderService.GetAll()
                                      .AsNoTracking();

            if (!string.IsNullOrEmpty(searchId))
            {
                orders = orders.Where(o => o.OrderID.ToString().Contains(searchId));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (!Enum.TryParse(statusFilter, out OrderStatus orderStatus))
                {
                    return BadRequest();
                }
                orders = orders.Where(o => o.Status.Equals(orderStatus));
            }

            if (hideCompleted.HasValue && hideCompleted.Value)
            {
                orders = orders.Where(o => o.Status != OrderStatus.Completed);
            }

            switch (sortOrder)
            {
                case "date_desc":
                    orders = orders.OrderByDescending(o => o.OrderDate);
                    break;
                case "date_asc":
                    orders = orders.OrderBy(o => o.OrderDate);
                    break;
                default:
                    orders = orders.OrderBy(o => o.OrderDate);
                    break;
            }

            IEnumerable<IndexOrderViewModel> indexOrders = await orders.Select(order => new IndexOrderViewModel
            {
                OrderID = order.OrderID,
                OrderDate = order.OrderDate,
                DeliveryAddress = order.DeliveryAddress,
                PickupAddress = order.PickupAddress,
                Status = order.Status.ToString(),
                isCanceled = order.isCanceled
            }).ToListAsync();

            return View(indexOrders);
        }

        [HttpGet]
        public IActionResult Create()
        {
            AdminCreateOrderViewModel adminCreateOrderViewModel = new AdminCreateOrderViewModel
            {
                Users = GetUsersInRole(UserRoles.User.ToString()),
            };

            return View(adminCreateOrderViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminCreateOrderViewModel createOrderViewModel)
        {
            if (!ModelState.IsValid)
            {
                createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());

                return View(createOrderViewModel);
            }

            if (!await _orderService.CreateOrderAsync(createOrderViewModel))
            {
                createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());

                return View(createOrderViewModel);
            }

            return RedirectToAction(nameof(Index), "Order");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                return BadRequest("Invalid Order ID format.");
            }

            AdminCreateOrderViewModel? createOrderViewModel = await _orderService.GetAll()
                                                                           .AsNoTracking()
                                                                           .Where(o => o.OrderID.Equals(orderId))
                                                                           .Select(o => new AdminCreateOrderViewModel
                                                                           {
                                                                               UsersId = o.UserID,
                                                                               DeliveryAddress = o.DeliveryAddress,
                                                                               PickupAddress = o.PickupAddress,
                                                                               DeliveryInstructions = o.DeliveryInstructions
                                                                           }).SingleOrDefaultAsync();

            createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());

            if (createOrderViewModel == null)
            {
                return NotFound();
            }

            return View(createOrderViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdminCreateOrderViewModel createOrderViewModel, string? id)
        {
            if (!ModelState.IsValid)
            {
                createOrderViewModel.Users = GetUsersInRole(UserRoles.User.ToString());

                return View(createOrderViewModel);
            }

            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                return BadRequest("Invalid Order ID format.");
            }

            var order = await _orderService.GetAll()
                                           .AsNoTracking()
                                           .Where(o => o.OrderID.Equals(orderId))
                                           .SingleOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            if (!await _orderService.UpdateOrderAsync(createOrderViewModel, orderId))
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Detail), "Order", new { id = id });
        }

        [HttpGet]
        public IActionResult Detail(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            if (!Guid.TryParse(id, out Guid orderId))
            {
                return BadRequest("Invalid Order ID format.");
            }

            if (!Guid.TryParse(this.GetUserId(), out Guid userId))
            {
                return BadRequest("Invalid User ID format.");
            }

            var order = _orderService.GetAll()
                                     .AsNoTracking()
                                     .Include(o => o.User)
                                     .Include(o => o.Payments)
                                     .Include(o => o.OrderTrucks)
                                     .ThenInclude(TO => TO.Truck)
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
                                         Status = o.Status.ToString(),
                                         isCanceled = o.isCanceled,
                                         TrucksLicensePlates = o.OrderTrucks.Select(to => to.Truck!.LicensePlate).ToList(),
                                         Payments = o.Payments.Select(payment => new PaymentViewModel
                                         {
                                             Id = payment.Id,
                                             PaymentDate = payment.PaymentDate,
                                             Amount = payment.Amount,
                                             PaymentDescription = payment.PaymentDescription
                                         }).ToList(),
                                         TotalPrice = o.Payments.ToList().Sum(p => p.Amount)
                                     }).SingleOrDefault();

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                return BadRequest("Invalid Order ID format.");
            }

            var order = await _orderService.GetAll()
                                           .AsNoTracking()
                                           .Where(o => o.OrderID.Equals(orderId))
                                           .SingleOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }
            if (!await _orderService.CancelOrderAsync(orderId, order.UserID))
            {
                return BadRequest("Failed to cancel the order.");
            }

            return RedirectToAction(nameof(Detail), "Order", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> Reactivate(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                return BadRequest("Invalid Order ID format.");
            }

            if (!await _orderService.ReactivateOrderAsync(orderId))
            {
                return BadRequest("Failed to reactivate the order.");
            }

            return RedirectToAction(nameof(Detail), "Order", new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string? id, string? status)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(status))
            {
                return NotFound();
            }

            if (!Guid.TryParse(id, out Guid orderId))
            {
                return BadRequest("Invalid Order ID format.");
            }

            if (!await _orderService.ChangeOrderStatusAsync(orderId, status))
            {
                return BadRequest("Failed to uncancel the order.");
            }

            return RedirectToAction(nameof(Detail), "Order", new { id = id });
        }


        private Dictionary<Guid, string?>? GetUsersInRole(string roleName)
        {
            return _userManager.GetUsersInRoleAsync(roleName)
                               .Result
                               .ToDictionary(
                                       user => user.Id,
                                       user => user.UserName
                                       );
        }

    }
}