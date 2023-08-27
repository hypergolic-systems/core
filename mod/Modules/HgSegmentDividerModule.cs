namespace Hgs.Mod.Modules;

public class HgSegmentDividerModule : PartModule {


  [KSPField]
  public string style = "decoupler";

  public Part ForeignPart {
    get {
      if (this.style == "decoupler") {
        return this.part.parent;
      } else if (this.style == "dockingPort") {
        if (this.part.children.Count == 0) {
          return null;
        } else if (this.part.children.Count == 1) {
          return this.part.children[0];
        } else {
          throw new System.Exception("Docking port has more than one child?");
        }
      } else {
        throw new System.Exception("Unknown segment divider style: " + this.style);
      }
    }
  }
}
