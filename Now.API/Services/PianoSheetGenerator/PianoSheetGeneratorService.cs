using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Now.API.Services.PianoSheetGenerator
{
    public class PianoSheetGeneratorService : IPianoSheetGeneratorService
    {
        private readonly ILogger<PianoSheetGeneratorService> _logger;
        private readonly YoutubeClient _youtubeClient;
        private readonly string _tempDirectory;

        public PianoSheetGeneratorService(ILogger<PianoSheetGeneratorService> logger)
        {
            _logger = logger;
            _youtubeClient = new YoutubeClient();
            _tempDirectory = Path.Combine(Directory.GetCurrentDirectory(), "TempFiles");
            
            // Create temp directory if it doesn't exist
            if (!Directory.Exists(_tempDirectory))
            {
                Directory.CreateDirectory(_tempDirectory);
            }
        }

        public async Task<string> GenerateSheetMusicAsync(string youtubeUrl)
        {
            try
            {
                _logger.LogInformation($"Starting sheet music generation for URL: {youtubeUrl}");
                
                // Extract video ID from URL
                var videoId = ExtractVideoId(youtubeUrl);
                if (string.IsNullOrEmpty(videoId))
                {
                    throw new ArgumentException("Invalid YouTube URL");
                }

                // 1. Download audio from YouTube
                var video = await _youtubeClient.Videos.GetAsync(youtubeUrl);
                var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(youtubeUrl);
                var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                
                // Create a unique filename for this video
                var audioFilePath = Path.Combine(_tempDirectory, $"{videoId}.{audioStreamInfo.Container}");
                
                // Download the audio file
                _logger.LogInformation($"Downloading audio to: {audioFilePath}");
                await _youtubeClient.Videos.Streams.DownloadAsync(audioStreamInfo, audioFilePath);
                
                // For now, return the path to the downloaded audio file
                return audioFilePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sheet music");
                throw;
            }
        }

        public async Task<GenerationStatus> GetGenerationStatusAsync(string jobId)
        {
            // For now, check if the file exists
            if (File.Exists(jobId))
            {
                return new GenerationStatus
                {
                    JobId = jobId,
                    State = GenerationState.Completed,
                    Progress = 100,
                    ResultUrl = $"/api/PianoSheetGenerator/audio/{Path.GetFileName(jobId)}"
                };
            }

            return new GenerationStatus
            {
                JobId = jobId,
                State = GenerationState.Failed,
                ErrorMessage = "File not found"
            };
        }

        private string ExtractVideoId(string url)
        {
            try
            {
                // Handle different YouTube URL formats
                if (url.Contains("youtu.be/"))
                {
                    return url.Split(new[] { "youtu.be/" }, StringSplitOptions.None)[1].Split('?')[0];
                }
                if (url.Contains("youtube.com/watch?v="))
                {
                    return url.Split(new[] { "v=" }, StringSplitOptions.None)[1].Split('&')[0];
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        public string GetTempDirectory()
        {
            return _tempDirectory;
        }
    }
} 