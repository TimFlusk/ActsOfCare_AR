using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ActsOfCare.Data;
using ActsOfCare.Utility;
using Minio.DataModel;
using TNG_Framework.TingTing.Services;
using UnityEngine;
using Object = UnityEngine.Object;
namespace ActsOfCare.Uploading
{
    /// <summary>
    /// Call TriggerUpload() immediately after saving a PNG to disk.
    /// This is the only class the rest of your project needs to know about.
    ///
    /// Attach to the same persistent GameObject as UploadQueue and UploadService,
    /// or call it as a static helper if you prefer.
    ///
    /// Example:
    ///   TextureUtility.SaveToPNG(texture, user.FileName + ".png");
    ///   UploadCoordinator.Instance.TriggerUpload(user);
    /// </summary>
    public class UploadCoordinator : IService
    {
        public UploadBridge UploadBridge { get; private set; }
        
        private const string CONFIGURATION_PATH = "UploadConfiguration";
        private const string POSTER_CACHE_SUBFOLDER = "Posters";
        private const string PORTRAIT_CACHE_SUBFOLDER = "Portraits";

        private UploadConfiguration configuration;
        private UploadQueue queue;
        private bool isInitialised;

        private UserDetails currentUserDetails;
        
        public UploadCoordinator(string path)
        {
            configuration = Resources.Load<UploadConfiguration>(path);
        }

        public UploadCoordinator()
        {
        }

        public void LoadInformation(string path)
        {
            var configurationRequest = Resources.LoadAsync<UploadConfiguration>(path);
            configurationRequest.completed += op =>
            {
                configuration = configurationRequest.asset as UploadConfiguration;
                UploadBridge = new UploadBridge(configuration);
                queue = new UploadQueue(configuration, UploadBridge);
                isInitialised = true;
                Debug.Log("Upload Coordinator Initialized");
            };
        }

        public void SaveScreenshot(Texture2D screenshot)
        {
            var bytes = screenshot.EncodeToPNG();
            currentUserDetails ??= UserDetails.Default();
            var directory = currentUserDetails.SaveLocation;
            FileUtility.EnsurePathExists(directory);
            var fileName = currentUserDetails.FileName;
            if (string.IsNullOrEmpty(fileName))
            {
                currentUserDetails.AssignFileName();
                fileName = currentUserDetails.FileName;
            }
            var path = Path.Combine(directory, fileName);
            File.WriteAllBytes(path, bytes);
        }

        public void SetUserDetails(UserDetails userDetails)
        {
            currentUserDetails = userDetails;
        }
        
        /// <summary>
        /// Called right after the image is written to disk.
        /// Passes the absolute path through from UserDetails.SaveLocation + FileName.
        /// </summary>
        public void TriggerUpload(UserDetails user, PayloadContext context)
        {
            var payload = UserDetailsPayload.From(user);

            Debug.Log($"[UploadCoordinator] Queuing upload for {user.FileName}");
            queue.Enqueue(user.AbsoluteSaveLocation, payload, context);
        }

        public void TriggerUpload(PayloadContext context)
        {
            TriggerUpload(currentUserDetails, context);
        }

        // ── Retrieval API ────────────────────────────────────────────────────

        /// <summary>
        /// Downloads every poster from the server, writes each PNG to
        /// Application.persistentDataPath/Posters, and returns them as Sprites.
        /// </summary>
        public async Task<List<Sprite>> GetAllPosterImages()
        {
            return await GetAllImagesForContext(PayloadContext.Poster);
        }

        /// <summary>
        /// Downloads every portrait from the server, writes each PNG to
        /// Application.persistentDataPath/Portraits, and returns them as Sprites.
        /// </summary>
        public async Task<List<Sprite>> GetAllPortraitImages()
        {
            return await GetAllImagesForContext(PayloadContext.Portrait);
        }

        private async Task<List<Sprite>> GetAllImagesForContext(PayloadContext context)
        {
            var sprites = new List<Sprite>();

            var (success, error, keys) = context == PayloadContext.Poster
                ? await UploadBridge.ListPosterKeysAsync()
                : await UploadBridge.ListPortraitKeysAsync();

            if (!success)
            {
                Debug.LogError($"[UploadCoordinator] Failed to list {context} keys: {error}");
                return sprites;
            }
            Debug.Log($"[UploadCoordinator] Successfully loaded {keys} keys");

            foreach (var key in keys)
            {
                var sprite = await DownloadAndCacheImage(key, context);
                if (sprite != null)
                {
                    sprites.Add(sprite);
                }
            }

            return sprites;
        }

        /// <summary>
        /// Downloads a single image, writes it to the appropriate cache
        /// subfolder under Application.persistentDataPath, and returns a Sprite.
        /// </summary>
        private async Task<Sprite> DownloadAndCacheImage(string key, PayloadContext context)
        {
            // The server route takes the file name without the extension and
            // re-appends ".png" itself, so strip it before requesting.
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(key);

            var (success, error, data) = context == PayloadContext.Poster
                ? await UploadBridge.DownloadPosterAsync(fileNameWithoutExtension)
                : await UploadBridge.DownloadPortraitAsync(fileNameWithoutExtension);

            if (!success || data == null)
            {
                Debug.LogError($"[UploadCoordinator] Failed to download {key}: {error}");
                return null;
            }

            var subfolder = context == PayloadContext.Poster
                ? POSTER_CACHE_SUBFOLDER
                : PORTRAIT_CACHE_SUBFOLDER;
            subfolder = string.Empty;
            var directory = Path.Combine(Application.persistentDataPath, subfolder);
            directory = Application.persistentDataPath;
            FileUtility.EnsurePathExists(directory);

            var path = Path.Combine(directory, key);
            try
            {
                File.WriteAllBytes(path, data);
            }
            catch (Exception e)
            {
                Debug.LogError($"[UploadCoordinator] Failed to write {path}: {e.Message}");
            }

            var texture = new Texture2D(2, 2);
            if (!texture.LoadImage(data))
            {
                Debug.LogError($"[UploadCoordinator] Failed to decode image data for {key}");
                return null;
            }

            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
        }

        public string Id => GetType().Name;
        public bool IsInitialised => isInitialised;
        public Type GetService()
        {
            return GetType();
        }
        public Object GetObject()
        {
            return null;
        }
        public void OnInit()
        {
            // Load Scriptable object, and initialise supporting features
            LoadInformation(CONFIGURATION_PATH);
        }
        
        public void OnRegister()
        {
            Debug.Log("Upload Coordinator Registered");
        }
        
        public void OnDeregister()
        {
            
        }
        
    }
}
