

using Hgs.Core.Resources;
using Hgs.Core.Virtual;
using Hgs.Game.Components.Tankage;

namespace Hgs.Test.Util;

public class TestVessel : VirtualVessel {
  public static uint nextPartId = 1;

  public VirtualPart AddNewPart() {
    return new VirtualPart() {
      id = nextPartId++,
    };
  }

  public Tank AddTestTank(Resource resource, float capacity, bool full = true) {
    var part = AddNewPart();

    var tank = new Tank {
      Resource = resource,
      index = 0,
      part = part,
      Capacity = capacity,
      Amount = full ? capacity : 0,
    };
    part.AddComponent(tank);
    virtualParts.Add(part.id, part);
    tank.OnActivate(this);
    return tank;
  }

  public VirtualComponent AddTestEngine() {
    var part = AddNewPart();

    var engine = new TestEngine();
    part.AddComponent(engine);
    virtualParts.Add(part.id, part);
    engine.OnActivate(this);
    return engine;
  }

  private class TestEngine : VirtualComponent {
    public override void OnActivate(VirtualVessel virtualVessel) {}

    protected override void Load(ConfigNode node) {}

    protected override void Save(ConfigNode node) {
      throw new System.NotImplementedException();
    }
  }
}
