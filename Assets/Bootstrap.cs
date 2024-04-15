using System;
using System.Collections;
using UnityEngine;
using LootLocker.Requests;
using TMPro;
using Unity.VisualScripting;

public class Bootstrap : MonoBehaviour
{
    private string _holderUlid;
    private string _walletID;

    [SerializeField] private string _balance;
    [SerializeField] private string _characterID;

    public TextMeshProUGUI moneyText;
    
    private void Awake()
    {
        StartCoroutine(GuestLogin());
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            AddGold("1");
        }
    }

    IEnumerator GuestLogin()
    {
        yield return new WaitForSeconds(1);
        bool hasConnected = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Guest session started");
                _holderUlid = response.player_ulid;
                hasConnected = true;
            }
            else
            {
                Debug.Log("Error" + response.errorData.message);
                hasConnected = true;
            }
        });

        yield return new WaitUntil(() => hasConnected);
        Debug.Log("Guest login complete");
        
        LootLockerSDKManager.GetPlayerProgression("plevel", (response) =>
        {
            if (!response.success)
            {
                Debug.Log("Error getting progression tier: " + response.errorData.message);
            }
            else
            {                
                Debug.Log("Progression tier: " + response.points);
            }
        });

        StartCoroutine(GetWallet());
    }

    #region  GetPlayerInfo
    
    IEnumerator GetWallet()
    {
        bool responseReceived = false;
        LootLocker.LootLockerEnums.LootLockerWalletHolderTypes player = LootLocker.LootLockerEnums.LootLockerWalletHolderTypes.player;
        LootLockerSDKManager.GetWalletByHolderId(_holderUlid, player, (response) =>
        {
            if(!response.success)
            {
                //If wallet is not found, it will automatically create one on the holder.
                Debug.Log("error: " + response.errorData.message);
                Debug.Log("request ID: " + response.errorData.request_id);
                responseReceived = true;
            }
            else
            {
                _walletID = response.id;
                responseReceived = true;
            }
        });
        yield return new WaitUntil(() => responseReceived);
        
        Debug.Log("Found wallet " + _walletID);
        StartCoroutine(GetMoneyAmount());

    }
    
    
    private void AddGold(string amount)
    {
        int Bal;
        try
        {
            Bal = int.Parse(_balance);
        }
        catch (FormatException)
        {
            Debug.Log("Invalid balance format. Setting balance to 0.");
            Bal = 0;
        }

        int Amount = int.Parse(amount);
        moneyText.text = "Money: " + (Bal + Amount);
        LootLockerSDKManager.CreditBalanceToWallet(_walletID, "01HTWQ30JTBCKE8X24NC34X1B4", amount, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Gold added");
            }
            else
            {
                Debug.Log("Error adding gold: " + response.errorData.message);
            }
        });
        StartCoroutine(GetMoneyAmount());
        
        
    }
    
    IEnumerator GetMoneyAmount()
    {
        bool responseReceived = false;
        LootLockerSDKManager.ListBalancesInWallet(_walletID, (response) =>
        {
            if (response.success)
            {
                _balance = response.balances[0].amount;
                responseReceived = true;
            }
            else
            {
                Debug.Log("Error getting money amount: " + response.errorData.message);
                AddGold("100");
                responseReceived = true;
            }
        });
        yield return new WaitUntil(() => responseReceived);
        moneyText.text = "Money: " + _balance;
    }
    

    #endregion
}
