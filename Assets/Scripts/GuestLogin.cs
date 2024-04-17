using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;

public class GuestLogin : MonoBehaviour
{
    // Start is called before the first frame update
    private string walletID;
    private string holderUlid;
    
    void Start()
    {
        StartCoroutine(guestLogin());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    IEnumerator guestLogin()
    {
        yield return new WaitForSeconds(1);
        bool hasConnected = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Guest session started");
                holderUlid = response.player_ulid;
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

    }

    IEnumerator GetWallet()
    {
        bool responseReceived = false;
        LootLocker.LootLockerEnums.LootLockerWalletHolderTypes player = LootLocker.LootLockerEnums.LootLockerWalletHolderTypes.player;
        LootLockerSDKManager.GetWalletByHolderId(holderUlid, player, (response) =>
        {
            if(!response.success)
            {
                //If wallet is not found, it will automatically create one on the holder.
                Debug.Log("error: " + response.errorData.message);
                Debug.Log("request ID: " + response.errorData.request_id);
                responseReceived = true;
                return;
            }
            else
            {
                walletID = response.id;
                responseReceived = true;
            }

        });
        yield return new WaitUntil(() => responseReceived);
        Debug.Log("Found wallet " + walletID);
    }
}
