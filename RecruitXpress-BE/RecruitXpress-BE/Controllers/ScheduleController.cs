using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly IGoogleService _googleService;
    private readonly IScheduleRepository _scheduleRepository;

    public ScheduleController(IGoogleService googleService, IScheduleRepository scheduleRepository)
    {
        _googleService = googleService;
        _scheduleRepository = scheduleRepository;
    }
    
     //GET: api/Schedule
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetListSchedules() => await _scheduleRepository.GetListSchedules();

        //GET: api/Schedule/{accountId}
        [HttpGet("accountId")]
        public async Task<ActionResult<ScheduleResponse>> GetSchedule(int accountId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var schedule = await _scheduleRepository.GetListSchedules(accountId, startDate, endDate);
                if (schedule == null)
                {
                    return NotFound("Schedule not found!");
                }
        
                return schedule;
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        //POST: api/Schedule
        [HttpPost]
        public async Task<ActionResult<ScheduleDTO>> AddSchedule(ScheduleDTO schedule)
        {
            try
            {
                var result = await _scheduleRepository.AddSchedule(schedule);
                return CreatedAtAction(nameof(AddSchedule), new { id = result.ScheduleId }, result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //PUT: api/Schedule/{id}
        [HttpPut("id")]
        public async Task<IActionResult> UpdateSchedule(int id, ScheduleDTO Schedule)
        {
            try
            {
                var result = await _scheduleRepository.UpdateSchedules(id, Schedule);
                return CreatedAtAction(nameof(UpdateSchedule), new { id = result.ScheduleId }, result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest(e.Message);
            }
        }
        
        //DELETE: api/Schedule/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            try
            {
                var deleted = await _scheduleRepository.DeleteSchedule(id);
                if (deleted)
                {
                    return Ok();
                }
                else
                {
                    return NotFound("Schedule not found!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, e.Message);
            }
        }
    
}