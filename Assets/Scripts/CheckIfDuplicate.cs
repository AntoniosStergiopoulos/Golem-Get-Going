using UnityEngine;

public class CheckIfDuplicate : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject gameObjectToControl;
    [SerializeField]private GameObjectTags gameObjectTag;
    private enum GameObjectTags { GUI, PlayerManager };
    #endregion

    #region Initialization
    void Start()
    {
        int numberOfObjectWithSameTag = 0;
        foreach (var go in GameObject.FindGameObjectsWithTag(gameObjectTag.ToString()))
        {
            numberOfObjectWithSameTag++;
        }
        if (numberOfObjectWithSameTag >= 2)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObjectToControl.SetActive(true);
        }
    }
    #endregion
}
