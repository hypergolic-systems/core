using Hgs.Part;

namespace Hgs.Virtual {

  public abstract class VirtualPart {
    public uint partId;
    public uint index;

    public IVirtualizedModule liveModule;

    public VirtualPart(uint partId, uint index) {
      this.partId = partId;
      this.index = index;
    }

    public virtual void Save(ConfigNode node) {}

    public virtual void Load(ConfigNode node) {}
  }
}