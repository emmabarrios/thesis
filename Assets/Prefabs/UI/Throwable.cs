using System.Collections;
using UnityEngine;

public class Throwable : MonoBehaviour, IUsable
{
    [SerializeField] private GameObject projectile;
    public float throwForce;
    public float throwUpwardForce;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Thrower thrower;
    public float timer;
    private bool isTiming;

    public float delayTime;

    private void Start() {
        thrower = GameObject.Find("Bomb Thrower").GetComponent<Thrower>();
    }

    public void Use() {
        isTiming = true;
        //StartCoroutine(DelayedUse());
    }

    private void Update() {
        if (isTiming) {
            timer -= Time.deltaTime;
            if (timer < 0.1f) {
                timer = 0;;
                thrower.Throw(projectile, throwForce, throwUpwardForce, offset);
                isTiming = false;
            }
        }
    }

    //private IEnumerator DelayedUse() {
    //    yield return new WaitForSeconds(delayTime);
    //    GameObject _projectile = Instantiate(projectile, thrower.position, thrower.rotation);
    //    Vector3 force = thrower.transform.forward * throwForce + (transform.up * throwUpwardForce);
    //    _projectile.GetComponent<Projectile>().ApplyForceWithOffset(force);

    //    Destroy(gameObject);
    //}
}

