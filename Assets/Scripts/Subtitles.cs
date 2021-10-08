using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Subtitles : MonoBehaviour
{
    #region Fields
    [SerializeField] private string[] subtitles;
    [SerializeField] private float writingSpeed;
    [SerializeField] private TextMeshProUGUI subtitleText;
    private KeyBindings keyBindings;
    private int currentSubtitleIndex;
    private char[] subtitlesChracters;
    private int currentCharacterIndex;
    private Coroutine writeCharsCoroutine;
    #endregion

    #region Initialization
    private void Awake()
    {
        keyBindings = new KeyBindings();
    }

    private void Start()
    {
        keyBindings.Player.NextDialogue.performed += cxt => NextDialogue(cxt.ReadValue<float>());
        keyBindings.Player.SkipIntro.performed += cxt => SkipIntro(cxt.ReadValue<float>());
        writeCharsCoroutine = StartCoroutine(WriteCharsCoroutine());
    }
    #endregion

    #region Events
    private void OnEnable()
    {
        keyBindings.Enable();
    }

    private void OnDisable()
    {
        keyBindings.Disable();
    }
    private void NextDialogue(float value)
    {
        if (value == 1)
        {
            if (subtitles[currentSubtitleIndex].Length == currentCharacterIndex + 1)
            {
                if (subtitles.Length != currentSubtitleIndex + 1)
                {
                    currentSubtitleIndex++;
                    writeCharsCoroutine = StartCoroutine(WriteCharsCoroutine());
                }
                else
                {
                    NavigateToFirstLevel();
                }
            }
            else
            {
                StopCoroutine(writeCharsCoroutine);
                WriteTheWholeSubtitle();
            }
        }
    }
    private void WriteTheWholeSubtitle()
    {
        subtitleText.text = subtitles[currentSubtitleIndex];
        currentCharacterIndex = subtitles[currentSubtitleIndex].Length - 1;
    }

    private void SkipIntro(float value)
    {
        if (value == 1)
        {
            NavigateToFirstLevel();
        }
    }

    private void NavigateToFirstLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    #endregion

    #region Coroutines
    private IEnumerator WriteCharsCoroutine()
    {
        subtitleText.text = "";
        currentCharacterIndex = 0;
        subtitlesChracters = subtitles[currentSubtitleIndex].ToCharArray();
        for (int i = 0; i < subtitlesChracters.Length; i++)
        {
            yield return new WaitForSeconds(writingSpeed);
            subtitleText.text += subtitlesChracters[i];
            currentCharacterIndex = i;
        }
    }
    #endregion
}
