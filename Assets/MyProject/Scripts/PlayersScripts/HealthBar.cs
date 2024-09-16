using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class HealthBar : MonoBehaviourPunCallbacks
{

    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxValue(float value)
    {
        photonView.RPC(nameof(SetMaxHealth), RpcTarget.All, value);
    }

    public void UpdateValue(float value)
    {
        photonView.RPC(nameof(SetHealth), RpcTarget.All, value);
    }

    [PunRPC]
    void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;

       //fill.color = gradient.Evaluate(1f);
    }

    [PunRPC]
    void SetHealth(float health)
    {
        slider.value = health;

        //fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
