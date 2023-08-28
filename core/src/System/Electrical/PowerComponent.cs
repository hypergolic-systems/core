using Hgs.Core.Virtual;

namespace Hgs.Core.System.Electrical;

public interface PowerComponent {
  public abstract Voltage Voltage { get; }

  public int Priority { get; }

  public int Demand { get; }

  public PowerComponentType Type { get; }

  public void PowerPrepare(uint seconds);
  public int PowerIn(int power);
  public int PowerOut(int demand);
  public void PowerFinished(uint seconds);
}

public enum PowerComponentType {
  Producer,
  Consumer,
  Storage,
}
