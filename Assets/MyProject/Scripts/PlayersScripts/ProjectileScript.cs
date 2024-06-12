using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    [Header("Acionando o projétil")]
    [SerializeField] float projectileSpeed;
    [SerializeField] Rigidbody2D rb;

    public Vector2 direction;

    [Header("Efeitos do projétil")]
    bool isMoving = true;
    [SerializeField] GameObject impactEffect;
    [SerializeField] float timeToDestroy;
    [SerializeField] Animator anim;

    [Header("Dano do projétil")]
    [SerializeField] bool canBeDefended;
    [SerializeField] int damage;
    [SerializeField] float knockbackValue;
    [SerializeField] float knockupValue;

    private void Start()
    {
        transform.localScale = new Vector2(direction.x, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        //adicionando velocidade ao tiro
        if (isMoving) rb.velocity = direction * projectileSpeed;
        else rb.velocity = Vector2.zero;
    }

    //Destruindo o tiro ao encostar em outro objeto
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.tag == "ProjectileTwo" && other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<CharacterSet>().TakeDamage(damage, knockbackValue, knockupValue, canBeDefended, false, true);
        }
        if (gameObject.tag == "ProjectileOne" && other.gameObject.tag == "PlayerTwo")
        {
            other.gameObject.GetComponent<CharacterSet>().TakeDamage(damage, knockbackValue, knockupValue, canBeDefended, false, true);
        }

        //efeito do impacto do tiro
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
        else StartCoroutine(DestroyProjectile());

    }


    private IEnumerator DestroyProjectile()
    {
        isMoving = false;
        anim.SetTrigger("Impact");
        yield return new WaitForSeconds(timeToDestroy);
        Destroy(gameObject);
    }
    //destruindo o tiro ao sair da câmera
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
