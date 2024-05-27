using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterSet : MonoBehaviourPun
{
    public static CharacterSet instance;

    [Header("Base Stats")]
    //status de vida do Player
    [SerializeField] protected bool isInvencible;
    [SerializeField] float maxHealth;
    //Está em serializefield para testes de funcionamento, após testado e aprovado remover o serializefield
    public float currentHealth;

    //status de movimentação do Player
    public bool canMove;
    public bool isCrouched;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float jumpForce;

    [Header("Combat Stats")]
    //status de combate    
    //Após testado e aprovado remover o serializefield
    [SerializeField]protected float currentDamage;
    [SerializeField] protected float[] damages;
    //Após testado e aprovado remover o serializefield
    [SerializeField] protected float currentAttackRange;
    [SerializeField] protected float[] attackRanges;
    //Após testado e aprovado remover o serializefield
    [SerializeField] protected bool canAttack;
    [SerializeField] protected LayerMask oponentLayer;
    //Após testado e aprovado remover o serializefield
    [SerializeField] protected float currentKnockupValue;
    //Após testado e aprovado remover o serializefield
    [SerializeField] protected float currentKnockbackValue;

    [Header("Attacks")]
    //Após testado e aprovado remover o serializefield
    [SerializeField] protected attackTypes currentAttackType;
    [SerializeField] protected float[] knockupValues;
    [SerializeField] protected float[] knockbackValues;
    protected int gatlingCombo;

    [Header("Special Stats")]
    [SerializeField] protected float[] specialKnockbackValues;
    [SerializeField] protected float[] specialKnockupValues;
    [SerializeField] protected float maxEnergy;
    //Após testado e aprovado remover o serializefield
    [SerializeField] protected float currentEnergy;
    [SerializeField] protected float[] energyCost;
    [SerializeField] protected bool canUseSpecial;
    //Após testado e aprovado remover o serializefield
    [SerializeField] protected attackTypes specialAttacks;
    [SerializeField] protected GameObject projectile;

    [Header("Defense")]
    public bool isDefending;
    [SerializeField] protected float defenseDecrement;
    [SerializeField] protected float maxDefenseShield;
    [SerializeField]protected float currentDefenseShield;
    //Após testado e aprovado remover o serializefield
    protected bool isOutStamina = false;
    [SerializeField] protected float timeStuned;
    protected enum attackTypes {quickAttack, mediumAttack, heavyAttack};

    [Header("Components")]
    public Rigidbody2D rb;
    public Transform currentAttackPoint;
    public Transform[] attackPoints;
    public Transform oponentDirection;
    public Transform projectilePoint;

    //variáveis para controle do pulo
    [SerializeField] protected Transform foot;
    [SerializeField] protected LayerMask ground;

    //variáveis para input e mudança de direção
    protected Vector2 inputDirection;
    protected float walkSpeed = 1;
    protected bool canCrouch;

    //variável para animator do personagem
    [SerializeField] protected Animator anim;


    protected virtual void Awake()
    {
        instance = this;
        currentHealth = maxHealth;
        currentDefenseShield = maxDefenseShield;
        currentEnergy = maxEnergy;
    }

    protected virtual void Update()
    {
            //diminuição e recarga do escudo
        ShieldUsing();

            //Animações
        Animations();
    }

    //métodos de combate (efeitos, aplicação e recepção de dano)
    #region Combat Methods

    //método de recepção de dano
    public void TakeDamage(float _damage, float _knockback, float _knockup, bool _canDefend, bool _crouchAttack, bool _midAttack)
    {
        if (!isInvencible)
        {
            bool defendDamage;

            //evento de mitigação de dano
            if (isDefending && _canDefend)
            {
                if (_midAttack || _crouchAttack == isCrouched)
                {
                    currentHealth = Mathf.Max(currentHealth - _damage * defenseDecrement, 0);
                    defendDamage = true;
                }
                else
                {
                    currentHealth = Mathf.Max(currentHealth - _damage, 0);
                    defendDamage = false;
                }
            }
            else
            {
                currentHealth = Mathf.Max(currentHealth - _damage, 0);
                defendDamage = false;
            }
            Debug.Log(currentHealth);

            //evento pós mitigação de dano
            if (currentHealth == 0 && photonView.IsMine) Death();
            else DamageEffect(_knockback, _knockup, defendDamage);
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

    //Método para chamar ataque básico
    protected IEnumerator Attacking()
    {
        if (OnGround()) canMove = false;
        else gatlingCombo = 1;

        anim.SetInteger("AttackType", (int)currentAttackType);
        currentDamage = damages[(int)currentAttackType];
        currentAttackRange = attackRanges[(int)currentAttackType];
        currentKnockbackValue = knockbackValues[(int)specialAttacks];
        currentKnockupValue = knockupValues[(int)specialAttacks];

        if (!isCrouched) currentAttackPoint = attackPoints[0];
        else currentAttackPoint = attackPoints[1];

        canAttack = false;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(.1f);
        anim.SetBool("CanGatling", canAttack);
    }

    //Método para chamar ataque especial
    protected virtual void SpecialAttack()
    {
        if (canAttack && currentEnergy >= energyCost[(int)specialAttacks] && OnGround())
        {
            currentEnergy -= energyCost[(int)specialAttacks];
            anim.SetInteger("AttackType", (int)specialAttacks);
            gatlingCombo = 1;
            currentKnockupValue = specialKnockupValues[(int)specialAttacks];
            currentKnockbackValue = specialKnockbackValues[(int)specialAttacks];
            currentDamage = damages[(int)specialAttacks];
            currentAttackRange = attackRanges[(int)specialAttacks];
            canAttack = false;
            canMove = false;
            anim.SetTrigger("SpecialAttack");
        }
    }



    //método de morte
    public void Death()
    {

    }

    //método de efeito de dano
    public void DamageEffect(float _knockback, float _knockup, bool _isDefended)
    {
        //mudança de efeitos em caso de defesa
        if (_isDefended) 
        {
            
        }
        else
        {
            rb.velocity = new Vector2(_knockback, _knockup);
            anim.SetTrigger("Damaged");
        }
    }
    #endregion


    //Métodos chamados nas animações para causar dano ou resetar ataques
    #region On Animation Methods

    //Método para aplicar dano comum
    public virtual void DealDamage()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(currentAttackPoint.position, currentAttackRange * 2, oponentLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<CharacterSet>().TakeDamage(currentDamage, currentKnockbackValue, currentKnockupValue, true, isCrouched, false);
            if(enemy.GetComponent<CharacterSet>().isDefending == false)
            {
                currentEnergy++;
            }
        }
    }

    //Método para chamar projétil
    public virtual void ProjectileAttack()
    {
        if (gameObject.layer == 7) projectile.tag = "ProjectileTwo";
        else if (gameObject.layer == 8) projectile.tag = "ProjectileOne";

        Instantiate(projectile, projectilePoint.position, projectilePoint.rotation).GetComponent<ProjectileScript>().direction = new Vector2(transform.localScale.x, 0f); 
    }
    
    //Combat restore
    public void BeforeAttack()
    {
        if (gatlingCombo == 0) return;
        else
        {
            canAttack = true;
            canMove = true;
            gatlingCombo = 0;
            Debug.Log(gatlingCombo);
        }
    }

    //ativação do Gatling por meio de animation event
    public void GatlingActivate()
    {
        if (gatlingCombo < 3)
        {
            canAttack = true;
            //anim.SetBool("CanGatling", canAttack);
            gatlingCombo++;
            Debug.Log(gatlingCombo);
        }
        else canAttack = false; 
            
        anim.SetBool("CanGatling", canAttack);
    }
    #endregion

    //método para verificação do Player no chão
    protected bool OnGround()
    {
        return Physics2D.OverlapCircle(foot.position, .1f, ground);
    }

    //método para animações
    public void Animations()
    {

        //Chamando propriedade onground para mudar animação
        anim.SetBool("OnGround", OnGround());

        //mudança de direção do sprite do player
        if ((oponentDirection.position.x > gameObject.transform.position.x && transform.localScale.x < 0) || (oponentDirection.position.x < gameObject.transform.position.x && transform.localScale.x > 0))
        {
            Vector2 _localScale = transform.localScale;
            _localScale.x *= -1f;
            transform.localScale = _localScale;
            walkSpeed = _localScale.x;
        }

        //controle das animações de "andar" com base na velocidade do Player
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("WalkSpeed", walkSpeed * inputDirection.x);
    }

    //método para agachar
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

        anim.SetBool("Crouched", isCrouched);
    }

    #region defense
    //sequência de quebra de defesa
    protected IEnumerator OutStamina()
    {
        isOutStamina = true;
        isDefending = false;
        canAttack = false;
        canMove = false;
        anim.SetTrigger("OutStamina");
        yield return new WaitForSeconds(timeStuned);
        anim.SetTrigger("Recharged");
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
