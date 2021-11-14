using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/**
 * temporarily Class for testing purposes
 * will be Called from MainMenuUiManager over the temp Test Database Button
 * It generally only calls the DataInterface class or checks for task completion
 */
public class testValueRetriever
{
    
    private string testinput;
    private ConstructionValues databaseOutput;
    private static Boolean resultsPrinted = false;

    private static System.Collections.Generic.List<Task<ConstructionValues>> tasks;

    private static Dictionary<DataInterface.ConstructionVehicleNames, Task<ConstructionValues>> map;


    //recommend always testing only with one method. otherwise the debug statements will be messed up
    public static void readDB(Boolean hasBeenCalledAlready) {

        //tests the getAllVehiclesAsMapAsync method
        testGetAllVehcilesAsMap(hasBeenCalledAlready);

        //method tests the RequestConstructionVehicleAsync mehtod with each object in the ConstructionVehicleNames enum
        //testRequestAllVehiclesSeperated(hasBeenCalledAlready);

    }

    private static void testRequestAllVehiclesSeperated(bool hasBeenCalledAlready)
    {

        if(!hasBeenCalledAlready || resultsPrinted)
        {
            initTasks();
            Debug.Log("Tasks have been initialised. Call the test again for the results");
            resultsPrinted = false;
        }
        else
        {
            printTasks();
        }
    }

    private static void printTasks()
    {
        foreach (Task<ConstructionValues> task in tasks)
        {
            checkTaskForCompletionAndPrint(task);
        }
    }

    private static void initTasks()
    {
        tasks = new System.Collections.Generic.List<Task<ConstructionValues>>();
        foreach (DataInterface.ConstructionVehicleNames vehicle in Enum.GetValues(typeof(DataInterface.ConstructionVehicleNames)))
        {
            tasks.Add(DataInterface.RequestConstructionVehicleAsync(vehicle));
        }
    }



    private static void testGetAllVehcilesAsMap(Boolean hasBeenCalledAlready)
    {  
        if(!hasBeenCalledAlready || resultsPrinted) 
        {
            initMap();
            Debug.Log("map has been initialised. call the test again for results");
            resultsPrinted = false;
        } 
        else
        {

            printMap();
        }
    }

    private static void initMap()
    {
        map = DataInterface.getAllVehiclesAsMapAsync();
    }
    private static void printMap()
    {

        foreach (DataInterface.ConstructionVehicleNames vehicle in Enum.GetValues(typeof(DataInterface.ConstructionVehicleNames)))
        {
            if (!map.ContainsKey(vehicle))
            {
                Debug.Log("ERROR map doesnt contain " + vehicle.ToString());
            }
            else
            {
                Task<ConstructionValues> value;
                map.TryGetValue(vehicle, out value);
                Debug.Log(vehicle.ToString() + " : ");
                checkTaskForCompletionAndPrint(value); 
            }
        }

        resultsPrinted = true;
    }

    private static void checkTaskForCompletionAndPrint(Task<ConstructionValues> task)
    {

        if(task.IsCompleted)
        {
            Debug.Log(task.Result.toString() + " is finished");
            resultsPrinted = true;
        }
        else
        {
            Debug.Log("Task isnt finished yet");
        }
    }

}
