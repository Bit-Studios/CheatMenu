using SpaceWarp.API.Mods;
using SpaceWarp.API.Assets;
using SpaceWarp;
using BepInEx;
using ShadowUtilityLIB;
using ShadowUtilityLIB.UI;
using UnityEngine;
using System.Collections;
using Logger = ShadowUtilityLIB.logging.Logger;
using UnityEngine.UIElements;
using UitkForKsp2.API;
using KSP.Game;
using DragManipulator = CheatMenu.UI.DragManipulator;
using KSP.Sim.impl;
using KSP.Sim.ResourceSystem;
using Newtonsoft.Json;

namespace CheatMenu
{
    [BepInPlugin("com.shadowdev.cheatmenu", "Cheat Menu", "1.0.0")]
    [BepInDependency(ShadowUtilityLIBMod.ModId, ShadowUtilityLIBMod.ModVersion)]
    [BepInDependency(SpaceWarpPlugin.ModGuid, SpaceWarpPlugin.ModVer)]
    public class CheatMenuMod : BaseSpaceWarpPlugin
    {
        public static string ModId = "com.shadowdev.cheatmenu";
        public static string ModName = "Cheat Menu";
        public static string ModVersion = "1.0.0";

        private static CheatMenuMod Instance { get; set; }
        private Logger logger = new Logger(ModName, ModVersion);
        public static Manager manager;

        private bool showUI = false;
        public static bool IsDev = false;

        public static string SelectedMenu = "vessel";

        private static List<PartComponent> partList = new List<PartComponent>();
        private static int SelectedPartInt = 0;
        private static PartComponent selectedPart;

        private static IEnumerable<ContainedResourceData> resourceList;
        private static int SelectedResourcent = 0;
        private static ContainedResourceData selectedResource;

        private static List<KerbalInfo> KerbalList = new List<KerbalInfo>();
        private static int SelectedKerbalInt = 0;
        private static KerbalInfo selectedKerbal;

        private static int KerbalCheatMode = 0;
        private static bool AeroGUIEnabled = false;
        private static Dictionary<string, string> CustomKerbal = new Dictionary<string, string> {
            {"Fname","" },
            {"Lname","Kermin" }
        };
        private static IGGuid SelectedKerbal;

        private static DragManipulator dragArea = new DragManipulator();
        public override void OnInitialized()
        {
            manager = new Manager();
            GenerateUI();
            manager.Set("CheatMenu", false);
            SetSelectdMenu("vessel");
            AppBar.Add(AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/Flightbutton.png"), AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/OABbutton.png"), "Cheat Menu", "BTN-Cheat", ToggleButton, ToggleButton, new bool[] { true, true });
            ShadowUtilityLIBMod.RunCr(UIupdate());
            logger.Log("Initilised");

        }
        void Awake()
        {
            
            if (IsDev)
            {
                logger.Log($"{ModName} dev mode");
                ShadowUtilityLIBMod.EnableDebugMode();
            }
        }
        void ToggleButton(bool toggle)
        {
            showUI = !showUI;
            manager.Set("CheatMenu", showUI);
            logger.Debug($"Button Toggle: {showUI}");
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F12))
            {
                if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                {
                    showUI = !showUI;
                    manager.Set("CheatMenu", showUI);
                    logger.Debug($"CheatMenu Toggle: {showUI}");
                }
                else
                {
                    AeroGUIEnabled = !AeroGUIEnabled;
                    GameManager.Instance.Game.AeroGUI.IsVisible = AeroGUIEnabled;
                }
            }
        }
        IEnumerator UIupdate()
        {
            while (true)
            {
                yield return new WaitForSeconds(5);
                if(SelectedMenu == "vessel")
                {
                    SetSelectdMenu(SelectedMenu);
                }
                
            }
        }
        void SetSelectdMenu(string in_SelectionChoice)
        {
            SelectedMenu = in_SelectionChoice;
            switch (in_SelectionChoice)
            {
                case "vessel":
                    GenerateUIvessel();
                    break;
                case "resource":
                    GenerateUIresource();
                    break;
                case "kerbal":
                    GenerateUIkerbal();
                    break;
            }
        }
        void GenerateUIvessel()
        {
            int fontsizeLabel = 8;
            int fontsizeButton = 8;
            int buttonHeight = 9;
            int gapSize = 1;
            int borderRadius = 4;

            VisualElement CheatMenuLabelsList = manager.Get("CheatMenu").rootVisualElement.Q<VisualElement>("CheatMenuLabelsList");
            VisualElement CheatMenuOptionsList = manager.Get("CheatMenu").rootVisualElement.Q<VisualElement>("CheatMenuOptionsList");
            CheatMenuLabelsList.Clear();
            CheatMenuOptionsList.Clear();

            Button GenerateButton(string name = "", string text = "") { 
            
                Button GeneratedButton = Element.Button(name, text);
                GeneratedButton.style.paddingTop = gapSize;
                GeneratedButton.style.paddingBottom = gapSize;
                GeneratedButton.style.height = buttonHeight;
                GeneratedButton.style.fontSize = fontsizeButton;
                GeneratedButton.style.marginTop = gapSize + 3.9f;
                GeneratedButton.style.borderBottomLeftRadius = borderRadius;
                GeneratedButton.style.borderBottomRightRadius = borderRadius;
                GeneratedButton.style.borderTopLeftRadius = borderRadius;
                GeneratedButton.style.borderTopRightRadius = borderRadius;
                return GeneratedButton;
            }
            Label GenerateLabel(string name = "", string text = "")
            {

                Label GeneratedLabel = Element.Label(name, text);
                GeneratedLabel.style.fontSize = fontsizeLabel;
                GeneratedLabel.style.marginTop = gapSize;
                GeneratedLabel.style.paddingTop = gapSize;
                GeneratedLabel.style.paddingBottom = gapSize;
                return GeneratedLabel;
            }

            Label CheatMenuVesselLabels_NoCrashDamage_Label = GenerateLabel("GetNoCrashDamage", "No Crash Damage");
            CheatMenuLabelsList.Add(CheatMenuVesselLabels_NoCrashDamage_Label);

            Button CheatMenuVesselOptions_NoCrashDamage_Button = GenerateButton("GetNoCrashDamageButton", "Toggle");
            if (GameManager.Instance.Game.CheatSystem.GetNoCrashDamage())
            {
                CheatMenuVesselOptions_NoCrashDamage_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
                
            }
            else
            {
                CheatMenuVesselOptions_NoCrashDamage_Button.style.backgroundColor = Color.red;
            }
            CheatMenuVesselOptions_NoCrashDamage_Button.clickable = new Clickable(() => {
                EasyCheatToggle(1);
                if (GameManager.Instance.Game.CheatSystem.GetNoCrashDamage())
                {
                    CheatMenuVesselOptions_NoCrashDamage_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
                }
                else
                {
                    CheatMenuVesselOptions_NoCrashDamage_Button.style.backgroundColor = Color.red;
                }
            });
            CheatMenuOptionsList.Add(CheatMenuVesselOptions_NoCrashDamage_Button);


            Label CheatMenuVesselLabels_InfinitePropellant_Label = GenerateLabel("GetInfinitePropellant", "Infinite Propellant");
            CheatMenuLabelsList.Add(CheatMenuVesselLabels_InfinitePropellant_Label);

            Button CheatMenuVesselOptions_InfinitePropellant_Button = GenerateButton("GetInfinitePropellantButton", "Toggle");
            if (GameManager.Instance.Game.CheatSystem.GetInfinitePropellant())
            {
                CheatMenuVesselOptions_InfinitePropellant_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
            }
            else
            {
                CheatMenuVesselOptions_InfinitePropellant_Button.style.backgroundColor = Color.red;
            }
            CheatMenuVesselOptions_InfinitePropellant_Button.clickable = new Clickable(() => {
                EasyCheatToggle(2);
                if (GameManager.Instance.Game.CheatSystem.GetInfinitePropellant())
                {
                    CheatMenuVesselOptions_InfinitePropellant_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
                }
                else
                {
                    CheatMenuVesselOptions_InfinitePropellant_Button.style.backgroundColor = Color.red;
                }
            });
            CheatMenuOptionsList.Add(CheatMenuVesselOptions_InfinitePropellant_Button);


            Label CheatMenuVesselLabels_InfiniteElectricity_Label = GenerateLabel("GetInfiniteElectricity", "Infinite Electricity");
            CheatMenuLabelsList.Add(CheatMenuVesselLabels_InfiniteElectricity_Label);

            Button CheatMenuVesselOptions_InfiniteElectricity_Button = GenerateButton("GetInfiniteElectricityButton", "Toggle");
            if (GameManager.Instance.Game.CheatSystem.GetInfiniteElectricity())
            {
                CheatMenuVesselOptions_InfiniteElectricity_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
            }
            else
            {
                CheatMenuVesselOptions_InfiniteElectricity_Button.style.backgroundColor = Color.red;
            }
            CheatMenuVesselOptions_InfiniteElectricity_Button.clickable = new Clickable(() => {
                EasyCheatToggle(3);
                if (GameManager.Instance.Game.CheatSystem.GetInfiniteElectricity())
                {
                    CheatMenuVesselOptions_InfiniteElectricity_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
                }
                else
                {
                    CheatMenuVesselOptions_InfiniteElectricity_Button.style.backgroundColor = Color.red;
                }
            });
            CheatMenuOptionsList.Add(CheatMenuVesselOptions_InfiniteElectricity_Button);


            Label CheatMenuVesselLabels_UnbreakableJoints_Label = GenerateLabel("GetUnbreakableJoints", "Unbreakable Joints");
            CheatMenuLabelsList.Add(CheatMenuVesselLabels_UnbreakableJoints_Label);

            Button CheatMenuVesselOptions_UnbreakableJoints_Button = GenerateButton("GetUnbreakableJointsButton", "Toggle");
            if (GameManager.Instance.Game.CheatSystem.GetUnbreakableJoints())
            {
                CheatMenuVesselOptions_UnbreakableJoints_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
            }
            else
            {
                CheatMenuVesselOptions_UnbreakableJoints_Button.style.backgroundColor = Color.red;
            }
            CheatMenuVesselOptions_UnbreakableJoints_Button.clickable = new Clickable(() => {
                EasyCheatToggle(4);
                if (GameManager.Instance.Game.CheatSystem.GetUnbreakableJoints())
                {
                    CheatMenuVesselOptions_UnbreakableJoints_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
                }
                else
                {
                    CheatMenuVesselOptions_UnbreakableJoints_Button.style.backgroundColor = Color.red;
                }
            });
            CheatMenuOptionsList.Add(CheatMenuVesselOptions_UnbreakableJoints_Button);


            Label CheatMenuVesselLabels_UnbreakableParts_Label = GenerateLabel("GetUnbreakableParts", "Unbreakable Parts");
            CheatMenuLabelsList.Add(CheatMenuVesselLabels_UnbreakableParts_Label);

            Button CheatMenuVesselOptions_UnbreakableParts_Button = GenerateButton("GetUnbreakablePartsButton", "Toggle");
            if (GameManager.Instance.Game.CheatSystem.GetUnbreakableParts())
            {
                CheatMenuVesselOptions_UnbreakableParts_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
            }
            else
            {
                CheatMenuVesselOptions_UnbreakableParts_Button.style.backgroundColor = Color.red;
            }
            CheatMenuVesselOptions_UnbreakableParts_Button.clickable = new Clickable(() => {
                EasyCheatToggle(5);
                if (GameManager.Instance.Game.CheatSystem.GetUnbreakableParts())
                {
                    CheatMenuVesselOptions_UnbreakableParts_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
                }
                else
                {
                    CheatMenuVesselOptions_UnbreakableParts_Button.style.backgroundColor = Color.red;
                }
            });
            CheatMenuOptionsList.Add(CheatMenuVesselOptions_UnbreakableParts_Button);


            Label CheatMenuVesselLabels_DisableGravity_Label = GenerateLabel("GetDisableGravity", "Disable Gravity");
            CheatMenuLabelsList.Add(CheatMenuVesselLabels_DisableGravity_Label);

            Button CheatMenuVesselOptions_DisableGravity_Button = GenerateButton("GetDisableGravityButton", "Toggle");
            if (GameManager.Instance.Game.CheatSystem.GetDisableGravity())
            {
                CheatMenuVesselOptions_DisableGravity_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
            }
            else
            {
                CheatMenuVesselOptions_DisableGravity_Button.style.backgroundColor = Color.red;
            }
            CheatMenuVesselOptions_DisableGravity_Button.clickable = new Clickable(() => {
                EasyCheatToggle(6);
                if (GameManager.Instance.Game.CheatSystem.GetDisableGravity())
                {
                    CheatMenuVesselOptions_DisableGravity_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
                }
                else
                {
                    CheatMenuVesselOptions_DisableGravity_Button.style.backgroundColor = Color.red;
                }
            });
            CheatMenuOptionsList.Add(CheatMenuVesselOptions_DisableGravity_Button);


            Label CheatMenuVesselLabels_DisableAerodynamics_Label = GenerateLabel("GetDisableAerodynamics", "Disable Aerodynamics");
            CheatMenuLabelsList.Add(CheatMenuVesselLabels_DisableAerodynamics_Label);

            Button CheatMenuVesselOptions_DisableAerodynamics_Button = GenerateButton("GetDisableAerodynamicsButton", "Toggle");
            if (GameManager.Instance.Game.CheatSystem.GetDisableAerodynamics())
            {
                CheatMenuVesselOptions_DisableAerodynamics_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
            }
            else
            {
                CheatMenuVesselOptions_DisableAerodynamics_Button.style.backgroundColor = Color.red;
            }
            CheatMenuVesselOptions_DisableAerodynamics_Button.clickable = new Clickable(() => {
                EasyCheatToggle(7);
                if (GameManager.Instance.Game.CheatSystem.GetDisableAerodynamics())
                {
                    CheatMenuVesselOptions_DisableAerodynamics_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
                }
                else
                {
                    CheatMenuVesselOptions_DisableAerodynamics_Button.style.backgroundColor = Color.red;
                }
            });
            CheatMenuOptionsList.Add(CheatMenuVesselOptions_DisableAerodynamics_Button);


            Label CheatMenuVesselLabels_DisableThermodynamics_Label = GenerateLabel("GetDisableThermodynamics", "Disable Thermodynamics");
            CheatMenuLabelsList.Add(CheatMenuVesselLabels_DisableThermodynamics_Label);

            Button CheatMenuVesselOptions_DisableThermodynamics_Button = GenerateButton("GetDisableThermodynamicsButton", "Toggle");
            if (GameManager.Instance.Game.CheatSystem.GetDisableThermodynamics())
            {
                CheatMenuVesselOptions_DisableThermodynamics_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
            }
            else
            {
                CheatMenuVesselOptions_DisableThermodynamics_Button.style.backgroundColor = Color.red;
            }
            CheatMenuVesselOptions_DisableThermodynamics_Button.clickable = new Clickable(() => {
                EasyCheatToggle(8);
                if (GameManager.Instance.Game.CheatSystem.GetDisableThermodynamics())
                {
                    CheatMenuVesselOptions_DisableThermodynamics_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));;
                }
                else
                {
                    CheatMenuVesselOptions_DisableThermodynamics_Button.style.backgroundColor = Color.red;
                }
            });
            CheatMenuOptionsList.Add(CheatMenuVesselOptions_DisableThermodynamics_Button);


            Label CheatMenuVesselLabels_IgnoreMaxTemp_Label = GenerateLabel("GetIgnoreMaxTemp", "Ignore Max Temp");
            CheatMenuLabelsList.Add(CheatMenuVesselLabels_IgnoreMaxTemp_Label);

            Button CheatMenuVesselOptions_IgnoreMaxTemp_Button = GenerateButton("GetIgnoreMaxTempButton", "Toggle");
            if (GameManager.Instance.Game.CheatSystem.GetIgnoreMaxTemp()){
                CheatMenuVesselOptions_IgnoreMaxTemp_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));
            }else{
                CheatMenuVesselOptions_IgnoreMaxTemp_Button.style.backgroundColor = Color.red;
            }
            CheatMenuVesselOptions_IgnoreMaxTemp_Button.clickable = new Clickable(() => {
                EasyCheatToggle(9);
                if (GameManager.Instance.Game.CheatSystem.GetIgnoreMaxTemp()){
                    CheatMenuVesselOptions_IgnoreMaxTemp_Button.style.backgroundColor = new StyleColor(new Color32(73, 204, 134, 255));
                }else{
                    CheatMenuVesselOptions_IgnoreMaxTemp_Button.style.backgroundColor = Color.red;
                }
            });
            CheatMenuOptionsList.Add(CheatMenuVesselOptions_IgnoreMaxTemp_Button);
        }
        void GenerateUIresource()
        {
            try
            {
                int fontsizeLabel = 8;
                int fontsizeButton = 8;
                int buttonHeight = 9;
                int gapSize = 1;
                int borderRadius = 4;

                Button GenerateButton(string name = "", string text = "")
                {

                    Button GeneratedButton = Element.Button(name, text);
                    GeneratedButton.style.paddingTop = gapSize;
                    GeneratedButton.style.paddingBottom = gapSize;
                    GeneratedButton.style.height = buttonHeight;
                    GeneratedButton.style.fontSize = fontsizeButton;
                    GeneratedButton.style.marginTop = gapSize + 3.9f;
                    GeneratedButton.style.borderBottomLeftRadius = borderRadius;
                    GeneratedButton.style.borderBottomRightRadius = borderRadius;
                    GeneratedButton.style.borderTopLeftRadius = borderRadius;
                    GeneratedButton.style.borderTopRightRadius = borderRadius;
                    return GeneratedButton;
                }
                Label GenerateLabel(string name = "", string text = "")
                {

                    Label GeneratedLabel = Element.Label(name, text);
                    GeneratedLabel.style.fontSize = fontsizeLabel;
                    GeneratedLabel.style.marginTop = gapSize;
                    GeneratedLabel.style.paddingTop = gapSize;
                    GeneratedLabel.style.paddingBottom = gapSize;
                    return GeneratedLabel;
                }

                VisualElement CheatMenuLabelsList = manager.Get("CheatMenu").rootVisualElement.Q<VisualElement>("CheatMenuLabelsList");
                VisualElement CheatMenuOptionsList = manager.Get("CheatMenu").rootVisualElement.Q<VisualElement>("CheatMenuOptionsList");
                CheatMenuLabelsList.Clear();
                CheatMenuOptionsList.Clear();

                Label CheatMenuResourceLabels_FillAll_Label = GenerateLabel("ResourceFillAll", "Fill all Resources");
                CheatMenuLabelsList.Add(CheatMenuResourceLabels_FillAll_Label);
                Button CheatMenuResourceOptions_FillAll_Button = GenerateButton("ResourceFillAllButton", "Fill All");
                CheatMenuResourceOptions_FillAll_Button.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                CheatMenuResourceOptions_FillAll_Button.clickable = new Clickable(() =>
                {
                    EasyCheatSet(1, 0);
                });
                CheatMenuOptionsList.Add(CheatMenuResourceOptions_FillAll_Button);

                Label CheatMenuResourceLabels_PartToFill_Label = GenerateLabel("ResourcePartToFill", "Part to fill");
                CheatMenuLabelsList.Add(CheatMenuResourceLabels_PartToFill_Label);
                DropdownField CheatMenuResourceOptions_PartToFill_Dropdown = new DropdownField();
                CheatMenuResourceOptions_PartToFill_Dropdown.style.height = buttonHeight + 5;
                CheatMenuResourceOptions_PartToFill_Dropdown.style.fontSize = fontsizeButton;
                CheatMenuResourceOptions_PartToFill_Dropdown.style.marginTop = gapSize;
                CheatMenuResourceOptions_PartToFill_Dropdown.style.paddingLeft = 1;
                CheatMenuResourceOptions_PartToFill_Dropdown.style.paddingRight = 1;
                CheatMenuResourceOptions_PartToFill_Dropdown.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));

                CheatMenuResourceOptions_PartToFill_Dropdown.textElement.style.width = 500;
                CheatMenuResourceOptions_PartToFill_Dropdown.textElement.style.marginLeft = 1;
                CheatMenuResourceOptions_PartToFill_Dropdown.textElement.style.marginRight = 1;
                CheatMenuResourceOptions_PartToFill_Dropdown.textElement.style.paddingLeft = 1;
                CheatMenuResourceOptions_PartToFill_Dropdown.textElement.style.paddingRight = 1;
                CheatMenuResourceOptions_PartToFill_Dropdown.visualInput.style.height = buttonHeight + 5;
                CheatMenuResourceOptions_PartToFill_Dropdown.visualInput.style.width = 500;
                CheatMenuResourceOptions_PartToFill_Dropdown.visualInput.style.marginLeft = 1;
                CheatMenuResourceOptions_PartToFill_Dropdown.visualInput.style.marginRight = 1;
                CheatMenuResourceOptions_PartToFill_Dropdown.visualInput.style.paddingLeft = 1;
                CheatMenuResourceOptions_PartToFill_Dropdown.visualInput.style.paddingRight = 1;
                CheatMenuResourceOptions_PartToFill_Dropdown.visualInput.style.fontSize = fontsizeButton;
                CheatMenuResourceOptions_PartToFill_Dropdown.visualInput.style.paddingTop = 1;
                CheatMenuResourceOptions_PartToFill_Dropdown.visualInput.style.paddingBottom = 1;
                CheatMenuResourceOptions_PartToFill_Dropdown.visualInput.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));

                

                partList = GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().PartOwner.Parts.ToList();
                
                partList.ForEach((partdata) => 
                {
                    
                    CheatMenuResourceOptions_PartToFill_Dropdown.choices.Add($"{partdata.Name}");
                    DropdownUtils.SetLabel($"{partdata.Name}", 6);
                    
                });
                CheatMenuResourceOptions_PartToFill_Dropdown.index = SelectedPartInt;
                selectedPart = GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().PartOwner.Parts.ToList()[SelectedPartInt];
                CheatMenuResourceOptions_PartToFill_Dropdown.value = GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().PartOwner.Parts.ToList()[SelectedPartInt].Name;
                CheatMenuResourceOptions_PartToFill_Dropdown.RegisterValueChangedCallback((evt) =>
                {
                    SelectedPartInt = CheatMenuResourceOptions_PartToFill_Dropdown.index;
                    selectedPart = GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().PartOwner.Parts.ToList()[SelectedPartInt];
                    SelectedResourcent = 0;
                    SetSelectdMenu(SelectedMenu);
                });
                CheatMenuOptionsList.Add(CheatMenuResourceOptions_PartToFill_Dropdown);
                
                if (partList.Count > 0)
                {
                    
                    Label CheatMenuResourceLabels_ResourceToFill_Label = GenerateLabel("ResourceToFill", "Resource to fill");
                    CheatMenuLabelsList.Add(CheatMenuResourceLabels_ResourceToFill_Label);
                    DropdownField CheatMenuResourceOptions_ResourceToFill_Dropdown = new DropdownField();
                    CheatMenuResourceOptions_ResourceToFill_Dropdown.style.height = buttonHeight + 5;
                    CheatMenuResourceOptions_ResourceToFill_Dropdown.style.fontSize = fontsizeButton;
                    CheatMenuResourceOptions_ResourceToFill_Dropdown.style.marginTop = gapSize;
                    CheatMenuResourceOptions_ResourceToFill_Dropdown.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    CheatMenuResourceOptions_ResourceToFill_Dropdown.visualInput.style.height = buttonHeight + 5;
                    CheatMenuResourceOptions_ResourceToFill_Dropdown.visualInput.style.fontSize = fontsizeButton;
                    CheatMenuResourceOptions_ResourceToFill_Dropdown.visualInput.style.paddingTop = 1;
                    CheatMenuResourceOptions_ResourceToFill_Dropdown.visualInput.style.paddingBottom = 1;
                    CheatMenuResourceOptions_ResourceToFill_Dropdown.visualInput.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    


                    resourceList = selectedPart.PartResourceContainer.GetAllResourcesContainedData();

                    foreach (var resource in resourceList)
                    {
                        CheatMenuResourceOptions_ResourceToFill_Dropdown.choices.Add($"{GameManager.Instance.Game.ResourceDefinitionDatabase.GetResourceNameFromID(resource.ResourceID)}");
                    }
                    CheatMenuResourceOptions_ResourceToFill_Dropdown.index = SelectedResourcent;
                    selectedResource = selectedPart.PartResourceContainer.GetResourceContainedData(resourceList.ElementAtOrDefault<ContainedResourceData>(SelectedResourcent).ResourceID);
                    CheatMenuResourceOptions_ResourceToFill_Dropdown.value = GameManager.Instance.Game.ResourceDefinitionDatabase.GetResourceNameFromID(resourceList.ElementAtOrDefault<ContainedResourceData>(SelectedResourcent).ResourceID);
                    CheatMenuResourceOptions_ResourceToFill_Dropdown.RegisterValueChangedCallback((evt) =>
                    {
                        SelectedResourcent = CheatMenuResourceOptions_ResourceToFill_Dropdown.index;
                        selectedResource = selectedPart.PartResourceContainer.GetResourceContainedData(GameManager.Instance.Game.ResourceDefinitionDatabase.GetResourceIDFromName($"{evt.newValue}"));
                        SetSelectdMenu(SelectedMenu);
                    });
                    CheatMenuOptionsList.Add(CheatMenuResourceOptions_ResourceToFill_Dropdown);
                    

                    Label CheatMenuResourceLabels_ResourceFillAmount_Label = GenerateLabel("ResourceFillAmount", "Resource fill amount");
                    CheatMenuLabelsList.Add(CheatMenuResourceLabels_ResourceFillAmount_Label);

                    Slider CheatMenuResourceOptions_ResourceFillAmount_Slider = Element.Slider("ResourceFillAmountSlider", 0f, (float)selectedResource.CapacityUnits, (float)selectedResource.StoredUnits);
                    CheatMenuResourceOptions_ResourceFillAmount_Slider.style.height = buttonHeight;
                    CheatMenuResourceOptions_ResourceFillAmount_Slider.style.fontSize = fontsizeButton;
                    CheatMenuResourceOptions_ResourceFillAmount_Slider.style.marginTop = gapSize;
                    CheatMenuResourceOptions_ResourceFillAmount_Slider.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    CheatMenuResourceOptions_ResourceFillAmount_Slider.dragContainer.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    
                    CheatMenuResourceOptions_ResourceFillAmount_Slider.dragElement.style.backgroundColor = new StyleColor(new Color32(255, 255, 255, 255));
                    CheatMenuResourceOptions_ResourceFillAmount_Slider.dragElement.style.marginTop = -2;
                    CheatMenuResourceOptions_ResourceFillAmount_Slider.dragElement.style.width = 15;
                    CheatMenuResourceOptions_ResourceFillAmount_Slider.dragElement.style.height = 15;
                    CheatMenuResourceOptions_ResourceFillAmount_Slider.dragBorderElement.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));

                    CheatMenuResourceOptions_ResourceFillAmount_Slider.RegisterValueChangedCallback((evt) =>
                    {
                        EasyCheatSet(2, evt.newValue);
                        //SetSelectdMenu(SelectedMenu);
                    });
                    CheatMenuOptionsList.Add(CheatMenuResourceOptions_ResourceFillAmount_Slider);


                }
            }
            catch (Exception e)
            {
                logger.Error($"{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}");
            }
        }
        void GenerateUIkerbal()
        {
            try {


                int fontsizeLabel = 5;
                int fontsizeButton = 5;
                int buttonHeight = 6;
                int gapSize = 1;



                VisualElement CheatMenuLabelsList = manager.Get("CheatMenu").rootVisualElement.Q<VisualElement>("CheatMenuLabelsList");
                VisualElement CheatMenuOptionsList = manager.Get("CheatMenu").rootVisualElement.Q<VisualElement>("CheatMenuOptionsList");
                CheatMenuLabelsList.Clear();
                CheatMenuOptionsList.Clear();

                Label CheatMenuKerbalLabels_SelectKerbal_Label = Element.Label("SelectKerbal", "Select Kerbal");
                CheatMenuKerbalLabels_SelectKerbal_Label.style.fontSize = fontsizeLabel;
                CheatMenuKerbalLabels_SelectKerbal_Label.style.marginTop = gapSize;
                CheatMenuLabelsList.Add(CheatMenuKerbalLabels_SelectKerbal_Label);

                DropdownField CheatMenuKerbalOptions_SelectKerbal_Dropdown = new DropdownField();
                CheatMenuKerbalOptions_SelectKerbal_Dropdown.style.height = buttonHeight + 5;
                CheatMenuKerbalOptions_SelectKerbal_Dropdown.style.fontSize = fontsizeButton;
                CheatMenuKerbalOptions_SelectKerbal_Dropdown.style.marginTop = gapSize;
                CheatMenuKerbalOptions_SelectKerbal_Dropdown.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                CheatMenuKerbalOptions_SelectKerbal_Dropdown.visualInput.style.height = buttonHeight + 5;
                CheatMenuKerbalOptions_SelectKerbal_Dropdown.visualInput.style.fontSize = fontsizeButton;
                CheatMenuKerbalOptions_SelectKerbal_Dropdown.visualInput.style.paddingTop = 1;
                CheatMenuKerbalOptions_SelectKerbal_Dropdown.visualInput.style.paddingBottom = 1;
                CheatMenuKerbalOptions_SelectKerbal_Dropdown.visualInput.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));



                KerbalList = GameManager.Instance.Game.SessionManager.KerbalRosterManager.GetAllKerbals();

                CheatMenuKerbalOptions_SelectKerbal_Dropdown.choices.Add($"Create New Kerbal");
                KerbalList.ForEach((kerbalData) =>
                {
                    CheatMenuKerbalOptions_SelectKerbal_Dropdown.choices.Add($"{kerbalData.NameKey}");
                });
                CheatMenuKerbalOptions_SelectKerbal_Dropdown.index = SelectedKerbalInt;
                if (SelectedKerbalInt > 0)
                {
                    selectedKerbal = KerbalList[SelectedKerbalInt - 1];
                    CheatMenuKerbalOptions_SelectKerbal_Dropdown.value = selectedKerbal.NameKey;
                }
                else
                {
                    CheatMenuKerbalOptions_SelectKerbal_Dropdown.value = "Create New Kerbal";
                }
               
                CheatMenuKerbalOptions_SelectKerbal_Dropdown.RegisterValueChangedCallback((evt) =>
                {
                    SelectedKerbalInt = CheatMenuKerbalOptions_SelectKerbal_Dropdown.index;
                    if (SelectedKerbalInt > 0)
                    {
                        selectedKerbal = KerbalList[SelectedKerbalInt];
                    }
                    SetSelectdMenu(SelectedMenu);
                });
                CheatMenuOptionsList.Add(CheatMenuKerbalOptions_SelectKerbal_Dropdown);
                if (SelectedKerbalInt > 0)
                {
                    selectedKerbal.Attributes.Attributes.ToList().ForEach((skevt) => {
                        logger.Debug($"{skevt.Key} {skevt.Value.valueType} {skevt.Value.value} {skevt.Value.attachToName}");
                    });

                    Color kerbalColor = (Color)selectedKerbal.Attributes.GetAttribute("SKINCOLOR");
                    Label CheatMenuKerbalLabels_KerbalName_Label = Element.Label("KerbalName", "First Name");
                    CheatMenuKerbalLabels_KerbalName_Label.style.fontSize = fontsizeLabel;
                    CheatMenuKerbalLabels_KerbalName_Label.style.marginTop = gapSize;
                    CheatMenuLabelsList.Add(CheatMenuKerbalLabels_KerbalName_Label);

                    TextField CheatMenuKerbalOptions_KerbalName_Input = Element.TextField("KerbalNameinput", selectedKerbal.Attributes.FirstName);
                    CheatMenuKerbalOptions_KerbalName_Input.style.height = buttonHeight + 5;
                    CheatMenuKerbalOptions_KerbalName_Input.style.fontSize = fontsizeButton;
                    CheatMenuKerbalOptions_KerbalName_Input.style.marginTop = gapSize + 5;
                    CheatMenuKerbalOptions_KerbalName_Input.style.paddingTop = 0;
                    CheatMenuKerbalOptions_KerbalName_Input.style.paddingBottom = 0;
                    CheatMenuKerbalOptions_KerbalName_Input.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.marginTop = 0;
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.marginBottom = 0;
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.paddingTop = 0;
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.paddingBottom = 0;
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.fontSize = fontsizeButton;
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.height = buttonHeight + 5;
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    CheatMenuKerbalOptions_KerbalName_Input.RegisterValueChangedCallback((evt) => {
                        selectedKerbal.Attributes.Attributes["NAME"].value = CheatMenuKerbalOptions_KerbalName_Input.textInput.text;
                    });
                    CheatMenuOptionsList.Add(CheatMenuKerbalOptions_KerbalName_Input);

                    selectedKerbal.Attributes.Attributes.ToList().ForEach((skevt) => {
                        logger.Debug($"{skevt.Key} {skevt.Value.valueType} {skevt.Value.value} {skevt.Value.attachToName}");
                    });

                    Label CheatMenuKerbalLabels_KerbalLastName_Label = Element.Label("KerbalNameLast", "Surname Name");
                    CheatMenuKerbalLabels_KerbalLastName_Label.style.fontSize = fontsizeLabel;
                    CheatMenuKerbalLabels_KerbalLastName_Label.style.marginTop = gapSize;
                    CheatMenuLabelsList.Add(CheatMenuKerbalLabels_KerbalLastName_Label);

                    TextField CheatMenuKerbalOptions_KerbalNameSurname_Input = Element.TextField("KerbalNameinputSurname", selectedKerbal.Attributes.Surname);
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.style.height = buttonHeight + 5;
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.style.fontSize = fontsizeButton;
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.style.marginTop = gapSize + 5;
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.style.paddingTop = 0;
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.style.paddingBottom = 0;
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.textInput.style.marginTop = 0;
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.textInput.style.marginBottom = 0;
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.textInput.style.paddingTop = 0;
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.textInput.style.paddingBottom = 0;
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.textInput.style.fontSize = fontsizeButton;
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.textInput.style.height = buttonHeight + 5;
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.textInput.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    CheatMenuKerbalOptions_KerbalNameSurname_Input.RegisterValueChangedCallback((evt) => {
                        selectedKerbal.Attributes.Attributes["SURNAME"].value = CheatMenuKerbalOptions_KerbalNameSurname_Input.textInput.text;
                    });
                    CheatMenuOptionsList.Add(CheatMenuKerbalOptions_KerbalNameSurname_Input);

                    Label CheatMenuKerbalLabels_KerbalType_Label = Element.Label("KerbalType", "Type");
                    CheatMenuKerbalLabels_KerbalType_Label.style.fontSize = fontsizeLabel;
                    CheatMenuKerbalLabels_KerbalType_Label.style.marginTop = gapSize;
                    CheatMenuLabelsList.Add(CheatMenuKerbalLabels_KerbalType_Label);

                    TextField CheatMenuKerbalOptions_KerbalType_Input = Element.TextField("KerbalNameinputType", $"{selectedKerbal.Attributes.Attributes["TYPE"].value}");
                    CheatMenuKerbalOptions_KerbalType_Input.style.height = buttonHeight + 5;
                    CheatMenuKerbalOptions_KerbalType_Input.style.fontSize = fontsizeButton;
                    CheatMenuKerbalOptions_KerbalType_Input.style.marginTop = gapSize + 5 ;
                    CheatMenuKerbalOptions_KerbalType_Input.style.paddingTop = 0;
                    CheatMenuKerbalOptions_KerbalType_Input.style.paddingBottom = 0;
                    CheatMenuKerbalOptions_KerbalType_Input.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    CheatMenuKerbalOptions_KerbalType_Input.textInput.style.marginTop = 0;
                    CheatMenuKerbalOptions_KerbalType_Input.textInput.style.marginBottom = 0;
                    CheatMenuKerbalOptions_KerbalType_Input.textInput.style.paddingTop = 0;
                    CheatMenuKerbalOptions_KerbalType_Input.textInput.style.paddingBottom = 0;
                    CheatMenuKerbalOptions_KerbalType_Input.textInput.style.fontSize = fontsizeButton;
                    CheatMenuKerbalOptions_KerbalType_Input.textInput.style.height = buttonHeight + 5;
                    CheatMenuKerbalOptions_KerbalType_Input.textInput.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    CheatMenuKerbalOptions_KerbalType_Input.RegisterValueChangedCallback((evt) => {
                        switch (CheatMenuKerbalOptions_KerbalType_Input.textInput.text)
                        {
                            case "CIRCLE":
                                selectedKerbal.Attributes.Attributes["TYPE"].value = KSP.Sim.KerbalType.CIRCLE;
                                break;
                            case "SQUARE":
                                selectedKerbal.Attributes.Attributes["TYPE"].value = KSP.Sim.KerbalType.SQUARE;
                                break;
                            case "BUILDER":
                                selectedKerbal.Attributes.Attributes["TYPE"].value = KSP.Sim.KerbalType.BUILDER;
                                break;
                            default:

                                break;
                        }
                    });
                    CheatMenuOptionsList.Add(CheatMenuKerbalOptions_KerbalType_Input);

                    logger.Debug($"{(Color)selectedKerbal.Attributes.GetAttribute("SKINCOLOR")}");

                    Label CheatMenuKerbalLabels_KerbalSKINCOLOR_Label = Element.Label("SKINCOLOR", "Skin color");
                    CheatMenuKerbalLabels_KerbalSKINCOLOR_Label.style.fontSize = fontsizeLabel;
                    CheatMenuKerbalLabels_KerbalSKINCOLOR_Label.style.marginTop = gapSize;
                    CheatMenuLabelsList.Add(CheatMenuKerbalLabels_KerbalSKINCOLOR_Label);

                    SliderInt CheatMenuKerbalOptions_SKINCOLORR_Input = Element.SliderInt("SKINCOLORR", 0, 255, ((Color32)kerbalColor).r);
                    CheatMenuKerbalOptions_SKINCOLORR_Input.style.height = buttonHeight;
                    CheatMenuKerbalOptions_SKINCOLORR_Input.style.width = 83;
                    CheatMenuKerbalOptions_SKINCOLORR_Input.style.fontSize = fontsizeButton;
                    CheatMenuKerbalOptions_SKINCOLORR_Input.style.marginTop = gapSize;
                    

                    CheatMenuKerbalOptions_SKINCOLORR_Input.dragElement.style.backgroundColor = new StyleColor(new Color32(255, 255, 255, 255));
                    CheatMenuKerbalOptions_SKINCOLORR_Input.dragElement.style.marginTop = -2;
                    CheatMenuKerbalOptions_SKINCOLORR_Input.dragElement.style.width = 15;
                    CheatMenuKerbalOptions_SKINCOLORR_Input.dragElement.style.height = 15;
                    CheatMenuKerbalOptions_SKINCOLORR_Input.dragBorderElement.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    CheatMenuKerbalOptions_SKINCOLORR_Input.dragBorderElement.style.height = buttonHeight;


                    SliderInt CheatMenuKerbalOptions_SKINCOLORG_Input = Element.SliderInt("SKINCOLORG", 0, 255, ((Color32)kerbalColor).g);
                    CheatMenuKerbalOptions_SKINCOLORG_Input.style.height = buttonHeight;
                    CheatMenuKerbalOptions_SKINCOLORG_Input.style.width = 83;
                    CheatMenuKerbalOptions_SKINCOLORG_Input.style.fontSize = fontsizeButton;
                    CheatMenuKerbalOptions_SKINCOLORG_Input.style.top = -buttonHeight - (gapSize * 4);
                    CheatMenuKerbalOptions_SKINCOLORG_Input.style.marginLeft = 85;
                    CheatMenuKerbalOptions_SKINCOLORG_Input.dragBorderElement.style.height = buttonHeight;
                    CheatMenuKerbalOptions_SKINCOLORG_Input.dragElement.style.backgroundColor = new StyleColor(new Color32(255, 255, 255, 255));
                    CheatMenuKerbalOptions_SKINCOLORG_Input.dragElement.style.marginTop = -2;
                    CheatMenuKerbalOptions_SKINCOLORG_Input.dragElement.style.width = 15;
                    CheatMenuKerbalOptions_SKINCOLORG_Input.dragElement.style.height = 15;
                    CheatMenuKerbalOptions_SKINCOLORG_Input.dragBorderElement.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));

                    SliderInt CheatMenuKerbalOptions_SKINCOLORB_Input = Element.SliderInt("SKINCOLORB", 0, 255, ((Color32)kerbalColor).b);
                    CheatMenuKerbalOptions_SKINCOLORB_Input.style.height = buttonHeight;
                    CheatMenuKerbalOptions_SKINCOLORB_Input.style.width = 83;
                    CheatMenuKerbalOptions_SKINCOLORB_Input.style.fontSize = fontsizeButton;
                    CheatMenuKerbalOptions_SKINCOLORB_Input.style.top = -(buttonHeight * 2) - (gapSize * 8);
                    CheatMenuKerbalOptions_SKINCOLORB_Input.style.marginLeft = 85 * 2;
                    CheatMenuKerbalOptions_SKINCOLORB_Input.dragBorderElement.style.height = buttonHeight;
                    CheatMenuKerbalOptions_SKINCOLORB_Input.dragElement.style.backgroundColor = new StyleColor(new Color32(255, 255, 255, 255));
                    CheatMenuKerbalOptions_SKINCOLORB_Input.dragElement.style.marginTop = -2;
                    CheatMenuKerbalOptions_SKINCOLORB_Input.dragElement.style.width = 15;
                    CheatMenuKerbalOptions_SKINCOLORB_Input.dragElement.style.height = 15;
                    CheatMenuKerbalOptions_SKINCOLORB_Input.dragBorderElement.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));

                    CheatMenuKerbalOptions_SKINCOLORR_Input.RegisterValueChangedCallback((evt) =>
                    {
                        UpdateColor();
                    });
                    CheatMenuKerbalOptions_SKINCOLORG_Input.RegisterValueChangedCallback((evt) =>
                    {
                        UpdateColor();
                    });
                    CheatMenuKerbalOptions_SKINCOLORB_Input.RegisterValueChangedCallback((evt) =>
                    {
                        UpdateColor();
                    });

                    void UpdateColor()
                    {
                        CheatMenuKerbalOptions_SKINCOLORR_Input.style.backgroundColor = new StyleColor(new Color32((byte)CheatMenuKerbalOptions_SKINCOLORR_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORG_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORB_Input.value, 255));
                        CheatMenuKerbalOptions_SKINCOLORG_Input.style.backgroundColor = new StyleColor(new Color32((byte)CheatMenuKerbalOptions_SKINCOLORR_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORG_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORB_Input.value, 255));
                        CheatMenuKerbalOptions_SKINCOLORB_Input.style.backgroundColor = new StyleColor(new Color32((byte)CheatMenuKerbalOptions_SKINCOLORR_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORG_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORB_Input.value, 255));

                        CheatMenuKerbalOptions_SKINCOLORR_Input.dragContainer.style.backgroundColor = new StyleColor(new Color32((byte)CheatMenuKerbalOptions_SKINCOLORR_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORG_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORB_Input.value, 255));
                        CheatMenuKerbalOptions_SKINCOLORG_Input.dragContainer.style.backgroundColor = new StyleColor(new Color32((byte)CheatMenuKerbalOptions_SKINCOLORR_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORG_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORB_Input.value, 255));
                        CheatMenuKerbalOptions_SKINCOLORB_Input.dragContainer.style.backgroundColor = new StyleColor(new Color32((byte)CheatMenuKerbalOptions_SKINCOLORR_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORG_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORB_Input.value, 255));

                        selectedKerbal.Attributes.SetAttribute("SKINCOLOR", new VarietyPreloadInfo((Color)new Color32((byte)CheatMenuKerbalOptions_SKINCOLORR_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORG_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORB_Input.value, 255),typeof(Color), ""));
                    }

                    CheatMenuKerbalOptions_SKINCOLORR_Input.style.backgroundColor = new StyleColor(new Color32((byte)CheatMenuKerbalOptions_SKINCOLORR_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORG_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORB_Input.value, 255));
                    CheatMenuKerbalOptions_SKINCOLORG_Input.style.backgroundColor = new StyleColor(new Color32((byte)CheatMenuKerbalOptions_SKINCOLORR_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORG_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORB_Input.value, 255));
                    CheatMenuKerbalOptions_SKINCOLORB_Input.style.backgroundColor = new StyleColor(new Color32((byte)CheatMenuKerbalOptions_SKINCOLORR_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORG_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORB_Input.value, 255));

                    CheatMenuKerbalOptions_SKINCOLORR_Input.dragContainer.style.backgroundColor = new StyleColor(new Color32((byte)CheatMenuKerbalOptions_SKINCOLORR_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORG_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORB_Input.value, 255));
                    CheatMenuKerbalOptions_SKINCOLORG_Input.dragContainer.style.backgroundColor = new StyleColor(new Color32((byte)CheatMenuKerbalOptions_SKINCOLORR_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORG_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORB_Input.value, 255));
                    CheatMenuKerbalOptions_SKINCOLORB_Input.dragContainer.style.backgroundColor = new StyleColor(new Color32((byte)CheatMenuKerbalOptions_SKINCOLORR_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORG_Input.value, (byte)CheatMenuKerbalOptions_SKINCOLORB_Input.value, 255));
                    CheatMenuOptionsList.Add(CheatMenuKerbalOptions_SKINCOLORR_Input);
                    CheatMenuOptionsList.Add(CheatMenuKerbalOptions_SKINCOLORG_Input);
                    CheatMenuOptionsList.Add(CheatMenuKerbalOptions_SKINCOLORB_Input);
                }
                else
                {

                    
                    Label CheatMenuKerbalLabels_KerbalName_Label = Element.Label("KerbalName", "Kerbal Name");
                    CheatMenuKerbalLabels_KerbalName_Label.style.fontSize = fontsizeLabel;
                    CheatMenuKerbalLabels_KerbalName_Label.style.unityFont = new StyleFont((Font)Resources.Load("FontName"));
                    CheatMenuKerbalLabels_KerbalName_Label.style.marginTop = gapSize;
                    CheatMenuLabelsList.Add(CheatMenuKerbalLabels_KerbalName_Label);

                    TextField CheatMenuKerbalOptions_KerbalName_Input = Element.TextField("KerbalNameinput", "Kerbal Name");
                    CheatMenuKerbalOptions_KerbalName_Input.style.height = buttonHeight + 5;
                    CheatMenuKerbalOptions_KerbalName_Input.style.fontSize = fontsizeButton;
                    CheatMenuKerbalOptions_KerbalName_Input.style.marginTop = gapSize;
                    CheatMenuKerbalOptions_KerbalName_Input.style.paddingTop = 0;
                    CheatMenuKerbalOptions_KerbalName_Input.style.paddingBottom = 0;
                    CheatMenuKerbalOptions_KerbalName_Input.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.marginTop = 0;
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.marginBottom = 0;
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.paddingTop = 0;
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.paddingBottom = 0;
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.fontSize = fontsizeButton;
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.height = buttonHeight + 5;
                    CheatMenuKerbalOptions_KerbalName_Input.textInput.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    CheatMenuOptionsList.Add(CheatMenuKerbalOptions_KerbalName_Input);
                    
                    Button CheatMenuKerbalOptions_KerbalCreate_Button = Element.Button("KerbalCreateButton", "Create Kerbal");
                    CheatMenuKerbalOptions_KerbalCreate_Button.style.height = buttonHeight;
                    CheatMenuKerbalOptions_KerbalCreate_Button.style.fontSize = fontsizeButton;
                    CheatMenuKerbalOptions_KerbalCreate_Button.style.marginTop = gapSize;
                    CheatMenuKerbalOptions_KerbalCreate_Button.style.backgroundColor = new StyleColor(new Color32(50, 50, 50, 255));
                    CheatMenuKerbalOptions_KerbalCreate_Button.clickable = new Clickable(() =>
                    {
                        GameManager.Instance.Game.SessionManager.KerbalRosterManager.CreateKerbalByName(CheatMenuKerbalOptions_KerbalName_Input.textInput.text);
                        SelectedKerbalInt = KerbalList.Count;
                        SetSelectdMenu(SelectedMenu);
                    });
                    CheatMenuOptionsList.Add(CheatMenuKerbalOptions_KerbalCreate_Button);

                }
                
            }
            catch (Exception e)
            {
                logger.Error($"{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}");
            }
        }
        void GenerateUI()
        {
            try
            {
                float arbitrary_Limitation_button_height = 12f;
                int arbitrary_Limitation_button_top = 5;

                float spaceVector = 2;
                int fontsize = 7;
                int fontsizeLG = 10;
                VisualElement CheatMenu = Element.Root("CheatMenu");
                dragArea = new DragManipulator();
                dragArea.deadzone = new Vector2[2] { new Vector2(70,15),new Vector2(200,200)};
                CheatMenu.AddManipulator(dragArea);

                logger.Debug($"width {Screen.width}");
                logger.Debug($"height {Screen.height}");
                CheatMenu.style.position = Position.Absolute;
                CheatMenu.style.width = 250;
                CheatMenu.style.height = 312.5f;
                CheatMenu.style.paddingBottom = 0;
                CheatMenu.style.paddingLeft = 0;
                CheatMenu.style.paddingRight = 0;
                CheatMenu.style.paddingTop = 0;
                CheatMenu.style.borderBottomWidth = 1;
                CheatMenu.style.borderTopWidth = 1;
                CheatMenu.style.borderLeftWidth = 1;
                CheatMenu.style.borderRightWidth = 1;
                CheatMenu.style.backgroundColor = new StyleColor(new Color32(255, 255, 255, 0));
                CheatMenu.style.borderRightColor = new StyleColor(new Color32(67, 81, 89, 255));
                CheatMenu.style.borderLeftColor = new StyleColor(new Color32(67, 81, 89, 255));
                CheatMenu.style.borderTopColor = new StyleColor(new Color32(67, 81, 89, 255));
                CheatMenu.style.borderBottomColor = new StyleColor(new Color32(67, 81, 89, 255));
                CheatMenu.style.left = manager.arbitrary_Limitation_because_of_an_update_to_uitk_that_limits_screen_size_due_to_space_warp_being_shit___Width / 1.5f;
                CheatMenu.style.top = manager.arbitrary_Limitation_because_of_an_update_to_uitk_that_limits_screen_size_due_to_space_warp_being_shit___height / 4;
                CheatMenu.style.backgroundImage = AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/Background1.png");

                Button CheatMenuButtonVessel = Element.Button("CheatMenuButtonVessel", $"Vessel");
                //CheatMenuButtonVessel.style.backgroundImage = AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/ButtonVessel.png");
                CheatMenuButtonVessel.style.position = Position.Absolute;
                //CheatMenuButtonVessel.style.width = 50;
                CheatMenuButtonVessel.style.height = arbitrary_Limitation_button_height;
                CheatMenuButtonVessel.style.top = arbitrary_Limitation_button_top;
                CheatMenuButtonVessel.style.left = spaceVector;
                CheatMenuButtonVessel.style.fontSize = fontsizeLG;
                CheatMenuButtonVessel.style.backgroundColor = new StyleColor(new Color32(30, 30, 30, 255));
                CheatMenuButtonVessel.style.borderRightColor = new StyleColor(new Color32(40, 40, 40, 255));
                CheatMenuButtonVessel.style.borderLeftColor = new StyleColor(new Color32(40, 40, 40, 255));
                CheatMenuButtonVessel.style.borderTopColor = new StyleColor(new Color32(40, 40, 40, 255));
                CheatMenuButtonVessel.style.borderBottomColor = new StyleColor(new Color32(40, 40, 40, 0));
                CheatMenuButtonVessel.style.borderBottomLeftRadius = 0;
                CheatMenuButtonVessel.style.borderBottomRightRadius = 0;
                CheatMenuButtonVessel.style.borderTopLeftRadius = 5;
                CheatMenuButtonVessel.style.borderTopRightRadius = 5;
                CheatMenuButtonVessel.style.paddingTop = 5;
                CheatMenuButtonVessel.style.paddingLeft = 4;
                CheatMenuButtonVessel.style.paddingRight = 4;
                CheatMenuButtonVessel.style.paddingBottom = 2;
                CheatMenuButtonVessel.clickable = new Clickable(() => { SetSelectdMenu("vessel"); });
                CheatMenu.Add(CheatMenuButtonVessel);
                spaceVector = spaceVector + 50;

                Button CheatMenuButtonResource = Element.Button("CheatMenuButtonResource", $"Resource");
                //CheatMenuButtonResource.style.backgroundImage = AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/ButtonResource.png");
                CheatMenuButtonResource.style.position = Position.Absolute;
                //CheatMenuButtonResource.style.width = 50;
                CheatMenuButtonResource.style.height = arbitrary_Limitation_button_height;
                CheatMenuButtonResource.style.top = arbitrary_Limitation_button_top;
                CheatMenuButtonResource.style.left = spaceVector;
                CheatMenuButtonResource.style.fontSize = fontsizeLG;
                CheatMenuButtonResource.style.backgroundColor = new StyleColor(new Color32(30, 30, 30, 255));
                CheatMenuButtonResource.style.borderRightColor = new StyleColor(new Color32(40, 40, 40, 255));
                CheatMenuButtonResource.style.borderLeftColor = new StyleColor(new Color32(40, 40, 40, 255));
                CheatMenuButtonResource.style.borderTopColor = new StyleColor(new Color32(40, 40, 40, 255));
                CheatMenuButtonResource.style.borderBottomColor = new StyleColor(new Color32(40, 40, 40, 0));
                CheatMenuButtonResource.style.borderBottomLeftRadius = 0;
                CheatMenuButtonResource.style.borderBottomRightRadius = 0;
                CheatMenuButtonResource.style.borderTopLeftRadius = 5;
                CheatMenuButtonResource.style.borderTopRightRadius = 5;
                CheatMenuButtonResource.style.paddingTop = 5;
                CheatMenuButtonResource.style.paddingLeft = 4;
                CheatMenuButtonResource.style.paddingRight = 4;
                CheatMenuButtonResource.style.paddingBottom = 2;
                CheatMenuButtonResource.clickable = new Clickable(() => { SetSelectdMenu("resource"); });
                CheatMenu.Add(CheatMenuButtonResource);
                spaceVector = spaceVector + 62.5f;

                Button CheatMenuButtonKerbal = Element.Button("CheatMenuButtonKerbal", $"Kerbal");
                //CheatMenuButtonKerbal.style.backgroundImage = AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/ButtonKerbal.png");
                CheatMenuButtonKerbal.style.position = Position.Absolute;
                //CheatMenuButtonKerbal.style.width = 50;
                CheatMenuButtonKerbal.style.height = arbitrary_Limitation_button_height;
                CheatMenuButtonKerbal.style.top = arbitrary_Limitation_button_top;
                CheatMenuButtonKerbal.style.left = spaceVector;
                CheatMenuButtonKerbal.style.fontSize = fontsizeLG;
                CheatMenuButtonKerbal.style.backgroundColor = new StyleColor(new Color32(30, 30, 30, 255));
                CheatMenuButtonKerbal.style.borderRightColor = new StyleColor(new Color32(40, 40, 40, 255));
                CheatMenuButtonKerbal.style.borderLeftColor = new StyleColor(new Color32(40, 40, 40, 255));
                CheatMenuButtonKerbal.style.borderTopColor = new StyleColor(new Color32(40, 40, 40, 255));
                CheatMenuButtonKerbal.style.borderBottomColor = new StyleColor(new Color32(40, 40, 40, 0));
                CheatMenuButtonKerbal.style.borderBottomLeftRadius = 0;
                CheatMenuButtonKerbal.style.borderBottomRightRadius = 0;
                CheatMenuButtonKerbal.style.borderTopLeftRadius = 5;
                CheatMenuButtonKerbal.style.borderTopRightRadius = 5;
                CheatMenuButtonKerbal.style.paddingTop = 5;
                CheatMenuButtonKerbal.style.paddingLeft = 4;
                CheatMenuButtonKerbal.style.paddingRight = 4;
                CheatMenuButtonKerbal.style.paddingBottom = 2;
                CheatMenuButtonKerbal.clickable = new Clickable(() => { SetSelectdMenu("kerbal"); });
                CheatMenu.Add(CheatMenuButtonKerbal);
                spaceVector = spaceVector + 60;

                Button CheatMenuButtonClose = Element.Button("CheatMenuButtonClose", $"X");
                //CheatMenuButtonKerbal.style.backgroundImage = AssetManager.GetAsset<Texture2D>($"{SpaceWarpMetadata.ModID}/images/ButtonKerbal.png");
                CheatMenuButtonClose.style.position = Position.Absolute;
                CheatMenuButtonClose.style.width = 10;
                CheatMenuButtonClose.style.height = 10;
                CheatMenuButtonClose.style.paddingTop = 2;
                CheatMenuButtonClose.style.paddingBottom = 2;
                CheatMenuButtonClose.style.paddingLeft = 2;
                CheatMenuButtonClose.style.paddingRight = 2;
                CheatMenuButtonClose.style.top = 3.5f;
                CheatMenuButtonClose.style.right = 5;
                CheatMenuButtonClose.style.fontSize = fontsizeLG;
                CheatMenuButtonClose.style.color = Color.white;
                CheatMenuButtonClose.style.backgroundColor = Color.red;
                CheatMenuButtonClose.style.borderRightColor = new StyleColor(new Color32(255, 255, 255, 0));
                CheatMenuButtonClose.style.borderLeftColor = new StyleColor(new Color32(255, 255, 255, 0));
                CheatMenuButtonClose.style.borderTopColor = new StyleColor(new Color32(255, 255, 255, 0));
                CheatMenuButtonClose.style.borderBottomColor = new StyleColor(new Color32(255, 255, 255, 0));
                CheatMenuButtonClose.clickable = new Clickable(() => {
                    showUI = !showUI;
                    manager.Set("CheatMenu", showUI);
                    logger.Debug($"Button Toggle: {showUI}");
                });
                CheatMenu.Add(CheatMenuButtonClose);


                VisualElement CheatMenuLabelsList = new VisualElement();
                CheatMenuLabelsList.style.position = Position.Absolute;
                CheatMenuLabelsList.style.width = 115f;
                CheatMenuLabelsList.style.height = 287.5f;
                CheatMenuLabelsList.style.top = 18.75f;
                CheatMenuLabelsList.style.left = 0;
                CheatMenuLabelsList.style.paddingLeft = 2;
                CheatMenuLabelsList.style.backgroundColor = new StyleColor(new Color32(255, 255, 255, 0));
                CheatMenuLabelsList.name = "CheatMenuLabelsList";
                CheatMenu.Add(CheatMenuLabelsList);

                VisualElement CheatMenuOptionsList = new VisualElement();
                CheatMenuOptionsList.style.position = Position.Absolute;
                CheatMenuOptionsList.style.width = 135f;
                CheatMenuOptionsList.style.height = 287.5f;
                CheatMenuOptionsList.style.top = 18.75f;
                CheatMenuOptionsList.style.left = 116f;
                CheatMenuOptionsList.style.paddingLeft = 10;
                CheatMenuOptionsList.style.paddingRight = 20;
                CheatMenuOptionsList.style.backgroundColor = new StyleColor(new Color32(255, 255, 255, 0));
                CheatMenuOptionsList.name = "CheatMenuOptionsList";
                CheatMenu.Add(CheatMenuOptionsList);

                UIDocument window = Window.CreateFromElement(CheatMenu);
                manager.Add("CheatMenu", window);
            }
            catch (Exception e)
            {
                logger.Error($"{e.Message}\n{e.InnerException}\n{e.Source}\n{e.Data}\n{e.HelpLink}\n{e.HResult}\n{e.StackTrace}\n{e.TargetSite}");
            }
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
                    GameManager.Instance.Game.ViewController.GetActiveVehicle().GetSimulationObject().PartOwner.Parts.ToList().ForEach(part => part.PartResourceContainer.FillAllResourcesToCapacity());
                    break;
                case 2:
                    selectedPart.PartResourceContainer.SetResourceStoredUnits(selectedResource.ResourceID, (double)value);
                    break;

            }
        }
    }
}