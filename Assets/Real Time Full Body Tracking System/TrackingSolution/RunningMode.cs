

namespace TrackerPro.Unity
{
  [System.Serializable]
  public enum RunningMode
  {
    Async,
    Sync,
  }

  public static class RunningModeExtension
  {
    public static bool IsSynchronous(this RunningMode runningMode)
    {
      return runningMode == RunningMode.Sync;
    }
  }
}
