using UnityEngine;

public class Quit : MonoBehaviour
{
    #region Called from other scripts
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
