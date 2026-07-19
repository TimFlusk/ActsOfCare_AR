using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TNG_Framework.TingTing.Services.Services;
using UnityEngine;
namespace ActsOfCare.Uploading
{
    /// <summary>
    /// A single pending upload job.
    /// Serialised to disk so it survives app restarts.
    /// </summary>
    [Serializable]
    public class UploadJob
    {
        public string JobId      = Guid.NewGuid().ToString();
        public string ImagePath;
        public UserDetailsPayload User;
        public int    RetryCount = 0;
        public string CreatedAt  = DateTime.UtcNow.ToString("O");
        public PayloadContext Context;
    }

    /// <summary>
    /// Persists upload jobs to disk and retries them on a background Task loop.
    /// Attach to the same persistent GameObject as UploadService.
    /// </summary>
    public class UploadQueue : IDisposable
    {
        private readonly float retryIntervalSeconds;

        private readonly int maxRetries;

        private const string QUEUE_FILE_NAME = "upload_queue.json";
        private string QueueFilePath => Path.Combine(Application.persistentDataPath, QUEUE_FILE_NAME);

        private List<UploadJob> queue = new();
        private UploadBridge uploadBridge;
        private CancellationTokenSource cancellationTokenSource;

        public UploadQueue(UploadConfiguration configuration, UploadBridge bridge)
        {
            retryIntervalSeconds = configuration.RetryIntervalSeconds;
            maxRetries = configuration.MaxRetries;
            uploadBridge = bridge;
            LoadQueue();
            
            cancellationTokenSource = new CancellationTokenSource();
            _ = RetryLoopAsync(cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
        }

        // ── Public API ───────────────────────────────────────────────────────

        /// <summary>Enqueue a new upload job and immediately attempt it.</summary>
        public void Enqueue(string imagePath, UserDetailsPayload user, PayloadContext context)
        {
            var job = new UploadJob { ImagePath = imagePath, User = user };
            job.Context = context;
            queue.Add(job);
            SaveQueue();
            Debug.Log($"CT cancelled: {cancellationTokenSource.IsCancellationRequested}");
            AttemptJobAsync(job, cancellationTokenSource.Token).ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Debug.LogError($"[UploadQueue] AttemptJobAsync faulted: {t.Exception}");
                }

            });
        }

        // ── Retry loop ───────────────────────────────────────────────────────

        private async Task RetryLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(retryIntervalSeconds), ct)
                    .ContinueWith(_ => { }, TaskContinuationOptions.OnlyOnCanceled); // swallow cancel

                if (ct.IsCancellationRequested) break;

                if (queue.Count > 0)
                {
                    Debug.Log($"[UploadQueue] Retrying {queue.Count} pending job(s).");
                    var snapshot = new List<UploadJob>(queue);
                    foreach (var job in snapshot)
                    {
                        if (ct.IsCancellationRequested) break;
                        await AttemptJobAsync(job, ct);
                    }
                }
            }
        }

        private async Task AttemptJobAsync(UploadJob job, CancellationToken ct)
        {
            job.RetryCount++;
            SaveQueue();
            Debug.Log("JOB CONTEXT:  " + job.Context);
            bool success;
            string error;
            switch (job.Context)
            {
                case PayloadContext.None:
                    success = false;
                    error = "Invalid Context";
                    break;
                case PayloadContext.Poster:
                    Debug.Log("Payload context poster");
                    (success, error) = await uploadBridge.UploadPosterAsync(job.ImagePath, job.User, ct);
                    break;
                case PayloadContext.Portrait:
                    Debug.Log("Payload context portraits");
                    (success, error) = await uploadBridge.UploadPortraitAsync(job.ImagePath, job.User, ct);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Debug.Log($"SUCCESS: {success}, Error: {error}");
            
            // Upload User
            var (userSuccess, userErr) = await UploadUser(job, ct);
            if (!userSuccess)
            {
                Debug.LogError($"Unable to upload user details: {userErr}");
            }
            
            if (success)
            {
                queue.Remove(job);
                SaveQueue();
                Debug.Log($"[UploadQueue] Job {job.JobId} succeeded.");
            }
            else
            {
                Debug.LogWarning($"[UploadQueue] Job {job.JobId} failed (attempt {job.RetryCount}): {error}");

                if (job.RetryCount >= maxRetries)
                {
                    Debug.LogError($"[UploadQueue] Job {job.JobId} exceeded max retries. Dropping.");
                    queue.Remove(job);
                    SaveQueue();
                }
            }
        }
        private async Task<(bool, string Message)> UploadUser(UploadJob job, CancellationToken ct)
        {
            try
            {
                var (userSuccess, userError) = await uploadBridge.SendUserAsync(job.User, ct);
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
            return (true, string.Empty);
        }

        // ── Persistence ──────────────────────────────────────────────────────

        private void LoadQueue()
        {
            if (!File.Exists(QueueFilePath))
            {
                queue = new List<UploadJob>();
                return;
            }

            try
            {
                string json = File.ReadAllText(QueueFilePath);
                queue = JsonConvert.DeserializeObject<List<UploadJob>>(json) ?? new List<UploadJob>();
                Debug.Log($"[UploadQueue] Loaded {queue.Count} pending job(s) from disk.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[UploadQueue] Failed to load queue: {e.Message}");
                queue = new List<UploadJob>();
            }
        }

        private void SaveQueue()
        {
            try
            {
                string json = JsonConvert.SerializeObject(queue, Formatting.Indented);
                File.WriteAllText(QueueFilePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[UploadQueue] Failed to save queue: {e.Message}");
            }
        }
    }

    public enum PayloadContext
    {
        None,
        Poster,
        Portrait
    }
}
