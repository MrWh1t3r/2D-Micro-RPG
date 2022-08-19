using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")] 
    public int curHp;
    public int maxHp;
    public float moveSpeed;
    public int xpToGive;

    [Header("Target")] 
    public float chaseRange;
    public float attackRange;
    private Player _player;

    [Header("Attack")] 
    public int damage;
    public float attackRate;
    private float _lastAttackTime;
    
    private Rigidbody2D _rig;

    private void Awake()
    {
        _player = FindObjectOfType<Player>();
        
        _rig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float playerDist = Vector2.Distance(transform.position, _player.transform.position);

        if (playerDist <= attackRange)
        {
            if(Time.time - _lastAttackTime >= attackRange)
                Attack();

                _rig.velocity = Vector2.zero;
        }
        else if (playerDist <= chaseRange)
        {
            Chase();
        }
        else
        {
            _rig.velocity = Vector2.zero;
        }
    }

    void Chase()
    {
        Vector2 dir = (_player.transform.position - transform.position).normalized;

        _rig.velocity = dir * moveSpeed;
    }

    void Attack()
    {
        _lastAttackTime = Time.time;
        
        _player.TakeDamage(damage);
    }

    public void TakeDamage(int damageTaken)
    {
        curHp -= damageTaken;
        
        if(curHp<=0)
            Die();
    }

    void Die()
    {
        _player.AddXp(xpToGive);
        Destroy(gameObject);
    }
}
