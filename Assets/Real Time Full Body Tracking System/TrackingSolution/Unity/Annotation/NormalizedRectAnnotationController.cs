

namespace TrackerPro.Unity
{
  public class NormalizedRectAnnotationController : AnnotationController<RectangleAnnotation>
  {
    private NormalizedRect _currentTarget;

    public void DrawNow(NormalizedRect target)
    {
      _currentTarget = target;
      SyncNow();
    }

    public void DrawLater(NormalizedRect target)
    {
      UpdateCurrentTarget(target, ref _currentTarget);
    }

    protected override void SyncNow()
    {
      isStale = false;
      annotation.Draw(_currentTarget);
    }
  }
}
