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
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
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
    
    
    #region Login

    private IEnumerator GuestLogin()
    {
        GameManager.instance.switchToLoadingView();
        yield return new WaitForSeconds(1);

        bool hasConnected = false;
        LootLockerSDKManager.StartGuestSession(response =>
        {
            hasConnected = true;
            if (response.success)
            {
                Debug.Log("Guest session started");
                _holderUlid = response.player_ulid;
                RegisterPlayer();
            }
            else
            {
                Debug.Log("Error" + response.errorData.message);
            }
        });

        yield return new WaitUntil(() => hasConnected);
        Debug.Log("Guest login complete");
    }

    private void RegisterPlayer()
    {
        StartCoroutine(GetWallet());

        string[] progressionKeys = { "plevel", "pstrength", "pspeed", "pstamina", "phealth" };
        foreach (var key in progressionKeys)
        {
            RegisterProgression(key);
        }

        StartCoroutine(FinishedLogin());
    }

    private IEnumerator FinishedLogin()
    {
        yield return new WaitForSeconds(1);
        GameManager.instance.switchToGameView();
        StartCoroutine(GetComponent<PlayerName>().GetUsername());
        updateProgress();
        RetrievePlayerLevel();
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
    
    public void RetrieveProgression(
        )
    {
        LootLockerSDKManager.GetPlayerProgressions(response =>
        {
            if (!response.success) {
                Debug.Log("Failed: " + response.errorData.message);
            }

            
            
            //Speed bar
            GameManager.instance.speedBar.maxValue = (float)response.items[3].next_threshold;
            GameManager.instance.speedBar.currentPercent = (float)response.items[3].points;
            
            //Stamina bar
            GameManager.instance.stamianBar.maxValue = (float)response.items[1].next_threshold;
            GameManager.instance.stamianBar.currentPercent = (float)response.items[1].points;
            
            //Health bar
            GameManager.instance.healthBar.maxValue = (float)response.items[2].next_threshold;
            GameManager.instance.healthBar.currentPercent = (float)response.items[2].points;
            
            //strength bar
            GameManager.instance.strengthBar.maxValue = (float)response.items[4].next_threshold;
            GameManager.instance.strengthBar.currentPercent = (float)response.items[4].points;

        });
    }

    public void RetrievePlayerLevel()
    {
        LootLockerSDKManager.GetPlayerProgressions(response =>
        {
            if (!response.success) {
                Debug.Log("Failed: " + response.errorData.message);
            }

            float level = (float)response.items[0].step;
            GameManager.instance.levelText.text = "lvl: " + level;
        });
    }

    void updateProgress()
    {
        RetrieveProgression();
    }
    #endregion
}
