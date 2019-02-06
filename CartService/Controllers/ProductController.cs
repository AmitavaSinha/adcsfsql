using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cart.DAL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IOrderRepository _Repository;

        public ProductController(IOrderRepository Repository)
        {
            _Repository = Repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts(CancellationToken ct = default(CancellationToken))
        {
            try
            {
                return new ObjectResult(await _Repository.GetAllProducts(ct));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

            //List<EmployeeDetails> emp = await _Repository.GetEmployeeAsync(ct);
            //return emp;
        }

        [HttpGet("{searchvalue}")]
        public async Task<IActionResult> GetProductsByKey(string searchvalue, CancellationToken ct = default(CancellationToken))
        {
            try
            {
                return new ObjectResult(await _Repository.GetProductsByKey(searchvalue, ct));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }


    }
}