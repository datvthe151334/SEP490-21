using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using RecruitXpress_BE.Models;
using RecruitXpress_BE.Repositories;

namespace RecruitXpress_BE.Controllers
{

    [Route("api/CvTemplate")]
    [ApiController]
    public class CvtemplateController : ControllerBase
    {
        private readonly ICVTemplateRepository _cvtemplateRepository;
        private readonly RecruitXpressContext _context;

        public CvtemplateController(ICVTemplateRepository cvtemplateRepository, RecruitXpressContext context)
        {
            _cvtemplateRepository = cvtemplateRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CvtemplateDTO>>> GetCvtemplates()
        {
            var cvtemplates = await _cvtemplateRepository.GetAllCvtemplates();
            return Ok(cvtemplates);
        }

        [HttpGet("{cvtemplateId}")]
        public async Task<ActionResult<CvtemplateDTO>> GetCvtemplateById(int cvtemplateId)
        {
            var cvtemplate = await _cvtemplateRepository.GetCvtemplateById(cvtemplateId);

            if (cvtemplate == null)
            {
                return NotFound();
            }

            return Ok(cvtemplate);
        }

        [HttpPost("CreateTemplate1")]
        public async Task<IActionResult> CreateCvtemplate( Cvtemplate cvtemplate)
        {
            try
            {
                var createdCvtemplate = await _cvtemplateRepository.CreateCvtemplate(cvtemplate);
                return CreatedAtAction(nameof(GetCvtemplateById), new { cvtemplateId = createdCvtemplate.CvtemplateId }, createdCvtemplate);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating Cvtemplate: {ex.Message}");
            }
        }
        [HttpPost("CreateTemplate")]
        public async Task<IActionResult> CreateCvtemplate([FromForm] CvTemplateRequest request)
        {
            try
            {

                var cvtemplate = request.CvTemplate;
                var fileData = request.FileData;
                var createdCvtemplate = await _cvtemplateRepository.CreateCvTemplateWithFile(cvtemplate, fileData);
                         return CreatedAtAction(nameof(GetCvtemplateById), new { cvtemplateId = createdCvtemplate.CvtemplateId }, createdCvtemplate);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating Cvtemplate: {ex.Message}");
            }
        }

        [HttpPost("CreateTemplate2")]
        public async Task<IActionResult> CreateCvtemplate([FromForm] int id, IFormFile fromFile)
        {
            try
            {


                var createdCvtemplate = await _cvtemplateRepository.CvTemplateThumbNail(id, fromFile);
                return CreatedAtAction(nameof(GetCvtemplateById), new { cvtemplateId = createdCvtemplate.CvtemplateId }, createdCvtemplate);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating Cvtemplate: {ex.Message}");
            }
        }

        [HttpPut("{cvtemplateId}")]
        public async Task<ActionResult<Cvtemplate>> UpdateCvtemplate(int cvtemplateId, [FromBody] Cvtemplate cvtemplate)
        {
            var updatedCvtemplate = await _cvtemplateRepository.UpdateCvtemplate(cvtemplateId, cvtemplate);

            if (updatedCvtemplate == null)
            {
                return NotFound();
            }

            return Ok(updatedCvtemplate);
        }

        [HttpDelete("{cvtemplateId}")]
        public async Task<ActionResult> DeleteCvtemplate(int cvtemplateId)
        {
            var result = await _cvtemplateRepository.DeleteCvtemplate(cvtemplateId);

            if (result)
            {
                return NoContent();
            }

            return NotFound();
        }

        public class CvTemplateRequest
        {
            public CvRequestClass CvTemplate { get; set; }
            public IFormFile FileData { get; set; }
        }

    }
}

