using System.Collections;
using TMPro;
using UnityEngine;

public class ImageSpawner : MonoBehaviour
{
    public GameObject damageUI; // Drag your image prefab here in the Unity editor
    public Attacker attacker;
    public string _text;

    private void Awake() {
        StartCoroutine(WaitForPlayerAndSetupEvents());
        //Player player = GameObject.Find("Player").GetComponent<Player>();
        //attacker = player.transform.GetComponentInChildren<Attacker>();
        //attacker.OnAttackLanded += SpawnImage;
    }

    private void SpawnImage(string text) {
        GameObject spawnedImage = Instantiate(damageUI, transform.position, Quaternion.identity);
        spawnedImage.transform.parent = this.transform;
        TextMeshProUGUI textMeshPro = spawnedImage.GetComponentInChildren<TextMeshProUGUI>();
        textMeshPro.text = text;
    }

    private IEnumerator WaitForPlayerAndSetupEvents() {
        // Wait until the "Player" object is loaded into the scene
        GameObject playerObject = null;

        while (playerObject == null) {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            yield return null;
        }

        // Once the "Player" object is found, set up the events
        Player player = playerObject.GetComponent<Player>();
        attacker = player.transform.GetComponentInChildren<Attacker>();
        attacker.OnAttackLanded += SpawnImage;
    }

}
