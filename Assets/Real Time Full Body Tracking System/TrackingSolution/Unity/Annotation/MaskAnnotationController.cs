

namespace TrackerPro.Unity
{
  public class MaskAnnotationController : AnnotationController<MaskAnnotation>
  {
    private int _maskWidth;
    private int _maskHeight;

    private ImageFrame _currentTarget;

    public void InitScreen(int maskWidth, int maskHeight)
    {
      _maskWidth = maskWidth;
      _maskHeight = maskHeight;
      annotation.Init(_maskWidth, _maskHeight);
    }

    public void DrawNow(ImageFrame target)
    {
      _currentTarget = target;
      UpdateMaskArray(_currentTarget);
      SyncNow();
    }

    public void DrawLater(ImageFrame target)
    {
      UpdateCurrentTarget(target, ref _currentTarget);
      UpdateMaskArray(_currentTarget);
    }

    private void UpdateMaskArray(ImageFrame imageFrame)
    {
      if (imageFrame != null)
      {
        annotation.Read(imageFrame);
      }
    }

    protected override void SyncNow()
    {
      isStale = false;
      if (_currentTarget == null)
      {
        annotation.Clear();
      }
      else
      {
        annotation.Draw();
      }
    }
  }
}
