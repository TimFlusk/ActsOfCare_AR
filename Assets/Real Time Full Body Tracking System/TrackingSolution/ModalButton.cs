

using UnityEngine;

namespace TrackerPro.Unity.Sample.UI
{
  public class ModalButton : MonoBehaviour
  {
    [SerializeField] private GameObject _modalPanel;
    [SerializeField] private GameObject _contents;

    private Modal modal => _modalPanel.GetComponent<Modal>();

    public void Open()
    {
      if (_contents != null)
      {
        modal.Open(_contents);
      }
    }

    public void OpenAndPause()
    {
      if (_contents != null)
      {
        modal.OpenAndPause(_contents);
      }
    }
  }
}
