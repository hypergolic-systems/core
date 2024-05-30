using Hgs.Core.Virtual;

public class HgPartEngine : HgVirtualPartModule {


 
  public override void InitializeComponents() {
  }

  public override void OnStart(StartState state) {
    base.OnStart(state);

    // FX Group Setup
  }

  [KSPEvent(guiActive = true, guiName = "Activate")]
   public void Activate() {
   }
}
