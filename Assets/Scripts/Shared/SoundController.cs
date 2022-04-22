using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static float MasterVolume;
    public static float BGMVolume;
    public static float SEVolume;
    public static float VoiceVolume;
    public static GameObject Source;

    private static AudioSource _voiceSource;

    static void Start()
    {
        Source = GameObject.Find("Sound");
        if (Source == null) Source = GameObject.Find("Sound(Clone)");
        SaveSystem.LoadOptionMenu();

        if (_voiceSource == null) _voiceSource = CreateSource();
    }

    public static void SetMasterVolume(float sliderValue)
    {
        SoundController.MasterVolume = sliderValue;
        SaveSystem.SaveOptionMenu();
    }

    public static void SetBGMVolume(float sliderValue)
    {
        SoundController.BGMVolume = sliderValue;
        SaveSystem.SaveOptionMenu();
    }

    public static void SetSEVolume(float sliderValue)
    {
        SoundController.SEVolume = sliderValue;
        SaveSystem.SaveOptionMenu();
    }

    public static void SetVoiceVolume(float sliderValue)
    {
        SoundController.VoiceVolume = sliderValue;
        SaveSystem.SaveOptionMenu();
    }

    public static AudioSource CreateSource(AudioClip sound = null)
    {
        GameObject obj = Instantiate(Source);
        DontDestroyOnLoad(obj);
        AudioSource source = obj.GetComponent<AudioSource>();
        source.clip = sound;
        //obj.transform.SetParent(gameObject.transform);
        return source;
    }

    public static void Play(AudioClip sound)
    {
        if (Source == null) Start();
        AudioSource source = GameObject.FindObjectsOfType<AudioSource>().ToList().Find(x => x.clip == sound);

        if (source == default(AudioSource)) source = CreateSource(sound);

        source.volume = SEVolume * MasterVolume;
        source.Play();
    }

    public static void PlayVoice(AudioClip sound = null)
    {
        _voiceSource.clip = sound;
        _voiceSource.volume = VoiceVolume * MasterVolume;
        _voiceSource.Play();
    }

    public static void Pause()
    {
        _voiceSource.Stop();
    }
}
