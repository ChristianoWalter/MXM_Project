using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class EnergyBar : MonoBehaviourPunCallbacks
{
    public Slider slider;

    public void SetMaxValue(float value)
    {
        photonView.RPC(nameof(SetMaxHealth), RpcTarget.All, value);
    }

    public void UpdateValue(float value)
    {
        photonView.RPC(nameof(SetHealth), RpcTarget.All, value);
    }

    [PunRPC]
    void SetMaxHealth(float energy)
    {
        slider.maxValue = energy;
        slider.value = energy;
    }

    [PunRPC]
    void SetHealth(float energy)
    {
        slider.value = energy;
    }
}
