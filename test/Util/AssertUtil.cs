using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hgs.Test.Util;

  public static class AssertUtil {
    public static void WithinEpsilon(double expected, double actual) {
      Assert.IsTrue(Math.Abs(expected - actual) < 0.0001, $"Expected {expected} but got {actual}.");
    }
  }