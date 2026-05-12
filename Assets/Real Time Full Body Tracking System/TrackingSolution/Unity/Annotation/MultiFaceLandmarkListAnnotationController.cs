

using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro.Unity
{
  public class MultiFaceLandmarkListAnnotationController : AnnotationController<MultiFaceLandmarkListAnnotation>
  {
    [SerializeField] private bool _visualizeZ = false;

    private IReadOnlyList<NormalizedLandmarkList> _currentTarget;

    public void DrawNow(IReadOnlyList<NormalizedLandmarkList> target)
    {
      _currentTarget = target;
      SyncNow();
    }

    public void DrawLater(IReadOnlyList<NormalizedLandmarkList> target)
    {
      UpdateCurrentTarget(target, ref _currentTarget);
    }

    protected override void SyncNow()
    {
      isStale = false;
      annotation.Draw(_currentTarget, _visualizeZ);
    }
  }
}
