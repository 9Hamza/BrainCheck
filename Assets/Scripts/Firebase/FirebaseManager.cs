using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using Google.MiniJSON;
using Newtonsoft.Json;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    private static DatabaseReference _mainDB;
    private static DatabaseReference _usersDB;
    private static DatabaseReference _friendRequestsDB;

    public List<User> users;

    private void Awake()
    {
        _mainDB = FirebaseDatabase.DefaultInstance.RootReference;
        _usersDB = _mainDB.Child("users");
        _friendRequestsDB = _mainDB.Child("friendRequests");

        Debug.Log(FriendRequestStatus.Pending);
        Debug.Log((int)FriendRequestStatus.Pending);

        int[][] matrix = new int[][]
        {
            new int[] { 1, 2, 3 },
            new int[] { 4, 5, 6 },
            new int[] { 7, 8, 9 }
        };

        int row = 1;
        int column = 2;
        int value = matrix[row][column];

        Debug.Log("Value at row " + row + ", column " + column + ": " + value);

    }

    private void Start()
    {
        // User _rammo = new User("Rammo", "ahmadb2000@gmail.com", "Ahmad Ramadhan");
        // WriteNewUser(_rammo);
        
        // WriteNewUser(new User()
        // {
        //     Username = "Rammo",
        //     Email = "ahmadb2000@gmail.com",
        //     Name = "Ahmad Ramadhan"
        // });
        
        SetUpUsers();
        TestSendFriendRequestAsync();
        
        // GetUsersFromDatabase();
        // RetrieveUsers();
    }

    private async void SetUpUsers()
    {
        users = new List<User>();
        var usersFromDB = await RetrieveAllUserValues();
        users = usersFromDB;

        // foreach (User user in users)
        // {
        //     Debug.Log(user.Username);
        // }
    }

    private async void TestSendFriendRequestAsync()
    {
        FriendRequest friendRequest = new FriendRequest
        {
            senderID = "hamboozy",
            receiverID = "evilghostlee",
            status = 2
        };
        var json = JsonUtility.ToJson(friendRequest);
        await _friendRequestsDB.Push().SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log("-- Sent a friend request successfully --");
            }
            else
            {
                Debug.Log($"-- Error: {task.Exception} --");
            }
        } );
    }

    private void WriteNewUser(User user)
    {
        Debug.Log($"-- {nameof(WriteNewUser)} --");
        var json = JsonUtility.ToJson(user);
        _mainDB.Child("users").Push().SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log($"-- {nameof(WriteNewUser)} -- completed successfully");
            }
            else
            {
                Debug.Log($"-- Error -- {task.Exception}");
            }
        });
    }

    private void WriteNewUser(string username, string fullName, string email)
    {
        Debug.Log($"-- {nameof(WriteNewUser)} --");
        User user = new User(username, fullName, email);
        
        var json = JsonUtility.ToJson(user);
        _mainDB.Child("users").Push().SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log($"-- {nameof(WriteNewUser)} -- completed successfully");
            }
            else
            {
                Debug.Log($"-- Error -- {task.Exception}");
            }
        });
    }

    private void GetUsersFromDatabase()
    {
        Debug.Log($"-- {nameof(GetUsersFromDatabase)} --");
        
        // User[] users;
        _mainDB.Child("users").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                // Debug.Log($"-- {nameof(GetUsersFromDatabase)} -- task is completed successfully.");
                // Debug.Log(JsonUtility.FromJson<User>(task.Result.GetRawJsonValue()));
                // Debug.Log(task.Result.GetRawJsonValue());
                // Debug.Log(task.Result);
                Debug.Log(Json.Deserialize(task.Result.GetRawJsonValue()));
            
                Dictionary<string, object> usersData = (Dictionary<string, object>)Json.Deserialize(task.Result.GetRawJsonValue());

                // foreach (var kvp in usersData)
                // {
                //     Dictionary<string, object> userData = (Dictionary<string, object>)kvp.Value;
                //     foreach (var data in userData)
                //     {
                //         Debug.Log(data.Key);
                //     }
                // }
                
                // Convert each user to User class
                List<User> userList = new List<User>();
                foreach (var userData in usersData)
                {
                    Dictionary<string, object> userDataDict = (Dictionary<string, object>)userData.Value;

                    // This is case sensitive so MAKE SURE the spelling is the same =))))))
                    string username = userDataDict["Username"].ToString();
                    string name = userDataDict["Name"].ToString();
                    string email = userDataDict["Email"].ToString();

                    User user = new User(username, name, email);
                    userList.Add(user);
                }

                // Do something with the retrieved users
                foreach (User user in userList)
                {
                    Debug.Log("Username: " + user.Username);
                    Debug.Log("Name: " + user.Name);
                    Debug.Log("Email: " + user.Email);
                }
                
            }
        });
    }
    
    // Heavily referenced from Ludwik code in Ghostlee/FirebaseDBController.cs
    public static async Task<List<User>> RetrieveAllUserValues()
    {
        var values = await _usersDB.GetValueAsync();
        var snapshot = values.GetRawJsonValue();
        var decompress = JsonConvert.DeserializeObject<Dictionary<string, object>>(snapshot);
        
        var users = new List<User>();
        foreach (KeyValuePair<string, object> keyValuePair in decompress)
        {
            users.Add(JsonUtility.FromJson<User>(keyValuePair.Value.ToString()));
        }

        return users;
    }
    
    private void RetrieveUsers()
    {
        Debug.Log($"-- {nameof(RetrieveUsers)} --");
        // Listen for changes in the "users" node
        FirebaseDatabase.DefaultInstance.GetReference("users").ValueChanged += HandleUsersValueChanged;
    }

    private void HandleUsersValueChanged(object sender, ValueChangedEventArgs args)
    {
        Debug.Log($"-- {nameof(HandleUsersValueChanged)} --");
        if (args.DatabaseError != null)
        {
            Debug.LogError("DatabaseError: " + args.DatabaseError.Message);
            return;
        }

        if (args.Snapshot != null && args.Snapshot.Exists)
        {
            // Convert JSON data to a dictionary
            Dictionary<string, object> usersData = (Dictionary<string, object>)Json.Deserialize(args.Snapshot.GetRawJsonValue());

            // Convert each user to User class
            List<User> userList = new List<User>();
            foreach (var userData in usersData)
            {
                Dictionary<string, object> userDataDict = (Dictionary<string, object>)userData.Value;

                string username = userDataDict["username"].ToString();
                string name = userDataDict["name"].ToString();
                string email = userDataDict["email"].ToString();

                User user = new User(username, name, email);
                userList.Add(user);
            }

            // Do something with the retrieved users
            foreach (User user in userList)
            {
                Debug.Log("Username: " + user.Username);
                Debug.Log("Name: " + user.Name);
                Debug.Log("Email: " + user.Email);
            }
        }
    }

}

public enum FriendRequestStatus
{
    Accepted,       // 0
    Declined,       // 1
    Pending         // 2
}

[Serializable]
public class FriendRequest
{
    public string senderID;
    public string receiverID;
    public int status;
}
