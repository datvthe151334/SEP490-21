using Microsoft.AspNetCore.Mvc;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;

namespace RecruitXpress_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralTestController : ControllerBase
    {
        private readonly IGeneralTestRepository _repository;

        public GeneralTestController(IGeneralTestRepository repository)
        {
            _repository = repository;
        }



        // GET: api/GeneralTest
        [HttpGet]
        public async Task<IActionResult> GetAllGeneralTests([FromQuery] GeneralTestRequest request)
        {
            try
            {
                var generalTests = await _repository.GetAllGeneralTests(request);
                return Ok(generalTests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/GeneralTest/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGeneralTest(int id)
        {
            var generalTest = await _repository.GetGeneralTestById(id);
            if (generalTest == null)
            {
                return NotFound("Không tìm thấy Id");
            }
            return Ok(generalTest);
        }

        // POST: api/GeneralTest
        [HttpPost]
        public async Task<IActionResult> CreateGeneralTest([FromBody] GeneralTest generalTestDTO)
        {
            await _repository.CreateGeneralTest(generalTestDTO);
            return CreatedAtAction("GetGeneralTest", new { id = generalTestDTO.GeneralTestId }, generalTestDTO);
        }
        [HttpPost("SubmitTest")]
        public async Task<IActionResult> SubmitTest([FromBody] GeneralTest generalTestDTO)
        {
            await _repository.SubmitGeneralTest(generalTestDTO);
            return CreatedAtAction("GetGeneralTest", new { id = generalTestDTO.GeneralTestId }, generalTestDTO);
        }

        // PUT: api/GeneralTest/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGeneralTest(int id, GeneralTest generalTestDTO)
        {
            if (id != generalTestDTO.GeneralTestId)
            {
                return BadRequest("Không được thay đổi Id");
            }

            await _repository.UpdateGeneralTest(id, generalTestDTO);

            return NoContent();
        }

        // DELETE: api/GeneralTest/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGeneralTest(int id)
        {
            var success = await _repository.DeleteGeneralTest(id);
            if (!success)
            {
                return NotFound("Không tìm thấy Id");
            }
            return NoContent();
        }
    }
}
