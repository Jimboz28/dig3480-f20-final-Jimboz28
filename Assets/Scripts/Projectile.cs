using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2d;
    public int bossHealth = 5;
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }
    public void Launch(Vector2 direction, float force)
    {
        rigidbody2d.AddForce(direction * force);
    }
    // Update is called once per frame
    void Update()
    {
        if (transform.position.magnitude > 1000.0f)
        {
            Destroy(gameObject);
        }
    }
    /*void OnTriggerEnter2D(Collider2D other)
    {
       BossRobot damage = other.GetComponent<BossRobot>();
       
       if (damage != null)
       {
           if (damage.bossHealth < damage.bossMaxHealth)
           {
           damage.ChangeBossHealth(-1);
           Destroy (gameObject);
           }
       }
    }*/
    void OnCollisionEnter2D(Collision2D other)
    {
        EnemyController e = other.collider.GetComponent<EnemyController>();
        if (e != null)
        {
            e.Fix();
        }

        HardRobot b = other.collider.GetComponent<HardRobot>();
        if (b != null)
        {
            b.Fix();
        }
        BossRobot c = other.collider.GetComponent<BossRobot>();
        if (c != null)
        {
            if (c.bossHealth >= 0)
           {
           c.ChangeBossHealth(1);
           }
            if (c.bossHealth <= 0)
            {
                c.Fix();
            }
        }
    Destroy(gameObject);
    }
}