using System;
using ActsOfCare.Data;
using Newtonsoft.Json;
namespace ActsOfCare.Uploading
{
    /// <summary>
    /// Mirrors the existing UserDetails class for serialization.
    /// Use this when sending to the server — it flattens SaveLocation
    /// which is a computed property on the original class.
    /// </summary>
    [Serializable]
    public class UserDetailsPayload
    {
        [JsonProperty("email")]
        public string Email;

        [JsonProperty("consent")]
        public bool Consent;

        [JsonProperty("saveLocation")]
        public string SaveLocation;

        [JsonProperty("fileName")]
        public string FileName;

        public static UserDetailsPayload From(UserDetails source) => new UserDetailsPayload
        {
            Email = source.Email,
            Consent = source.Consent,
            SaveLocation = source.AbsoluteSaveLocation,
            FileName = source.FileName,
        };
    }
}
