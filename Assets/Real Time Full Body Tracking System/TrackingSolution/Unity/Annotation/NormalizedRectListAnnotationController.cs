

using System.Collections.Generic;

namespace TrackerPro.Unity
{
  public class NormalizedRectListAnnotationController : AnnotationController<RectangleListAnnotation>
  {
    private IReadOnlyList<NormalizedRect> _currentTarget;

    public void DrawNow(IReadOnlyList<NormalizedRect> target)
    {
      _currentTarget = target;
      SyncNow();
    }

    public void DrawLater(IReadOnlyList<NormalizedRect> target)
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
