
namespace Hgs.Core.Virtual;

public abstract class VirtualPart {
  public uint partId;
  public uint index;

  public VirtualizedModule liveModule;

  public VirtualPart(uint partId, uint index) {
    this.partId = partId;
    this.index = index;
  }

  public virtual void Save(object node) {}

  public virtual void Load(object node) {}
}
