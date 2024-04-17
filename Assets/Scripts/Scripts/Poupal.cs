using UnityEngine;


public class Poupal : MonoBehaviour
{
    //Player Stats
    public int playerLevel;
    public float playerXp;
    
    //Styrke
    public int strengthLevel;
    public float strengthXp;
    
    //Fart
    public int speedLevel;
    public float speedXp;
    
    //Stamina
    public int staminaLevel;
    public float staminaXp;
    
    //Health
    public int healthLevel;
    public float healthXp;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void LevelUp()
    {
        playerLevel++;
    }
}
