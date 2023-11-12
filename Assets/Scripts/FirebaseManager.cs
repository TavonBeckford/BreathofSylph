using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine.UI;

public class FirebaseManager : MonoBehaviour
{


    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;


    //User Data variables
    [Header("UserData")]
    public TMP_InputField usernameField;
    public TMP_Text usernameText;
    public Button resumeGame;

    public delegate void DataLoadedCallback();
    public static event DataLoadedCallback OnDataLoaded;


    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
                
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void Update()
    {
        string status = (string)PlayerPrefs.GetString("SavedGameAvailable");
        Debug.Log(status);
        if (status.Equals("true"))
        {
            resumeGame.interactable = true;
        }
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }


    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));

    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    //Function for the sign out button
    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.instance.LoginScreen();
        ClearRegisterFields();
        ClearLoginFields();

    }

    public void ResetButton()
    {
        StartCoroutine(ResetPassword(emailLoginField.text));
    }


    public void NewGamePlayerSetup()
    {
        StartCoroutine(ClearUserData());
        StartCoroutine(UpdateUsernameDatabase(usernameField.text));
        StartCoroutine(UpdateSavedDataAvailability("false"));
        

    }

    public void ResumeGame()
    {
        StartCoroutine(LoadUserData());
        
    }


    public void SetPlayerHomeScreenUsername()
    {
        usernameText.text = User.DisplayName;
    }


    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            PlayerPrefs.DeleteAll();
            StartCoroutine(FetchSavedDataAvailability());

            SetPlayerHomeScreenUsername();
            UIManager.instance.PlayerHomeScreen();



        }
    }


    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result.User;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    Task ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        UIManager.instance.LoginScreen();
                        ClearRegisterFields();
                        warningRegisterText.text = "";

                    }
                }
            }
        }
    }

    public void ClearRegisterFields()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    public void ClearLoginFields()
    {

        emailLoginField.text = "";
        passwordLoginField.text = "";

    }


    private IEnumerator ResetPassword(string _email)
    {
        //Call the Firebase auth signin function passing the email and password

        Task ResetTask = auth.SendPasswordResetEmailAsync(_email);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ResetTask.IsCompleted);

        if (ResetTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to reset task with {ResetTask.Exception}");
            FirebaseException firebaseEx = ResetTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Reset Failed!";
            switch (errorCode)
            {
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;

            }
            warningRegisterText.text = message;
        }
        else
        {
          
            confirmLoginText.text = "Email Sent";

        }
    }


    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        
        //Set the currently logged in user username in the database
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            Debug.Log("New Game for Player Successfully Injected to Database");

            //PlayerPrefs to setup persistent data for username in the next scene
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("UserId", User.UserId.ToString());
            PlayerPrefs.SetString("PlayerName", _username);
            Debug.Log("This is the player name" + PlayerPrefs.GetString("PlayerName"));
            PlayerPrefs.Save();

            //Debug.Log(PlayerPrefs.GetString("PlayerName"));
            SceneLoaderManager.instance.LoadGameWorld();
        }
        
    }

    private IEnumerator UpdateSavedDataAvailability(string gameStatus)
    {
        //Set the currently logged in user kills
        Task DBTask = DBreference.Child("users").Child(User.UserId).Child("savedgame").SetValueAsync(gameStatus);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Game status updated
        }
    }


    private IEnumerator FetchSavedDataAvailability()
    {
        //Get the currently logged in user data
        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            
            Debug.Log("Unsuccessful Retrieval");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            PlayerPrefs.SetString("SavedGameAvailable", snapshot.Child("savedgame").Value.ToString());
            PlayerPrefs.Save();
        }
    }


    private IEnumerator LoadUserData()
    {
        //Get the currently logged in user data
        Task<DataSnapshot> DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            /*
            //No data exists yet
            xpField.text = "0";
            killsField.text = "0";
            deathsField.text = "0";*/
            Debug.Log("Unsuccessful Retrieval");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            PlayerPrefs.SetString("HitPoints", snapshot.Child("hp").Value.ToString());
            PlayerPrefs.SetString("ManaPoints", snapshot.Child("mana").Value.ToString());
            PlayerPrefs.SetString("Inventory", snapshot.Child("inventory").Value.ToString());
            PlayerPrefs.SetString("SavedGameAvailable", snapshot.Child("savedgame").Value.ToString());
            PlayerPrefs.Save();
            // Notify that the data has been loaded
            OnDataLoaded?.Invoke();
            //SceneLoaderManager.instance.LoadGameWorld();
        }
    }



    private IEnumerator ClearUserData()
    {
        // Clear the data under the user's User.UserId
        Task clearTask = DBreference.Child("users").Child(User.UserId).RemoveValueAsync();

        // Wait for the task to complete
        yield return new WaitUntil(() => clearTask.IsCompleted);

        if (clearTask.Exception != null)
        {
            Debug.LogWarning($"Failed to clear user data with exception: {clearTask.Exception}");
        }
        else
        {
            Debug.Log("Player data purged from database");
        }
    }




}
