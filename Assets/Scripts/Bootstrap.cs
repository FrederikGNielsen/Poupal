using System;
using System.Collections;
using UnityEngine;
using LootLocker.Requests;
using TMPro;

public class Bootstrap : MonoBehaviour
{
    
    public static Bootstrap instance;
    
    private string _holderUlid;
    private string _walletID;

    [SerializeField] private string balance;
    [SerializeField] private string characterID;
    
    //Strength
    public int strengthLevel;

    public TextMeshProUGUI moneyText;


    private void Awake()
    {
        //singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(GuestLogin());
    }
    
    
    #region login
    
    IEnumerator GuestLogin()
    {
        GameManager.instance.switchToLoadingView();
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
        
        RegisterPlayer();
    }
    
    public void RegisterPlayer()
    {
        StartCoroutine(GetWallet());
        
        RegisterProgression("plevel");
        RegisterProgression("pstrength");
        RegisterProgression("pspeed");
        RegisterProgression("pstamina");
        RegisterProgression("phealth");
        GetPlayerProgression("phealth");
        FinishedLogin();
    }
    
    public void FinishedLogin()
    {
        GameManager.instance.switchToGameView();
        RetrieveProgression("plevel");
    }
    
    #endregion
    
    #region playerWallet
    
    private IEnumerator GetWallet()
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
        
        AddGold("0");
        
        StartCoroutine(GetMoneyAmount());

    }
    
    public string GetWalletID()
    {
        return _walletID;
    }
    
    public void AddGold(string amount)
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
        moneyText.text = "$" + (bal + Amount);
        LootLockerSDKManager.CreditBalanceToWallet(_walletID, "01HTWQ30JTBCKE8X24NC34X1B4", amount, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Gold added");
                StartCoroutine(GetMoneyAmount());
            }
            else
            {
                Debug.Log("Error adding gold: " + response.errorData.message);
            }
        });
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
                responseReceived = true;
            }
        });
        yield return new WaitUntil(() => responseReceived);
        moneyText.text = "$" + balance;
    }
    
    #endregion
    
    #region playerProgression
    
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
    
    public void GetPlayerProgression(string key)
    {
        LootLockerSDKManager.GetPlayerProgression(key, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Progression retrieved");
                Debug.Log("Progression: " + response.points);
                
            }
            else
            {
                Debug.Log("Error getting progression: " + response.errorData.message);
            }
        });
    }
    
    public void RetrieveProgression(string key)
    {
        LootLockerSDKManager.GetPlayerProgressions(response =>
        {
            if (!response.success) {
                Debug.Log("Failed: " + response.errorData.message);
            }

            // Output the player level and show how much points are needed to progress to the next tier for all player progressions
            foreach (var playerProgression in response.items)
            {
                Debug.Log($"Current level in {playerProgression.progression_name} is {playerProgression.step}");
                if (playerProgression.next_threshold != null)
                {
                    Debug.Log($"Points needed to reach next level in {playerProgression.progression_name}: {playerProgression.next_threshold - playerProgression.points}");
                }
            }
            
            //strength bar
            GameManager.instance.strengthBar.maxValue = (float)response.items[1].next_threshold;
            GameManager.instance.strengthBar.currentPercent = (float)response.items[1].points;
        });
    }
    #endregion
}
