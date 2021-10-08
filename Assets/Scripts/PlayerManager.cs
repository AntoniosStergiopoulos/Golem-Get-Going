using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Fields
    public static PlayerManager _instance;
    public static PlayerManager instance { get { return _instance; } }
    public int playerCurrentHealth;
    #endregion

    #region Initialization
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion
}
