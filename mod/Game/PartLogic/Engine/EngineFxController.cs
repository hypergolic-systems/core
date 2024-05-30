
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EngineFxController {

  private FXGroup disengageFx;
  private FXGroup engageFx;
  private FXGroup flameoutFx;
  private FXGroup powerFx;
  private FXGroup runningFx;
  
  private AudioSource powerSfx;

  private List<FXGroup> flameoutFxPerThrust = new();
  private List<FXGroup> powerFxPerThrust = new();
  private List<FXGroup> runningFxPerThrust = new();

  private EngineFxController() {}

  public static EngineFxController InitializeFor(PartModule partModule, string prefix, string thrustVectorTransformName, Vector3 fxOffset) {
    var part = partModule.part;

    var controller = new EngineFxController() {
      // Start off by initializing the FX groups from the part.
      disengageFx = CloneFx(part, prefix, "disengage"),
      engageFx = CloneFx(part, prefix, "engage"),
      flameoutFx =  CloneFx(part, prefix, "flameout"),
      powerFx =  CloneFx(part, prefix, "power"),
      runningFx =  CloneFx(part, prefix, "running"),
    };

    var index = 0;
    foreach (var transform in part.FindModelTransforms(thrustVectorTransformName)) {
      if (controller.flameoutFx != null) {
        controller.flameoutFxPerThrust.Add(CloneFxGroupPerThrust(part, controller.flameoutFx, controller.flameoutFx, $"{prefix}Flameout{index}", transform, fxOffset));
      }
      if (controller.powerFx != null) {
        controller.powerFxPerThrust.Add(CloneFxGroupPerThrust(part, controller.powerFx, controller.flameoutFx, $"{prefix}Power{index}", transform, fxOffset));
      }
      if (controller.runningFx != null) {
        controller.runningFxPerThrust.Add(CloneFxGroupPerThrust(part, controller.runningFx, controller.flameoutFx, $"{prefix}Running{index}", transform, fxOffset));
      }

      if (controller.powerFx != null) {
        var powerSfx = controller.powerSfx = partModule.gameObject.AddComponent<AudioSource>();
        powerSfx.playOnAwake = false;
        powerSfx.loop = true;
        powerSfx.rolloffMode = AudioRolloffMode.Logarithmic;
        powerSfx.dopplerLevel = 0f;
        powerSfx.volume = GameSettings.SHIP_VOLUME;
        powerSfx.spatialBlend = 1f;
        controller.powerFx.begin(powerSfx);
      }
      index++;
    }

    DestroyOriginalFxGroupInternals(controller.flameoutFx);
    DestroyOriginalFxGroupInternals(controller.powerFx);
    DestroyOriginalFxGroupInternals(controller.runningFx);

    foreach (var group in controller.powerFxPerThrust) {
      group.setActive(false);
    }

    foreach (var group in controller.runningFxPerThrust) {
      group.setActive(false);
    }

    return controller;
  }

  public static FXGroup CloneFx(Part part, string prefix, string name) {
    var group = part.findFxGroup(name);
    if (group == null) {
      return null;
    }

    var clone = CloneFxGroup(group, prefix + name);
  
    clone.sfx = group.sfx;
    clone.audio = group.audio;

    part.fxGroups.Add(clone);
    clone = part.findFxGroup(prefix + group.name);
    clone.setActive(false);

    return group;
  }

  public static FXGroup CloneFxGroupPerThrust(Part part, FXGroup group, FXGroup flameoutGroup, string name, Transform thruster, Vector3 fxOffset) {
    var clone = CloneFxGroup(group, name);
    
    // TODO: Why does this only happen if `flameoutGroup` is not null?
    // This seems like leftover logic that could be cleaned up.
    if (flameoutGroup != null) {
      foreach (var light in group.lights) {
        var position = light.transform.position;
        light.transform.parent = thruster;
        if (!light.gameObject.name.Contains("Keep Pos")) {
          light.transform.localPosition = fxOffset;
        } else {
          light.transform.localPosition = position;
        }
        light.transform.localRotation = Quaternion.identity;
      }

      foreach (var emitter in group.fxEmittersNewSystem) {
        var position = emitter.transform.position;
        emitter.transform.parent = thruster;
        if (!emitter.gameObject.name.Contains("Keep Pos")) {
          emitter.transform.localPosition = fxOffset;
        } else {
          emitter.transform.localPosition = position;
        }
        emitter.transform.localRotation = Quaternion.identity;
        emitter.transform.Rotate(-90f, 0f, 0f);
      }
    }

    part.fxGroups.Add(clone);
    return clone;
  }

  private static FXGroup DestroyOriginalFxGroupInternals(FXGroup group) {
    if (group == null) {
      return null;
    }

    foreach (var light in group.lights) {
      Object.Destroy(light);
    }
    group.lights.Clear();

    foreach (var emitter in group.fxEmittersNewSystem) {
      Object.Destroy(emitter);
    }
    group.fxEmittersNewSystem.Clear();

    return group;
  }

  private static FXGroup CloneFxGroup(FXGroup group, string name) {
    var clone = new FXGroup(name);
    clone.lights.AddRange(group.lights.Select(light => Object.Instantiate(light)));
    clone.fxEmittersNewSystem.AddRange(group.fxEmittersNewSystem.Select(emitter => Object.Instantiate(emitter)));
    return clone;
  }
}
 