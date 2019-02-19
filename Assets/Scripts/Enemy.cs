using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region movement_variables
    public float movespeed;
    #endregion

    #region physics_componenets
    Rigidbody2D enemyRB;
    #endregion

    #region targeting_variables
    public Transform player;
    #endregion

    #region attack_variables
    public float explosionDamage;
    public float explosionRadius;
    public GameObject explosionObj;
    #endregion

    #region health_variables
    public float maxHealth;
    float currHealth;
    #endregion

    #region Unity_functions

    private void Awake()
    {
        enemyRB = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //Check to see if know where player is
        if (player == null)
        {
            return;
        }
        Move();
    }

    #endregion

    #region movement_functions
    //moves @ player
    private void Move()
    {
        //Calculate mvmnt vector
        Vector2 direction = player.position - transform.position;//direction
        enemyRB.velocity = direction.normalized * movespeed;//magnitude
    }
    #endregion

    #region attack_functions
    //creates dage box and causes damage
    private void Explode()
    {
        FindObjectOfType<AudioManager>().Play("Explosion");
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, explosionRadius, Vector2.zero);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Player"))
            {
                //cause damage
                Debug.Log("Hit player with explosion");
                //spawn explosion prefab
                Instantiate(explosionObj, transform.position, transform.rotation);
                hit.transform.GetComponent<Player_controller>().TakeDamage(explosionDamage);

            }
        }
        Destroy(this.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Explode();
        }
    }
    #endregion

    #region health_functions
    public void TakeDamage(float value)
    {
        FindObjectOfType<AudioManager>().Play("BatHurt");
        currHealth -= value;
        Debug.Log("Health is now " + currHealth.ToString());

        if (currHealth <= 0)
        {
            Die();
        }
    }
    //Destroys Enemey
    void Die()
    {
        Destroy(this.gameObject);
    }
    #endregion
}
