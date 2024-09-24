using System.Linq;

namespace Hgs.Core.Engine;

public class OperatingMode {
  public static float G = 9.80665f;
  public string Name;
  public PropellantRecipe Recipe;
  public FloatCurve IspCurve;
  public float MaxThrust;
  public float MaxMassFlowRate;
  public float MaxVolumetricFlow;

  public static OperatingMode FromConfig(ConfigNode node) {
    var ispCurve = new FloatCurve();
    ispCurve.Load(node.GetNode("ISP"));
    var maxThrust = float.Parse(node.GetValue("maxThrust"));

    var recipe = PropellantRecipe.Get(node.GetValue("recipe"));
    // maxThrust is specified in kN, but mass flow rate must be calcuated using N.
    var maxMassFlowRate = 1000f * maxThrust / (ispCurve.Evaluate(0) * G);
    var maxVolumetricFlow = recipe.Ingredients.Sum(ing => ing.VolumePartInRecipe * maxMassFlowRate);

    return new OperatingMode() {
      Name = node.GetValue("name"),
      Recipe = recipe,
      IspCurve = ispCurve,
      MaxThrust = maxThrust,
      MaxMassFlowRate = maxMassFlowRate,
      MaxVolumetricFlow = maxVolumetricFlow,
    };
  }

  public void SaveToConfig(ConfigNode node) {
    node.AddValue("name", Name);
    node.AddValue("recipe", Recipe.Id);
    node.AddValue("maxThrust", MaxThrust.ToString());
    var ispNode = node.AddNode("ISP");
    IspCurve.Save(ispNode);
  }
}
