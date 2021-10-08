using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadLevel : MonoBehaviour
{
    #region Fields
    public Slider slider;
    public TextMeshProUGUI percentageText;
    public GameObject canvas;
    #endregion

    #region Events
    private void OnEnable()
    {
        StartCoroutine(LoadAsynchronously());
    }
    #endregion

    #region Coroutines
    IEnumerator LoadAsynchronously()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(1);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            slider.value = progress;
            percentageText.text = progress * 100f + "%";
            yield return null;
        }
        gameObject.SetActive(false);
        canvas.SetActive(false);
    }
    #endregion
}
