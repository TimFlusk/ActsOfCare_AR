

namespace TrackerPro.Unity
{
  public class ConnectionAnnotation : LineAnnotation
  {
    private Connection _currentTarget;

    public bool isEmpty => _currentTarget == null;

    public void Draw(Connection target)
    {
      _currentTarget = target;

      if (ActivateFor(_currentTarget))
      {
        Draw(_currentTarget.start.gameObject, _currentTarget.end.gameObject);
      }
    }

    public void Redraw()
    {
      Draw(_currentTarget);
    }

    protected bool ActivateFor(Connection target)
    {
      if (target == null || !target.start.isActiveInHierarchy || !target.end.isActiveInHierarchy)
      {
        SetActive(false);
        return false;
      }
      SetActive(true);
      return true;
    }
  }
}
