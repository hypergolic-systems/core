using System.Collections.Generic;

namespace Hgs.Core.Resources;

public class Resource {
  public string Id;
  public string Name;
  public string Abbreviation;
  public string Description;
  public string Unit;

  // Mass of the resource in kg per unit
  public float Density;

  public static Dictionary<string, Resource> Resources = new();

  public static Resource Get(string id) => Resources[id];

  public static Resource Add(Resource resource) {
    Resources[resource.Id] = resource;
    return resource;
  }

  public static Resource ElectricCharge = Add(new Resource {
    Id = "ElectricCharge",
    Name = "Electric Charge",
    Abbreviation = "EC",
    Description = "Electrical energy",
    Unit = "W",
    Density = 0f,
  });

  public static Resource LiquidFuel = Add(new Resource {
    Id = "LiquidFuel",
    Name = "Liquid Fuel",
    Abbreviation = "LF",
    Description = "A highly refined hydrocarbon fuel",
    Unit = "L",
    Density = 0.9f,
  });

  public static Resource LiquidOxygen = Add(new Resource {
    Id = "LiquidOxygen",
    Name = "Liquid Oxygen",
    Abbreviation = "LOX",
    Description = "Liquid oxygen, stored under pressure",
    Unit = "L",
    Density = 1.1f,
  });

  public static Resource LiquidHydrogen = Add(new Resource {
    Id = "LiquidHydrogen",
    Name = "Liquid Hydrogen",
    Abbreviation = "LH2",
    Description = "Liquid hydrogen, cryogenically stored",
    Unit = "L",
    Density = 0.07f,
  });

  public static Resource Hydrazine = Add(new Resource {
    Id = "Hydrazine",
    Name = "Hydrazine",
    Abbreviation = "N2H4",
    Description = "A toxic monopropellant fuel",
    Unit = "L",
    Density = 1f,
  });

  public static Resource Xenon = Add(new Resource {
    Id = "Xenon",
    Name = "Xenon Gas",
    Abbreviation = "Xe",
    Description = "A noble gas used in ion propulsion",
    Unit = "L",
    // KSP uses a density of around ~1 kg/L for Xenon. However, Dawn used supercritical Xenon at a
    // density of ~2 kg/L, so we use the more realistic value.
    Density = 2f,
  });

  public static Resource SolidFuel = Add(new Resource {
    Id = "SolidFuel",
    Name = "Solid Fuel",
    Abbreviation = "SF",
    Description = "A solid propellant used in boosters",
    Unit = "L",
    // A realistic guess at the density of a solid fuel grain, including any necessary perforations.
    Density = 1.75f,
  });
}
