using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public string nickName;
    public int level;
    public int currentXP;

    public PlayerInfo(string _nickName, int _level, int _currentXP)
    {
        nickName = _nickName;
        level = _level;
        currentXP = _currentXP;
    }
}
