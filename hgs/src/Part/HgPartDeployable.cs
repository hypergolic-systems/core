using System;
using KSP;

namespace Hgs.Part {
  public class HgPartDeployable : ModuleDeployablePart {
    public override void OnAwake() {
      base.OnAwake();
      foreach (BaseAction action in this.Actions)
      {
        Console.WriteLine(" Action: {0} {1}", action.name, action.guiName);
      }
    }
  }
}
