using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_controller : MonoBehaviour
{
    #region movement_variable
    public float movespeed;
    float x_input;
    float y_input;
    #endregion

    #region attack_variables
    public float damage;
    public float attackspeed;
    float attackTimer;
    public float hitboxTiming;
    public float endAnimationTiming;
    bool isAttacking;
    Vector2 CurrDirection;
    #endregion

    #region health_variables
    public float maxHealth;
    float currHealth;
    public Slider hpSlider;
    #endregion

    #region physics_functions
    Rigidbody2D PlayerRB;
    #endregion

    #region animation_compoents
    Animator anim;
    #endregion

    #region Unity_functions
    //called on creation
    private void Awake()
    {
        currHealth = maxHealth;
        anim = GetComponent<Animator>();
        PlayerRB = GetComponent<Rigidbody2D>();
        attackTimer = 0;
        hpSlider.value = currHealth / maxHealth;
    }
    //called every frame
    private void Update()
    {
        //does nothing if attacking
        if (isAttacking)
        {
            return;
        }
        //get input
        x_input = Input.GetAxisRaw("Horizontal");
        y_input = Input.GetAxisRaw("Vertical");

        Move();

        //attacking
        if (Input.GetKeyDown(KeyCode.J)&& attackTimer <= 1)//only records initial press
        {
            Attack();
        }
        else
        {
            attackTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Interact();
        }
    }
    #endregion

    #region movement_functions
    //moves player from inputs
    private void Move()
    {
        anim.SetBool("Moving", true);
        //if player pressing D, a, w, s
        if (x_input > 0)
        {
            PlayerRB.velocity = Vector2.right * movespeed;
            CurrDirection = Vector2.right;
        }
        else if (x_input < 0)
        {
            PlayerRB.velocity = Vector2.left * movespeed;
            CurrDirection = Vector2.left;
        }
        else if (y_input > 0)
        {
            PlayerRB.velocity = Vector2.up * movespeed;
            CurrDirection = Vector2.up;
        }
        else if (y_input < 0) 
        {
            PlayerRB.velocity = Vector2.down * movespeed;
            CurrDirection = Vector2.down;
        }
        else
        {
            PlayerRB.velocity = Vector2.zero;
            anim.SetBool("Moving", false);
        }
        //animation variables
        anim.SetFloat("DirX", CurrDirection.x);
        anim.SetFloat("DirY", CurrDirection.y);
    }
    #endregion

    #region attack_functions
    //Attacks in direction player facing
    private void Attack()
    {

        Debug.Log("Attacking now");
        Debug.Log(CurrDirection);

        attackTimer = attackspeed;
        //
        StartCoroutine(AttackRoutine());
        attackTimer = attackspeed;
    }
    //Handles animations and hitboxes
    IEnumerator AttackRoutine()
    {
        //freezes player during attack
        isAttacking = true;
        PlayerRB.velocity = Vector2.zero;
        //start animation
        anim.SetTrigger("Attacking");

        //start sound effect
        FindObjectOfType<AudioManager>().Play("PlayerAttack");
        //brief pause before hitbox calculation
        yield return new WaitForSeconds(hitboxTiming);

        Debug.Log("Cast hitbox now");

        //create hitbox
        RaycastHit2D[] hits = Physics2D.BoxCastAll(PlayerRB.position + CurrDirection, Vector2.one, 0f, Vector2.zero, 0);
        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log(hit.transform.name);
            if (hit.transform.CompareTag("Enemy"))
            {
                Debug.Log("MASSIVE DAMAAAAAGE!!!!");
                hit.transform.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
        yield return new WaitForSeconds(endAnimationTiming);

        //reenables movement
        isAttacking = false;
    }
    #endregion

    #region health_functions
    //Take damage based on value parimeter
    public void TakeDamage(float value)
    {
        //sound FX
        FindObjectOfType<AudioManager>().Play("PlayerHurt");
        //Decrement health
        currHealth -= value;
        Debug.Log("Health is now " + currHealth.ToString());
        //Change UI
        hpSlider.value = currHealth / maxHealth;
        if (currHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(float value)
    {
        //increment health
        currHealth += value;
        currHealth = Mathf.Min(currHealth, maxHealth);
        Debug.Log("Health is now " + currHealth.ToString());
        hpSlider.value = currHealth / maxHealth;
    }

    //destroys player & triggers end scene
    private void Die()
    {
        //Death sound
        FindObjectOfType<AudioManager>().Play("PlayerDeath");
        //death
        Destroy(this.gameObject);
        GameObject gm = GameObject.FindWithTag("GameController");
        gm.GetComponent<GameManager>().LoseGame();
    }
    #endregion

    #region interact_functions
    void Interact()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll(PlayerRB.position + CurrDirection, new Vector2(0.5f,0.5f), 0f, Vector2.zero,0);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.CompareTag("Chest"))
            {
                hit.transform.GetComponent<chest>().Interact();
            }
        }
    }
    #endregion
}
