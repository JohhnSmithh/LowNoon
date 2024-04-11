using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionObject : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private List<string> _combatLevels;
    [SerializeField] private List<string> _barrelLevels;
    [SerializeField] private List<string> _mazeLevels;
    [SerializeField] private List<string> _minecartLevels;

    [Header("Randomization Weights")]
    [SerializeField] private float _combatProb = 0.4f;
    [SerializeField] private float _barrelProb = 0.2f;
    [SerializeField] private float _mazeProb = 0.2f;
    [SerializeField] private float _minecartProb = 0.2f;

    [Header("Boss Level")]
    [SerializeField] private string _bossLevel;
    [SerializeField, Tooltip("number of rooms that must be cleared before able to fight boss")] private int _roomsBeforeBoss = 5;

    [Header("Tracking Number of Runs")]
    [SerializeField, Tooltip("Used by the exit patch in the hub to determine if to increment number of runs tracker")] private bool _isStartPatch = false;

    private string _sceneToLoad;

    public delegate void OnSceneTransition();
    public static event OnSceneTransition onSceneTransition; // Event for when the bullet fires

    // Start is called before the first frame update
    void Start()
    {
        // increment number of runs if necessary
        if (_isStartPatch)
            GameManager.Instance.AddRun();

        // check if it is time for the boss
        if (GameManager.Instance.PlayerData.NumRooms >= _roomsBeforeBoss)
            _sceneToLoad = _bossLevel;
        else // determine random scene to load
        {
            // increment number of rooms traversed
            GameManager.Instance.PlayerData.NumRooms++;

            float rand = Random.Range(0f, 1.0f);

            if (rand < _combatProb) // Combat Room
                _sceneToLoad = _combatLevels[Random.Range(0, _combatLevels.Count)];
            else if (rand < _combatProb + _barrelProb) // Barrel Room
            {
                if (GameManager.Instance.PlayerData.HadBarrelRoom) // already done
                    _sceneToLoad = _combatLevels[Random.Range(0, _combatLevels.Count)];
                else // not done yet
                {
                    _sceneToLoad = _barrelLevels[Random.Range(0, _barrelLevels.Count)];
                    GameManager.Instance.PlayerData.HadBarrelRoom = true;
                }
            }
            else if (rand < _combatProb + _barrelProb + _mazeProb) // Maze Room
            {
                if (GameManager.Instance.PlayerData.HadMazeRoom) // already done
                    _sceneToLoad = _combatLevels[Random.Range(0, _combatLevels.Count)];
                else // not done yet
                {
                    _sceneToLoad = _mazeLevels[Random.Range(0, _mazeLevels.Count)];
                    GameManager.Instance.PlayerData.HadMazeRoom = true;
                }
            }
            else if (rand < _combatProb + _barrelProb + _mazeProb + _minecartProb) // Minecart Room
            {
                if (GameManager.Instance.PlayerData.HadMinecartRoom) // already done
                    _sceneToLoad = _combatLevels[Random.Range(0, _combatLevels.Count)];
                else // not done yet
                {
                    _sceneToLoad = _minecartLevels[Random.Range(0, _minecartLevels.Count)];
                    GameManager.Instance.PlayerData.HadMinecartRoom = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    { }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Collider>().tag == "Player")
        {
            StartCoroutine(DoSceneChange(other.gameObject));
        }
    }

    private IEnumerator DoSceneChange(GameObject player)
    {
        // Invoke the scene transtion event, which
        //      Sets velocity to 0 and disables the player controller
        //      Triggers the burrow down animation  
        //      Pulls down the UI curtain
        onSceneTransition?.Invoke();

        // Wait until the player animation and UI scene transition animation are done
        yield return new WaitForSeconds(player.GetComponent<PlayerAnimator>().BurrowDownDuration + 1f);

        SceneManager.LoadScene(_sceneToLoad);
        yield return null;
    }
}
