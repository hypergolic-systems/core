namespace Hgs.Virtual {

  public interface ISimulated {
    void PreTick(uint seconds, Vessel vessel);
    void Tick(uint seconds, Vessel vessel);
    void PostTick(uint seconds, Vessel vessel);
  }
}
