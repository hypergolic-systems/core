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
  public Segment remoteSegment;
  public uint localPartId;
  public uint remotePartId;
}

public class Spacecraft {
  public CompositeSpacecraft vessel;
  public String name;
  public List<Segment> segments = new List<Segment>();
}

public class CompositeSpacecraft {
  public List<Spacecraft> spacecraft = new List<Spacecraft>();
  public Bus highVoltageBus = new Bus(Voltage.High);
  public Bus lowVoltageBus = new Bus(Voltage.Low);

  public object liveVessel = null;

  public static CompositeSpacecraft fromLiveVessel(object liveVessel) {
    var composite = new CompositeSpacecraft();
    composite.liveVessel = liveVessel;
    return composite;
  }

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
}

