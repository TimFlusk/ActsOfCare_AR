

using System.Collections;

namespace TrackerPro.Unity.Sample
{
  public static class AssetLoader
  {
    private static IResourceManager _ResourceManager;

    public static void Provide(IResourceManager manager) => _ResourceManager = manager;

    public static IEnumerator PrepareAssetAsync(string name, string uniqueKey, bool overwrite = false)
    {
      if (_ResourceManager == null)
      {
        throw new System.InvalidOperationException("ResourceManager is not provided");
      }
      return _ResourceManager.PrepareAssetAsync(name, uniqueKey, overwrite);
    }

    public static IEnumerator PrepareAssetAsync(string name, bool overwrite = false) => PrepareAssetAsync(name, name, overwrite);
  }
}
