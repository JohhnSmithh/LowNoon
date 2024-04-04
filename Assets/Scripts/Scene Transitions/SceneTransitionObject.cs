using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionObject : MonoBehaviour
{
    public string SceneName;

    public delegate void OnSceneTransition();
    public static event OnSceneTransition onSceneTransition; // Event for when the bullet fires

    // Start is called before the first frame update
    void Start()
    { }

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

        SceneManager.LoadScene(SceneName);
        yield return null;
    }
}
