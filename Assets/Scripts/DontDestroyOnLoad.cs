using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    #region Initialization
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    #endregion
}
