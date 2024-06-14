using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationsEvents : MonoBehaviour
{
    [SerializeField] UnityEvent recalAttackEvent;
    [SerializeField] UnityEvent damageApplyEvent;
    [SerializeField] UnityEvent projectileEvent;

    public void RecalAttack()
    {
        recalAttackEvent.Invoke();
    }

    public void ApplyDamage()
    {
        damageApplyEvent.Invoke();
    }

    public void CallProjectile()
    {
        projectileEvent.Invoke();
    }
}
