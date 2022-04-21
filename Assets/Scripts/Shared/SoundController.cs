using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundController : MonoBehaviourSingleton<SoundController>
{
    public static float MasterVolume;
    public static float BGMVolume;
    public static float SEVolume;
    public static float VoiceVolume;

    public GameObject Source;

    void Start()
    {
        SaveSystem.LoadOptionMenu();
    }

    public void SetMasterVolume(float sliderValue)
    {
        SoundController.MasterVolume = sliderValue;
        SaveSystem.SaveOptionMenu();
    }

    public void SetBGMVolume(float sliderValue)
    {
        SoundController.BGMVolume = sliderValue;
        SaveSystem.SaveOptionMenu();
    }

    public void SetSEVolume(float sliderValue)
    {
        SoundController.SEVolume = sliderValue;
        SaveSystem.SaveOptionMenu();
    }

    public void SetVoiceVolume(float sliderValue)
    {
        SoundController.VoiceVolume = sliderValue;
        SaveSystem.SaveOptionMenu();
    }

    public AudioSource CreateSource(AudioClip sound)
    {
        GameObject obj = Instantiate(Source);
        DontDestroyOnLoad(obj);
        AudioSource source = obj.GetComponent<AudioSource>();
        source.clip = sound;
        //obj.transform.SetParent(gameObject.transform);
        return source;
    }

    public void Play(AudioClip sound)
    {
        AudioSource source = GameObject.FindObjectsOfType<AudioSource>().ToList().Find(x => x.clip == sound);

        if (source == default(AudioSource)) source = CreateSource(sound);

        source.volume = SEVolume * MasterVolume;
        source.Play();
    }

    public void PlayVoice(AudioClip sound)
    {
        AudioSource source = GameObject.FindObjectsOfType<AudioSource>().ToList().Find(x => x.clip == sound);

        if (source == default(AudioSource)) source = CreateSource(sound);

        source.volume = VoiceVolume * MasterVolume;
        source.Play();
    }
}
