using Cart.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CartService.Controllers
{
    [Route("api/[controller]/[action]")]
    
    [ApiController]
    public class CartController : ControllerBase
    {

        private readonly IOrderRepository _Repository;

        public CartController(IOrderRepository Repository)
        {
            _Repository = Repository;
        }
        
        [HttpGet("{userID}")]        
        public async Task<IActionResult> GetUserOrders(string userID, CancellationToken ct = default(CancellationToken))
        {
            var orderDetails = await _Repository.GetUserOrders(userID, ct);
            if (orderDetails == null)
            {
                return NotFound();
            }
            return Ok(orderDetails);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId, CancellationToken ct = default(CancellationToken))
        {
            var orderDetails = await _Repository.GetOrderDetails(orderId, ct);
            if (orderDetails == null)
            {
                return NotFound();
            }
            return Ok(orderDetails);
        }
               
        //POST api/cart
        [HttpPost]
        public async Task<IActionResult> AddProductToCart([FromBody]UserEntity userEntity, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                if (userEntity.userId == null || userEntity.ProductId < 1)
                    return BadRequest();

                return StatusCode(201, await _Repository.AddProductToCart(userEntity.userId, userEntity.ProductId, ct));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }        

        [HttpDelete]
        public async Task<ActionResult> DeleteProductFromCart([FromBody]UserEntity userEntity, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                if (await _Repository.DeleteProductFromCart(userEntity.userId, userEntity.ProductId, ct))
                {
                    return Ok();
                }

                return StatusCode(500);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmOrder([FromBody]CartOrder cartOrder, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                if (cartOrder.userId == null || cartOrder.shippingAddress == null || cartOrder.paymentMethod == null)
                    return BadRequest();

                return StatusCode(201, await _Repository.ConfirmOrder(cartOrder.userId, cartOrder.shippingAddress, cartOrder.paymentMethod, ct));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }      

    }
}