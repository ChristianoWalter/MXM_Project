using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterSet 
{
    Player photonPlayer;

    public bool isInMatch = true;

        //vari�vel para evitar que o jogador pule sempre que mudar de dire��o com o bot�o pressionado
    bool canInputJump;

    protected override void Awake()
    {
        //execu��o do m�todo m�e
        base.Awake();

        if (isPuppet) return;
        if (photonView.IsMine) canInputJump = true;
        else rb.isKinematic = true;
    }

    protected override void Update()
    {
        base.Update();
        if (!photonView.IsMine || !isInMatch || isPuppet) return;

            //m�todo de controle da velocidade e movimenta��odo player
        if (canMove) rb.velocity = new Vector2(inputDirection.x * moveSpeed, rb.velocity.y);
        else if (!canMove || isDefending) rb.velocity = new Vector2(0f, rb.velocity.y);

    }

    protected override void Death()
    {
        photonView.RPC(nameof(StopPlayer), RpcTarget.AllBuffered);
        if (!photonView.IsMine) return;
        GameManager.instance.GameOver();
    }

    [PunRPC]
    void StopPlayer()
    {
        isInMatch = false;
    }

    #region inputs de ataque
    //m�todo de input para ataque leve
    public void QuickAttackAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine || !isInMatch) return;
        if (value.performed)
        {
            currentAttackType = attackTypes.quickAttack;
            //StartCoroutine(Attacking());
            Attack();
        }
    }

    //m�todo de input para ataque m�dio
    public void MediumAttackAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine || !isInMatch) return;
        if (value.performed)
        {
            currentAttackType = attackTypes.mediumAttack;
            //StartCoroutine(Attacking());
            Attack();
        }
    }

    //m�todo de input para ataque pesado
    public void HeavyAttackAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine || !isInMatch) return;
        if (value.performed)
        {
            currentAttackType = attackTypes.heavyAttack;
            //StartCoroutine(Attacking());
            Attack();
        }
    }
    
    //m�todo para input de ataque especial
    public void SpecialAttackAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine || !isInMatch) return;
        if (value.performed && OnGround() && !isCrouched)
        {
            SpecialAttack();
        }
    }
    #endregion

        //m�todo para input de defesa
    public void DefenseAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine || !isInMatch) return;
        if (value.performed && !isOutStamina)
        {
            canMove = false;
            isDefending = true;
        }
        else if (value.canceled && !isOutStamina)
        {
            canMove = true;
            isDefending = false;
        }
    }

        //a��o de agachar
    public void CrouchAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine || !isInMatch) return;

        if (value.performed)
        {
            Crouch();
        }
        else if (value.canceled && isCrouched) Crouch();
    } 

        //m�todo para receber input e movimentar o Player
    public void MovementAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine || !isInMatch) return;

        inputDirection = value.ReadValue<Vector2>();

            //m�todos input para definir o tipo de ataque especial
        if (inputDirection.x > 0) specialAttacks = attackTypes.mediumAttack;
        if (inputDirection.x < 0) specialAttacks = attackTypes.heavyAttack;

            //a��o de pulo
        if (inputDirection.y > 0 && OnGround() && canInputJump && !isDefending && !isOutStamina)
        {
            if (!canMove) return;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canInputJump = false;
            Debug.Log("cima");
        }

            //input out
        if (value.canceled)
        {
            if(inputDirection.x == 0) specialAttacks = attackTypes.quickAttack;        
            if(inputDirection.y == 0) canInputJump = true; 
        }
    }
}
