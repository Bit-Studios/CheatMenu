using UnityEngine;
using KSP.Game;
using System.Collections.Generic;
using System.Linq;
using SpaceWarp.API.Mods;
using Screen = UnityEngine.Screen;
using SpaceWarp.API.AssetBundles;
using SpaceWarp.API.Toolbar;
using SpaceWarp.API;
using KSP.UI.Binding;
using Shapes;

namespace CheatMenu
{
    [MainMod]
     public class CheatMenuMod : Mod
    {
        private int windowWidth = 350;
        private int windowHeight = 700;
        private Rect windowRect;
        private static GUIStyle boxStyle;
        private bool showUI = false;
        private GUISkin _spaceWarpUISkin;
        public override void OnInitialized()
        {
            ResourceManager.TryGetAsset($"space_warp/swconsoleui/swconsoleUI/spacewarpConsole.guiskin", out _spaceWarpUISkin);
            SpaceWarpManager.RegisterAppButton(
                "Cheat Menu",
                "BTN-Cheat",
                SpaceWarpManager.LoadIcon(), ToggleButton);
            Logger.Info("Mod is initialized");
        }
        void Awake()
        {

            windowRect = new Rect((Screen.width * 0.85f) - (windowWidth / 2), (Screen.height / 2) - (windowHeight / 2), 0, 0);
        }
        void ToggleButton(bool toggle)
        {
            showUI = toggle;
            GameObject.Find("BTN-Cheat")?.GetComponent<UIValue_WriteBool_Toggle>()?.SetValue(toggle);
        }

        void Update()
        {
            
        }
        void OnGUI()
        {
            GUI.skin = _spaceWarpUISkin;
            if (showUI)
            {
                windowRect = GUILayout.Window(
                    GUIUtility.GetControlID(FocusType.Passive),
                    windowRect,
                    FillWindow,
                    "Cheat Menu",
                    GUILayout.Height(0),
                    GUILayout.Width(500));
            }
        }

        private void FillWindow(int windowID)
        {
            boxStyle = GUI.skin.GetStyle("Box");
            GUILayout.BeginVertical();

            GUILayout.Label($"Active Vessel: {GameManager.Instance.Game.ViewController.GetActiveSimVessel().DisplayName}");

            GUILayout.BeginHorizontal();
            GUILayout.Label($"No crash damage: {GameManager.Instance.Game.CheatSystem.GetNoCrashDamage()}", GUILayout.Width(windowWidth / 2));
            if (GUILayout.Button("Toggle"))
                EasyCheatToggle(1);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Infinite Propellant: {GameManager.Instance.Game.CheatSystem.GetInfinitePropellant()}", GUILayout.Width(windowWidth / 2));
            if (GUILayout.Button("Toggle"))
                EasyCheatToggle(2);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Infinite Electricity: {GameManager.Instance.Game.CheatSystem.GetInfiniteElectricity()}", GUILayout.Width(windowWidth / 2));
            if (GUILayout.Button("Toggle"))
                EasyCheatToggle(3);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Unbreakable Joints: {GameManager.Instance.Game.CheatSystem.GetUnbreakableJoints()}", GUILayout.Width(windowWidth / 2));
            if (GUILayout.Button("Toggle"))
                EasyCheatToggle(4);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Unbreakable Parts: {GameManager.Instance.Game.CheatSystem.GetUnbreakableParts()}", GUILayout.Width(windowWidth / 2));
            if (GUILayout.Button("Toggle"))
                EasyCheatToggle(5);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"No Gravity: {GameManager.Instance.Game.CheatSystem.GetDisableGravity()}", GUILayout.Width(windowWidth / 2));
            if (GUILayout.Button("Toggle"))
                EasyCheatToggle(6);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"No Aerodynamics: {GameManager.Instance.Game.CheatSystem.GetDisableAerodynamics()}", GUILayout.Width(windowWidth / 2));
            if (GUILayout.Button("Toggle"))
                EasyCheatToggle(7);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"No Thermodynamics: {GameManager.Instance.Game.CheatSystem.GetDisableThermodynamics()}", GUILayout.Width(windowWidth / 2));
            if (GUILayout.Button("Toggle"))
                EasyCheatToggle(8);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"Ignore Max Temp: {GameManager.Instance.Game.CheatSystem.GetIgnoreMaxTemp()}", GUILayout.Width(windowWidth / 2));
            if (GUILayout.Button("Toggle"))
                EasyCheatToggle(9);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 10000, 500));
        }
        public void EasyCheatToggle(int cheatID)
        {
            switch(cheatID)
            {
                case 1:
                    GameManager.Instance.Game.CheatSystem.SetNoCrashDamage(!GameManager.Instance.Game.CheatSystem.GetNoCrashDamage());
                    break;
                case 2:
                    GameManager.Instance.Game.CheatSystem.SetInfinitePropellant(!GameManager.Instance.Game.CheatSystem.GetInfinitePropellant());
                    break;
                case 3:
                    GameManager.Instance.Game.CheatSystem.SetInfiniteElectricity(!GameManager.Instance.Game.CheatSystem.GetInfiniteElectricity());
                    break;
                case 4:
                    GameManager.Instance.Game.CheatSystem.SetUnbreakableJoints(!GameManager.Instance.Game.CheatSystem.GetUnbreakableJoints());
                    break;
                case 5:
                    GameManager.Instance.Game.CheatSystem.SetUnbreakableParts(!GameManager.Instance.Game.CheatSystem.GetUnbreakableParts());
                    break;
                case 6:
                    GameManager.Instance.Game.CheatSystem.SetDisableGravity(!GameManager.Instance.Game.CheatSystem.GetDisableGravity());
                    break;
                case 7:
                    GameManager.Instance.Game.CheatSystem.SetDisableAerodynamics(!GameManager.Instance.Game.CheatSystem.GetDisableAerodynamics());
                    break;
                case 8:
                    GameManager.Instance.Game.CheatSystem.SetDisableThermodynamics(!GameManager.Instance.Game.CheatSystem.GetDisableThermodynamics());
                    break;
                case 9:
                    GameManager.Instance.Game.CheatSystem.SetIgnoreMaxTemp(!GameManager.Instance.Game.CheatSystem.GetIgnoreMaxTemp());
                    break;

            }
        }
        public void EasyCheatSet(int cheatID,dynamic value)
        {
            switch (cheatID)
            {
                case 1:
                    GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().PartOwner.Parts.ForEach(part => part.PartResourceContainer.FillAllResourcesToCapacity());
                    break;
                case 2:
                    GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().PartOwner.Parts.ForEach(part => part.PartResourceContainer.GetAllResourcesContainedData().ForEach(Cdata => part.PartResourceContainer.SetResourceStoredUnits(Cdata.ResourceID, value)));
                    break;

            }
        }
    }
}