using System.Collections.Generic;
using System.Linq;
using Hgs.Core.Resources;

namespace Hgs.Core.Engine;

public class PropellantRecipe {
  
  public string Id;

  public Ingredient[] Ingredients;

  private static Dictionary<string, PropellantRecipe> recipes = new();

  public static PropellantRecipe InitRecipe(PropellantRecipe recipe) {
    // For ease of configuration, ingredients are specified directly as volumes, which means the
    // total volume is arbitrary. Thus, the total mass of the recipe is also arbitrary, so compute
    // it here.
    var totalRecipeMass = recipe.Ingredients.Sum(i => i.VolumePartInRecipe * i.Resource.Density);

    // Given the total mass proportion, we could now calculate a normalized fraction of each
    // ingredient by mass or volume. But what we really need is to know, given a fixed mass flow
    // rate for the whole propellant system, what the volumetric flow rate of each ingredient will
    // be. To make that easy, we calculate the volumetric flow rate of each ingredient per each 1kg
    // of total propellant mass flow.
    foreach (var ingredient in recipe.Ingredients) {
      ingredient.VolumetricFlowPerUnitMass = ingredient.VolumePartInRecipe / totalRecipeMass;
    }
    
    recipes[recipe.Id] = recipe;
    return recipe;
  }

  public static PropellantRecipe Get(string id) => recipes[id];

  public static PropellantRecipe LF_LOX = InitRecipe(new PropellantRecipe() {
    Id = "LF:LOX",
    Ingredients = [
      new Ingredient() {
        Resource = Resource.LiquidFuel,
        VolumePartInRecipe = 3,
      },
      new Ingredient() {
        Resource = Resource.LiquidOxygen,
        VolumePartInRecipe = 1,
      }
    ],
  });


  public static PropellantRecipe LH2_LOX = InitRecipe(new PropellantRecipe() {
    Id = "LH2:LOX",
    Ingredients = [
      new Ingredient() {
        Resource = Resource.LiquidHydrogen,
        VolumePartInRecipe = 99,
      },
      new Ingredient() {
        Resource = Resource.LiquidOxygen,
        VolumePartInRecipe = 1,
      }
    ],
  });

  public class Ingredient {
    public Resource Resource;
    public float VolumePartInRecipe;

    // Volume of this ingredient per 1kg of total propellant flow.
    //
    // This is used to calculate the volumetric flow rate of this ingredient based on the total mass
    // flow rate.
    public float VolumetricFlowPerUnitMass;
  }
}
