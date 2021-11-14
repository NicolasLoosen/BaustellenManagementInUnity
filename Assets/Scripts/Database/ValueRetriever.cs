using Proyecto26;
using System;
using UnityEngine;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;

/* 
 * This class is used to get the needed values (such as the working area of vehicles) from our database.
 * To get the Data of the specific value, call the DataInterface!
 * 
 */
public class ValueRetriever
{
    //these are the url "parts", which are used to create a request url 
    private static readonly String BaseUrlForWebRequests = "https://baumanagement-9b929.firebaseio.com/";
    private static readonly String applicationJson = "application/json";
    private static readonly String dotJson = ".json?auth="; // ".json?auth=" + idToken to give access to the firebase realtime database
    private static readonly String bearer = "Bearer";

    //Variables to authenticate the user and to check on the token 
    private static String idToken;
    private static readonly String tokenIsOutdatedFailMessage = "Authentication token of Database is expired. It's updating right now, please wait a short moment to receive Data.";
    private static DateTime authTokenGenerationTime;

    //tokenMaxLifetime is the time (in minutes) at which the acces token for the database expires, which is set 3600s == 60 minutes
    private static readonly int tokenMaxLifeTime = 60;
    //tokenMinLifeTime says at how many minutes after token generation, it will be 
    //updated for a new token. (Only if a request is manually done and not automatically)
    private static readonly int tokenMinLifeTime = 50;

    public ValueRetriever()
    {
        initializeToken();
    }


    static void initializeToken()
    {
        generateToken();
    }

    /*
     * Method requests Data from the given BaseUrl 
     * with the endpoint ConstructionVehicleName.json
     * and returns the requested Data, which will then be parsed
     * from Json to ConstructionValues
     */
    public async Task<ConstructionValues> requestData(String ConstructionVehicleName) {
        
        using (HttpClient client = new HttpClient())
        {
            checkAuthTokenLifeTime();

            setUpHttpClientForGetRequest(client);
            //reqeuest Data from Database
            HttpResponseMessage response = await client.GetAsync(ConstructionVehicleName + dotJson + idToken);
            //ensure theres no Error Status Code responding from Database
            response.EnsureSuccessStatusCode();

            //take the response as String (task and convert to String)
            Task<String> responseAsString = response.Content.ReadAsStringAsync();

            //using JsonConvert to convert the Json String (response in Json format) to an ConstructionValues object in the form of a task
            return JsonConvert.DeserializeObject<ConstructionValues>(responseAsString.Result);
        }

    }
    private void checkAuthTokenLifeTime()
    {
        TimeSpan tokenLifeTime = DateTime.Now - authTokenGenerationTime;
        evaluateTokenLifeTime(tokenLifeTime);
    }


    private void evaluateTokenLifeTime(TimeSpan tokenLifeTime)
    {
        if(!checkTokenAlmostExpired(tokenLifeTime))
        {
           checkTokenExpired(tokenLifeTime);   
        }

    }

    /*
     * If the token has reached its max lifetime an error message will be printed and a new token will be generated
     * for the next requests.
     * 
     */
    private void checkTokenExpired(TimeSpan tokenLifeTime)
    {
        if (tokenLifeTime.Minutes >= tokenMaxLifeTime)
        {
            Debug.Log(tokenIsOutdatedFailMessage);
            initializeToken();
        }
    }
    
    /*
     * If the token is in a livetime between its min and max tokenlivetime (defined above)
     * it will be updated for the next requests.
     * 
     */
    private Boolean checkTokenAlmostExpired (TimeSpan tokenLifeTime)
    {
        if (tokenLifeTime.Minutes >= tokenMinLifeTime && tokenLifeTime.Minutes < tokenMaxLifeTime)
        {
            initializeToken();
            return true;
        }
        return false;
    }




    /*
     * This method sets up the needed parameters for the request, before
     * the request will be made
     */
    void setUpHttpClientForGetRequest(HttpClient client)
    {
        client.BaseAddress = new Uri(BaseUrlForWebRequests);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(applicationJson));

        var authValue = new AuthenticationHeaderValue(bearer, idToken);
        client.DefaultRequestHeaders.Authorization = authValue;
    }


    /*
     * This method is used to authenticate the program-user by requesting the idToken of the user, which is identified by the values email and password in the Userdata class. 
     * The token can then be used to connect to the database with the given privileges (in our case readonly).
     */
     public static void generateToken()
     {
         RestClient.Post<AuthentResponse>( Userdata.getSignInUrlForTokenGeneration() + Userdata.getAuthKey(), Userdata.getCombinedUserData()).Then( 
         response =>
         {
             idToken = response.idToken;
             authTokenGenerationTime = DateTime.Now; 
         }).Catch(error =>
         {
             Debug.Log(error);
         });
     
     }

   /*
    * This class contains as the name suggests, the userdata of the database user we created.
    * It exists, to split the data from the rest of the main class.
    *
    */
    class Userdata
    {
        //Web API key in the project settings of the firebase project
        private static readonly String AuthKey = "AIzaSyBaZdsDsMzdD4mMkMKfUKy_ybaJ9PwBZ8w";
        //Login credentials to access the database
        private static readonly String email = "bsmu-htwsaar@firebase.com";
        private static readonly String password = "yktcWDDe2k";

        private static readonly String signInUrlForTokenGeneration = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=";


        public static String getSignInUrlForTokenGeneration()
        {
            return signInUrlForTokenGeneration;
        }
        public static String getAuthKey()
        {
            return AuthKey;
        }

        public static String getCombinedUserData()
        {
            return "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";
        }

    }
}
