using System.Collections;
using UnityEngine;

public class Potion : MonoBehaviour, IUsable
{
    public float healthRecoveryPoints;

    public float delayTime;
    public Player user;

    public void Use() {
        GameObject.Find("Player Visual").GetComponent<Animator>().Play("Use_Item");
        StartCoroutine(DelayedUse());
    }

    public void Start() {
        user = GameObject.Find("Player").GetComponent<Player>();
    }

    private IEnumerator DelayedUse() {
        yield return new WaitForSeconds(delayTime);

        if (user != null) {
            user.RecoverHealth(healthRecoveryPoints);
        }
    }
}
