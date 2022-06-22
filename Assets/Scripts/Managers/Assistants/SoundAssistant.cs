using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAssistant : MonoBehaviour
{
    [SerializeField]
    private string _soundVolumeSettingsName;
    [SerializeField]
    private string _musicVolumeSettingsName;
    [SerializeField]
    private AudioClip _commonLevelSoundtrack;
    [SerializeField]
    private AudioClip _rampageLevelSoundtrack;
    [SerializeField]
    private AudioClip _endLevelSoundtrack;

    private AudioSource _cameraAudioSource;

    private void Start()
    {
        _cameraAudioSource = FindObjectOfType<Camera>().gameObject.GetComponent<AudioSource>();
        CommonLevelSoundtrack();

        if (PlayerPrefs.HasKey(_soundVolumeSettingsName))
        {
            float value = PlayerPrefs.GetFloat(_soundVolumeSettingsName);
            var finalBossAudio = GetComponent<FinalLevelAssistant>()?.getBoss.GetComponent<AudioSource>();  
            if (finalBossAudio != null)
                finalBossAudio.volume = finalBossAudio.volume * value;
            AudioSource[] _audioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audio in _audioSources)
            {
                if (audio != _cameraAudioSource)
                    audio.volume = audio.volume * value;
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError($"В памяти нет настроек под названием {_soundVolumeSettingsName}");
#endif
        }

        if (PlayerPrefs.HasKey(_musicVolumeSettingsName))
        {
            float value = PlayerPrefs.GetFloat(_musicVolumeSettingsName);
            _cameraAudioSource.volume = _cameraAudioSource.volume * value;
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError($"В памяти нет настроек под названием {_musicVolumeSettingsName}");
#endif
        }
    }

    public void CommonLevelSoundtrack()
    {
        _cameraAudioSource.clip = _commonLevelSoundtrack;
        _cameraAudioSource.Play();
    }

    public void RampageLevelSoundtrack()
    {
        _cameraAudioSource.clip = _rampageLevelSoundtrack;
        _cameraAudioSource.Play();
    }

    public void EndLevelSoundtrack()
    {
        _cameraAudioSource.clip = _endLevelSoundtrack;
        _cameraAudioSource.Play();
    }
}
