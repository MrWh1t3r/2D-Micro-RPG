using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Experience")]
    public int curLevel;
    public int curXp;
    public int xpToNextLevel;
    public float levelXpModifier;
    
    [Header("Stats")]
    public int curHp;
    public int maxHp;
    public int damage;
    public float moveSpeed;
    public float interactRange;
    public List<string> inventory = new List<string>();

    private Vector2 _facingDirection;

    [Header("Combat")] 
    public KeyCode attackKey;
    public float attackRange;
    public float attackRate;
    private float _lastAttackTime;
    
    [Header("Sprites")] 
    public Sprite downSprite;
    public Sprite upSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private Rigidbody2D _rig;
    private SpriteRenderer _sr;
    private ParticleSystem _hitEffect;
    private PlayerUI _ui;

    private void Awake()
    {
        _rig = GetComponent<Rigidbody2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _hitEffect = gameObject.GetComponentInChildren<ParticleSystem>();
        _ui = FindObjectOfType<PlayerUI>();
    }

    private void Start()
    {
        _ui.UpdateLevelText();
        _ui.UpdateHealthBar();
        _ui.UpdateXpBar();
    }

    private void Update()
    {
        Move();
        UpdateSpriteDirection();
        
        if (Input.GetKeyDown(attackKey))
        {
            if (Time.time - _lastAttackTime >= attackRate)
                Attack();
        }
        
        CheckInteract();
    }

    void CheckInteract()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _facingDirection, attackRange, 1 << 7);

        if (hit.collider != null)
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            _ui.SetInteractText(hit.collider.transform.position,interactable.interactDescription);
            
            if(Input.GetKeyDown(attackKey))
                interactable.Interact();
        }
        else
        {
            _ui.DisableInteractText();
        }
    }

    void Attack()
    {
        _lastAttackTime = Time.time;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, _facingDirection, attackRange, 1 << 6);

        if (hit.collider != null)
        {
            hit.collider.GetComponent<Enemy>().TakeDamage(damage);
            
            // play hit effect
            _hitEffect.transform.position = hit.collider.transform.position;
            _hitEffect.Play();
        }
    }

    void Move()
    {
        //get the horizontal and vertical keyboard inputs
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector2 velocity = new Vector2(x, y);

        if (velocity.magnitude != 0)
            _facingDirection = velocity;

        _rig.velocity = velocity * moveSpeed;
    }

    void UpdateSpriteDirection()
    {
        if (_facingDirection == Vector2.up)
            _sr.sprite = upSprite;
        else if (_facingDirection == Vector2.down)
            _sr.sprite = downSprite;
        else if (_facingDirection == Vector2.left)
            _sr.sprite = leftSprite;
        else if (_facingDirection == Vector2.right)
            _sr.sprite = rightSprite;
    }
    
    public void TakeDamage(int damageTaken)
    {
        curHp -= damageTaken;
        
        _ui.UpdateHealthBar();
        
        if(curHp<=0)
            Die();
    }

    void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void AddXp(int xp)
    {
        curXp += xp;
        
        _ui.UpdateXpBar();
        
        if(curXp>=xpToNextLevel)
            LevelUp();
    }

    void LevelUp()
    {
        curXp -= xpToNextLevel;
        curLevel++;

        xpToNextLevel = Mathf.RoundToInt((float)xpToNextLevel * levelXpModifier);
        
        _ui.UpdateLevelText();
        _ui.UpdateXpBar();
    }

    public void AddItemToInventory(string item)
    {
        inventory.Add(item);
        _ui.UpdateInventoryText();
    }
}
