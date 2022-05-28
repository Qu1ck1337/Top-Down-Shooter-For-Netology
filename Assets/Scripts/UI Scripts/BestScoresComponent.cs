using RotaryHeart.Lib.SerializableDictionary;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BestScoresComponent : MonoBehaviour
{
    public BestScorePlaceholders _bestScoreLevelsNamesPlaceholders = new BestScorePlaceholders();

    private TextMeshProUGUI _text;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();

        foreach (string sceneName in _bestScoreLevelsNamesPlaceholders.Keys)
        {
            if (PlayerPrefs.HasKey(sceneName))
            {
                int score = PlayerPrefs.GetInt(sceneName);
                _bestScoreLevelsNamesPlaceholders.TryGetValue(sceneName, out string placeholder);
                _text.text += "\n" + placeholder + ": " + score.ToString();
            }
        }
    }


}

[System.Serializable]
public class BestScorePlaceholders : SerializableDictionaryBase<string, string> { }
