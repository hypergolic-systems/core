using System;
using Hgs.Core.Virtual;

public interface IVirtualPartModule {
  VirtualPart VirtualPart { get; set; }
  VirtualVessel VirtualVessel { get; set; }

  // Required lifecycle hooks which must be delegated to the `VirtualPart` mechanism:
  void OnStart(PartModule.StartState state);
  void OnLoad(ConfigNode node);
  void OnSave(ConfigNode node);
  void OnDestroy();

  // The main interface between a `VirtualPart` and its managing `PartModule`:
  void OnSynchronized();
  void InitializeComponents();
}
