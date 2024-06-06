using System;

public class BaseField {
  public string guiName;
  public UI_Control uiControlEditor;
  public UI_Control uiControlFlight;

  public object GetValue(object host) {
    return null;
  }

  public void SetValue(object host, object value) {
    throw new NotImplementedException();
  }

  public event Action<object> OnValueModified;
}