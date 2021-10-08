using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverHeatBar : MonoBehaviour
{
    #region Fields
    [SerializeField] private int maxOveHeat = 100;
    [SerializeField] private int timeToStartCooling = 2;
    [SerializeField] private int coolSpeed = 2;
    [SerializeField] private float coolSecondsPerTick = 0.1f;
    [SerializeField] private int overHeatDuration = 3;
    public static OverHeatBar _instance;
    public static OverHeatBar instance { get { return _instance; } }
    private Slider overHeatBar;
    public bool overHeated;
    private int currentOverHeat;
    private Coroutine cool;
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
        overHeatBar = GetComponent<Slider>();
    }

    void Start()
    {
        currentOverHeat = 0;
        overHeatBar.maxValue = maxOveHeat;
        overHeatBar.value = 0;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentOverHeat = 0;
        overHeatBar.value = 0;
        StopAllCoroutines();
        overHeated = false;
    }
    #endregion

    #region Events
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }
    #endregion

    #region Called from other scripts
    public void UseSteam(int value)
    {
        currentOverHeat += value;
        overHeatBar.value = currentOverHeat;
        if (cool != null)
            StopCoroutine(cool);

        cool = StartCoroutine(CoolEngine());
    }

    public bool HaveEnoughSteam(int value)
    {
        return currentOverHeat + value <= maxOveHeat;
    }

    public void OverHeat()
    {
        StartCoroutine(OverHeated());
    }
    #endregion

    #region Coroutines
    private IEnumerator CoolEngine()
    {
        yield return new WaitForSeconds(timeToStartCooling);

        while (currentOverHeat > 0)
        {
            currentOverHeat -= coolSpeed;
            overHeatBar.value = currentOverHeat;
            yield return new WaitForSeconds(coolSecondsPerTick);
        }
    }

    private IEnumerator OverHeated()
    {
        if (cool != null)
            StopCoroutine(cool);
        overHeated = true;
        currentOverHeat = maxOveHeat;
        overHeatBar.value = maxOveHeat;
        yield return new WaitForSeconds(overHeatDuration);
        overHeated = false;
        cool = StartCoroutine(CoolEngine());
    }
    #endregion
}
