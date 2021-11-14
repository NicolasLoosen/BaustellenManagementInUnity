using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using SFB;
using Assets.Scripts.SaveAndLoad;
using Assets.Scripts.PrefabClasses;
using Assets.Scripts;
using System.Threading;
using System.Threading.Tasks;

public class ConstructionSiteUiManager : MonoBehaviour
{
    public Button closeSettings;
    public Button settings;
    public Dropdown constDropdown;
    public Text constText;
    public Button buildingButton;
    public Button vehicleButton;
    public Button anyButton;
    public Button placedObjButton;

    public Button saveButton;
    public Button exitButton;
    public Button newphaseButton;
    public Scrollbar saveChange;
    public Slider sliderUI;
    public Slider moveSpeedSlider;
    public Slider scrollSpeedSlider;
    public Slider rotationSpeedSlider;

    public GameObject constObjScrollViewContent;

    public Sprite buttonOnClickImage;
    public Sprite buttonImage;

    public GameObject buildingsTabScrollViewContent;
    public GameObject vehiclesTabScrollViewContent;
    public GameObject miscTabScrollViewContent;
    public GameObject placedObjectsTabScrollViewContent;

    public GameObject listEntryPrefab;

    public GameObject rightClickMenuPrefab;
    public GameObject replaceObjectInfoPrefab;
    public static float snapValue;
    public static float scrollSpeedValue;
    public static float moveSpeedValue;
    public static float rotationSpeedValue;
    public static float GROUNDDIST = 0.1F;  //distance toground

    public GameObject exitConfirmationDialog;
    public GameObject settingsDialog;
    public Canvas canvas;

    private Color onMouseOverButtonColor = new Color(1f, 0f, 0f, 0.5f);
    private Color onListEntryClickedColor = new Color(1f, 0f, 0f, 0.5f);

    private int lastClickedElementIndex;
    private PrefabType lastClickedElementType;
    private bool wasObjectPlaced;
    private Text textSliderValue;
    private Text textSrollSpeed;
    private Text textMoveSpeed;
    private Text textRotationSpeed;

    public Camera birdsEyeCamera;

    private ColorBlock theColor;


    //Is used to check if the mouse hovers over a different list element,
    //and highlight it accordingly
    private GameObject lastClickedListEntry;


    public PrefabModificationOption CurrentModificationOption { get => constructionSiteManager.CurrentModificationOption; }
    public int LastClickedElementIndex { get => lastClickedElementIndex; }
    public PrefabType LastClickedElementType { get => lastClickedElementType; }
    public bool WasObjectPlaced { get => wasObjectPlaced; }

    public ConstructionSiteManager constructionSiteManager;
    public SaveManager saveManager;
    public BirdsEyeViewCameraController cameraController;
    public MessagePrinter messagePrinter;

    private GameObject rightClickMenu;
    private GameObject deleteOptionsMenu;
    private GameObject resetRotationOptionsMenu;
    private GameObject drawCircleOptionsMenu;
    private GameObject replaceObjectInfo;

    private GameObject objectToReplace;

    private bool isExitConfirmationDialogOpen = false;
    private bool isSettingsDialogOpen = false;
    private bool isReplacing;

    private int currentPhase;
    private int currentHighestPhase;

    public Text changeAs;

    public bool IsExitConfirmationDialogOpen
    {
        get
        {
            return isExitConfirmationDialogOpen;
        }
    }

    public bool IsReplacingEnabled { get => isReplacing; }

    private const string BUILDING_LIST_ENTRY_PREFIX = "buildingListEntry_";
    private const string MISC_OBJECTS_LIST_ENTRY_PREFIX = "miscObjectsListEntry_";
    private const string VEHICLES_LIST_ENTRY_PREFIX = "vehiclesListEntry_";
    private const string PLACED_OBJECTS_LIST_ENTRX_PREFIX = "placedObjectsListEntry_";
    private const string UNKNOWN_TYPE_PREFIX= "unknownType_";

    private const string LIST_ENTRY_PREFAB_IMAGE = "PrefabImage";
    private const string LIST_ENTRY_BACKGROUND_IMAGE = "BackgroundImage";
   
    private const string BUILDING_LIST = "BuildingsList";
    private const string VEHICLES_LIST = "VehiclesList";
    private const string MISC_OBJ_LIST = "MiscObjList";

    private const string SHOW_MORE = "+";
    private const string SHOW_LESS = "-";

    private const string MAIN_MENU_SCENE= "MainMenu";



    // Start is called before the first frame update
    void Start()
    {

        wasObjectPlaced = true;
       
        InitDialogs();
        InitObjectsTab();
        InitConstructionPhasesPanel();
        SwitchModificationMode();
        InitMenuButtons();

        currentPhase = constructionSiteManager.CurrentPhase;
        currentHighestPhase = constructionSiteManager.HighestPhase;

        RefreshPlacedObjectsList();
        textSliderValue = sliderUI.GetComponentInChildren<Text>();
        textSrollSpeed = scrollSpeedSlider.GetComponentInChildren<Text>();
        textMoveSpeed = moveSpeedSlider.GetComponentInChildren<Text>();
        textRotationSpeed = rotationSpeedSlider.GetComponentInChildren<Text>();
        UpdateGridSnap();
        ShowSettingsValues();
    }

    private void Update()
    {
        CheckForPhaseChange();

        if (!constructionSiteManager.IsPolierActive)
        {
            CheckForRightClickMenu();
        }

        //If the user rotates the camera, we don't want to display
        //any right click menu
        if (IsCameraRotating())
        {
            DestroyRightClickMenus();
        }

        if (!constructionSiteManager.IsPolierActive)
        {
            CheckForPlacingObject();
        }

        CheckForPlacedObjectListEmpty();

        if (Input.GetButtonDown(InputAxis.UNLOCK_CURSOR) && !constructionSiteManager.IsPolierActive)
        {
            if (isExitConfirmationDialogOpen == false) {
                OpenExitOption();
            }
            else
            {
                CloseExitOption();
            }

        }

    }

    private void CheckForPlacingObject()
    {
        RaycastHit hit;

        if (Input.GetButtonDown(InputAxis.PRIMARY_CLICK) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            DestroyRightClickMenus();

            Ray rayUi = birdsEyeCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(rayUi, out hit))
            {
                if (!WasObjectPlaced)
                {
                    constructionSiteManager.AddObject(
                        LastClickedElementIndex,
                        LastClickedElementType,
                        CurrentModificationOption,
                        hit.point
                    );
                    if (!Input.GetButton(InputAxis.MULTISELECT))
                    {
                        ResetLastClickedElementHighlighting();
                    }
                    RefreshPlacedObjectsList();

                }
            }
        }
    }

    private void InitDialogs()
    {
        isExitConfirmationDialogOpen = false;
        exitConfirmationDialog.SetActive(false);
    }

    private void InitMenuButtons()
    {
        exitButton.onClick.AddListener(OpenExitOption);
        saveButton.onClick.AddListener(() => Save());
        newphaseButton.onClick.AddListener(AddNewPhase);
        buildingButton.onClick.AddListener(()=>SwitchTabVisibility(buildingsTabScrollViewContent.transform,buildingButton));
        vehicleButton.onClick.AddListener(() => SwitchTabVisibility(vehiclesTabScrollViewContent.transform, vehicleButton));
        anyButton.onClick.AddListener(() => SwitchTabVisibility(miscTabScrollViewContent.transform, anyButton));
        placedObjButton.onClick.AddListener(() => SwitchTabVisibility(placedObjectsTabScrollViewContent.transform, placedObjButton));

}

    private void SwitchTabVisibility(Transform child, Button button)
    {
        if (child.gameObject.activeSelf)
        {
            child.gameObject.SetActive(false);
            button.GetComponentInChildren<Text>().text = SHOW_MORE;
            
        }
        else if (child.childCount > 0)
        {
            child.gameObject.SetActive(true);
            button.GetComponentInChildren<Text>().text = SHOW_LESS;
        }
    }

    public void openSettingsMenu()
    {
        settingsDialog.SetActive(true);
        isSettingsDialogOpen = true;
    }

    public void closeSettingsMenu()
    {
        settingsDialog.SetActive(false);
        isSettingsDialogOpen = false;
    }

    private void OpenExitOption()
    {
        exitConfirmationDialog.SetActive(true);
        isExitConfirmationDialogOpen = true;
    }

    public void CloseExitOption()
    {
        exitConfirmationDialog.SetActive(false);
        isExitConfirmationDialogOpen = false;
    }

    public void Exit()
    {
        BsmuSceneManager.Load(MAIN_MENU_SCENE);
    }


    private bool Save()
    {
        string path = StandaloneFileBrowser.SaveFilePanel("Baustelle speichern", Application.dataPath, "Baustelle", "bin");
        if (path != "")
        {
            saveManager.SaveCurrentScene(constructionSiteManager.constructionPhasesManager, path);
        }
        else
        {
            return false;
        }
        return true;
    }

    public void SaveExit()
    {
        if (Save())
        {
            BsmuSceneManager.Load(MAIN_MENU_SCENE);
        }
    }

	 private void InitConstructionPhasesPanel() {
        constDropdown.options.Clear();
        List<Dictionary<int, PlacedObjectState>> allPhases = constructionSiteManager.GetAllPhases();
        List<string> constrPhases = new List<string>();
        string constrphase = "Bauphase ";
        string backslash = "/";
        for (int i = 0; i < allPhases.Count; i++)
        {
            string paseNumber = (i + 1).ToString();
            string phaseText = constrphase + paseNumber + backslash + allPhases.Count;
            constrPhases.Add(phaseText);
        }
        constDropdown.AddOptions(constrPhases);
        changePhaseSelectionText();
        constDropdown.onValueChanged.AddListener(delegate { ConstructionPhaseSelected(); });
    }

	private void ConstructionPhaseSelected()
    {
        constructionSiteManager.GoToPhase(changePhaseSelectionText());
    }

    private void changeExpandCollapsIcon()
    {
        string pathToSprite = "Materials/style01/shape033_style01_color12";
        string arrow = "Arrow";
        Sprite sp = Resources.Load<Sprite>(pathToSprite);
        GameObject.Find(arrow).GetComponent<Image>().sprite = sp;
        constDropdown.RefreshShownValue();
    }

    private int changePhaseSelectionText()
    {
        int index = constDropdown.value;
        constText.text = constDropdown.options[index].text;
        return index;
    }

    private void SwitchModificationMode()
    {
        // round to int to catch floating point problems.
        int currentStep = Mathf.RoundToInt(saveChange.value / (1f / (float)saveChange.numberOfSteps));

        string option = "";
        string optionComplete= "COMPLETE";
        string optionCurrentAndFw= "CURRENT_AND_FOLLOWING";
        string optionOnlyInCurrent= "ONLY_IN_CURRENT";

        string changeComplete = "Änderung: Vollständig";
        string changeCurrentAndFw = "Änderung: Aktuelle und folgende Phasen";
        string changeOnlyInCurrent = "Änderung: Nur aktuelle Phase";

        int step1, step2, step3;
        step1 = 0; step2 = 2; step3 = 3;

        if (currentStep == step1)
            {
                option = optionComplete;
                changeAs.text = changeComplete;

                constructionSiteManager.SwitchModificationMode(option);
            }
            if (currentStep == step2)
            {
                option = optionCurrentAndFw;
                changeAs.text = changeCurrentAndFw;

                constructionSiteManager.SwitchModificationMode(option);
            }

            if (currentStep == step3)
            {
                option = optionOnlyInCurrent;
                changeAs.text = changeOnlyInCurrent;

                constructionSiteManager.SwitchModificationMode(option);
            }
    }

    private void CheckForPhaseChange()
    {
        if (currentPhase != constructionSiteManager.CurrentPhase)
        {
            
            if (currentPhase < constructionSiteManager.CurrentPhase)
            {
                
                if (currentHighestPhase < constructionSiteManager.HighestPhase)
                {
                    currentHighestPhase = constructionSiteManager.HighestPhase;
                }
            }
            currentPhase = constructionSiteManager.CurrentPhase;
            RefreshPlacedObjectsList();
        }
    }

 private void AddNewPhase()
    {
        constructionSiteManager.AddNewPhase();
        InitConstructionPhasesPanel();
        changePhaseSelectionText();
        constDropdown.Show();
    }


    private void InitObjectsTab()
    {
        int buildingListPosition = 1;
        int vehiclesListPosition = 3;
        int miscObjListPosition = 5;
        int placedObjectListPosition =7;
        
        InitBuildingsList();
        InitVehiclesList();
        InitMiscObjectsList();

        ActivateConstObjList(buildingsTabScrollViewContent.transform, buildingListPosition);
        ActivateConstObjList(vehiclesTabScrollViewContent.transform, vehiclesListPosition);
        ActivateConstObjList(miscTabScrollViewContent.transform, miscObjListPosition);
        ActivateConstObjList(placedObjectsTabScrollViewContent.transform, placedObjectListPosition);
    }


    private void ActivateConstObjList(Transform transform, int position)
    {
        transform.SetParent(constObjScrollViewContent.transform);
        transform.SetSiblingIndex(position);
        transform.gameObject.SetActive(false);
    }
    /// <summary>
    /// Listener for the corresponding UI Button
    /// </summary>
    /// 
    private void InitBuildingsList()
    {
        IEnumerable<PrefabContainer> buildings = constructionSiteManager.constructionPhasesManager.prefabRegistry.GetPrefabs(PrefabType.BUILDING);
        InitObjectsList(buildings, PrefabType.BUILDING, buildingsTabScrollViewContent.transform);
    }

    private void InitVehiclesList()
    {
        IEnumerable<PrefabContainer> vehicles = constructionSiteManager.constructionPhasesManager.prefabRegistry.GetPrefabs(PrefabType.VEHICLE);
        InitObjectsList(vehicles, PrefabType.VEHICLE, vehiclesTabScrollViewContent.transform);
    }

    private void InitMiscObjectsList()
    {
        IEnumerable<PrefabContainer> miscObjects = constructionSiteManager.constructionPhasesManager.prefabRegistry.GetPrefabs(PrefabType.MISC);
        InitObjectsList(miscObjects, PrefabType.MISC, miscTabScrollViewContent.transform);
    }


    /// <summary>
    /// Generic method that generates list elements for the tab list view.
    /// </summary>
    /// <param name="elements"></param>
    /// <param name="type"></param>
    /// <param name="parent"></param>
    private void InitObjectsList(IEnumerable<PrefabContainer> elements, PrefabType type, Transform parent)
    {
        string elementNamePrefix;
        string typeNotFound = "Object type not found: type: ";
        switch (type)
        {
            case PrefabType.BUILDING:
                elementNamePrefix = BUILDING_LIST_ENTRY_PREFIX;
                break;
            case PrefabType.VEHICLE:
                elementNamePrefix = VEHICLES_LIST_ENTRY_PREFIX;
                break;
            case PrefabType.MISC:
                elementNamePrefix = MISC_OBJECTS_LIST_ENTRY_PREFIX;
                break;
            default:
                Debug.Log(typeNotFound + type.ToString());
                elementNamePrefix = UNKNOWN_TYPE_PREFIX;
                break;
        }

        foreach (PrefabContainer element in elements)
        {
            GameObject newListEntry = (GameObject)Instantiate(listEntryPrefab, parent, false);
            newListEntry.name = elementNamePrefix + parent.childCount;

            foreach (Image image in newListEntry.transform.GetComponentsInChildren<Image>())
            {
                if (image.name == LIST_ENTRY_PREFAB_IMAGE)
                {
                    image.gameObject.GetComponent<Button>().onClick.AddListener(() => TabListItemClicked(newListEntry));

                    string path = "Icons/" + element.Id;
                    Sprite texture = LoadSprite(path);
                    if (texture != null)
                    {
                        image.sprite = texture;
                        image.transform.Rotate(Vector3.forward * 90);
                    }
                    else
                    {
                        texture = LoadSprite("Materials/Mats/MissingImage");
                        image.sprite = texture;
                    }

                    AddCustomEventHandler(image.gameObject, newListEntry);
                }
                AddCustomEventHandler(newListEntry.GetComponentInChildren<Text>().gameObject, newListEntry);
                newListEntry.GetComponentInChildren<Text>().text = element.Id;
            }
            newListEntry.GetComponentInChildren<Text>().gameObject.GetComponent<Button>().onClick.AddListener(() => TabListItemClicked(newListEntry));
        }
    
    }

    private Sprite LoadSprite(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            var texture = Resources.Load<Texture2D>(path);
            if (texture != null)
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                return sprite;
            }
            else
            {
                try
                {
                    var bytes = System.IO.File.ReadAllBytes(System.IO.Directory.GetCurrentDirectory() + "/ImportIFC/" + path + ".png");
                    texture = new Texture2D(2, 2);
                    texture.LoadImage(bytes);
                }
                catch
                {
                    return null;
                }

                if (texture != null)
                {
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    return sprite;
                }
            }
        }
        return null;
    }

    /// <summary>
    // Adds Listeners to the game object list entries, that take care of coloring the list entries when the mouse moves over them.
    /// </summary>

    private void AddCustomEventHandler(GameObject triggerGameObject, GameObject customEventHandlerTarget)
    {
        triggerGameObject.AddComponent<CustomPointerEnterExitImpl>();
        triggerGameObject.GetComponent<CustomPointerEnterExitImpl>().AddOnPointerEnterCallback(OnPointerEnterOverPrefabImage, customEventHandlerTarget);
        triggerGameObject.GetComponent<CustomPointerEnterExitImpl>().AddOnPointerExitCallback(OnPointerExitFromPrefabImage, customEventHandlerTarget);
    }

    private void OnPointerEnterOverPrefabImage(GameObject listEntry)
    {
        if (IsExitConfirmationDialogOpen)
        {
            return;
        }
        //Change Color of other button
        foreach (Image image in listEntry.GetComponentsInChildren<Image>())
        {
            if (image.gameObject.name == LIST_ENTRY_BACKGROUND_IMAGE)
            {
                image.color = onMouseOverButtonColor;
            }
        }
    }

    private void OnPointerExitFromPrefabImage(GameObject listEntry)
    {
        if (IsExitConfirmationDialogOpen)
        {
            return;
        }
        //Reverse color change only if element was not clicked on
        if (lastClickedListEntry != listEntry)
        {
            foreach (Image image in listEntry.GetComponentsInChildren<Image>())
            {
                if (image.gameObject.name == LIST_ENTRY_BACKGROUND_IMAGE)
                {
                    image.color = new Color(0f, 0f, 0f, 0f); //Transparent Image
                }
            }
        }
    }

    private void TabListItemClicked(GameObject listEntry)
    {
        if (IsExitConfirmationDialogOpen)
        {
            return;
        }

        ResetLastClickedElementHighlighting();

        wasObjectPlaced = false;
        lastClickedListEntry = listEntry;

        GameObject clickedObjectParent = listEntry.transform.parent.gameObject;
       
        if (clickedObjectParent.name== BUILDING_LIST)
        {
            lastClickedElementType = PrefabType.BUILDING;

        }
        else if (clickedObjectParent.name == VEHICLES_LIST)
        {
            lastClickedElementType = PrefabType.VEHICLE;

        }
        else if (clickedObjectParent.name == MISC_OBJ_LIST)
        {
            lastClickedElementType = PrefabType.MISC;

        }

        lastClickedElementIndex = listEntry.transform.GetSiblingIndex();

        foreach (Image image in listEntry.GetComponentsInChildren<Image>())
        {
            if (image.name == LIST_ENTRY_BACKGROUND_IMAGE)
            {
                image.color = onListEntryClickedColor;
            }
        }

        if (IsReplacingEnabled)
        {
            constructionSiteManager.ReplaceObject(objectToReplace, lastClickedElementIndex, lastClickedElementType, constructionSiteManager.CurrentModificationOption);
            ResetLastClickedElementHighlighting();
            DestroyReplaceObjectInfo();
            RefreshPlacedObjectsList();
            wasObjectPlaced = true;
            isReplacing = false;
        }
    }

   

    public void ResetLastClickedElementHighlighting()
    {
        if (lastClickedListEntry != null)
        {//&& !listEntry.Equals(lastClickedListEntry)) {
            wasObjectPlaced = true;

            foreach (Image image in lastClickedListEntry.GetComponentsInChildren<Image>())
            {
                if (image.name == LIST_ENTRY_BACKGROUND_IMAGE)
                {
                    image.color = new Color(0f, 0f, 0f, 0f); //Transparent Image
                }
            }
            lastClickedListEntry = null;
        }
    }

    public void ShowSettingsValues()
    {

        moveSpeedValue = moveSpeedSlider.value / 100;
        rotationSpeedValue = rotationSpeedSlider.value / 10 * 2;
        scrollSpeedValue = scrollSpeedSlider.value;

        textSliderValue.text = sliderUI.value.ToString();
        textMoveSpeed.text = moveSpeedValue.ToString();
        textRotationSpeed.text = rotationSpeedValue.ToString();
        textSrollSpeed.text = scrollSpeedValue.ToString();
            
    }

    public void UpdateGridSnap()
    {
        snapValue = sliderUI.value;
        
    }

    public void UpdateScrollSpeed()
    {
        cameraController.scrollSpeed = scrollSpeedValue;
    }
    public void UpdateMoveSpeed()
    {
        cameraController.moveSpeed = moveSpeedValue;
    }
    public void UpdateRotationSpeed()
    {
        cameraController.rotationSpeed = rotationSpeedValue;
    }

    public void RefreshPlacedObjectsList()
    {
        Dictionary<int, PrefabContainer> placedObjects = constructionSiteManager.constructionPhasesManager.PlacedObjects;
        Dictionary<int, Assets.Scripts.PlacedObjectState> currentPlacedObjectsStates = constructionSiteManager.constructionPhasesManager.PlacedObjectStatesInCurrentPhase;

        foreach (Transform child in placedObjectsTabScrollViewContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (PrefabContainer placedObjectContainer in placedObjects.Values)
        {
            GameObject newListEntry = (GameObject)Instantiate(listEntryPrefab, placedObjectsTabScrollViewContent.transform);
            newListEntry.name = PLACED_OBJECTS_LIST_ENTRX_PREFIX + placedObjectsTabScrollViewContent.transform.childCount;

            PlacedObjectState placedObjectState;

            currentPlacedObjectsStates.TryGetValue(placedObjectContainer.Prefab.GetInstanceID(), out placedObjectState);


            newListEntry.GetComponentInChildren<Text>().text = placedObjectContainer.Id + "\n" + "Status: " + placedObjectState.IsActive;

            foreach (Image image in newListEntry.transform.GetComponentsInChildren<Image>())
            {
                if (image.name == LIST_ENTRY_PREFAB_IMAGE)
                {
                    image.gameObject.GetComponent<Button>().onClick.AddListener(() => AllObjectsListItemClicked(placedObjectContainer.Prefab, newListEntry));
                    image.gameObject.AddComponent<ClickableObject>();
                    image.gameObject.GetComponent<ClickableObject>().setObjectToClick(placedObjectContainer.Prefab);
                    image.gameObject.GetComponent<ClickableObject>().setConstructionSiteManager(this);

                    string path = "Icons/" + placedObjectContainer.Id;
                    Sprite texture = LoadSprite(path);
                    if (texture != null)
                    {
                        image.sprite = texture;
                        image.transform.Rotate(Vector3.forward * 90);
                    }
                    else
                    {
                        texture = LoadSprite("Materials/Mats/MissingImage");
                        image.sprite = texture;
                    }

                    AddCustomEventHandler(image.gameObject, newListEntry);
                }
                AddCustomEventHandler(newListEntry.GetComponentInChildren<Text>().gameObject, newListEntry);
            }
            newListEntry.GetComponentInChildren<Text>().gameObject.AddComponent<ClickableObject>();
            newListEntry.GetComponentInChildren<Text>().gameObject.GetComponent<ClickableObject>().setObjectToClick(placedObjectContainer.Prefab);
            newListEntry.GetComponentInChildren<Text>().gameObject.GetComponent<ClickableObject>().setConstructionSiteManager(this);
            newListEntry.GetComponentInChildren<Text>().gameObject.GetComponent<Button>().onClick.AddListener(() => AllObjectsListItemClicked(placedObjectContainer.Prefab, newListEntry));
        }
        placedObjectsTabScrollViewContent.transform.gameObject.SetActive(false);
        placedObjButton.GetComponentInChildren<Text>().text = SHOW_MORE;
    }



    private void AllObjectsListItemClicked(GameObject objectToUpdate, GameObject listEntry)
    {
        if (objectToUpdate.activeSelf)
        {
            objectToUpdate.SetActive(false);
        }
        else
        {
            objectToUpdate.SetActive(true);
        }
        constructionSiteManager.constructionPhasesManager.UpdateObject(objectToUpdate, PrefabModificationOption.ONLY_IN_CURRENT);
        listEntry.GetComponentInChildren<Text>().text = objectToUpdate.name + "\n" + "Status: " + objectToUpdate.activeSelf;
    }


    // check if Placed object List is empty and close it
    private void CheckForPlacedObjectListEmpty()
    {
        if (placedObjectsTabScrollViewContent.transform.childCount == 0 && placedObjButton.GetComponentInChildren<Text>().text == SHOW_LESS)
        {
            placedObjectsTabScrollViewContent.transform.gameObject.SetActive(false);
            placedObjButton.GetComponentInChildren<Text>().text = SHOW_MORE;
        }
    }


    public void CreateRightClickMenu(Vector3 menuPosition, GameObject associatedObject)
    {
        const string caseDelete="Delete";
        const string caseReplace = "Replace"; 
        const string caseResetRotation = "ResetRotation"; 
        const string caseArbeitsFl = "Arbeitsflaeche"; 
        const string caseStandFl = "Standflaeche"; 
        const string caseFahrweg = "Fahrweg"; 
        const string caseCancel ="Cancel";

        string delete, replace, resetRt, requestData, workingSurface, noData, requestingData, driveway, cancel, plattform;
        delete = "Löschen"; replace = "Ersetzen"; resetRt = "Rotation zurücksetzen"; requestData = "Daten anfragen"; workingSurface= "Arbeitsfläche";
        requestingData = "Daten werden angefragt"; noData= "Keine Daten in Datenbank"; plattform= "Standfläche";  driveway= "Fahrweg"; cancel= "Abbrechen";

        ResetLastClickedElementHighlighting();
        if (rightClickMenu)
        {
            DestroyRightClickMenu();
        }
        rightClickMenu = (GameObject)Instantiate(rightClickMenuPrefab, menuPosition, Quaternion.identity, canvas.transform);

        foreach (Button button in rightClickMenu.transform.GetComponentsInChildren<Button>())
        {
            Debug.Log(associatedObject);
            Debug.Log(menuPosition);
            switch (button.gameObject.name)
            {
                case caseDelete : 
                    button.GetComponentInChildren<Text>().text = delete;
                    button.onClick.AddListener(() =>
                    {
                        constructionSiteManager.DeleteObject(associatedObject, constructionSiteManager.CurrentModificationOption);
                    });
                    break;

                case caseReplace:
                    button.GetComponentInChildren<Text>().text = replace;
                    button.onClick.AddListener(() =>
                    {
                        CreateReplaceObjectInfo(menuPosition);
                    });
                    objectToReplace = associatedObject;
                    break;

                case caseResetRotation :
                    button.GetComponentInChildren<Text>().text = resetRt;
                    button.onClick.AddListener(() =>
                    {
                        ObjectData objectdata = associatedObject.GetComponent<ObjectData>();
                        if (objectdata != null)
                        {
                            associatedObject.transform.rotation = objectdata.rotation;
                        }
                        constructionSiteManager.UpdateObject(associatedObject, constructionSiteManager.CurrentModificationOption);
                    });
                    break;

                /*
                 * serves as data request button at first then gets changed to button that can draw Arbeitsflaeche:
                 *
                 * 1: data gets requested over the "daten anfragen" button
                 *
                 * 2: data gets put into task dictionary in constructionsitephasesmanager "databaseTasks"
                 *
                 * 3: if task is finished it gets put into "databaseValues" in constructionphasesmanager
                 *
                 * 4: data can be drawn out with respective function
                 */
                case caseArbeitsFl:
                    if (constructionSiteManager.isDatabaseRequestable(associatedObject))
                    {
                        button.interactable = true;
                        //checks if the data has already been requested
                        if (!(constructionSiteManager.constructionPhasesManager.DatabaseTasks.ContainsKey(associatedObject.GetInstanceID())))
                        {
                            //mode 1: the button becomes a request button for the database
                            button.interactable = true;
                            button.GetComponentInChildren<Text>().text = requestData;
                            button.onClick.AddListener(() =>
                            {
                                constructionSiteManager.Request(associatedObject);
                            });
                        }
                        else
                        {
                            if (constructionSiteManager.checkTask(associatedObject))
                            {
                                //mode 3: the data is ready and can be shown
                                button.GetComponentInChildren<Text>().text = workingSurface;
                                button.GetComponentInChildren<Text>().color = Color.red;
                                button.onClick.AddListener(() =>
                                {
                                    constructionSiteManager.drawCircleWithValuesForArbeitsflaeche(associatedObject);
                                });
                            }
                            else
                            {
                                //mode 2: the data is being requested and the button cant be pressed
                                button.interactable = false;
                                button.GetComponentInChildren<Text>().text = requestingData;
                                button.onClick.RemoveAllListeners();
                            }
                        }
                    }
                    else
                    {
                        //if the object is not in the database the button becomes unpressable 
                        button.interactable = false;
                        button.GetComponentInChildren<Text>().text = noData;
                    }
                    break;

                case caseStandFl: 
                    button.interactable = false;
                    button.GetComponentInChildren<Text>().text = plattform;
                    if (constructionSiteManager.isDatabaseRequestable(associatedObject) && (constructionSiteManager.constructionPhasesManager.DatabaseTasks.ContainsKey(associatedObject.GetInstanceID())))
                    {
                        if (constructionSiteManager.checkTask(associatedObject))
                        {
                            button.interactable = true;
                            button.GetComponentInChildren<Text>().color = Color.green;
                            button.onClick.AddListener(() =>
                            {
                                constructionSiteManager.drawCircleWithValuesForStandflaeche(associatedObject);
                            });
                        }
                    }
                    break;

                case caseFahrweg:
                    button.interactable = false;
                    button.GetComponentInChildren<Text>().text = driveway;
                    if (constructionSiteManager.isDatabaseRequestable(associatedObject) && (constructionSiteManager.constructionPhasesManager.DatabaseTasks.ContainsKey(associatedObject.GetInstanceID())))
                    {
                        if (constructionSiteManager.checkTask(associatedObject))
                        {
                            button.interactable = true;
                            button.GetComponentInChildren<Text>().color = Color.blue;
                            button.onClick.AddListener(() =>
                            {
                                constructionSiteManager.drawCircleWithValuesForFahrweg(associatedObject);
                            });
                        }
                    }
                    break;

                case caseCancel:
                    button.GetComponentInChildren<Text>().text = cancel;
                    break;
            }

            button.onClick.AddListener(() =>
            {
                DestroyRightClickMenu();
                RefreshPlacedObjectsList();
            });
        }
    }


      private void CreateReplaceObjectInfo(Vector3 position)
      {
        string cancel = "Abbrechen";
          if (rightClickMenu)
          {
              DestroyRightClickMenu();
          }
          replaceObjectInfo = (GameObject)Instantiate(replaceObjectInfoPrefab, position, Quaternion.identity, canvas.transform);
          isReplacing = true;

          Button cancelButton = replaceObjectInfo.GetComponentInChildren<Button>();

          cancelButton.onClick.AddListener(() =>
          {
              DestroyReplaceObjectInfo();
              objectToReplace = null;
              isReplacing = false;
          });
          cancelButton.GetComponentInChildren<Text>().text = cancel;
      }

      public void DestroyRightClickMenus()
      {
          DestroyRightClickMenu();
          DestroyDeleteOptionsMenu();
          DestroyReplaceObjectInfo();
          DestroyResetRotationsOptionsMenu();
          DestroyDrawCircleOptionsMenu();
      }
      public void DestroyRightClickMenu()
      {
          Destroy(rightClickMenu);
          rightClickMenu = null;
      }

      public void DestroyResetRotationsOptionsMenu()
      {
          Destroy(resetRotationOptionsMenu);
          resetRotationOptionsMenu = null;
      }

      public void DestroyDrawCircleOptionsMenu()
      {
          Destroy(drawCircleOptionsMenu);
          drawCircleOptionsMenu = null;
      }

      public void DestroyDeleteOptionsMenu()
      {
          Destroy(deleteOptionsMenu);
          deleteOptionsMenu = null;
      }

      public void DestroyReplaceObjectInfo()
      {
          Destroy(replaceObjectInfo);
          replaceObjectInfo = null;
      }

    private void CheckForRightClickMenu()
    {
        if (Input.GetMouseButtonDown(1) && !IsCameraRotating())
        {
            DestroyRightClickMenus();
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool moveable = true;
                bool miss = false;

                if (Physics.Raycast(ray, out hit, 500))
                {
                    try
                    {
                        bool test = hit.transform.gameObject.GetComponent<ObjectData>().AllowMove;
                    }
                    catch (MissingComponentException)
                    {
                        moveable = false;
                    }
                    catch (NullReferenceException)
                    {
                        miss = true;
                    }
                    if (moveable && !miss)
                    {  ////todo remove dependency from hardcoded string
                        CreateRightClickMenu(Input.mousePosition, hit.transform.gameObject);
                    }
                }
            }
        }
    }


    private bool IsCameraRotating()
    {
        float y = Input.GetAxis(InputAxis.MOUSE_X);
        float x = Input.GetAxis(InputAxis.MOUSE_Y);
        bool cameraRotation = false;

        if (Input.GetButtonDown(InputAxis.SECONDARY_CLICK))
        {
            if (x != 0f || y != 0f)
            {
                cameraRotation = true;
            }
        }
        return cameraRotation;
    }

}
