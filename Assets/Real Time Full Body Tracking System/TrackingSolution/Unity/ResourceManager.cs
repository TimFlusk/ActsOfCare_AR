
using System.Collections;

namespace TrackerPro.Unity
{
  public interface IResourceManager
  {
    /// <summary>
    ///   Saves <paramref name="name" /> as <paramref name="uniqueKey" /> asynchronously.
    /// </summary>
    /// <remarks>
    ///   To make it possible for MediaPipe to read the saved asset, the asset path must be registered
    ///   by calling <see cref="ResourceUtil.AddAssetPath" /> or <see cref="ResourceUtil.SetAssetPath"/> internally.
    /// </remarks>
    /// <param name="overwriteDestination">
    ///   Specifies whether the destination file will be overwritten if it already exists.
    /// </param>
    public IEnumerator PrepareAssetAsync(string name, string uniqueKey, bool overwriteDestination = true);

    public IEnumerator PrepareAssetAsync(string name, bool overwrite = true) => PrepareAssetAsync(name, name, overwrite);
  }
}
