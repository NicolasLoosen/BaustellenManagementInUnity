using SFB;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUiManager : MonoBehaviour {

    public Button newSiteButton;
    //public Button loadSiteButton;
    public Button exitButton;
    public Button loadSiteButton;
    /***public Button testdbButton;
    
    //temp boolean for testing database with testdbButton
    /***private Boolean hasDatabaseBeenCalledYet;*/


    // Start is called before the first frame update
    void Start() {
        InitDataBase();
        //newSiteButton.GetComponentInChildren<Text>().text = "Neue Baustelle anlegen";
        //loadSiteButton.GetComponentInChildren<Text>().text = "Baustelle aus Datei laden";
        //exitButton.GetComponentInChildren<Text>().text = "Beenden";
        /***testdbButton.GetComponentInChildren<Text>().text = "Test database";*/


        newSiteButton.onClick.AddListener(CreateNewConstructionSite);
        exitButton.onClick.AddListener(ExitApplication);
        loadSiteButton.onClick.AddListener(LoadConstructionSite);
        

        /***hasDatabaseBeenCalledYet = false;
        
        /***testdbButton.onClick.AddListener(testDB); This button is used to test the functionality of the Database via the testDB method */
 
    }
    
    private void LoadConstructionSite()
    {
        string[] path = StandaloneFileBrowser.OpenFilePanel("Baustelle laden", Application.dataPath, "bin", false);
        if(path.Length > 0)
        {
            BsmuSceneManager.SetParam("Load", path[0]);
            BsmuSceneManager.Load("ConstructionSite");
        }
    }

    private void CreateNewConstructionSite() {
        BsmuSceneManager.Load("ConstructionSite");
    }

    private void ExitApplication() { 
        Application.Quit();
    }

    /* This method uses the testclass testValueRetriever and it's method readDB
     * It is used to iterate over all the known ConstructionValues and test the functionality 
     * of the database via debuglogs. To use this button and the assosiated class, just remove the /*** 
     * and add a TestDB Button to the Canvas>Panel that can then be used */
  /***
      private void testDB()
    {
        testValueRetriever.readDB(hasDatabseBeenCalledYet);
        hasDatabaseBeenCalledYet = true;
    }
    */


    private void InitDataBase()
    {
        DataInterface.init();
    }


}
