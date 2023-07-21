namespace Hgs.Core;

public class Adapter {
  static IAdapter Instance;
  public static string ConfigNode_Get(object nodeObj, string name) {
    return Adapter.Instance.ConfigNode_Get(nodeObj, name);
  }

  public static void ConfigNode_Set(object nodeObj, string name, string value) {
    Adapter.Instance.ConfigNode_Set(nodeObj, name, value);
  }
}

public interface IAdapter {
  string ConfigNode_Get(object nodeObj, string name);
  void ConfigNode_Set(object nodeObj, string name, string value);
}
