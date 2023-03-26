namespace Hgs.System {

  public interface ISimulated {
    void PreTick(uint seconds);
    void Tick(uint seconds);
    void PostTick(uint seconds);
  }
}
