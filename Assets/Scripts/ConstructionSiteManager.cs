using Assets.Scripts.SaveAndLoad;
using System.Collections.Generic;
using UnityEngine;
using RuntimeGizmos;
using Assets.Scripts;

public class ConstructionSiteManager : MonoBehaviour {
    private Dictionary<string, string> args;
    public ConstructionSiteUiManager constructionSiteUiManager;
    public ConstructionPhasesManager constructionPhasesManager;
    public LoadManager loadManager;

    public TransformGizmo transformGizmo;

    public GameObject polierPrefab;
    public Camera birdsEyeCamera;
    public Vector3 newPolierPosition = new Vector3(0, 2f, 0);

    private Camera polierCamera;
    private GameObject polier;
    private bool polierActive = false;

    private PrefabModificationOption currentModificationOption;

    public PrefabModificationOption CurrentModificationOption { get => currentModificationOption; }
    public int HighestPhase { get => constructionPhasesManager.HighestPhase; }
    public int CurrentPhase { get => constructionPhasesManager.CurrentPhase; }

    public bool IsPolierActive { get => polierActive; }



    // Start is called before the first frame update
    void Start() {
        currentModificationOption = PrefabModificationOption.COMPLETE;
    }

    // Update is called once per frame
    void Update() {

        CheckForPolierInput();

        /*
        if (Input.GetButtonDown(InputAxis.SWITCH_PHASE) && Input.GetAxisRaw(InputAxis.SWITCH_PHASE) > 0) {
            GoToNextPhase();
        }

        if (Input.GetButtonDown(InputAxis.SWITCH_PHASE) && Input.GetAxisRaw(InputAxis.SWITCH_PHASE) < 0) {
            GoToPreviousPhase();
        }
        */
    }
    
    public void GoToNextPhase() {
        constructionPhasesManager.GoToNextPhase();
        Debug.Log("DONE");
        transformGizmo.ClearTargets();
    }

    public void GoToPreviousPhase() {
        constructionPhasesManager.GoToPreviousPhase();
        transformGizmo.ClearTargets();
    }

    public void GoToPhase(int phase)
    {
        constructionPhasesManager.GoToPhase(phase);
        Debug.Log("GotoPhase" + phase);
        transformGizmo.ClearTargets();
    }

    public void AddNewPhase()
    {
        constructionPhasesManager.AddPhases(1);
    }

    public List<Dictionary<int, PlacedObjectState>> GetAllPhases()
    {
        return constructionPhasesManager.PlacedObjectsStates;
    }


    private void CheckForPolierInput() {
        RaycastHit hit;
        newPolierPosition = birdsEyeCamera.gameObject.transform.position;

        if (Input.GetButton(InputAxis.SWITCH_POLIER_VIEW) && Input.GetButtonDown(InputAxis.PRIMARY_CLICK) && polierActive == false) {
            Ray ray = birdsEyeCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out hit)) {
                newPolierPosition = hit.point + (Vector3.up * 2);
            }
        }
        if (Input.GetButtonUp(InputAxis.SWITCH_POLIER_VIEW)) {

            if (polierActive == false) {
                polier = Instantiate(polierPrefab, newPolierPosition, birdsEyeCamera.gameObject.transform.rotation);
                polierCamera = polier.transform.GetChild(0).GetComponent<Camera>();
                polierCamera.gameObject.SetActive(true);
                birdsEyeCamera.gameObject.SetActive(false);
                polierActive = true;
            } else {
                newPolierPosition = polier.transform.position;
                birdsEyeCamera.gameObject.transform.position = polierCamera.gameObject.transform.position;
                birdsEyeCamera.gameObject.transform.rotation = polierCamera.gameObject.transform.rotation;
                polierCamera.gameObject.SetActive(false);
                birdsEyeCamera.gameObject.SetActive(true);
                Destroy(polier);
                polierActive = false;
            }
        }
    }

    private void Awake() {
        args = BsmuSceneManager.GetSceneParameters();

        if (args.ContainsKey("Load")) {
            loadManager.LoadScene(args["Load"]);
            args.Remove("Load");
        }
    }

    public void UpdateObject(GameObject objectToUpdate, PrefabModificationOption option) {
        constructionPhasesManager.UpdateObject(objectToUpdate, option);
    }

    public void DeleteObject(GameObject objectToDelete, PrefabModificationOption option) {
        constructionPhasesManager.removeObjectTaskOrValues(objectToDelete);
        constructionPhasesManager.DeleteObject(objectToDelete, option);
    }

    public void ReplaceObject(GameObject objectToReplace, int indexOfReplacementObject, PrefabType replacementType, PrefabModificationOption option) {
        constructionPhasesManager.ReplaceObject(objectToReplace,indexOfReplacementObject, replacementType, option);
    }

    public void AddObject(int prefabListIndex, PrefabType prefabType, PrefabModificationOption option, Vector3 position) {
        constructionPhasesManager.AddObject(prefabListIndex, prefabType, option, position);
    }

    public bool isDatabaseRequestable(GameObject objectToCheck) {
        return constructionPhasesManager.isDatabaseRequestable(objectToCheck);
    }
    
    public void Request(GameObject objectToRequest)
    {
        constructionPhasesManager.request(objectToRequest);
    }

    public bool checkTask(GameObject objectToCheck)
    {
        return constructionPhasesManager.checkTask(objectToCheck);
    }

    public void drawCircleWithValuesForArbeitsflaeche(GameObject objectToDrawCircleAround) 
    {
        constructionPhasesManager.drawCircleWithValuesForArbeitsflaeche(objectToDrawCircleAround);
    }

    public void drawCircleWithValuesForStandflaeche(GameObject objectToDrawCircleAround) 
    {
        constructionPhasesManager.drawCircleWithValuesForStandflaeche(objectToDrawCircleAround);
    }

    public void drawCircleWithValuesForFahrweg(GameObject objectToDrawCircleAround) 
    {
        constructionPhasesManager.drawCircleWithValuesForFahrweg(objectToDrawCircleAround);
    }

    public void SwitchModificationMode(string option) {
        switch (option) {
            case "COMPLETE":
                currentModificationOption =  PrefabModificationOption.COMPLETE;
                break;

            case "CURRENT_AND_FOLLOWING":
                currentModificationOption = PrefabModificationOption.CURRENT_AND_FOLLOWING;
                break;

            case "ONLY_IN_CURRENT":
                currentModificationOption = PrefabModificationOption.ONLY_IN_CURRENT;
                break;
        }
    }
}
