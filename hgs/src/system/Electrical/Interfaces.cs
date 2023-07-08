namespace Hgs.Virtual.Electrical {
  public enum Voltage {
    Low,
    High,
  }

  public enum ProducerKind {
    Free,
    HighVoltageBridge,
    Fueled,
    Storage,

  }

  public interface IBus {
    Voltage GetVoltage();

    int TryDrawPower(int watts);
  }
  
  public interface IConsumer {
    Voltage GetVoltage();
    void OnCalculateDemand(uint timeSeconds);
    void OnPowerAvailable(IBus bus);

  }
  
  public interface IProducer {
    Voltage GetVoltage();

    void OnCalculateProduction(uint timeSeconds, Vessel vessel);

    int TryDrawPower(int watts);
  }

  public interface IStorage : IProducer {
    // Total number of watts stored.
    int GetWattsStored();

    // Total storage capacity (which can change over time).
    int GetWattsCapacity();

    void OnRecharge(int watts);
  }


}