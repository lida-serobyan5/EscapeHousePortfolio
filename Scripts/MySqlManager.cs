using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public static class MySqlManager
{
    readonly static string SERVER_URL = "http://localhost/Users/";

    public static async Task<bool> RegisterUser(string email, string password, string username) {
        string REGISTER_USER_URL = $"{SERVER_URL}/RegisterUser.php";

        return (await SendPostRequest(REGISTER_USER_URL, new Dictionary<string, string>()
        {
            {"email", email},
            {"username", username},
            {"password", password}
        })).success;

    }

    public static async Task<(bool success, string username)> LoginUser(string email, string password)
    {
        string LOGIN_USER_URL = $"{SERVER_URL}/Login.php";

        return await SendPostRequest(LOGIN_USER_URL, new Dictionary<string, string>()
        {
            {"email", email},
            {"password", password}
        });

    }

    static async Task<(bool success, string returnMessage)> SendPostRequest(string url, Dictionary<string, string> data)
    {
        using (UnityWebRequest req = UnityWebRequest.Post(url, data))
        {
            req.SendWebRequest();

            while (!req.isDone) await Task.Delay(100);

            //when the task is done
            if (req.error != null || !string.IsNullOrWhiteSpace(req.error) || HasErrorMessage(req.downloadHandler.text))
                return (false, req.downloadHandler.text);

            //on success
            return (true, req.downloadHandler.text);
        }
    }

    static bool HasErrorMessage(string msg) => int.TryParse(msg, out var res);
} 

public class DatabaseUser
{
    public string Email;
    public string Username;
    public string Password;
}