using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Linq;

public class DataManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    [Header("User Data")]
    public TMP_Text playerNameText;
    public Player player;

    private void Awake()
    {
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;

    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PlayerName: " + PlayerPrefs.GetString("PlayerName"));
        Debug.Log("HitPoints: " + PlayerPrefs.GetString("HitPoints"));
        Debug.Log("ManaPoints: " + PlayerPrefs.GetString("ManaPoints"));
        Debug.Log("Inventory: " + PlayerPrefs.GetString("Inventory"));
        Debug.Log("SavedGameAvailable: " + PlayerPrefs.GetString("SavedGameAvailable"));

        player.characterName = PlayerPrefs.GetString("PlayerName") ?? "DefaultPlayerName";
        playerNameText.text = player.characterName;

        // Subscribe to the event
        FirebaseManager.OnDataLoaded += GetData;

        // Call GetData immediately to handle the case where data is already loaded
        GetData();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        FirebaseManager.OnDataLoaded -= GetData;
    }


    // Called when the data is loaded
    private void OnDataLoaded()
    {
        // Do any additional setup or checks if needed

        // Load the game scene
        SceneLoaderManager.instance.LoadGameWorld();
    }



    public void GetData()
    {
        string json = PlayerPrefs.GetString("Inventory");
        List<Items> deserializedItemsList = JsonHelper.FromJson<Items>(json);
        Inventory.instance.items = deserializedItemsList;


        player.hitPoints = int.Parse(PlayerPrefs.GetString("HitPoints"));
        player.manaPoints = int.Parse(PlayerPrefs.GetString("ManaPoints"));
        Debug.Log("The loaded hp val" + int.Parse(PlayerPrefs.GetString("HitPoints")));


    }


   


    public void SaveGame()
    {
        StartCoroutine(InsertSavedUserDataIntoDatabase());
    }


    private IEnumerator InsertSavedUserDataIntoDatabase()
    {

        //Serialize inventory items to save into database
        string jsonInventoryItems = JsonHelper.ToJson<Items>(Inventory.instance.items);

        string UserId = PlayerPrefs.GetString("UserId");
        //HP, Mana, Inventory, EnemyMobs, NPC Companion, Score 
        // Create tasks for setting multiple values
        Task hpTask = DBreference.Child("users").Child(UserId).Child("hp").SetValueAsync(player.hitPoints);
        Task manaTask = DBreference.Child("users").Child(UserId).Child("mana").SetValueAsync(player.manaPoints);
        Task inventoryTask = DBreference.Child("users").Child(UserId).Child("inventory").SetValueAsync(jsonInventoryItems);
        Task savedGameAvailableTask = DBreference.Child("users").Child(UserId).Child("savedgame").SetValueAsync("true");


        // Create an array of tasks to wait for
        Task[] tasksToWaitFor = { hpTask, manaTask, inventoryTask, savedGameAvailableTask };

        // Wait for all tasks to complete
        yield return new WaitUntil(() => Task.WhenAll(tasksToWaitFor).IsCompleted);

        // Check for exceptions in any of the tasks
        if (tasksToWaitFor.Any(task => task.Exception != null))
        {
            foreach (var task in tasksToWaitFor)
            {
                if (task.Exception != null)
                {
                    Debug.LogWarning($"Failed to update data with exception: {task.Exception}");
                }
            }
        }
        else
        {
            // All values are updated successfully
            Debug.Log("Game Saved Successfully");
        }
    }
}
