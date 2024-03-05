using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hgs.Test {
  public class Util {    
    public static void AssertWithinEpsilon(double expected, double actual) {
      Assert.IsTrue(Math.Abs(expected - actual) < 0.0001, $"Expected {expected} but got {actual}.");
    }
  }
}