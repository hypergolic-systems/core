using Hgs.Virtual;

namespace Hgs.System.Electrical {
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

  public interface PowerConsumer {
    Voltage GetVoltage();
    void OnCalculateDemand(uint timeSeconds);
    void OnPowerAvailable(Bus bus);

  }
  
  public interface PowerProducer {
    Voltage GetVoltage();

    ProducerKind Kind();

    void OnCalculateProduction(uint timeSeconds, VirtualVessel vessel);

    int TryDrawPower(int watts);
  }

  public interface PowerStorage : PowerProducer {
    // Total number of watts stored.
    int GetWattsStored();

    // Total storage capacity (which can change over time).
    int GetWattsCapacity();

    void OnRecharge(int watts);
  }
}
