using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestnewTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ILogger<EmployeesController> _logger;

        private readonly EmployeeDbContext dbcontext;

        public EmployeesController(ILogger<EmployeesController> logger, EmployeeDbContext employeeDbContext)
        {
            _logger = logger;

            dbcontext = employeeDbContext;
        }

        [HttpGet]
        public async Task<List<EmployesModel>> Get(bool? isBusy, string department)
        {
            _logger.LogInformation("Request date: " + DateTime.Now.ToString() + " | INFO | Content: isBusy: " + isBusy + " Department: " + department);


            var result = dbcontext.Table_Employee.Join(dbcontext.Table_Department,
                e => e.Department,
                d => d.Id,
                (e, d) => new EmployesModel() { Department = d.Name, isBusy = e.Busy, Id = e.Id });


            if (isBusy != null)
            {
               result = result.Where(x => x.isBusy == isBusy.Value);
            }
            if(department != null)
            {
                result = result.Where(x => x.Department == department);
            }

            _logger.LogInformation("Response date:" + DateTime.Now.ToString() + " | <INFO> | Status code: " + 200 + "| Content: " + result);

            return await result.ToListAsync();
        }

        [HttpGet]
        [Route("isbusy")]
        public async Task<IActionResult> GetIsBusy(Guid Id)
        {
            _logger.LogInformation("Request date: " + DateTime.Now.ToString() + " | INFO | Content: Id: " + Id);

            var entity = await dbcontext.Table_Employee.FirstOrDefaultAsync(x => x.Id == Id);

            if(entity == null)
            {
                _logger.LogError("Response date:" + DateTime.Now.ToString() + " | <ERROR> | Status code: " + 404 + "| Content: " + "сотрудник с id " + Id + " не найден");

                return NotFound("сотрудник с id " + Id + " не найден");
            }
            else
            {
                _logger.LogInformation("Response date:" + DateTime.Now.ToString() + " | <INFO> | Status code: " + 200 + "| Content: " + entity);

                return Ok(entity);
            }
        }

        [HttpPost]
        [Route("assign1")]
        public async Task<IActionResult> assignPost(DateTime startAt, string department)
        {
            _logger.LogInformation("Request date: " + DateTime.Now.ToString() + " | INFO | Content: startAt: " + startAt + " Department: " + department);

            try
            {
                var employee = await dbcontext.Table_Employee.Join(dbcontext.Table_Department,
                    e => e.Department,
                    d => d.Id,
                    (e, d) => new EmployesModel { Department = d.Name, isBusy = e.Busy, Id = e.Id, Timespan = e.Timespan })
                    .Where(x => x.isBusy == false && x.Department == department).ToListAsync();

                if (employee == null)
                {
                    _logger.LogError("Response date:" + DateTime.Now.ToString() + " | <ERROR> | Status code: " + 404 + "| Content: " + "сотрудник с id " + department + " не найден");
                    return NotFound("сотрудник с id " + department + " не найден");
                }

                var timemin = TimeSpan.MaxValue;

                EmployesModel tempEmployee = null;

                foreach (var elem in employee)
                {
                    var time = elem.Timespan.Split('-');
                    var timestart = TimeSpan.Parse(time[0]);
                    var timeend = TimeSpan.Parse(time[1]);
                    var timetask = startAt.TimeOfDay;

                    if (timestart < timetask && timeend > timetask)
                    {
                        if (timemin > timeend)
                        {
                            timemin = timeend;
                            tempEmployee = elem;
                        }
                    }
                }

                if (tempEmployee == null)
                {
                    _logger.LogError("Response date:" + DateTime.Now.ToString() + " | <ERROR> | Status code: " + 404 + "| Content: " + "В выбранное время все сотрудники заняты");
                    return NotFound("В выбранное время все сотрудники заняты");
                }

                var entity = await dbcontext.Table_Employee.FirstOrDefaultAsync(x => x.Id == tempEmployee.Id);

                entity.Busy = true;

                await dbcontext.SaveChangesAsync();

                _logger.LogInformation("Response date:" + DateTime.Now.ToString() + " | <INFO> | Status code: " + 200 + "| Content: ");
                return Ok();
            }
            catch(Exception e)
            {
                _logger.LogError("Response date:" + DateTime.Now.ToString() + " | <ERROR> | Status code: " + 500 + "| Content: " + e.Message);
                return StatusCode(500, new {message = e.Message, stackTrace = e.StackTrace});
            }
        }


        [HttpPost]
        [Route("assign")]
        public async Task<IActionResult> Post(DateTime startAt, Guid Id)
        {
            _logger.LogInformation("Request date: " + DateTime.Now.ToString() + " | INFO | Content: startAt: " + startAt + " Id: " + Id);

            var entity = await dbcontext.Table_Employee.FirstOrDefaultAsync(x => x.Id == Id);

            if(entity == null)
            {
                return NotFound("сотрудник с id " + Id + " не найден");
            }

            var time = entity.Timespan.Split('-');
            var timestart = TimeSpan.Parse(time[0]);
            var timeend = TimeSpan.Parse(time[1]);
            var timetask = startAt.TimeOfDay;

            if(timestart < timetask && timeend > timetask)
            {
                if (entity.Busy == false)
                {
                    entity.Busy = true;
                }
                else
                {
                    _logger.LogError("Response date:" + DateTime.Now.ToString() + " | <ERROR> | Status code: " + 500 + "| Content: " + " В выбранное время сотрудник занят. ");
                    return StatusCode(500, " В выбранное время сотрудник занят. ");
                }
            }

            await dbcontext.SaveChangesAsync();

            _logger.LogInformation("Response date:" + DateTime.Now.ToString() + " | <INFO> | Status code: " + 200 + "| Content: ");
            return Ok();
        }
    }
}
