using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Now.API.Services.PianoSheetGenerator;
using System.IO;
using Microsoft.AspNetCore.StaticFiles;
using System.ComponentModel.DataAnnotations;

namespace Now.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PianoSheetGeneratorController : ControllerBase
    {
        private readonly IPianoSheetGeneratorService _sheetGeneratorService;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider;

        public PianoSheetGeneratorController(IPianoSheetGeneratorService sheetGeneratorService)
        {
            _sheetGeneratorService = sheetGeneratorService;
            _contentTypeProvider = new FileExtensionContentTypeProvider();
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

        [HttpGet("audio/{filename}")]
        public IActionResult GetAudio(string filename)
        {
            try
            {
                var filePath = Path.Combine((_sheetGeneratorService as PianoSheetGeneratorService)?.GetTempDirectory() ?? "", filename);
                
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound($"Audio file {filename} not found");
                }

                // Determine content type
                if (!_contentTypeProvider.TryGetContentType(filename, out string contentType))
                {
                    contentType = "application/octet-stream";
                }

                // Return the file
                var fileStream = System.IO.File.OpenRead(filePath);
                return File(fileStream, contentType, filename);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("test/{filename}")]
        public IActionResult TestAudio(string filename)
        {
            try
            {
                var filePath = Path.Combine((_sheetGeneratorService as PianoSheetGeneratorService)?.GetTempDirectory() ?? "", filename);
                
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound($"Audio file {filename} not found");
                }

                var fileInfo = new FileInfo(filePath);
                return Ok(new
                {
                    filename = fileInfo.Name,
                    sizeBytes = fileInfo.Length,
                    createdAt = fileInfo.CreationTime,
                    lastModified = fileInfo.LastWriteTime,
                    fullPath = filePath
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class GenerateSheetMusicRequest
    {
        [Required]
        public string YoutubeUrl { get; set; } = string.Empty;
    }
} 