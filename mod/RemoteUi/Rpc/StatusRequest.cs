using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Hgs.Core.Virtual;
using Hgs.Game.Components.Electrical;

namespace Hgs.RemoteUi.Rpc;

public class StatusRequest : BaseRequest {
  public StatusRequest(HttpListenerContext context) : base(context) {}

  protected override object Handle() {
    var data = new Hashtable
    {
      { "time", Planetarium.GetUniversalTime() },
      { "scene", HighLogic.LoadedScene.ToString() },
      { "vessels", FlightGlobals.Vessels.Select(serializeVessel).ToArray() }
    };
    return data;
  }

  private Hashtable serializeVessel(Vessel vessel) {
    var vdata = new Hashtable
    {
      { "id", vessel.persistentId.ToString() },
      { "name", vessel.vesselName },
      { "type", vessel.vesselType.ToString() },
      { "loaded", vessel.loaded },
      { "situation", vessel.situation.ToString() },
    };
    if (vessel.parts != null) {
      vdata["parts"] = vessel.parts.Select(serializePart).ToArray();
    }
    if (vessel.rootPart != null) {
      vdata["rootPart"] = vessel.rootPart.persistentId.ToString();
    }
    return vdata;
  }

  private Hashtable serializePart(Part part) {
    var modules = new List<Hashtable>();
    foreach (var module in part.Modules) {
      modules.Add(serializePartModule(module));
    }
    return new Hashtable
    {
      { "id", part.persistentId.ToString() },
      { "name", part.name },
      { "type", part.partInfo.name },
      { "modules" , modules.ToArray() }
    };
  }

  private Hashtable serializePartModule(PartModule module) {
    var data = new Hashtable
    {
      { "name", module.moduleName },
      { "type", module.GetType().Name }
    };
    if (module is IVirtualPartModule pb && pb.VirtualPart != null) {
      data.Add("virtualComponents", pb.VirtualPart.Components.Select(serializeVirtualComponent).ToArray());
    }
    return data;
  }

  private Hashtable serializeVirtualComponent(VirtualComponent cmp) {
    var data = new Hashtable
    {
      { "type", cmp.GetType().Name }
    };
    if (cmp is Battery battery) {
      data.Add("capacity", battery.Capacity);
      data.Add("stored", battery.Amount);
      data.Add("flow", battery.Rate);
    } else if (cmp is RadioisotopeThermalGenerator rtg) {
      data.Add("production", rtg.BaselineProduction);
    }
    return data;
  }
}
