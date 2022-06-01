using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundAssistant : MonoBehaviour
{
    [SerializeField]
    private string _soundVolumeSettingsName;

    private void Start()
    {
        if (PlayerPrefs.HasKey(_soundVolumeSettingsName))
        {
            float value = PlayerPrefs.GetFloat(_soundVolumeSettingsName);
            AudioSource[] _audioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audio in _audioSources)
            {
                audio.volume = audio.volume * value;
            }
        }
        else
        {
//#if UNITY_EDITOR
//            Debug.LogError($"В памяти нет настроек под названием {_soundVolumeSettingsName}");
//#endif
        }
    }
}
