using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnKeyPress : MonoBehaviour
{
    public string sceneToLoad; // Name of the scene you want to load

    void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}