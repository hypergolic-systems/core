public class VesselModule {
  public Vessel vessel;

  protected virtual void OnAwake() {}

  protected virtual void OnStart() {}


  public virtual void OnLoadVessel() {}
  public virtual void OnUnloadVessel() {}

  protected virtual void OnLoad(ConfigNode node) {}
  protected virtual void OnSave(ConfigNode node) {}

  public virtual Activation GetActivation() {
    return Activation.AllScenes;
  }

  public enum Activation {
    AllScenes,
  }
}
