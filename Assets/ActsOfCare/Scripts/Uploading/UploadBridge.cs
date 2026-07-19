using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
namespace ActsOfCare.Uploading
{
    /// <summary>
    /// Handles HTTP calls to the Rust backend using HttpClient (async/await).
    /// No coroutines — all methods return Task and can be awaited directly.
    /// </summary>
    public class UploadBridge : IDisposable
    {
        private string serverBaseUrl = "http://192.168.0.75:6767";

        private readonly int timeoutSeconds = 30;

        private readonly HttpClient http;

        private const string USER_UPLOAD_ROUTE = "user";
        private const string POSTER_UPLOAD_ROUTE = "upload_poster";
        private const string PORTRAIT_UPLOAD_ROUTE = "upload_portrait";

        private const string POSTER_DOWNLOAD_ROUTE = "poster";
        private const string PORTRAIT_DOWNLOAD_ROUTE = "portrait";
        private const string LIST_ALL_ROUTE = "images/all";

        private const string POSTER_BUCKET_NAME = "acts-of-care-posters";
        private const string PORTRAIT_BUCKET_NAME = "acts-of-care-portraits";

        public UploadBridge(UploadConfiguration configuration)
        {
            serverBaseUrl = configuration.ServerBaseUrl;
            timeoutSeconds = configuration.TimeoutSeconds;
            http = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };
        }

        public void Dispose()
        {
            http?.Dispose();
        }

        // ── Upload API ───────────────────────────────────────────────────────
        public async Task<(bool success, string error)> UploadPortraitAsync(
            string imagePath,
            UserDetailsPayload user,
            CancellationToken ct = default)
        {
            Debug.Log("Sending Portrait");
            return await SendImageWithUserAsync(imagePath, PORTRAIT_UPLOAD_ROUTE,  user, ct);
        }
        
        
        public async Task<(bool success, string error)> UploadPosterAsync(
            string imagePath,
            UserDetailsPayload user,
            CancellationToken ct = default)
        {
            Debug.Log("SENDING POSTER");
            return await SendImageWithUserAsync(imagePath, POSTER_UPLOAD_ROUTE,  user, ct);
        }
        
        /// <summary>
        /// Send a PNG + UserDetails to POST /image.
        /// Returns (success: bool, error: string).
        /// Skips the network call entirely if consent is false.
        /// </summary>
        private async Task<(bool success, string error)> SendImageWithUserAsync(
            string imagePath,
            string route,
            UserDetailsPayload user,
            CancellationToken ct = default)
        {
            
            Debug.Log("SendImageWithUserAsync");
            if (!user.Consent)
            {
                Debug.Log("[UploadService] Consent not given — skipping upload.");
                return (true, null);
            }

            if (!File.Exists(imagePath))
            {
                return (false, $"Image file not found: {imagePath}");
            }
            
            byte[] imageBytes;
            try
            {
                imageBytes = await File.ReadAllBytesAsync(imagePath, ct);
            }
            catch (Exception e)
            {
                return (false, $"Could not read image: {e.Message}");
            }
            
            Debug.Log("Read BYTES");

            try
            {
                using var content = new MultipartFormDataContent();

                var metadataContent = new StringContent(
                    JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                content.Add(metadataContent, "metadata");

                var imageContent = new ByteArrayContent(imageBytes);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                content.Add(imageContent, "image", Path.GetFileName(imagePath));
                Debug.Log("BEFORE SENDING REQUEST");
                serverBaseUrl = "http://192.168.0.75:6767";
                var endpoint = $"{serverBaseUrl}/{route}";
                Debug.Log($"ENDPOINT: {endpoint}");
                var response = await http.PostAsync(endpoint, content, ct);
                Debug.Log("AFTER SENDING REQUEST");
                if (response.IsSuccessStatusCode)
                    return (true, null);
                Debug.Log($"Status Code: {response.StatusCode}");
                string body = await response.Content.ReadAsStringAsync();
                return (false, $"HTTP {(int)response.StatusCode}: {body}");
            }
            catch (TaskCanceledException)
            {
                return (false, "Request timed out.");
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }

        /// <summary>
        /// Send only a UserDetails record to POST /user (no image).
        /// </summary>
        public async Task<(bool success, string error)> SendUserAsync(
            UserDetailsPayload user,
            CancellationToken ct = default)
        {
            try
            {
                var content = new StringContent(
                    JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                var response = await http.PostAsync($"{serverBaseUrl}/{USER_UPLOAD_ROUTE}", content, ct);

                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                
                string body = await response.Content.ReadAsStringAsync();
                return (false, $"HTTP {(int)response.StatusCode}: {body}");
            }
            catch (TaskCanceledException)
            {
                return (false, "Request timed out.");
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
        }

        // ── Download API ─────────────────────────────────────────────────────

        /// <summary>
        /// Downloads raw PNG bytes for a poster by its GUID file name (no extension).
        /// </summary>
        public async Task<(bool success, string error, byte[] data)> DownloadPosterAsync(
            string fileName,
            CancellationToken ct = default)
        {
            return await DownloadImageAsync(POSTER_DOWNLOAD_ROUTE, fileName, ct);
        }

        /// <summary>
        /// Downloads raw PNG bytes for a portrait by its GUID file name (no extension).
        /// </summary>
        public async Task<(bool success, string error, byte[] data)> DownloadPortraitAsync(
            string fileName,
            CancellationToken ct = default)
        {
            return await DownloadImageAsync(PORTRAIT_DOWNLOAD_ROUTE, fileName, ct);
        }

        private async Task<(bool success, string error, byte[] data)> DownloadImageAsync(
            string route,
            string fileName,
            CancellationToken ct)
        {
            try
            {
                var endpoint = $"{serverBaseUrl}/{route}/{fileName}";
                var response = await http.GetAsync(endpoint, ct);

                if (!response.IsSuccessStatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync();
                    return (false, $"HTTP {(int)response.StatusCode}: {body}", null);
                }

                var bytes = await response.Content.ReadAsByteArrayAsync();
                return (true, null, bytes);
            }
            catch (TaskCanceledException)
            {
                return (false, "Request timed out.", null);
            }
            catch (Exception e)
            {
                return (false, e.Message, null);
            }
        }

        /// <summary>
        /// Lists all poster object keys (each key includes the ".png" suffix).
        /// </summary>
        public async Task<(bool success, string error, List<string> keys)> ListPosterKeysAsync(
            CancellationToken ct = default)
        {
            return await ListBucketKeysAsync(POSTER_BUCKET_NAME, ct);
        }

        /// <summary>
        /// Lists all portrait object keys (each key includes the ".png" suffix).
        /// </summary>
        public async Task<(bool success, string error, List<string> keys)> ListPortraitKeysAsync(
            CancellationToken ct = default)
        {
            return await ListBucketKeysAsync(PORTRAIT_BUCKET_NAME, ct);
        }

        private async Task<(bool success, string error, List<string> keys)> ListBucketKeysAsync(
            string bucketName,
            CancellationToken ct)
        {
            try
            {
                var endpoint = $"{serverBaseUrl}/{LIST_ALL_ROUTE}";
                Debug.Log($"Endpoint: {endpoint}");
                var response = await http.GetAsync(endpoint, ct);

                if (!response.IsSuccessStatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync();
                    return (false, $"HTTP {(int)response.StatusCode}: {body}", null);
                }

                string json = await response.Content.ReadAsStringAsync();
                var parsed = JsonConvert.DeserializeObject<AllImagesResponseDto>(json);

                var keys = parsed?.images?
                    .FirstOrDefault(b => b.bucket == bucketName)
                    ?.keys ?? new List<string>();

                return (true, null, keys);
            }
            catch (TaskCanceledException)
            {
                return (false, "Request timed out.", null);
            }
            catch (Exception e)
            {
                return (false, e.Message, null);
            }
        }

        // ── DTOs matching the Rust /images/all response shape ───────────────
        [Serializable]
        private class BucketImagesDto
        {
            public string bucket;
            public List<string> keys;
        }

        [Serializable]
        private class AllImagesResponseDto
        {
            public List<BucketImagesDto> images;
        }
    }
}
