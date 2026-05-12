

using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro.Unity
{
  public class FaceLandmarkListAnnotationController : AnnotationController<FaceLandmarkListWithIrisAnnotation>
  {
    [SerializeField] private bool _visualizeZ = false;
    [SerializeField] private int _circleVertices = 128;

    private IReadOnlyList<NormalizedLandmark> _currentTarget;

    public void DrawNow(IReadOnlyList<NormalizedLandmark> target)
    {
      _currentTarget = target;
      SyncNow();
    }

    public void DrawNow(NormalizedLandmarkList target)
    {
      DrawNow(target?.Landmark);
    }

    public void DrawLater(IReadOnlyList<NormalizedLandmark> target)
    {
      UpdateCurrentTarget(target, ref _currentTarget);
    }

    public void DrawLater(NormalizedLandmarkList target)
    {
      DrawLater(target?.Landmark);
    }

    protected override void SyncNow()
    {
      isStale = false;
      annotation.Draw(_currentTarget, _visualizeZ, _circleVertices);
    }
  }
}
