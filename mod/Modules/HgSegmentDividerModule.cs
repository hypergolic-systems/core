namespace Hgs.Mod.Modules;

public class HgSegmentDividerModule : PartModule {

  /**
    * Valid styles are 'decoupler' and 'dockingPort'.
    *
    * 'decoupler' is the default, and indicates that this part is the start of the child segment.
    * 'dockingPort' indicates that this part is the end of the parent segment, and the connected
    * part is in the child segment. Note that the connected part may also have a
    * HgSegmentDividerModule with style='dockingPort'.
    */
  [KSPField]
  public string style = "decoupler";

  public Part OtherSide {
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
