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

        //variável para evitar que o jogador pule sempre que mudar de direção com o botão pressionado
    bool canInputJump;

    protected override void Awake()
    {
        //execução do método mãe
        base.Awake();

        if (photonView.IsMine) canInputJump = true;
        else rb.isKinematic = true;
    }

    protected override void Update()
    {
        if (!isInMatch) return;
        base.Update();

        if (!photonView.IsMine) return;

            //método de controle da velocidade e movimentaçãodo player
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
    //método de input para ataque leve
    public void QuickAttackAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine) return;
        if (value.performed)
        {
            currentAttackType = attackTypes.quickAttack;
            StartCoroutine(Attacking());
        }
    }

    //método de input para ataque médio
    public void MediumAttackAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine) return;
        if (value.performed)
        {
            currentAttackType = attackTypes.mediumAttack;
            StartCoroutine(Attacking());
        }
    }

    //método de input para ataque pesado
    public void HeavyAttackAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine) return;
        if (value.performed)
        {
            currentAttackType = attackTypes.heavyAttack;
            StartCoroutine(Attacking());
        }
    }
    
    //método para input de ataque especial
    public void SpecialAttackAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine) return;
        if (value.performed && OnGround() && !isCrouched)
        {
            SpecialAttack();
        }
    }
    #endregion

        //método para input de defesa
    public void DefenseAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine) return;
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

        //ação de agachar
    public void CrouchAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine) return;

        if (value.performed)
        {
            Crouch();
        }
        else if (value.canceled && isCrouched) Crouch();
    } 

        //método para receber input e movimentar o Player
    public void MovementAction(InputAction.CallbackContext value)
    {
        if (!photonView.IsMine) return;

        inputDirection = value.ReadValue<Vector2>();

            //métodos input para definir o tipo de ataque especial
        if (inputDirection.x > 0) specialAttacks = attackTypes.mediumAttack;
        if (inputDirection.x < 0) specialAttacks = attackTypes.heavyAttack;

            //ação de pulo
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
            //if(inputDirection.y == 0 && isCrouched) Crouch(); canCrouch = true;     
            if(inputDirection.y == 0) canInputJump = true; 
        }
    }
}
