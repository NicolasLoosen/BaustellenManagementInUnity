using System;
using System.Collections.Generic;
using System.Threading.Tasks;


/*
 * This class is supposed to be an Interface to get the data for specific 
 * Construction Vehicles
 * 
 * 
 * How to get the data:
 * Simply call the RequestConstructionVehicleAsync method with the vehicle you want, 
 * chosen from the ConstructionVehicleNames enum below.
 * 
 * The method RequestConstructionVehicleAsync returns a generic Task which is of the type
 * ConstructionValues.
 * 
 * A simple example for getting the data of the object Bagger_Raeder :
 * Task<ConstructionVehicle> bagger_Raeder_As_ConstructionVehicle_Task = DataInterface.RequestConstructionVehicleAsync(DataInterface.ConstructionVehiclenames.Bagger_Raeder);
 * 
 * Due to the async execution, this task will probably still be running once you called the method.
 * In this case the completion of bagger_Raeder_As_ConstructionVehicleTask can from now on be checked.
 * This can be done with bagger_Raeder_As_ConstructionVehicleTask.IsCompleted
 * 
 * 
 * If it's completed, you can just get the result as a ConstructionValues object, which is shown in the following line:
 * ConstructionValues bagger_Raeder = bagger_Raeder_As_ConstructionVehicle_Task.Result;
 * 
 * 
 * WARNING.
 * If you want to continously check and wait until an task is finished, don't do a while loop like
 * while(!task.isCompleted) { ... }
 * This might freeze the programm. Rather, try checking it in an update method.
 * 
 * 
 * For adding Data :
 * If you add Objects into the Database, just add its name to the ConstructionVehicles enum below.
 * Please make sure to get the exact same spelling as in the Database (recommend copy pasting the name), because it will be used exactly
 * like that for requesting. If you did this correct, you can now call the RequestConstructionVehicle Method with the 
 * object you just added and therefor get the defined object.
 */
public class DataInterface
{

    static ValueRetriever retriever;

    /**
     * This call ensures that the acces token for the databse will be created.
     * Because this async initialization has to be done before the first request is made, it's important
     * this method will be called when the programm starts.
     */
    public static void init()
    {
        //create the ValueRetriever so the acces Token for the Database can be generated
        retriever = new ValueRetriever();
    }
    
   /*
    * This method is the one that should be used to request from the database and should be called if Values of a certain vehicle are needed. 
    * It returns a Task of the type ConstructionValues.
    */
    public static async Task<ConstructionValues> RequestConstructionVehicleAsync(ConstructionVehicleNames vehicle)
    {
        string vehicleStringForDatabase = getDatabaseStringForVehicle(vehicle);

        return await retriever.requestData(vehicleStringForDatabase);
    }

    static string getDatabaseStringForVehicle(ConstructionVehicleNames vehicle)
    {
        return vehicle.ToString("");
    }

    /**
    * Returns all Vehicles as a map.
    * Uses ConstructionVehicleNames enum as keys.
    * The Values are Tasks, which have to be checked, if they are finished
    */
    public static Dictionary<ConstructionVehicleNames, Task<ConstructionValues>> getAllVehiclesAsMapAsync()
    {
        var map = new Dictionary<ConstructionVehicleNames, Task<ConstructionValues>>();

        foreach (ConstructionVehicleNames vehicle in Enum.GetValues(typeof(ConstructionVehicleNames)))
        {

            //request data for vehicle and add it withk vehicle as key to the map.
            var value = RequestConstructionVehicleAsync(vehicle);

            map.Add(vehicle, value);

        }


        return map;
    }
    
    /*
     * This Enum holds the name for each object in the Database, so you can request every object
     * in the database. This way this list can be used, instead of calling the database everytime.
     * If objects are added to the database, also add its name here. That way you are able to request it via the code.
    */
    public enum ConstructionVehicleNames
    {
         Bagger_Raeder,
         Bagger_Raupe,

         Betonpumpe,

         Container_Pausenraum,
         Container_Sanitaer,

         Gabelstapler,

         LKW_Fahrmischer,
         LKW_Mulde,
         LKW_MuldeAbladbar,
         LKW_Pritsche,
         LKW_PritscheMitKran,

         Mobilkran,

         Turmdrehkran_Obendreher,
         Turmdrehkran_Untendreher

    }
}

                                                                                                