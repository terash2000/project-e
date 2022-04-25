using System.Linq;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static float MasterVolume;
    public static float BGMVolume;
    public static float SEVolume;
    public static float VoiceVolume;
    public static GameObject Source;
    private static AudioSource _bgmSource;
    private static AudioSource _voiceSource;

    static void Start()
    {
        Source = new GameObject("Sound");
        Source.AddComponent<AudioSource>();
        DontDestroyOnLoad(Source);
        SaveSystem.LoadOptionMenu();

        if (_voiceSource == null) _voiceSource = CreateSource();
        if (_bgmSource == null)
        {
            _bgmSource = CreateSource(SceneChanger.Instance.MainBGM);
            _bgmSource.loop = true;
            _bgmSource.volume = BGMVolume;
            _bgmSource.Play();
        }
    }

    public static void SetMasterVolume(float sliderValue)
    {
        SoundController.MasterVolume = sliderValue;
        SaveSystem.SaveOptionMenu();
    }

    public static void SetBGMVolume(float sliderValue)
    {
        _bgmSource.volume = sliderValue;
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

    public static void PlayVoice(AudioClip sound)
    {
        _voiceSource.clip = sound;
        _voiceSource.volume = VoiceVolume * MasterVolume;
        _voiceSource.Play();
    }

    public static void PlayBGM(AudioClip sound)
    {
        if (_bgmSource.clip == sound) return;
        _bgmSource.clip = sound;
        _bgmSource.Play();
    }

    public static void Pause()
    {
        _voiceSource.Stop();
    }
}
