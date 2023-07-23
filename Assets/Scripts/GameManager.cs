using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Limits {
        noLimit = 0,
        limit12 = 12,
        limit25 = 25,
        limit30 = 30,
        limit35 = 35,
        limit40 = 50,
        limit45 = 45,
        limit50 = 50,
        limit60 = 60
    }

    [Header("Framerate settings")]
    public Limits limits;
             
    
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = (int)limits;
        InitializeCombatInventoryUI();
    }
    
    public void InitializeCombatInventoryUI() {
        GameObject.Find("Carousel").GetComponent<CombatUIcarousel>().InitializeUIcarousel(CombatInventory.instance.itemLists);
    }

}
