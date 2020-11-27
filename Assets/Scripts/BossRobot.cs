﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRobot : MonoBehaviour
{
    public float speed;
    public bool vertical;
    public float changeTime = 3.0f;

    public int bossMaxHealth = 5;
    int bossCurrentHealth;
    public int bossHealth {get {return bossCurrentHealth; }}
    public ParticleSystem smokeEffect;

    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    bool broken = true;

    Animator animator;

    private RubyController rubyController;

    void Start()
    {
        bossCurrentHealth = bossMaxHealth;
        rigidbody2D = GetComponent<Rigidbody2D> ();
        timer = changeTime;
        animator = GetComponent<Animator>();

        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");
        if (rubyControllerObject != null)
        {
            rubyController = rubyControllerObject.GetComponent<RubyController>();
            print ("Found the RubyController Script!");

        }
        if (rubyController == null)
        {
            print ("Cannot find GameController Script!");
        }
    }

    void Update()
    {
        if (!broken)
        {
            return;
        }
        
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        if (!broken)
        {
            return;
        }
        
        Vector2 position = rigidbody2D.position;
        
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speed * direction;
            animator.SetFloat("Move X", 0);
            animator.SetFloat("Move Y", direction);
        }
        else
        {
        position.x = position.x + Time.deltaTime * speed * direction;
        animator.SetFloat("Move X", direction);
        animator.SetFloat("Move Y", 0);
        }

        rigidbody2D.MovePosition(position);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-5);
        }
    }
    public void ChangeBossHealth (int anumber)
    {
        bossCurrentHealth = Mathf.Clamp(bossCurrentHealth - anumber, 0, bossMaxHealth);
        Debug.Log(bossCurrentHealth + "/" + bossMaxHealth);
    }
    public void Fix()
    {
        
        broken = false;
        rigidbody2D.simulated = false;
        animator.SetTrigger("Fixed");
        rubyController.ChangeScore(1);
        rubyController.SetCountText();

        smokeEffect.Stop();
    }
}
