using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelNavigation : MonoBehaviour
{
    #region Public
    public void NavigateToLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }
    #endregion
}
