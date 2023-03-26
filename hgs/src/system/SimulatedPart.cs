using Hgs.Part;

namespace Hgs.System {

  public abstract class SimulatedPart {
    public uint partId;

    public HgSimulatedPartModule simModule;

    public SimulatedPart(uint partId) {
      this.partId = partId;
    }

    public virtual void Save(ConfigNode node) {}

    public virtual void Load(ConfigNode node) {}
  }
}