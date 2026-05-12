

using System.Collections.Generic;
using UnityEngine;

namespace TrackerPro.Unity
{
  public class PoseWorldLandmarkListAnnotationController : AnnotationController<PoseLandmarkListAnnotation>
  {
    [SerializeField] private float _hipHeightMeter = 0.9f;
    [SerializeField] private Vector3 _scale = new Vector3(100, 100, 100);
    [SerializeField] private bool _visualizeZ = true;

    private IReadOnlyList<Landmark> _currentTarget;

    protected override void Start()
    {
      base.Start();
      transform.localPosition = new Vector3(0, _hipHeightMeter * _scale.y, 0);
    }

    public void DrawNow(IReadOnlyList<Landmark> target)
    {
      _currentTarget = target;
      SyncNow();
    }

    public void DrawNow(LandmarkList target)
    {
      DrawNow(target?.Landmark);
    }

    public void DrawLater(IReadOnlyList<Landmark> target)
    {
      UpdateCurrentTarget(target, ref _currentTarget);
    }

    public void DrawLater(LandmarkList target)
    {
      DrawLater(target?.Landmark);
    }

    protected override void SyncNow()
    {
      isStale = false;
      annotation.Draw(_currentTarget, _scale, _visualizeZ);
    }
  }
}
