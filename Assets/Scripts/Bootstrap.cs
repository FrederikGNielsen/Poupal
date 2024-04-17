using System;
using System.Collections;
using UnityEngine;
using LootLocker.Requests;
using TMPro;

public class Bootstrap : MonoBehaviour
{
    private string _holderUlid;
    private string _walletID;

    [SerializeField] private string balance;
    [SerializeField] private string characterID;

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
        
        StartCoroutine(GetWallet());
        
        RegisterProgression("plevel");
        RegisterProgression("pstrength");
        RegisterProgression("pspeed");
        RegisterProgression("pstamina");
        RegisterProgression("phealth");
        
      

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
        
        AddGold("100");
        
        StartCoroutine(GetMoneyAmount());

    }
    
    
    private void AddGold(string amount)
    {
        int bal;
        try
        {
            bal = int.Parse(balance);
        }
        catch (FormatException)
        {
            Debug.Log("Invalid balance format. Setting balance to 0.");
            bal = 0;
        }

        int Amount = int.Parse(amount);
        moneyText.text = "Money: " + (bal + Amount);
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
                balance = response.balances[0].amount;
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
        moneyText.text = "Money: " + balance;
    }
    
    
    public void RegisterProgression(string key)
    {
        LootLockerSDKManager.RegisterPlayerProgression(key, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Progression registered");
            }
            else
            {
                Debug.Log("key: " + key);
                Debug.Log("Error registering progression: " + response.errorData.message);
            }
        });
    }

    #endregion
}
