using UnityEngine;
using UnityEngine.UI;

public class VolumeUI : MonoBehaviour

{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider MusicSlider;
    public void OnSFXSliderChanged()
    {
        AudioManager.Instance.ChangeVolume("SFX", SFXSlider.value);
        
    }
    // Update is called once per frame
    public void OnMusicSliderChanged()
    {
        AudioManager.Instance.ChangeVolume("Music", MusicSlider.value);
    }
}
