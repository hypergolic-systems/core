using System;
using System.Collections.Generic;
using UnityEngine;

public class FXGroup {
  public string name;

  public FXGroup(string name) {
    this.name = name;
  }
  public AudioClip sfx;
  public AudioSource audio;

  public List<Light> lights;
  public List<ParticleSystem> fxEmittersNewSystem;

  public void begin(AudioSource sfx) {
    throw new NotImplementedException();}

  public void setActive(bool active) {
    throw new NotImplementedException();
  }
}
