using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuView : View
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _optionsMenu;
    [SerializeField] private GameObject _creditsMenu;

    private CameraController _cameraController;
    private ManualCameraController _manualCameraController;
    private PlayerController _playerController;
    public override void Initialize()
    {
        _mainMenu.SetActive(true);
        _creditsMenu.SetActive(false);
        _optionsMenu.SetActive(false);
    }

    private void OnEnable()
    {
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        //_playerController.enabled = false;
        _cameraController = GameObject.Find("Player Camera").GetComponent<CameraController>();
        //_cameraController.enabled = false;
        _manualCameraController = GameObject.Find("Player Camera").GetComponent<ManualCameraController>();
        if (!GameManager.Instance.IsMainMenuLoaded)
        {
            _playerController.enabled = false;
            _cameraController.enabled = false;
        }

            //GameManager.onHubRevive += Deactivate;

        }

    private void OnDisable()
    {
        // necessary so that the camera is properly in normal player tracking mode when entering a new scene without main menu
        if (!GameManager.Instance.IsMainMenuLoaded)
        {
            _playerController.enabled = true;
            _cameraController.enabled = true;
        }

        //GameManager.onHubRevive -= Deactivate;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (GameManager.Instance.IsMainMenuLoaded)
        {
            Deactivate();
        }
    }

    // Public methods for all of the buttons ---
    public void StartButton()
    {
        StartCoroutine(DoStartGame());

    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void OptionsButton()
    {
        _mainMenu.SetActive(false);
        _creditsMenu.SetActive(false);
        _optionsMenu.SetActive(true);
    }

    public void CreditsButton()
    {
        _mainMenu.SetActive(false);
        _creditsMenu.SetActive(true);
        _optionsMenu.SetActive(false);
    }

    public void BackButton()
    {
        _mainMenu.SetActive(true);
        _creditsMenu.SetActive(false);
        _optionsMenu.SetActive(false);
    }

    private void Deactivate()
    {
        Debug.Log("Ah");
        //_playerController.enabled = true;
        //_cameraController.enabled = true;
        ViewManager.Show<InGameUIView>(false);
    }

    IEnumerator DoStartGame()
    {
        GameObject.Find("Ambient Audio").GetComponent<MusicController>().FadeIn();
        GameObject.Find("Title Music Audio").GetComponent<MusicController>().FadeOut();
        _manualCameraController.moveToGameStart();
        _mainMenu.SetActive(false);
        yield return new WaitUntil(() => !_manualCameraController.activeCoroutine);

        
        _playerController.enabled = true;
        _cameraController.enabled = true;
        GameManager.Instance.IsMainMenuLoaded = true;
        ViewManager.Show<InGameUIView>(false);
        yield return null;
    }
}
