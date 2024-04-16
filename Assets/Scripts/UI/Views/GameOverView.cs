using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverView : View
{
    public override void Initialize()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RetryButton()
    {
        //IDK how to get this to not load the main menu
        SceneManager.LoadScene("0_Hub");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("0_Hub");
    }

    public void ExitGameButton()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        // time scale should not be frozen, or player death animation won't play out
    }

    private void OnDisable()
    {
    }
}
