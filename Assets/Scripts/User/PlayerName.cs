using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using System.IO;
using UnityEngine.Networking;

public class PlayerName : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            StartCoroutine(GetUsername());
    }
    
    
    
    public void SaveUserName()
    {
        string name = GameObject.Find("NameField").GetComponent<TMPro.TMP_InputField>().text;
        string path = Application.persistentDataPath + "/playerName.txt";
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(name);
        writer.Close();
        
        bool isPublic = false;

        LootLockerSDKManager.UploadPlayerFile(path, "playerName", isPublic, response =>
        {
            if (response.success)
            {
                Debug.Log("Successfully uploaded player file, url: " + response.url);
            } 
            else
            {
                Debug.Log("Error uploading player file");
            }
        });
    }

    public IEnumerator GetUsername()
    {
        string url = "";

        LootLockerSDKManager.GetAllPlayerFiles((response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully retrieved player files: " + response.items[0].url);
                url = response.items[0].url;
            }
            else
            {
                Debug.Log("Error retrieving player storage");
            }
        });

        // Wait until the URL has been retrieved
        yield return new WaitUntil(() => url != "");

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string savePath = Path.Combine(Application.persistentDataPath, "playerName.txt");
                System.IO.File.WriteAllText(savePath, www.downloadHandler.text);
                Debug.Log("File downloaded successfully");

                string path = Application.persistentDataPath + "/playerName.txt";
                using (StreamReader reader = new StreamReader(path))
                {
                    string name = reader.ReadLine();

                    GameObject.Find("NameField").GetComponent<TMPro.TMP_InputField>().text = name;
                }
            }
        }
    }
}
