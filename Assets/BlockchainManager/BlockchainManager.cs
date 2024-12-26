using System.Collections;
using UnityEngine;
using Thirdweb;
using Thirdweb.Unity;
using TMPro;
using System.Numerics;
using System;
using UnityEngine.SceneManagement;

public class BlockchainManager : MonoBehaviour
{
    private const string playerTokenString = "PlayerToken";
    public TMP_Text logText;
    public TMP_Text balanceText;

    public string Address { get; private set; }
    public static BigInteger ChainId = 204;

    public UnityEngine.UI.Button playButton;
    public UnityEngine.UI.Button claimTokenButton;
    public UnityEngine.UI.Button getBalanceButton;

    string customSmartContractAddress = "0x6d4B65fC4ADd2AC639C46c5E3c46659E4eCFDb40";
    string abiString = "[{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"BallStatusReset\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"status\",\"type\":\"bool\"}],\"name\":\"BallStatusUpdated\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"activateBallStatus\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"getPlayerBallStatus\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"player\",\"type\":\"address\"}],\"name\":\"resetBallStatus\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

    int tokenAmount = 1;

    string notEnoughToken = " BNB";

    private void Start()
    {
        PlayerPrefs.SetInt(playerTokenString, 0);

        int currentTokens = PlayerPrefs.GetInt(playerTokenString, 0);
        if (currentTokens > 0)
        {
            balanceText.gameObject.SetActive(true);
            claimTokenButton.interactable = false;
        }
        else {
            balanceText.gameObject.SetActive(false);
            claimTokenButton.interactable = true;
        }
    }

    public void SwitchToMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void AddPlayerToken()
    {
        int currentTokens = PlayerPrefs.GetInt(playerTokenString, 0);
        currentTokens++;
        PlayerPrefs.SetInt(playerTokenString, currentTokens);
        PlayerPrefs.Save();
        currentTokens = PlayerPrefs.GetInt(playerTokenString, 0);
        balanceText.gameObject.SetActive(true);
    }

    private void HideAllButtons()
    {
        playButton.interactable = false;
        claimTokenButton.interactable = false;
        getBalanceButton.interactable = false;
    }

    private void ShowAllButtons()
    {
        playButton.interactable = true;
        getBalanceButton.interactable = true;

        int currentTokens = PlayerPrefs.GetInt(playerTokenString, 0);
        if (currentTokens > 0)
        {
            balanceText.gameObject.SetActive(true);
            claimTokenButton.interactable = false;
        }
        else
        {
            balanceText.gameObject.SetActive(false);
            claimTokenButton.interactable = true;
        }
    }

    private void UpdateStatus(string messageShow)
    {
        logText.text = messageShow;
    }

    private void BoughtSuccessFully()
    {
        AddPlayerToken();
        UpdateStatus("Got FireUp");
    }
    IEnumerator WaitAndExecute()
    {
        Debug.Log("Coroutine started, waiting for 3 seconds...");
        yield return new WaitForSeconds(3f);
        Debug.Log("3 seconds have passed!");
        BoughtSuccessFully();
        ShowAllButtons();
    }

    private async void Claim1Token()
    {
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        var contract = await ThirdwebManager.Instance.GetContract(
           customSmartContractAddress,
           ChainId,
           abiString
       );
        var address = await wallet.GetAddress();
        await ThirdwebContract.Write(wallet, contract, "activateBallStatus", 0, address);        
    }

    public async void GetTokens()
    {
        HideAllButtons();
        UpdateStatus("Getting FireUp...");
        var wallet = ThirdwebManager.Instance.GetActiveWallet();
        var balance = await wallet.GetBalance(chainId: ChainId);
        var balanceEth = Utils.ToEth(wei: balance.ToString(), decimalsToDisplay: 4, addCommas: true);
        Debug.Log("balanceEth1: " + balanceEth);
        if (float.Parse(balanceEth) <= 0f)
        {
            UpdateStatus("Not Enough" + notEnoughToken);
            ShowAllButtons();
            return;
        }

        StartCoroutine(WaitAndExecute());
        try
        {
            Claim1Token();
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred during the transfer: {ex.Message}");
        }
    }
}
