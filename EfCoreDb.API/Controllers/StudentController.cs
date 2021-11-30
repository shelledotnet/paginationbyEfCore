using EfCoreDb.Domain.Context;
using EfCoreDb.Domain.Model;
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
        public class StudentController : ControllerBase
        {
            private IApplicationDbContext _context;
            public StudentController(IApplicationDbContext context)
            {
                _context = context;
            }
            [HttpPost]
            public async Task<IActionResult> Create([FromBody]Student student)
            {
            try
            {
                var existingStudent =await _context.Students.Where(a => a.Name.ToLower().Equals(student.Name.ToLower())).FirstOrDefaultAsync();

                if (existingStudent is not null)
                {
                    return Conflict(new { detail = $"Cannot create student {student.Name} it already exists.", code = "55", description = "failed" });
                }
                else
                {

                    var stu = _context.Students.Add(student).Entity;
                    await _context.SaveChanges();
                    // return Ok(student.Id);

                    return CreatedAtRoute("StudentById", new { id = stu.Id }, new { code = "201", description = "success", data = stu });
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
            }
            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
            try
            {

                
                var students = await _context.Students
                                             .OrderByDescending(p=>p.Id)
                                              .Select(p => new
                                              {
                                                  SocialNumber = p.Class,
                                                  FullName = p.Name + "" + p.Section,
                                                  Salary= (decimal)(p.Age + 50.45),
                                                  Bonus = p.Age + ((decimal)0.50 * p.Age),
                                                  p.Age
                                                  
                                              })
                                             .ToListAsync();
                //for a list it will always hav record even if d query is null ..there4 its good practist to check on the count of records
                if (students is null || students.Count <=0) return NotFound(new { code = "99", description = "failed", data = $"no records for student " });
                return  Ok(new { code = "00", description = "success", data = students });
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
            }
            [HttpGet("{id}", Name = "StudentById")]
            public async Task<IActionResult> GetById(int id)
            {
            try
            {
                
                //we use find clause because its a primary key in table student
                var student = await _context.Students.FindAsync(id);   //var student = await _context.Students.Where(a => a.Id == id).FirstOrDefaultAsync();
                if (student == null) return NotFound(new { code = "99", description = "failed", data = $"student with an id: {id} dosent exist " });
                return Ok(new { code = "00", description = "success", data = student });
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
            }
            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(int id)
            {
            //we use find clause because its a primary key in table student
            var student = await _context.Students.FindAsync(id); //await _context.Students.Where(a => a.Id == id).FirstOrDefaultAsync();
            if (student == null) return NotFound(new { code = "99", description = "failed", data = $"student with an id: {id} dosent exist " });
            _context.Students.Remove(student);
                await _context.SaveChanges();
            //return Ok(student.Id);
               return NoContent();
            }
            [HttpPut("{id}")]
            public async Task<IActionResult> Update(int id, Student studentUpdate)
            {
            //we use find clause because its a primary key in table student
            var student =await _context.Students.FindAsync(id);// _context.Students.Where(a => a.Id == id).FirstOrDefault();

                if (student == null) return NotFound(new { code = "99", description = "failed", data = $"student with an id: {id} dosent exist " });
                else
                {
                    student.Age = studentUpdate.Age;
                    student.Name = studentUpdate.Name;
                    await _context.SaveChanges();
                    return Ok(student.Id);
                }
            }
        }
    
}
