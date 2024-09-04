using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankingObjectScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI playerVictories;
    [SerializeField] TextMeshProUGUI playerDefeats;
    public LayoutElement element;

    public void UpdateVisual(string _name, string _victories, string _defeats)
    {
        playerName.text = _name;
        playerVictories.text = _victories;
        playerDefeats.text = _defeats;
    }
}
