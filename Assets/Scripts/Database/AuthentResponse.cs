using System;


/* 
 * This class is used to give the authentication response of the Firebase Servers (originally json) 
 * a format we can use. In this case we filter the idToken to uphold a connection to 
 * firebase, which gives us access to the real time database (with read permissions).
 */
[Serializable]
public class AuthentResponse 
{
    //public string localId; //Would be for the uid of the created user
    public string idToken;   //Firebase Auth ID token for the user.
}
