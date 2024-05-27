using System.Collections.Generic;

public class ConfigNode {
  public bool HasValue(string name) {
    return false;
  }

  public ConfigNode GetNode(string name) {
    return null;
  }

  public string GetValue(string name) {
    return null;
  }


  public void AddValue(string name, string value) {}

  public void AddValue(string name, bool value) {}

  public void AddValue(string name, double value) {}

  public void SetValue(string name, string value) {}

  public void SetValue(string name, bool value) {}

  public void SetValue(string name, double value) {}

  public void AddNode(ConfigNode node) {}
  public ConfigNode AddNode(string name) {
    return null;
  }

  public IEnumerable<ConfigNode> GetNodes(string name) {
    return [];
  }
}