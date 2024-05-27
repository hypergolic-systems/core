using System;

namespace UnityEngine;

public class Debug {
  public static void Log(string message) {
    Console.WriteLine(message);
  }
}

public abstract class MonoBehaviour {
  public static void DontDestroyOnLoad(Object target) {}
}
