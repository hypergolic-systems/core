using System;
using Hgs.Core;
using Hgs.Core.Virtual;

namespace Hgs.Core.System.Electrical;

public class Battery : VirtualPart, PowerStorage {
  protected int stored = 0;
  protected int capacity = 0;
  public Battery(uint partId, uint index) : base(partId, index) {}

  public override void Load(object node) {
    base.Load(node);
    stored = int.Parse(Adapter.ConfigNode_Get(node, "stored"));
    capacity = int.Parse(Adapter.ConfigNode_Get(node, "capacity"));
  }

  public override void Save(object node) {
    base.Save(node);
    Adapter.ConfigNode_Set(node, "stored", stored.ToString());
    Adapter.ConfigNode_Set(node, "capacity", capacity.ToString());
  }

  public ProducerKind Kind() {
    return ProducerKind.Storage;
  }

  public Voltage GetVoltage() {
    return Voltage.Low;
  }

  public int GetWattsCapacity() {
    return capacity;
  }

  public int GetWattsStored() {
    return stored;
  }

  public int TryDrawPower(int wattsRequested) {
    var draw = Math.Min(wattsRequested, stored);
    stored -= draw;
    return draw;
  }

  public void OnCalculateProduction(uint seconds, CompositeSpacecraft vessel) {
    // Batteries don't need to calculate their production.
  }

  public void OnRecharge(int watts) {
    stored += watts;
  }

  public void InitializeCapacity(int watts) {
    stored = watts;
    capacity = watts;
  }
}

