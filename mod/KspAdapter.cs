using Hgs.Core;

namespace Hgs.Mod;

public class KspAdapter : IAdapter {
    public string ConfigNode_Get(object nodeObj, string name) {
      return (nodeObj as ConfigNode).GetValue(name);
    }

    public void ConfigNode_Set(object nodeObj, string name, string value) {
      (nodeObj as ConfigNode).SetValue(name, value, true);
    }
}
