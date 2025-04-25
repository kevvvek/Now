using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Now.API.Services.PianoSheetGenerator;

namespace Now.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PianoSheetGeneratorController : ControllerBase
    {
        private readonly IPianoSheetGeneratorService _sheetGeneratorService;

        public PianoSheetGeneratorController(IPianoSheetGeneratorService sheetGeneratorService)
        {
            _sheetGeneratorService = sheetGeneratorService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateSheetMusic([FromBody] GenerateSheetMusicRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _sheetGeneratorService.GenerateSheetMusicAsync(request.YoutubeUrl);
                return Ok(new { jobId = result });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("status/{jobId}")]
        public async Task<IActionResult> GetGenerationStatus(string jobId)
        {
            try
            {
                var status = await _sheetGeneratorService.GetGenerationStatusAsync(jobId);
                return Ok(status);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class GenerateSheetMusicRequest
    {
        public string YoutubeUrl { get; set; }
    }
} 