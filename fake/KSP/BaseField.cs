using System;

public class BaseField {
  public string guiName;
  public UI_Control uiControlEditor;
  public UI_Control uiControlFlight;


  public event Action<object> OnValueModified;
}