namespace Hgs.Game.Components.Tankage;

public enum TankedSubstance {
  Empty,
  RocketFuel,
  LiquidOxygen,
  HypergolicFuel,
  XenonGas,
}

static class TankedSubstanceMethods {
  public static string GetDisplayName(this TankedSubstance substance) {
    switch (substance) {
      case TankedSubstance.Empty: return "Empty";
      case TankedSubstance.RocketFuel: return "Rocket Fuel";
      case TankedSubstance.LiquidOxygen: return "Liquid Oxygen";
      case TankedSubstance.HypergolicFuel: return "Hypergolic Fuel";
      case TankedSubstance.XenonGas: return "Xenon Gas";
      default: return "Unknown";
    }
  }
}
