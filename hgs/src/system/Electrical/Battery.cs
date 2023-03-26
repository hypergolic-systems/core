using System;

namespace Hgs.System.Electrical {

  public class Battery : SimulatedPart, IStorage {
    private int stored = 0;
    private int capacity = 0;
    public Battery(uint partId) : base(partId) {}

    public override void Load(ConfigNode node) {
      base.Load(node);
      stored = int.Parse(node.GetValue("stored"));
      capacity = int.Parse(node.GetValue("capacity"));
    }

    public override void Save(ConfigNode node) {
      base.Save(node);
      node.SetValue("stored", stored.ToString(), true);
      node.SetValue("capacity", capacity.ToString(), true);
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

    public void OnCalculateProduction(uint seconds) {
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
}
