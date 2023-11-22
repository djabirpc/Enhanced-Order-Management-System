using Enhanced_Order_Management_System.Data;
using Enhanced_Order_Management_System.Models;
using Enhanced_Order_Management_System.Models.VMs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Enhanced_Order_Management_System.Helper.SD;

namespace Enhanced_Order_Management_System.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var titleUnique = _context.Orders.Any(x => x.Title.ToLower() == model.Title.ToLower());
                    if (titleUnique)
                    {
                        ModelState.AddModelError("ErrorMessages", "Villa already Exist");
                        return BadRequest(ModelState);
                    }

                    Order order = new Order()
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Price = model.Price,
                        Status = Helper.SD.OrderStatus.Pending
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    return CreatedAtRoute("GetOrder", order);
                }
                return BadRequest(model);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.ToString());
            }
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        public async Task<IActionResult> GetAllOrders()
        {

            try
            {
                List<Order> orderList = await _context.Orders.ToListAsync();

                return Ok(orderList);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.ToString());
            }

        }

        [HttpGet("{id:string}", Name = "GetVilla")]
        public async Task<IActionResult> GetByIdOrders(Guid id)
        {

            try
            {

                if (String.IsNullOrEmpty(id.ToString()))
                {

                    return BadRequest();
                }

                Order order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);

                return Ok(order);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.ToString());
            }

        }

        [Authorize]
        [HttpPut("id:string")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, UpdateOrderStatus order)
        {
            try
            {
                if (String.IsNullOrEmpty(id.ToString()))
                {

                    return BadRequest();
                }

                Order orderFromDb = await _context.Orders.FirstOrDefaultAsync(x => x.Id == id);

                if (orderFromDb == null)
                {
                    return NotFound("Order Not Found");
                }

                orderFromDb.Title = order.Title;
                orderFromDb.Description = order.Description;
                orderFromDb.Price = order.Price;

                // Checking the logic of Status
                if (orderFromDb.Status != order.Status)
                {
                    if (!IsOrderStatusValid(orderFromDb.Status, order.Status))
                    {
                        return BadRequest("Invalid Status Transaction");
                    }
                }
                orderFromDb.Status = order.Status;
                _context.Orders.Update(orderFromDb);
                await _context.SaveChangesAsync();

                return Ok(order);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.ToString());
            }
        }




        private bool IsOrderStatusValid(OrderStatus currentStatus, OrderStatus newStatus)
        {
            switch (currentStatus)
            {
                case OrderStatus.Pending:
                    return newStatus == OrderStatus.Processing;
                case OrderStatus.Processing:
                    return newStatus == OrderStatus.Shipped;
                case OrderStatus.Shipped:
                    return newStatus == OrderStatus.Delivered;
                default:
                    return false;
            }
        }


    }
}
