using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPotion : MonoBehaviour
{
    //private bool isActivated = false;
    //public bool IsActivated { set { isActivated = value; } }
    
    [SerializeField] private Vector3 offset;
    [SerializeField] private GameObject seal;
    [SerializeField] private GameObject SpawnSeal;
    [SerializeField] private bool isTiming = true;
    public bool IsTiming { get { return isTiming; } set { isTiming = value; } }
    [SerializeField] private float timer;
    [SerializeField] private float time;

    void Start()
    {
        timer = time;
    }

    private void Update() {

        if (isTiming == true) {
            timer -= Time.deltaTime;

            if (timer < 0.1f) {

                isTiming = false;
                timer = 0;
                seal.SetActive(false);

                Transform parentTransform = GameObject.Find("Player Attach Point").GetComponent<Transform>();
                GameObject instantiatedObject = Instantiate(SpawnSeal, parentTransform.position + offset, parentTransform.rotation);
                instantiatedObject.transform.rotation = parentTransform.rotation;
            }
        }

    }
}
