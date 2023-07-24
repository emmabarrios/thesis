using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ImageFill : MonoBehaviour
{
    public Image imageToFill;
    public bool coolingDown;
    public float waitTime;

    public Controller controller = null;


    // Start is called before the first frame update
    void Start()
    {
        controller.OnAttack += ResetFill;
        //ResetFill(1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (coolingDown == true) {
            imageToFill.fillAmount += 1.0f / waitTime * Time.deltaTime;
            if (imageToFill.fillAmount >= 1f) {
                coolingDown = false;
            }
        }
    }

    public void ResetFill(float coolDown) {
        imageToFill.fillAmount = 0f;
        waitTime = coolDown;
        coolingDown = true;
    }

}
