using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hgs.Core.Engine;
using Hgs.Core.Virtual;
using Hgs.Game.Components.Tankage;

namespace Hgs.Game.PartModules;

public class HgPartTank : HgVirtualPartModule {
  [KSPField]
  public float volume = 0f;

  [KSPField]
  public int maxBulkheads = 0;

  #region UI Editor Fields
  [KSPField(isPersistant = false, guiActive = false, guiActiveEditor = true, guiName = "Free Space", guiUnits = "L")]
  [UI_ProgressBar(affectSymCounterparts = UI_Scene.Editor, controlEnabled = false, minValue = 0f, maxValue = 1f)]
  public float freeSpace = 0f;

  [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Amount", guiUnits = "L", groupName = "resources", groupDisplayName = "Resources")]
  [UI_FloatRange(affectSymCounterparts = UI_Scene.Editor, controlEnabled = true, minValue = 0f, maxValue = 1f, stepIncrement = 1f)]
  public float tank0_amount = 0f;

  [KSPField(isPersistant = false, guiActive = true, guiActiveEditor = true, guiName = "Amount", guiUnits = "L", groupName = "resources", groupDisplayName = "Resources")]
  [UI_FloatRange(affectSymCounterparts = UI_Scene.Editor, controlEnabled = true, minValue = 0f, maxValue = 1f, stepIncrement = 1f)]
  public float tank1_amount = 0f;
  #endregion

  public IEnumerable<Tank> Tanks {
    get => VirtualPart.Components.OfType<Tank>();
  }

  private BaseField[] tankUIs;
  private FieldInfo[] tankFields;

  public override void OnAwake() {
    base.OnAwake();
    tankUIs = (new int[2] {0, 1}).Select(i => Fields[$"tank{i}_amount"]).ToArray();
    tankFields = (new int[2] {0, 1}).Select(i => typeof(HgPartTank).GetField($"tank{i}_amount")).ToArray();
  }

  public override void InitializeComponents() {
    var totalVolume = PropellantRecipe.LF_LOX.Ingredients.Sum(i => i.VolumePartInRecipe);
    foreach (var ingredient in PropellantRecipe.LF_LOX.Ingredients) {
      var amount = volume * ingredient.VolumePartInRecipe / totalVolume;
      VirtualPart.AddComponent(new Tank {
        Amount = amount,
        Volume = amount,
        Resource = ingredient.Resource,
      });
    }

    var tanks = Tanks.ToArray();
    for (var i = 0; i < tankUIs.Length; i++) {
      var tank = tanks[i];
      var bf = tankUIs[i];
      var ui = bf.uiControlEditor as UI_FloatRange;
      var field = tankFields[i];

      ui.minValue = 0f;
      ui.maxValue = tank.Volume;
      field.SetValue(this, tank.Amount);
      bf.guiName = tank.Resource.Name;
      if (IsInEditor) {
        bf.OnValueModified += (value) => {
          tank.Amount = (float) value;
        };
      }
    }
  }
}

