using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{

    public void ChangeSceneForStart(int sceneIndex) 
    {
        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadScene(sceneIndex);
    }
    public void ChangeSceneForLoad(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
