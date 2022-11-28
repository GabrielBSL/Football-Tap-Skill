using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;

    private Dictionary<string, Sound> sfxs = new Dictionary<string, Sound>();
    private Dictionary<string, Sound> musics = new Dictionary<string, Sound>();

    private string m_CurrentMusic = "-empty-";

    private float _fadeMusicVolume = 1;

    public float SFXVolume { get; private set; }
    public float MusicVolume { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (!PlayerPrefs.HasKey("sfx_volume"))
        {
            PlayerPrefs.SetFloat("sfx_volume", 1);
            PlayerPrefs.SetFloat("music_volume", 1);
        }

        SFXVolume = PlayerPrefs.GetFloat("sfx_volume");
        MusicVolume = PlayerPrefs.GetFloat("music_volume");

        foreach (Sound s in sounds)
        {
            if (s.isMusic)
            {
                AddToDictionary(s, musics, true);
                continue;
            }

            AddToDictionary(s, sfxs);
        }
    }

    public void PlaySFX(string name, bool continuous = false)
    {
        if (!sfxs.ContainsKey(name))
        {
            Debug.LogWarning("Efeito sonoro de nome '" + name + "' n�o encontrado");
            return;
        }

        if (!continuous)
        {
            sfxs[name].source.Stop();
        }

        sfxs[name].source.Play();
    }

    public void PlayMusic(string name)
    {
        if (!musics.ContainsKey(name))
        {
            Debug.LogWarning("M�sica de nome '" + name + "' n�o encontrado");
            return;
        }

        if (m_CurrentMusic.Equals(name))
        {
            Debug.LogWarning("M�sica '" + name + "' j� est� tocando");
            return;
        }

        if (m_CurrentMusic.Equals("-empty-"))
        {
            m_CurrentMusic = name;
        }

        musics[m_CurrentMusic].source.Stop();
        musics[name].source.Play();

        m_CurrentMusic = name;
    }

    public void FadeMusic(float desiredVolume, float duration)
    {
        LeanTween.value(_fadeMusicVolume, desiredVolume, duration)
            .setIgnoreTimeScale(true)
            .setOnUpdate((float value) =>
            {
                foreach (var music in musics)
                {
                    music.Value.source.volume = music.Value.volume * MusicVolume * value;
                }
            })
            .setOnComplete(() => 
            {
                _fadeMusicVolume = desiredVolume;
            });
    }

    private void AddToDictionary(Sound newSound, Dictionary<string, Sound> audioDictionary, bool loop = false)
    {
        newSound.source = gameObject.AddComponent<AudioSource>();
        newSound.source.clip = newSound.clip;

        newSound.source.volume = newSound.volume * (loop ? MusicVolume : SFXVolume);
        newSound.source.pitch = newSound.pitch;

        newSound.source.loop = loop;

        audioDictionary[newSound.name] = newSound;
    }

    public void SetSFXVolume(float newVolume)
    {
        PlayerPrefs.SetFloat("sfx_volume", newVolume);
        SFXVolume = newVolume;

        foreach (var sfx in sfxs)
        {
            sfx.Value.source.volume = sfx.Value.volume * newVolume;
        }
    }

    public void SetMusicVolume(float newVolume)
    {
        PlayerPrefs.SetFloat("music_volume", newVolume);
        MusicVolume = newVolume;

        foreach (var music in musics)
        {
            music.Value.source.volume = music.Value.volume * newVolume * _fadeMusicVolume;
        }
    }
}
