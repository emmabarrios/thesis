using UnityEngine;

public class Flask : MonoBehaviour
{
    //private bool isActivated = false;
    //public bool IsActivated { set { isActivated = value; } }
    
    [SerializeField] private Vector3 sealOffset;
    [SerializeField] private Vector3 flaskOffset;
    [SerializeField] private GameObject flaskSeal;
    [SerializeField] private GameObject flaskEmptyContainer;
    [SerializeField] private GameObject SealPrefab;

    [SerializeField] private PlayerAnimator playerAnimator;

    private void Start() {
        playerAnimator = GameObject.Find("Player").GetComponentInChildren<PlayerAnimator>();
        playerAnimator.OnOpenedFlask += Open;
        playerAnimator.OnDropedFlask += Drop;
    }

    public void Open() {
        flaskSeal.SetActive(false);
        Transform parentTransform = GameObject.Find("Player Attach Point").GetComponent<Transform>();
        GameObject _seal = Instantiate(SealPrefab, parentTransform.position + sealOffset, parentTransform.rotation);
        _seal.transform.rotation = parentTransform.rotation;
        playerAnimator.OnOpenedFlask -= Open;
    }

    public void Drop() {
        Transform parentTransform = GameObject.Find("Player Attach Point").GetComponent<Transform>();
        GameObject _emptyBottle = Instantiate(flaskEmptyContainer, parentTransform.position + flaskOffset, parentTransform.rotation);
        _emptyBottle.transform.rotation = parentTransform.rotation;
        playerAnimator.OnDropedFlask -= Drop;

        Destroy(GetComponentInParent<Consumable>().gameObject);
    }
}
