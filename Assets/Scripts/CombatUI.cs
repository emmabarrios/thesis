using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public Image healthImage;
    public Image staminaImage;

    public Player player;
    public float healthPoints;
    public float staminaPoints;

    // Start is called before the first frame update
    void Start()
    {
        player.OnHealthValueChanged += Update_HealthBar;
        player.OnStaminaValueChanged += Update_StaminaBar;

        healthPoints = player.Health;
        staminaPoints = player.Stamina;

        healthImage.fillAmount = healthPoints / healthPoints;
        staminaImage.fillAmount = staminaPoints / staminaPoints;
    }

    private void Update_HealthBar(object sender, Player.OnHealthValueChanged_EventArgs e) {
        healthImage.fillAmount = e.value / healthPoints;
    }
    
    private void Update_StaminaBar(object sender, Player.OnStaminaValueChanged_EventArgs e) {
        staminaImage.fillAmount = e.value / staminaPoints;
    }

}
