using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages player data saved between scenes
/// Manages save data saved between sessions
/// </summary>
public class GameManager : MonoBehaviour
{
    // singleton instance
    private static GameManager _instance;

    public bool IsMainMenuLoaded = false;
    public delegate void OnSceneBegin();
    public static event OnSceneBegin onSceneBegin;
    public delegate void OnHubRevive();
    public static event OnHubRevive onHubRevive;

    public static GameManager Instance
    {
        get
        {
            // setup GameManager as a singleton class
            if (_instance == null)
            {
                // create new game manager object
                GameObject newManager = new();
                newManager.name = "Game Manager";
                newManager.AddComponent<GameManager>();
                DontDestroyOnLoad(newManager);
                _instance = newManager.GetComponent<GameManager>();
            }
            return _instance;
        }
    }

    public static bool IsPaused = false;
    public bool IsGummy = false;

    #region PLAYER DATA
    // Player Data (saved between scenes)
    [System.Serializable]
    public class Stats
    {
        // Health
        public int MaxHealth; // Base Health + Health Upgrades
        public int CurrHealth; // Base Health + Health Upgrades - Damage Taken
        // Armor
        public int Armor; // Armor Upgrades (temporary/consumable)
        // Damage
        public float BulletDamageMult; // Base Damage + Damage Upgrades
        public int DamageUpgradeCount;
        // Fire Speed
        public float BulletCooldown; // Base Cooldown + Fire Speed Upgrades
        public int FireRateUpgradeCount; // How many fire rate upgrades we have
        // Move Speed
        public float MoveSpeed; // Base MoveSpeed + MoveSpeed Upgrades
        // Light
        public float LightFOV; // Base FOV + Light Upgrades
        public float LightIntensity; // Base Intensity + Light Upgrades
        public float LightRange; // Base Range + Light Upgrades
        public int LightUpgradeCount;   // How many light upgrades we have

        // for tracking levels that have been navigated
        public bool HadBarrelRoom;
        public bool HadMinecartRoom;
        public bool HadMazeRoom;
        public bool HadSmallCombatRoom;
        public bool HadLargeCombatRoom;
        public int NumRooms; // number of cave rooms which have been entered so far

        public bool CrumblingDeath;
    }
    private Stats _playerData;

    public Stats PlayerData
    {
        get
        {
            // initialize if necessary and possible
            if (_playerData == null)
            {
                ResetPlayerStats();
            }

            return _playerData;
        }
        private set
        {
            _playerData = value;
        }
    }

    /// <summary>
    /// initializes base stats of player data (use before starting a new run)
    /// </summary>
    public void ResetPlayerStats()
    {
        if (GameObject.Find("Player"))
        {
            Instance._playerData = GameObject.Find("Player").GetComponent<PlayerStats>().GetBasePlayerData();
            Instance._playerData.HadBarrelRoom = false;
            Instance._playerData.HadMazeRoom = false;
            Instance._playerData.HadMinecartRoom = false;
            Instance._playerData.HadSmallCombatRoom = false;
            Instance._playerData.HadLargeCombatRoom = false;
            Instance._playerData.NumRooms = 0;
            Instance._playerData.CrumblingDeath = false;
        }
        else
            Debug.LogError("Error: trying to access PlayerStats with no player in scene");
    }

    /// <summary>
    /// 
    /// </summary>
    public void ApplyUpgradeToStats(UpgradeController.UpgradeType upgradeType)
    {
        if (GameObject.Find("Player"))
        {
            Instance._playerData = GameObject.Find("Player").GetComponent<PlayerStats>().ApplyUpgrade(upgradeType);
        }
        else
            Debug.LogError("Error: trying to access PlayerStats with no player in scene");
    }
    #endregion

    #region SAVE DATA
    // save data (saved between sessions)
    [System.Serializable]
    public class Data
    {
        public int NumOfRuns; // adds one each time the player dies
        public float MasterVolumeSlider;
        public float PlayerVolumeSlider;
        public float EnemyVolumeSlider;
        public float EnvironmentVolumeSlider;
        public float MusicVolumeSlider;
        public float HorizontalSensitivity;
        public float VerticalSensitivity;
    }
    private Data _saveData;

    public Data SaveData
    {
        get
        {
            // initialize if necessary and possible
            if (_saveData == null)
            {
                // initialize and load save data
                Data newSaveData = new Data();

                // default save file configuration (in case some/all save data is missing)
                newSaveData.NumOfRuns = 0;
                newSaveData.MasterVolumeSlider = 0.5f;
                newSaveData.PlayerVolumeSlider = 0.5f;
                newSaveData.EnemyVolumeSlider = 0.5f;
                newSaveData.EnvironmentVolumeSlider = 0.5f;
                newSaveData.MusicVolumeSlider = 0.5f;
                newSaveData.HorizontalSensitivity = 1f;
                newSaveData.VerticalSensitivity = 1f;

                // read existing save data (if it exists)
                string path = Application.persistentDataPath + "\\savedata.json";
                if (File.Exists(path))
                {
                    // read json file into data object
                    string json = File.ReadAllText(path);
                    newSaveData = JsonUtility.FromJson<Data>(json);
                }
                Instance._saveData = newSaveData; // set private save data on current instance
            }

            return _saveData;
        }
        private set
        {
            _saveData = value;
        }
    }

    public void SetMasterVolumeSlider(float val)
    {
        SaveData.MasterVolumeSlider = val;
    }

    public void SetPlayerVolumeSlider(float val)
    {
        SaveData.PlayerVolumeSlider = val;
    }

    public void SetEnemyVolumeSlider(float val)
    {
        SaveData.EnemyVolumeSlider = val;
    }

    public void SetEnvironmentVolumeSlider(float val)
    {
        SaveData.EnvironmentVolumeSlider = val;
    }

    public void SetMusicVolumeSlider(float val)
    {
        SaveData.MusicVolumeSlider = val;
    }

    /// <summary>
    /// accounts for stacking with master volume
    /// </summary>
    public float GetPlayerVolume()
    {
        return SaveData.PlayerVolumeSlider * SaveData.MasterVolumeSlider;
    }

    /// <summary>
    /// accounts for stacking with master volume
    /// </summary>
    public float GetEnemyVolume()
    {
        return SaveData.EnemyVolumeSlider * SaveData.MasterVolumeSlider;
    }

    /// <summary>
    /// accounts for stacking with master volume
    /// </summary>
    public float GetEnvironmentVolume()
    {
        return SaveData.EnvironmentVolumeSlider * SaveData.MasterVolumeSlider;
    }

    /// <summary>
    /// accounts for stacking with master volume
    /// </summary>
    public float GetMusicVolume()
    {
        return SaveData.MusicVolumeSlider * SaveData.MasterVolumeSlider;
    }

    /// <summary>
    /// increments NumOfRuns counter in Data
    /// </summary>
    public void AddRun()
    {
        SaveData.NumOfRuns++;
    }

    private void OnApplicationQuit()
    {
        // save SavePointData to json file
        string json = JsonUtility.ToJson(SaveData);
        File.WriteAllText(Application.persistentDataPath + "\\savedata.json", json);
    }
    #endregion

    private void Update() { }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) // These are the parameters that the sceneLoaded event gives
    {
        // If the scene is the hub
        if(scene.name == "0_Hub")
        {
            ResetPlayerStats();
            if (IsMainMenuLoaded) {
                // Invoke onHubRevive, which
                // Plays the coffin-burst sequence for the coffin
                // Plays the coffin-burst sequence for the fake worm
                // Plays the coffin-burst sequence for the real worm
                // Correctly swap the UI
                // Disables the player until the animation is done
                // Correctly positions and locks the camera
                // Correctly sets the music/ambient audio
                onHubRevive?.Invoke();

                
            }
        }

        // Otherwise
        else
        {
            //Invoke onSceneBegin, which
            // Plays the player "burrow down" enter animation
            // Disables the player until the animation is done

            // This event also needs to (but doesn't currently)
            // Locks the camera until the animation is done
            onSceneBegin?.Invoke();
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}