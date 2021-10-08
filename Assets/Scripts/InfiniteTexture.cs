using UnityEngine;

public class InfiniteTexture : MonoBehaviour
{
    #region Fields
    [SerializeField] private Vector2 offset;
    [SerializeField] private Vector2 startingOffset;
    private Material material;
    #endregion

    #region Initialization
    private void Awake()
    {
        material = GetComponent<Renderer>().material;
        material.mainTextureOffset += startingOffset;
    }
    #endregion

    #region Update
    private void FixedUpdate()
    {
        material.mainTextureOffset += offset;
    }
    #endregion
}
