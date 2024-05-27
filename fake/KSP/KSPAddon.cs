

using System;

[AttributeUsage(AttributeTargets.Class)]
public class KSPAddon : Attribute
{
    public Startup startup;
    public bool once;

    public KSPAddon(Startup startup, bool once) {
      this.startup = startup;
      this.once = once;
    }

    public enum Startup {
      AllGameScenes,
    }
}