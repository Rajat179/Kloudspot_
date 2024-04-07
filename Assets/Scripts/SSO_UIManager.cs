using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class SSO_UIManager : MonoBehaviour
{
    public MicrosoftSSO microsoftSSORef;
    [SerializeField]
    private TMP_Text userNameUI;
    [SerializeField]
    private Button logInOutButton;
    [SerializeField]
    private GameObject whenLogedOut;
    [SerializeField]
    private GameObject whenLogedin;

    [SerializeField]
    private Button NextSceneBtn;

    private void Start()
    {
        microsoftSSORef.OnLogedInLogOutAction += UiChangesOnUserLoginLogOut;
    }
    private void OnDestroy()
    {
        microsoftSSORef.OnLogedInLogOutAction -= UiChangesOnUserLoginLogOut;
    }

    //SET UP UI 
    private void UiChangesOnUserLoginLogOut(bool isLogedIn, string userName)
    {
        if (isLogedIn)
        {
            logInOutButton.GetComponentInChildren<TextMeshProUGUI>().text = "Log Out";
            whenLogedin.SetActive(false);
            whenLogedOut.SetActive(true);
            userNameUI.GetComponent<TMP_Text>().text = "User Name : " + userName;
            NextSceneBtn.gameObject.SetActive(true);

        }
        else
        {
            logInOutButton.GetComponentInChildren<TextMeshProUGUI>().text = "Log In";
            whenLogedin.SetActive(true);
            whenLogedOut.SetActive(false);
            userNameUI.GetComponent<TMP_Text>().text = string.Empty;
            NextSceneBtn.gameObject.SetActive(false);
        }
    }

    public void GotoNextScene()
    {
        SceneManager.LoadScene("API Pagination", LoadSceneMode.Additive);
    }
}
