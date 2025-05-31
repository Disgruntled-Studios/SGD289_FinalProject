using UnityEngine;
using System;

public class LittleAlienScript : MonoBehaviour
{
    //On this script, I want the this small alien to take a player life if player collides with it, but
    //if the player jumps on top of it, I want the enemy to become false and for player to bounce.
    //enemies also deactivate when player is out of range (out of sight) (maybe this is an event for all enemies accessed by a collider on the player)
    //and reactivate when player gets back within range. (in sight)

    /*


    [Header("Damage Variables")]
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;
    //[SerializeField] private Rigidbody rb;
    [SerializeField] private LayerMask playerLayer;
    private float cooldownTimer = Mathf.Infinity;
    private Animator anim;

    public bool dead = false;

    //[SerializeField]
    //private GameObject attackHitbox;



    //Enemy health assign in inspector.
    //[SerializeField]
    private Health enemyHealth;
    private Health targetHealth;

    //PlayerAttack playerAttack;

    private EnemyPatrol enemyPatrol;
    public delegate void StopPatrollingDelegate();

    StopPatrollingDelegate sp;

    //Idea, it would be nice to have a class or variable or something that is assigned at the top with info for all stats.



    private void Awake()
    {

        dead = false;
        //anim = GetComponent<Animator>();
        //enemyHealth = this.GetComponent<Health>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
       // enemyHealth.OnHealthReachedZero += HandleEnemyDeath;
        //OnHealthReachedZero += HandleEnemyDeath;
        //OnDamaged += HandleDamage;
        sp = enemyPatrol.StopPatrolling;
    }

    private void Update()
    {
        if (!dead)
        {
            cooldownTimer += Time.deltaTime;
            //Attack only when player is in sight

            if (PlayerInSight())
            {
                if (cooldownTimer >= attackCooldown)
                {
                    cooldownTimer = 0;
                    anim.SetTrigger("meleeAttack");
                }
            }

            if (enemyPatrol != null)
            {
                //eney patrol is enabled when player is not in sight and vice versa.
                enemyPatrol.enabled = !PlayerInSight();
            }
        }
    }

    //enemy takes damage from player
    private void OnTriggerEnter(Collider other)
    {
        //if enemy runs into player, player loses a life. It also checks if player is out of lives.
        if(other.gameObject.CompareTag("Player"))
        {
                playerLives--;
                print("player lives is " + playerLives);
                CheckGameOver();
        }
    }



    //For visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //transform.local scale so it changes as we turn.
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance, new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
    }



    //function for event on attack animation
    private void DamagePlayer()
    {
        if (PlayerInSight())
        {
            //Damages player health. Maybe put an object to appear
            //targetHealth.Damage(damage);
            attackHitbox.SetActive(true);
            //print("doing " + damage + "to player");
        }
    }

    private void EndAttack()
    {
        attackHitbox.SetActive(false);
    }

    //Ok, so he said to make a hitbox and activate or deactivate it.




    public void HandleEnemyDamage()
    {

        if (enemyHealth.CurrentHealth < 1)
        {
            print("enemydying");
            HandleEnemyDeath();
        }
        else
        {
            anim.SetTrigger("hit");
            print("enemyhit");
        }

    }

    private void HandleEnemyDeath()
    {
        dead = true;
        sp?.Invoke();
        boxCollider.enabled = false;
        anim.SetTrigger("die");
        print("enemy died");
        //disable enemy and maybe fade them out.

    }

    private void TurnOffEnemy()
    {
        this.transform.parent.gameObject.SetActive(false);
    }

    */
}




