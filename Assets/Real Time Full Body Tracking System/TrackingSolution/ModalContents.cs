

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TrackerPro.Unity.Sample.UI
{
  public class ModalContents : MonoBehaviour
  {
    protected Modal GetModal()
    {
      return gameObject.transform.parent.gameObject.GetComponent<Modal>();
    }

    public virtual void Exit()
    {
      GetModal().Close();
    }

    protected void InitializeDropdown<T>(Dropdown dropdown, string defaultValue) where T : Enum
    {
      dropdown.ClearOptions();

      var options = Enum.GetValues(typeof(T)).Cast<T>().Select(x => x.GetDescription()).ToList();
      dropdown.AddOptions(options);

      var defaultValueIndex = options.FindIndex(option => option == defaultValue);

      if (defaultValueIndex >= 0)
      {
        dropdown.value = defaultValueIndex;
      }
    }
  }
}
