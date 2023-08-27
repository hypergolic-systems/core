namespace Hgs.Core.Virtual;

public interface SimulatedSystem {
  void PreTick(uint seconds, CompositeSpacecraft vessel);
  void Tick(uint seconds, CompositeSpacecraft vessel);
  void PostTick(uint seconds, CompositeSpacecraft vessel);
}

public interface SegmentDivider {

  public object Part { get; }

  /// <summary>
  /// 
  /// </summary>
  public object ForeignPart { get; }
}
