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
using KSP.Sim.impl;
using KSP.Iteration.UI.Binding;
using KSP.Sim.ResourceSystem;
using Mono.Cecil;
using static KSP.Api.UIDataPropertyStrings.View.Vessel.Stages;

namespace CheatMenu
{
    [MainMod]
     public class CheatMenuMod : Mod
    {
        private int windowWidth = 700;
        private int windowHeight = 700;
        private Rect windowRect;
        private static GUIStyle boxStyle;
        private bool showUI = false;
        private GUISkin _spaceWarpUISkin;
        private static Vector2 partScrollPosition;
        private static Vector2 resourceScrollPosition;
        private static int selectedItem = 0;
        private static PartComponent selectedPart;
        private static ContainedResourceData selectedResource;
        private static double resourceValue = 0;
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
                    GUILayout.Width(700));
            }
        }

        void BasicCheats()
        {
            boxStyle = GUI.skin.GetStyle("Box");
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
        }
        void VesselCheats()
        {
            boxStyle = GUI.skin.GetStyle("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Fill all resources", GUILayout.Width(windowWidth / 2));
            if (GUILayout.Button("Fill All"))
                EasyCheatSet(1, 0);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Fill part resource", GUILayout.Width(windowWidth / 4));
            GUILayout.BeginVertical(boxStyle);
            var parts = GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().PartOwner.Parts.ToList();
            partScrollPosition = GUILayout.BeginScrollView(partScrollPosition, false, true, GUILayout.Height(150));
            foreach (var part in parts)
            {
                if (GUILayout.Button(part.Name))
                {
                    selectedPart = part;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.BeginVertical(boxStyle);
            if (selectedPart == null)
            {
                selectedPart = parts.First();
            }
            var resources = selectedPart.PartResourceContainer.GetAllResourcesContainedData();
            resourceScrollPosition = GUILayout.BeginScrollView(resourceScrollPosition, false, true, GUILayout.Height(150));
            
            foreach (var resource in resources)
            {
                if (GUILayout.Button(resource.ResourceID.Value.ToString()))
                {
                    selectedResource = resource;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            
            if(selectedResource.ResourceID == null)
            {
                selectedResource = resources.First();
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(boxStyle);

            if (GUILayout.Button("get"))
            {
                resourceValue = selectedResource.CapacityUnits;
            }
            resourceValue = double.Parse(GUILayout.TextField($"{resourceValue}"));
            if (GUILayout.Button("Set"))
                EasyCheatSet(2, resourceValue);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }
        private void FillWindow(int windowID)
        {
            
            string[] menuOptions = { "Basic","Vessel" };
            selectedItem = GUILayout.SelectionGrid(selectedItem, menuOptions,2);
            boxStyle = GUI.skin.GetStyle("Box");
            GUILayout.BeginVertical();

            GUILayout.Label($"Active Vessel: {GameManager.Instance.Game.ViewController.GetActiveSimVessel().DisplayName}");

            switch (selectedItem)
            {
                case 0:
                    BasicCheats();
                    break;
                case 1:
                    VesselCheats();
                    break;
                default:
                    BasicCheats();
                    break;
            }

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
                    selectedPart.PartResourceContainer.SetResourceStoredUnits(selectedResource.ResourceID, value);
                    break;

            }
        }
    }
}