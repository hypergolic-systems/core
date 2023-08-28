using System;
using Hgs.Core.Virtual;
using Hgs.Core.System.Electrical;

namespace Hgs.Core.System.Electrical.Components;

public class Battery : VirtualComponent, PowerComponent {

  public int Stored = 0;
  public int Capacity = 0;



  public Voltage Voltage { get; } = Voltage.Low;
  public int Priority { get; set; } = 0;

  public int Demand { get; set; } = 0;

  public bool IsStorage { get; } = true;

  public PowerComponentType Type => PowerComponentType.Storage;

  public override void Load(object node) {
    base.Load(node);
    Stored = int.Parse(Adapter.ConfigNode_Get(node, "stored"));
    Capacity = int.Parse(Adapter.ConfigNode_Get(node, "capacity"));
  }

  public override void Save(object node) {
    base.Save(node);
    Adapter.ConfigNode_Set(node, "stored", Stored.ToString());
    Adapter.ConfigNode_Set(node, "capacity", Capacity.ToString());
  }

  public void InitializeCapacity(int watts) {
    Stored = 0;
    Capacity = watts;
  }

  public int PowerIn(int power) {
    var charge = Math.Min(Capacity - Stored, power);
    Stored += charge;
    return charge;
  }

  public int PowerOut(int demand) {
    var draw = Math.Min(Stored, demand);
    Stored -= draw;
    return draw;
  }

  public void PowerPrepare(uint seconds) {
    // Recharge if there's power available.
    this.Demand = Capacity - Stored;
  }
  public void PowerFinished(uint seconds) {}
}

