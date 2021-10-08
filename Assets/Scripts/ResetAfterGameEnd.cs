using UnityEngine;

public class ResetAfterGameEnd : MonoBehaviour
{
    #region Fields
    private GameObject playerManager;
    private GameObject gui;
    #endregion

    #region Initialization
    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager");
        gui = GameObject.FindGameObjectWithTag("GUI");
    }
    #endregion

    #region Called from other scripts
    public void ResetGame()
    {
        Destroy(playerManager);
        Destroy(gui);
    }
    #endregion
}
