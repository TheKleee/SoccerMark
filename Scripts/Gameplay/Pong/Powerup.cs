using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PowerType
{
    twice,
    speed,
    large,
    //Other powerups...
}
public class Powerup : MonoBehaviour
{
    #region Singleton:
    public static Powerup instance;
    private void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }
    #endregion

    
    private PowerType pType;

    [Header("Powerup Object:")]
    [SerializeField] private GameObject[] pUp = new GameObject[3];

    float randPowerup;
    float spawnPosX;
    float spawnPosY;
    float spawnPosZ;
    Vector3 spawnPos;

    public void SpawnPowerup(bool canSpawn)
    {
        if (canSpawn)
        {
            randPowerup = Random.Range(.0f, 1f);
            spawnPosX = Random.Range(-2.5f, 2.5f);
            spawnPosY = Random.Range(.25f, .75f);
            spawnPosZ = Random.Range(-3.75f, 3.25f);
            spawnPos = new Vector3(spawnPosX, spawnPosY, spawnPosZ);

            if (SaveData.instance.lvl > 1 && SaveData.instance.lvl <= 10) 
            {
                pType = PowerType.twice;
            }
            else if (SaveData.instance.lvl > 10 && SaveData.instance.lvl <= 20)
            {
                pType = randPowerup > .35f ? PowerType.twice : PowerType.speed;
            }
            else
            {
                //Any powerup...
                if(randPowerup <= .35f)
                {
                    pType = PowerType.large;
                }
                else if (randPowerup >= .75f)
                {
                    pType = PowerType.speed;
                }
                //This might change in the future:
                else
                {
                    pType = PowerType.twice;
                }
            }

            switch (pType)
            {
                case PowerType.twice:
                    Instantiate(pUp[0], spawnPos, Quaternion.identity);
                    break;

                case PowerType.speed:
                    Instantiate(pUp[1], spawnPos, Quaternion.identity);
                    break;

                case PowerType.large:
                    Instantiate(pUp[2], spawnPos, Quaternion.identity);
                    break;
            }
            
            
        }
    }


}
