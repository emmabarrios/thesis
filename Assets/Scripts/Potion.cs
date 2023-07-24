using System.Collections;
using UnityEngine;

public class Potion : MonoBehaviour, IUsable {

    public float healthPoints;
    
    public void Use() {
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        player.RecoverHealth(healthPoints);
    }

}
