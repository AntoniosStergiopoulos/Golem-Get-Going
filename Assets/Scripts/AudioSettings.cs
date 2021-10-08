using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    #region Fields
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    #endregion

    #region Events
    public void OnMasterSliderChanged(float value)
    {
        audioMixer.SetFloat("masterVolume", value);
    }
    public void OnMusicSliderChanged(float value)
    {
        audioMixer.SetFloat("musicVolume", value);
    }
    public void OnSfxSliderChanged(float value)
    {
        audioMixer.SetFloat("sfxVolume", value);
    }
    #endregion
}
