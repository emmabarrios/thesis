using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextSceneTest : MonoBehaviour
{
    public static LoadNextSceneTest instance;

    private void Awake() {
        //DontDestroyOnLoad(transform.gameObject);
        if (instance == null) {
            // If no instance exists, set the instance to this object
            instance = this;

            // Mark this object to not be destroyed when loading a new scene
            DontDestroyOnLoad(gameObject);
        } else {
            // If an instance already exists, destroy this object
            Destroy(gameObject);
        }
    }
    public void LoadSceneCombat(int _in) {
        StartCoroutine(Transition(_in));
    }


    private IEnumerator Transition(int inde) {
        yield return SceneManager.LoadSceneAsync(inde);
        GameManager.instance.SetGameManagerState(GameManager.GameStates.LoadingCombatAssets);
    }
}
