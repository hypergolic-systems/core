using System;
using System.Collections.Generic;
using Hgs.Core.System.Electrical;

namespace Hgs.Core.Virtual;

public class Segment {
  public Spacecraft craft;
  public List<SegmentLink> links = new List<SegmentLink>();
  public Dictionary<uint, List<VirtualPart>> virtualPartsMap = new Dictionary<uint, List<VirtualPart>>();
}

public class SegmentLink {
  public Segment sideA;
  public Segment sideB;
  public uint sideAPart;
  public uint sideBPart;
}

public class Spacecraft {
  public CompositeSpacecraft composite;
  public String name;
  public List<Segment> segments = new List<Segment>();
}

public class CompositeSpacecraft {
  public List<Spacecraft> spacecraft = new List<Spacecraft>();
  public List<SegmentLink> links = new List<SegmentLink>();
  public Bus highVoltageBus = new Bus(Voltage.High);
  public Bus lowVoltageBus = new Bus(Voltage.Low);

  public object liveVessel = null;

  public CompositeSpacecraft() {
    lowVoltageBus.AddProducer(new LvFromHvLink(highVoltageBus));
  }

  public void Tick(uint seconds) {
    this.highVoltageBus.PreTick(seconds, this);
    this.lowVoltageBus.PreTick(seconds, this);
    this.highVoltageBus.Tick(seconds, this);
    this.lowVoltageBus.Tick(seconds, this);

    // foreach (var parts in virtualPartsMap.Values) {
    //   foreach (var part in parts) {
    //     if (part.liveModule != null) {
    //       part.liveModule.OnSimulationUpdate(seconds);
    //     }
    //   }
    // }
  }

  public void PostTick(uint seconds) {

  }

  /**
   * Load the CompositeSpacecraft from a config node.
   */
  public static CompositeSpacecraft fromConfigNode(object node) {
    var composite = new CompositeSpacecraft();
    foreach (var craft in Adapter.ConfigNode_GetNodes(node, "SPACECRAFT")) {
      var sc = new Spacecraft();
      sc.name = Adapter.ConfigNode_Get(craft, "name");
      sc.composite = composite;
      composite.spacecraft.Add(sc);

      foreach (var segment in Adapter.ConfigNode_GetNodes(craft, "SEGMENT")) {
        var seg = new Segment();
        seg.craft = sc;
        sc.segments.Add(seg);
      }

      foreach (var linkNode in Adapter.ConfigNode_GetNodes(craft, "LINK")) {
        var fromSegment = sc.segments[int.Parse(Adapter.ConfigNode_Get(linkNode, "fromSegment"))];
        var toSegment = sc.segments[int.Parse(Adapter.ConfigNode_Get(linkNode, "toSegment"))];
        var link = new SegmentLink();
        link.sideA = fromSegment;
        link.sideB = toSegment;
        link.sideAPart = uint.Parse(Adapter.ConfigNode_Get(link, "fromPart"));
        link.sideBPart = uint.Parse(Adapter.ConfigNode_Get(link, "toPart"));
      
        fromSegment.links.Add(link);
        toSegment.links.Add(link);
        composite.links.Add(link);
      }
    }

    return composite;
  }

  public object toConfigNode() {
    var node = Adapter.ConfigNode_Create("COMPOSITE_SPACECRAFT");
    foreach (var sc in this.spacecraft) {
      var craftNode = Adapter.ConfigNode_Create("SPACECRAFT");
      Adapter.ConfigNode_Set(craftNode, "name", sc.name);
      foreach (var segment in sc.segments) {
        var segmentNode = Adapter.ConfigNode_Create("SEGMENT");
        Adapter.ConfigNode_Set(craftNode, "name", sc.name);
        Adapter.ConfigNode_AddNode(craftNode, segmentNode);
      }
      foreach (var link in this.links) {
        var linkNode = Adapter.ConfigNode_Create("LINK");
        Adapter.ConfigNode_Set(linkNode, "fromSegment", sc.segments.IndexOf(link.sideA).ToString());
        Adapter.ConfigNode_Set(linkNode, "toSegment", sc.segments.IndexOf(link.sideB).ToString());
        Adapter.ConfigNode_Set(linkNode, "fromPart", link.sideAPart.ToString());
        Adapter.ConfigNode_Set(linkNode, "toPart", link.sideBPart.ToString());
        Adapter.ConfigNode_AddNode(craftNode, linkNode);
      }
      Adapter.ConfigNode_AddNode(node, craftNode);
    }
    return node;
  }
}

