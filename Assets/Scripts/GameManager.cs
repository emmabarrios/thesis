using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Combat Scene Characters")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject enemy;
    

    public enum GameStates {
        WaitingToStart,
        LoadingCombatAssets,
        Combat,
        CombatIntro,
        CombatOutro,
        Wait,
        Overworld
    }


    public GameStates state;
    private float waitingToStartTimer = 1f;

    [Header("Combat Timers")]
    [SerializeField] private float countdownToStartCombatTimer = 3f;
    [SerializeField] private float combatLimitTimer;
    [SerializeField] private float combatLimitTimerMax = 10f;
    [SerializeField] private float combatOutroTimer = 3f;

    // Framerate limits
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

    [Header("Background music tracks")]
    public AudioClip battleAudio;
    AudioSource audioSource;

    [Header("Battle Settings")]
    public float entranceTime = 2f;

    private bool combatSceneLoaded = false;

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

        //if (instance != null) { return; }
        //instance = this;

        state = GameStates.CombatIntro;
    }

    private void Update() {
        switch (state) {
            //case GameStates.WaitingToStart:
            //    waitingToStartTimer -= Time.deltaTime;
            //    if (waitingToStartTimer < 0) {
            //        state = GameStates.CombatIntro;
            //    }
            //    break;

            case GameStates.CombatIntro:
                countdownToStartCombatTimer -= Time.deltaTime;

                if (countdownToStartCombatTimer <= 0) {
                    combatLimitTimer = combatLimitTimerMax;
                    PlayerStatsManager.instance.LoadCharacterStats(Player.instance);
                    CombatInventory.instance.LoadPlayerWeapon();
                    state = GameStates.Combat;
                    //ToggleCombatUI();
                    //InitializePlayer();
                }
                break;

            case GameStates.Combat:

                combatLimitTimer -= Time.deltaTime;

                if (combatLimitTimer < 0) {
                    EndCombatSequence();
                }
                break;

            case GameStates.CombatOutro:

                //combatOutroTimer -= Time.deltaTime;
                //if (combatOutroTimer < 0) {
                //    ToggleBattleResultsUI();
                //    state = GameStates.Wait;
                //}

                break;

            //case GameStates.LoadingCombatAssets:

            //    // Load the enemy game object
            //    // Load the player game object
            //    // Load player stats
            //    // Initialize combat UI carousell
            //    // Change state to combat intro

            //    //StartCoroutine(LoadCombatScene());

            //    if (!combatSceneLoaded) {
            //        StartCoroutine(LoadCombatScene());
            //        combatSceneLoaded = true;
            //    }

            //    break;

            case GameStates.Wait:
                break;

            case GameStates.Overworld:
                break;
        }
        Debug.Log(state);
    }

    public void EndCombatSequence() {
        ToggleCombatUI();
        ToggleBattleResultsUI();
        state = GameStates.CombatOutro;
    }

    void Start()
    {
        Application.targetFrameRate = (int)limits;
        audioSource = GetComponent<AudioSource>();
        //audioSource.PlayOneShot(battleAudio);

        //Enemy enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>();
    }

    //public void InitializeCombatInventoryUI() {
    //   GameObject.Find("Carousel").GetComponent<CombatUIcarousel>().InitializeUIcarousel(CombatInventory.instance.itemLists);
    //}

    public void InitializePlayer() {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>().LoadInputReferences();
        GameObject.FindGameObjectWithTag("Player").GetComponent<Controller>().SetLockTarget(GameObject.FindGameObjectWithTag("Enemy").GetComponent<Transform>());
        PlayerStatsManager.instance.LoadCharacterStats(Player.instance);
        CombatInventory.instance.LoadPlayerWeapon();
        
    }

    public bool IsGameOnCombat() {
        return state == GameStates.Combat;
    }

    private void ToggleCombatUI() {
        Transform playerCombatUI = GameObject.Find("Canvas").transform.GetChild(0);
        Transform enemyCombatUI = GameObject.FindGameObjectWithTag("Enemy").transform.GetChild(0);

        playerCombatUI.gameObject.SetActive(!playerCombatUI.gameObject.activeSelf);
        enemyCombatUI.gameObject.SetActive(!enemyCombatUI.gameObject.activeSelf);
    } 
    private void ToggleBattleResultsUI() {
        Transform battleResultsUI = GameObject.Find("Canvas").transform.GetChild(1);
        battleResultsUI.gameObject.SetActive(!battleResultsUI.gameObject.activeSelf);
    }

    public float GetCombatTimerNormalized() {
        return  (combatLimitTimer / combatLimitTimerMax);
    }

    public void SetToOutroState() {
        ToggleCombatUI();
        state = GameStates.CombatOutro;
    }

    private IEnumerator LoadCombatScene() {
       
        yield return LoadCharacter(enemy, GameObject.Find("Enemy Spawn Point").transform);
        Debug.Log("Enemy spawned");

        yield return LoadCharacter(player, GameObject.Find("Player Spawn Point").transform);
        Debug.Log("Player spawned");

        yield return new WaitUntil(EnemyComponentIsNotNull);
        yield return new WaitUntil(PlayerComponentIsNotNull);

        bool EnemyComponentIsNotNull() {
            return GameObject.FindGameObjectWithTag("Enemy").GetComponent<Enemy>() != null;
        }
        bool PlayerComponentIsNotNull() {
            return GameObject.FindGameObjectWithTag("Player").GetComponent<Player>() != null;
        }

        ToggleCombatUI();

        InitializePlayer();

        state = GameStates.CombatIntro;

    }

    private IEnumerator LoadCharacter(GameObject obj, Transform tsfm) {
        GameObject loadedCharacter = Instantiate(obj, tsfm.position, tsfm.rotation);

        // Wait until the loaded character's Awake and Start methods are called
        //yield return new WaitUntil(() => loadedCharacter.GetComponent<Character>() != null);

        yield return new WaitUntil(ObjectComponentIsNotNull);

        bool ObjectComponentIsNotNull() {
            return loadedCharacter.GetComponent<Character>() != null;
        }
    }

    public void SetGameManagerState(GameStates state) {
        this.state = state;
    }
}
