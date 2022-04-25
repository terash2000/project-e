using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundOptionInit : MonoBehaviour
{
    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _bgmVolumeSlider;
    [SerializeField] private Slider _seVolumeSlider;
    [SerializeField] private Slider _voiceVolumeSlider;

    public void Start()
    {
        _masterVolumeSlider.value = SoundController.MasterVolume;
        _bgmVolumeSlider.value = SoundController.BGMVolume;
        _seVolumeSlider.value = SoundController.SEVolume;
        _voiceVolumeSlider.value = SoundController.VoiceVolume;
    }
}
