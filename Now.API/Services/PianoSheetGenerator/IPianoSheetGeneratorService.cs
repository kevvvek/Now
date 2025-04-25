using System.Threading.Tasks;

namespace Now.API.Services.PianoSheetGenerator
{
    public interface IPianoSheetGeneratorService
    {
        /// <summary>
        /// Generates piano sheet music from a YouTube video URL
        /// </summary>
        /// <param name="youtubeUrl">The URL of the YouTube video</param>
        /// <returns>Path to the generated sheet music file</returns>
        Task<string> GenerateSheetMusicAsync(string youtubeUrl);

        /// <summary>
        /// Gets the current status of sheet music generation
        /// </summary>
        /// <param name="jobId">The ID of the generation job</param>
        /// <returns>Status of the generation process</returns>
        Task<GenerationStatus> GetGenerationStatusAsync(string jobId);
    }

    public class GenerationStatus
    {
        public string? JobId { get; set; }
        public GenerationState State { get; set; }
        public int Progress { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ResultUrl { get; set; }
    }

    public enum GenerationState
    {
        Queued,
        DownloadingVideo,
        ProcessingAudio,
        GeneratingSheet,
        Completed,
        Failed
    }
} 