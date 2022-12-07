using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 5;
    public TextMeshProUGUI fixedText;
    public TextMeshProUGUI AmmoText;
    private int ScoreNumber = 0; //The amount of robots Ruby has fixed

    //Speed Boost
    public float timeBoosting = 4.0f;
    float speedBoostTimer;
    bool isBoosting;
   
private RubyController rubyController;

    public int ammo { get { return currentAmmo; }}
    public int maxAmmo = 4; //Max Ammo Ruby can have
    public int currentAmmo;

    public int currentChicken;

    public GameObject projectilePrefab;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public GameObject winText;
    public GameObject loseText;
    bool GameOver;
    bool WinGame;
    
    public int health { get { return currentHealth; }}
    int currentHealth;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    public static int Level = 1;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);

    public ParticleSystem damageEffect;
    
    AudioSource audioSource;

    public AudioSource BackgroundManager;
    
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

         GameObject rubyControllerObject = GameObject.FindWithTag("RubyController"); //this line of code finds the RubyController script by looking for a "RubyController" tag on Ruby

        
        currentHealth = maxHealth;

        currentAmmo = maxAmmo;

        currentChicken = 0;

      
        AmmoText.text = "Current Cogs:" + currentAmmo.ToString() + "/4";

         fixedText.text = "Fixed Robots: " + ScoreNumber.ToString() + "/4";

            winText.SetActive(false);
        loseText.SetActive(false);
        bool WinGame = false;
        bool GameOver = false;

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();
            if (ammo > 0)
            {
                ChangeAmmo(-1);
                ammoText();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            if (GameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    if (ScoreNumber >= 4)
                    {
                        SceneManager.LoadScene("Level 2");
                        Level = 2;
                    }
                    else
                    {
                        character.DisplayDialog();
                    }
                    
                }
            }
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            
            PlaySound(hitSound);
        }

        if (currentHealth <= 1)
        {
            loseText.SetActive(true);
            bool GameOver = true;
            SoundManagerScript.PlaySound("FailSound");
            Destroy(gameObject);
            BackgroundManager.Stop();
        }
        
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        
        Instantiate(damageEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
    }
    
    void Launch()
    { if (currentAmmo > 0)
        {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        
        PlaySound(throwSound);
        }
    }

    public void ChangeAmmo(int amount)
    {
        currentAmmo = Mathf.Abs(currentAmmo + amount);
    Debug.Log("Ammo: " + currentAmmo);
    }

    public void ammoText()
    {
        AmmoText.text = "Ammo: " + currentAmmo.ToString();
    }

    public void CollectedChicken(int amount)
    {
        currentChicken = Mathf.Abs(currentChicken + amount);
        Debug.Log("Chickens Collected" + currentChicken);
    }

    public void FixedRobots(int amount)
    {
        ScoreNumber += amount;
        fixedText.text = "Fixed Robots: " + ScoreNumber.ToString() + "/4";

        Debug.Log("Fixed Robots: " + ScoreNumber);

        if (ScoreNumber == 2 && Level == 2 && currentChicken == 1)
        {
            WinGame = true;
            winText.SetActive(true);

            transform.position = new Vector3(-5f, 0f, -100f);
            speed = 0;

            Destroy(gameObject.GetComponent<SpriteRenderer>());

            SoundManagerScript.PlaySound("WinSound");

            BackgroundManager.Stop();

        }
    }
    
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    public void SpeedBoost(int amount)
    { if(amount >0)
        {
             speedBoostTimer = timeBoosting;
            isBoosting = true;
        }

    }
}