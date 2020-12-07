using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCustomSceneController : MonoBehaviour
{
    public void LoadScene(string scene) => SceneManager.LoadScene(scene); 
}
