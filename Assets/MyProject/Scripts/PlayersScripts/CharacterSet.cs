using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterSet : MonoBehaviourPun
{
    [Header("Base Stats")]
    //status de vida do Player
    [SerializeField] protected bool isInvencible;
    [SerializeField] float maxHealth;
    [SerializeField] HealthBar healthBar;
    //Est� em serializefield para testes de funcionamento, ap�s testado e aprovado remover o serializefield
    public float currentHealth;

    //status de movimenta��o do Player
    public bool canMove;
    public bool isCrouched;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float jumpForce;

    [Header("Combat Stats")]
    //status de combate    
    //Ap�s testado e aprovado remover o serializefield
    [SerializeField]protected float currentDamage;
    [SerializeField] protected float[] damages;
    //Ap�s testado e aprovado remover o serializefield
    [SerializeField] protected float currentAttackRange;
    [SerializeField] protected float[] attackRanges;
    //Ap�s testado e aprovado remover o serializefield
    [SerializeField] protected bool canAttack;
    [SerializeField] protected LayerMask oponentLayer;
    //Ap�s testado e aprovado remover o serializefield
    [SerializeField] protected float currentKnockupValue;
    //Ap�s testado e aprovado remover o serializefield
    [SerializeField] protected float currentKnockbackValue;

    [Header("Attacks")]
    //Ap�s testado e aprovado remover o serializefield
    [SerializeField] protected attackTypes currentAttackType;
    [SerializeField] protected float[] knockupValues;
    [SerializeField] protected float[] knockbackValues;
    protected int gatlingCombo;

    [Header("Special Stats")]
    [SerializeField] protected float[] specialKnockbackValues;
    [SerializeField] protected float[] specialKnockupValues;
    [SerializeField] protected float maxEnergy;
    //Ap�s testado e aprovado remover o serializefield
    [SerializeField] protected float currentEnergy;
    [SerializeField] protected float[] energyCost;
    [SerializeField] protected bool canUseSpecial;
    //Ap�s testado e aprovado remover o serializefield
    [SerializeField] protected attackTypes specialAttacks;
    [SerializeField] protected GameObject projectile;

    [Header("Defense")]
    public bool isDefending;
    [SerializeField] protected float defenseDecrement;
    [SerializeField] protected float maxDefenseShield;
    [SerializeField]protected float currentDefenseShield;
    //Ap�s testado e aprovado remover o serializefield
    protected bool isOutStamina = false;
    [SerializeField] protected float timeStuned;
    protected enum attackTypes {quickAttack, mediumAttack, heavyAttack};

    [Header("Components")]
    public Rigidbody2D rb;
    public Transform currentAttackPoint;
    public Transform[] attackPoints;
    public Transform oponentDirection;
    public Transform projectilePoint;

    //vari�veis para controle do pulo
    [SerializeField] protected Transform foot;
    [SerializeField] protected LayerMask ground;

    //vari�veis para input e mudan�a de dire��o
    protected Vector2 inputDirection;
    protected float walkSpeed = 1;
    protected bool canCrouch;

    //vari�vel para animator do personagem
    [SerializeField] protected Animator anim;


    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        //healthBar.SetMaxHealth(maxHealth);
        currentDefenseShield = maxDefenseShield;
        currentEnergy = maxEnergy;
    }

    protected virtual void Update()
    {
            //diminui��o e recarga do escudo
        ShieldUsing();

            //Anima��es
        Animations();
    }

    //m�todos de combate (efeitos, aplica��o e recep��o de dano)
    #region Combat Methods

    //m�todo de recep��o de dano 
    public void TakeDamage(float _damage, float _knockback, float _knockup, bool _canDefend, bool _crouchAttack, bool _midAttack)
    {
        photonView.RPC(nameof(DamageRPC), RpcTarget.All, _damage, _knockback, _knockup, _canDefend, _crouchAttack, _midAttack);
    }

    //método para validação de dano, via RPC para ambos os jogadores
    [PunRPC]
    protected void DamageRPC(float _damage, float _knockback, float _knockup, bool _canDefend, bool _crouchAttack, bool _midAttack)
    {
        if (!isInvencible)
        {
            bool defendDamage;

            //evento de mitiga��o de dano
            if (isDefending && _canDefend)
            {
                if (_midAttack || _crouchAttack == isCrouched)
                {
                    currentHealth = Mathf.Max(currentHealth - _damage * defenseDecrement, 0);
                    currentEnergy += .1f;
                    defendDamage = true;
                }
                else
                {
                    currentHealth = Mathf.Max(currentHealth - _damage, 0);
                    currentEnergy += .2f;
                    defendDamage = false;
                }
            }
            else
            {
                currentHealth = Mathf.Max(currentHealth - _damage, 0);
                currentEnergy += .2f;
                defendDamage = false;
            }
            Debug.Log(currentHealth);
            //healthBar.SetHealth(currentHealth);

            //evento p�s mitiga��o de dano
            if (currentHealth == 0) Death();
            else if (currentHealth > 0) DamageEffect(_knockback, _knockup, defendDamage);
        }
    }

    /*protected virtual void Attack()
    {
        if (canAttack)
        {
            if (OnGround()) canMove = false;
            else gatlingCombo = 1;

            anim.SetInteger("AttackType", (int)currentAttackType);
            currentDamage = damages[(int)currentAttackType];
            currentAttackRange = attackRanges[(int)currentAttackType];
            
            if(!isCrouched) currentAttackPoint = attackPoints[0];
            else currentAttackPoint = attackPoints[1];
            
            canAttack = false;
            anim.SetTrigger("Attack");
        }
    }*/

    //M�todo para chamar ataque b�sico
    protected IEnumerator Attacking()
    {
        if (OnGround()) canMove = false;
        //else gatlingCombo = 1;

        anim.SetInteger("AttackType", (int)currentAttackType);
        currentDamage = damages[(int)currentAttackType];
        currentAttackRange = attackRanges[(int)currentAttackType];
        currentKnockbackValue = knockbackValues[(int)specialAttacks];
        currentKnockupValue = knockupValues[(int)specialAttacks];

        if (!isCrouched) currentAttackPoint = attackPoints[0];
        else currentAttackPoint = attackPoints[1];

        canAttack = false;
        //anim.SetTrigger("Attack"); 
        photonView.RPC(nameof(CallTrigger), RpcTarget.All, "Attack");
        gatlingCombo++;

        yield return new WaitForSeconds(.1f);
        anim.SetBool("CanGatling", canAttack);
    }

    //M�todo para chamar ataque especial
    protected virtual void SpecialAttack()
    {
        if (canAttack && currentEnergy >= energyCost[(int)specialAttacks] && OnGround())
        {
            currentAttackPoint = attackPoints[0];
            currentEnergy -= energyCost[(int)specialAttacks];
            anim.SetInteger("AttackType", (int)specialAttacks);
            gatlingCombo = 1;
            currentKnockupValue = specialKnockupValues[(int)specialAttacks];
            currentKnockbackValue = specialKnockbackValues[(int)specialAttacks];
            currentDamage = damages[(int)specialAttacks];
            currentAttackRange = attackRanges[(int)specialAttacks];
            canAttack = false;
            canMove = false;
            photonView.RPC(nameof(CallTrigger), RpcTarget.All, "SpecialAttack");
            //anim.SetTrigger("SpecialAttack");
        }
    }



    //m�todo de morte
    public void Death()
    {

    }

    //m�todo de efeito de dano
    public void DamageEffect(float _knockback, float _knockup, bool _isDefended)
    {
        //mudan�a de efeitos em caso de defesa
        if (_isDefended)
        {

        }
        else
        {
            rb.velocity = new Vector2(_knockback * transform.localScale.x * -1, _knockup);
            StartCoroutine(DamagedMove());
            photonView.RPC(nameof(CallTrigger), RpcTarget.All, "Damaged");
            //anim.SetTrigger("Damaged");

            /*if (!OnGround() && _knockup <= 0)
            {
                rb.velocity = new Vector2(_knockback * transform.localScale.x * -1, .5f);
                StartCoroutine(DamagedMove());
                photonView.RPC(nameof(CallTrigger), RpcTarget.All, "Damaged");
                //anim.SetTrigger("Damaged");
            }
            else
            {
                rb.velocity = new Vector2(_knockback * transform.localScale.x * -1, _knockup);
                StartCoroutine(DamagedMove());
                photonView.RPC(nameof(CallTrigger), RpcTarget.All, "Damaged");
                //anim.SetTrigger("Damaged");
            }*/
        }
    }

    private IEnumerator DamagedMove()
    {
        canMove = false;
        yield return new WaitForSeconds(1f);
        canMove = true;

    }
    #endregion


    //M�todos chamados nas anima��es para causar dano ou resetar ataques
    #region On Animation Methods

    //M�todo para aplicar dano comum
    public virtual void DealDamage()
    {
        if (!photonView.IsMine) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(currentAttackPoint.position, currentAttackRange * 2, oponentLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<CharacterSet>().TakeDamage(currentDamage, currentKnockbackValue, currentKnockupValue, true, isCrouched, false); 
            if(enemy.GetComponent<CharacterSet>().isDefending == false)
            {
                currentEnergy += .4f;
                GatlingActivate();
            }
        }
    }

    //M�todo para chamar proj�til
    public virtual void ProjectileAttack()
    {
        if (gameObject.layer == 7) projectile.tag = "ProjectileTwo";
        else if (gameObject.layer == 6) projectile.tag = "ProjectileOne";

        Instantiate(projectile, projectilePoint.position, projectilePoint.rotation).GetComponent<ProjectileScript>().direction = new Vector2(transform.localScale.x, 0f); 
    }
    
    //Combat restore
    public void BeforeAttack()
    {
        if (gatlingCombo == 0) return;
        else
        {
            if (!isCrouched)
            {
                canMove = true;
            }
            canAttack = true;
            gatlingCombo = 0;
            Debug.Log(gatlingCombo);
        }
    }

    //ativa��o do Gatling por meio de animation event
    public void GatlingActivate()
    {
        if (gatlingCombo < 2)
        {
            canAttack = true;
            Debug.Log(gatlingCombo);
        }
        else canAttack = false; 
        anim.SetBool("CanGatling", canAttack);
    }
    #endregion

    //m�todo para verifica��o do Player no ch�o
    protected bool OnGround()
    {
        return Physics2D.OverlapCircle(foot.position, .1f, ground);
    }

    //método RPC para chamar triggers
    [PunRPC]
    protected void CallTrigger(string _triggerName)
    {
        anim.SetTrigger(_triggerName);
        Debug.Log(_triggerName);
    }

    //m�todo para anima��es
    public void Animations()
    {
        //Chamando propriedade onground para mudar anima��o
        anim.SetBool("OnGround", OnGround());

        //Mudan�a de dire��o do sprite do player
        if ((oponentDirection.position.x > gameObject.transform.position.x && transform.localScale.x < 0) || (oponentDirection.position.x < gameObject.transform.position.x && transform.localScale.x > 0))
        {
            Vector2 _localScale = transform.localScale;
            _localScale.x *= -1f;
            transform.localScale = _localScale;
            walkSpeed = _localScale.x;
        }

        //controle das anima��es de "andar" com base na velocidade do Player
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("WalkSpeed", walkSpeed * inputDirection.x);


        anim.SetBool("IsDefending", isDefending);
        anim.SetBool("Crouched", isCrouched);
        //anim.SetBool("CanGatling", canAttack);
    }

    //m�todo para agachar
    protected void Crouch()
    {
        if (OnGround() && !isCrouched && !isOutStamina)
        {
            canMove = false;
            isCrouched = true;
        }
        else if (isCrouched && !isOutStamina && !isDefending)
        {
            canMove = true;
            isCrouched = false;
        }
        else if (isCrouched && isOutStamina)
        {
            isCrouched = false;
        }
        else if (isCrouched && isDefending)
        {
            isCrouched = false;
        }
    }

    #region defense
    //sequ�ncia de quebra de defesa
    protected IEnumerator OutStamina()
    {
        isOutStamina = true;
        photonView.RPC(nameof(CallTrigger), RpcTarget.All, "OutStamina");
        //anim.SetTrigger("OutStamina");
        yield return new WaitForSeconds(.1f);
        isDefending = false;
        canAttack = false;
        canMove = false;
        yield return new WaitForSeconds(timeStuned);
        photonView.RPC(nameof(CallTrigger), RpcTarget.All, "Recharged");
        //anim.SetTrigger("Recharged");
        canAttack = true; 
        canMove = true;
        isOutStamina = false;
    }

    protected void ShieldUsing()
    {
        if (isDefending)
        {
            if (currentDefenseShield > 0) currentDefenseShield = Mathf.Max(currentDefenseShield - Time.deltaTime, 0);
            else if (currentDefenseShield == 0 && !isOutStamina) StartCoroutine(OutStamina());
        }
        else if (!isDefending && currentDefenseShield < maxDefenseShield)
        {
            currentDefenseShield = Mathf.Min(currentDefenseShield + Time.deltaTime, maxDefenseShield);
        }
    }

    //sistema de recarga de defesa
    protected IEnumerator ShieldRecharge()
    {
        yield return new WaitForSeconds(2);
        if (currentDefenseShield < maxDefenseShield)
        {
            currentDefenseShield = Mathf.Min(currentDefenseShield + Time.deltaTime, maxDefenseShield);
        }
    }
    #endregion
}
