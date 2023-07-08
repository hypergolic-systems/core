namespace Hgs.Virtual {

  public interface SimulatedSystem {
    void PreTick(uint seconds, VirtualVessel vessel);
    void Tick(uint seconds, VirtualVessel vessel);
    void PostTick(uint seconds, VirtualVessel vessel);
  }
}
