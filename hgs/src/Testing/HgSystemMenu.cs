using System.Collections;
using KSP.UI.Screens;
using UnityEngine;

namespace hgs.system {
  // [KSPAddon (KSPAddon.Startup.MainMenu, true)]
  public class HgSystemMenu : MonoBehaviour {
    
    public static HgSystemMenu instance;
    
    ApplicationLauncherButton menu = null;

    bool showUi = false;

    public HgSystemMenu() {
      UnityEngine.Debug.Log("HgSystemMenu Awake");
      HgSystemMenu.instance = this;
      this.enabled = true;
      DontDestroyOnLoad(this);
    }

    public void Start() {
      StartCoroutine(SetupMenu());
    }

    public IEnumerator SetupMenu() {
      while (!ApplicationLauncher.Ready) {
        yield return null;
      }

      Texture2D icon = new Texture2D(1, 1, UnityEngine.Experimental.Rendering.DefaultFormat.LDR, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
      icon.SetPixel(0, 0, Color.red);
      this.menu = ApplicationLauncher.Instance.AddModApplication(
        OnTrue,
        OnFalse,
        () => UnityEngine.Debug.Log("onHover"),
        () => UnityEngine.Debug.Log("OnHoverOut"),
        () => UnityEngine.Debug.Log("onEnable"),
        () => UnityEngine.Debug.Log("onDisable"),
        ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
        icon);
        UnityEngine.Debug.Log("Added Menu");
        menu.Enable();
    }
    Rect windowRect = new Rect(20, 20, 120, 50);

    public void OnTrue() {
      showUi = true;
    }

    public void OnFalse() {
      showUi = false;
    }

    public void OnGUI() {
      if (!showUi) {
        Debug.Log("!showUi");
        return;
      }
      Debug.Log("ShowUI!");
      windowRect = GUILayout.Window(1234, windowRect, DoMyWindow, "Hypergol");
    }

    public void DoMyWindow(int windowID) {
      GUILayout.TextArea("Hypergol Controller");
      if (GUILayout.Button("Do the thing")) {
        Debug.Log("The Thing");
      }
      GUI.DragWindow();
    }
  }
}