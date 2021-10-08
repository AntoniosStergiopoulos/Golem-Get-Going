using System.Collections;
using UnityEngine;

public class CurrencySpawner : MonoBehaviour
{
    #region Fields
    [SerializeField] private GameObject currency;
    private GameObject canvas;
    #endregion

    #region Initialization
    private void Start()
    {
        canvas = GameObject.Find("GameUI");
    }
    #endregion

    #region Called from other scripts
    private void StartSpawning(int numberOfCurrency)
    {
        StartCoroutine(SpawnCoins(numberOfCurrency));
    }
    #endregion

    #region Coroutines
    private IEnumerator SpawnCoins(int numberOfCurrency)
    {
        for (int i = 0; i < numberOfCurrency; i++)
        {
            yield return new WaitForSeconds(0.2f);
            GameObject go = Instantiate(currency, transform.position, transform.rotation);
            go.transform.parent = canvas.transform;
            go.transform.position = Camera.main.WorldToScreenPoint(go.transform.position);
            RectTransform rt = go.GetComponent(typeof(RectTransform)) as RectTransform;
            rt.sizeDelta = new Vector2(50, 50);
        }
        Destroy(gameObject);
    }
    #endregion
}
