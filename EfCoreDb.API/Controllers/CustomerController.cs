using EfCoreDb.Domain.Context;
using EfCoreDb.Domain.Filter;
using EfCoreDb.Domain.Helpers;
using EfCoreDb.Domain.Model;
using EfCoreDb.Domain.Service;
using EfCoreDb.Domain.Wrapperers;
using EfCoreDb.Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfCoreDb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private IApplicationDbContext _context;
        private readonly IUriService _uriService;
        public CustomerController(IApplicationDbContext context, IUriService uriService)
        {
            _context = context;
            _uriService = uriService;
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAlls()
        {
            try
            {


                var cust = await _context.Customers
                                             .OrderByDescending(p => p.Id)
                                             .ToListAsync();
                //for a list it will always hav record even if d query is null ..there4 its good practist to check on the count of records
                if (cust is null || cust.Count <= 0) return NotFound(new { code = "99", description = "failed", data = $"no records for student " });
                return Ok(new { code = "00", description = "success", data = cust });
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            try
            {
                var route = Request.Path.Value;
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var pagedData = await _context.Customers
                                              .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                                              .Take(validFilter.PageSize)
                                              .ToListAsync();
                var totalRecords = await _context.Customers.CountAsync();
                var pagedReponse = PaginationHelper.CreatePagedReponse<Customer>(pagedData, validFilter, totalRecords, _uriService, route);

                return Ok(pagedReponse);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }

        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var customer = await _context.Customers.Where(a => a.Id == id).FirstOrDefaultAsync();
                if (customer is null) return NotFound(new { code = "99", description = "failed", data = $"customer with an id: {id} dosent exist " });
               // return Ok(new { code = "00", description = "success", data = customer });
                return Ok(new PagedResponse<Customer>(customer,1,3));
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
    }
}
