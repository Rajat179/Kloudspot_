using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.Identity.Client;
using UnityEngine.Events;

public class MicrosoftSSO : MonoBehaviour
{
    public event Action<bool, string> OnLogedInLogOutAction;
    //Set the scope for API call to user.read
    string[] scopes = new string[] { "user.read" };
    bool isLoggedIn = false;
    AuthenticationResult authResult;
    private IPublicClientApplication _clientApp;
    private static string ClientId = "e1b29601-72b6-4936-afbd-674f2db85dc5";//app id 

    // Note: Tenant is important for the quickstart.
    private static string Tenant = "813ac086-d549-4a93-af4a-dcb44d577a16";
    private static string Instance = "https://login.microsoftonline.com/";

    // Start is called before the first frame update
    void Start()
    {
        _clientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithRedirectUri("http://localhost")
                .WithAuthority(AzureCloudInstance.AzurePublic, Tenant)
                .Build();
        UnityEngine.Debug.Log("Public client app");
    }

    public void OnLogInOut()
    {
        if (!isLoggedIn)
            StartCoroutine(AuthenticateCoroutine());
        else
            StartCoroutine(SignOutCoroutine());
    }

    IEnumerator SignOutCoroutine()
    {
        var signOutTask = Task.Run(SignOutTask);
        bool isTaskCompleted = false;
        while (!isTaskCompleted)
        {
            if (signOutTask.IsCompleted)
            {
                UnityEngine.Debug.Log($"Logged out.");
                isLoggedIn = false;
                isTaskCompleted = true;
                OnLogedInLogOutAction?.Invoke(false,string.Empty);
            }

            yield return null;
        }
    }

    IEnumerator AuthenticateCoroutine()
    {
        Task<AuthenticationResult> authTask = _clientApp.AcquireTokenInteractive(scopes).WithUseEmbeddedWebView(false) .ExecuteAsync();
        bool isTaskCompleted = false;
        while (!isTaskCompleted)
        {
            if (authTask.IsCompleted)
            {
                // Process the authentication result here
                authResult = authTask.Result;
                UnityEngine.Debug.Log($"Access token: {authResult.AccessToken}");
                isLoggedIn = true;
                OnLogedInLogOutAction?.Invoke(true,authResult.Account.Username);
                isTaskCompleted = true;
            }
            yield return null;
        }
    }

    private async Task SignOutTask()
    {
        var accounts = await _clientApp.GetAccountsAsync();
        if (accounts.Any())
        {
            try
            {
                await _clientApp.RemoveAsync(accounts.FirstOrDefault());
            }
            catch (MsalException ex)
            {
            }
        }
    }

    public async Task<string> GetHttpContentWithToken(string url, string token)
    {
        var httpClient = new System.Net.Http.HttpClient();
        System.Net.Http.HttpResponseMessage response;
        try
        {
            var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, url);
            //Add the token in Authorization header
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            response = await httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        catch (Exception ex)
        {
            return ex.ToString();
        }
    }

}
