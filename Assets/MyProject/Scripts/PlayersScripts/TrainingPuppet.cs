using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrainingPuppet : CharacterSet
{
    bool isRecoveringLife;

    protected override void Awake()
    {
        isPuppet = true;
        base.Awake();
    }

    
    [PunRPC]
    public override void DamageEffect(float _knockback, float _knockup, bool _isDefended)
    {
        base.DamageEffect(_knockback, _knockup, _isDefended);
        if (isRecoveringLife)
        {
            StopCoroutine(RecoverLife());
            isRecoveringLife = false;
        }
        StartCoroutine(RecoverLife());
    }

    IEnumerator RecoverLife()
    {
        if (!isRecoveringLife)
        {
            yield return new WaitForSeconds(2f);
            isRecoveringLife = true;
            for (float i = currentHealth; i < maxHealth; i++)
            {
                currentHealth = Mathf.Min(i, maxHealth);
                healthBar.UpdateValue(currentHealth);
                yield return new WaitForSeconds(.1f);
            }
        }
    }
}
