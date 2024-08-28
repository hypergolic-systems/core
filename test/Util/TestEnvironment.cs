using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Hgs.Core.Simulator;
using UnityEngine;

namespace Hgs.Test.Util;

public class TestEnvironment {

  private const int FPS = 10;

  private int FrameCount = 10;

  public HashSet<TestVessel> Vessels = new();
  public HashSet<MonoBehaviour> Behaviours = new();

  public TestEnvironment() {
    SimulationDriver.Initialize();
    SimulationDriver.Instance.Sync(1);
    Planetarium.Test_UniversalTime = 1;
  }

  public void TickFrames(int frames) {
    for (int i = 0; i < frames; i++) {
      FrameCount++;
      Planetarium.Test_UniversalTime = FrameCount / ((double)FPS);
      foreach (var behavior in Behaviours) {
        behavior.Test_InvokeFixedUpdate();
      }
      SimulationDriver.Instance.Sync(FrameCount / FPS);
    }
  }

  public void TickSeconds(int seconds) {
    TickFrames(seconds * FPS);
  }
}