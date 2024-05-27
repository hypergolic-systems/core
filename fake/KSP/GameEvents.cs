public static class GameEvents {
  public static EventData<ConfigNode> onGameStatePostLoad;
  public static EventData<Vessel> onVesselWasModified;
  public static EventData<Vessel> onVesselUnloaded;
}

public class EventData<T> {
  public void Add(OnEvent handler) {}
  public delegate void OnEvent(T data);
}