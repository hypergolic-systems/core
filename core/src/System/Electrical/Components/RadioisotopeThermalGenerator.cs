using System;
using Hgs.Core.Virtual;

namespace Hgs.Core.System.Electrical.Components;

public class RadioisotopeThermalGenerator : VirtualComponent, PowerComponent {
  public Voltage Voltage => Voltage.Low;

  public int Priority { get; set; } = 1;

  public PowerComponentType Type => PowerComponentType.Producer;

  public int Demand => 0;

  public int WattsPerSecond = 10;
  public int CurrentProduction = 0;

  public bool IsStorage { get; } = false;

  public void PowerPrepare(uint seconds) {
    CurrentProduction = WattsPerSecond * (int)seconds;
  }

  public int PowerIn(int power) {
    return 0;
  }

  public int PowerOut(int demand) {
    var used = Math.Min(CurrentProduction, demand);
    CurrentProduction -= used;
    return used;
  }

  public void PowerFinished(uint seconds) {
  }
}