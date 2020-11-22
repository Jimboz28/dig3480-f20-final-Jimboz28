using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public Text countText; 
    public Text gameOverText;
    public Text cogText;
    public bool gameOver = false;
    // public static bool staticVar = false;
    // public Text staticText;
    public float speed = 3.0f;
    public static int level = 1;

    public int maxHealth = 5;
    public int score {get {return currentScore; }}
    public int currentScore;
    public int minScore = 0;
    public int maxScore = 4;
    public int cogs;
    // public int scoreAmount = 0;

    public GameObject projectilePrefab;
    public GameObject healthPrefab;
    public GameObject damagePrefab;
    
    public AudioClip throwSound;
    public AudioClip hitSound;
    //bkg music?
    public AudioClip musicClipOne;
    public AudioClip musicClipTwo;
    public AudioClip musicClipThree;

    public int health {get {return currentHealth; }}
    int currentHealth;

    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    AudioSource audioSource;
    public AudioSource musicSource;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // ScoreAmount = 0;
        // countText.text = count.ToString();
        currentHealth = maxHealth;
        currentScore = minScore;
        gameOver = false;
        gameOverText.text = "";
        cogs = 4;
        SetCountText();
        //SetHealthText();
        audioSource = GetComponent <AudioSource>();
        musicSource.clip = musicClipOne;
        musicSource.Play(); 
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x,0.0f) || !Mathf.Approximately(move.y, 0.0f))
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cogs > 0)
            {
            Launch();
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f,LayerMask.GetMask("NPC"));
            if (hit.collider !=null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character !=null)
                {
                    if (currentScore >= 4)
                    {
                        
                        character.DisplayDialogNextLevel();
                        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
                        level = 2;                        
                    }
                    else
                    {
                    character.DisplayDialog();
                    }
                }
            }
        }
        // win condition
        if (currentScore >= 4)
        {
            gameOver = true;
            // staticVar = true;
            if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

            }
        }
        }
        if (health <= 0)
        {
            PlaySound(musicClipThree);
            speed = 0.0f;
            gameOver = true;
            if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active     
            }
        }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            musicSource.clip = musicClipThree;
            musicSource.Play();
            gameOverText.text = "Alt Music 2 (Lose song)";
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            musicSource.clip = musicClipOne;
            musicSource.Play();
            gameOverText.text = "";
        }
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth (int amount)
    {
        if (amount < 0)
        {
            if (isInvincible)
            return;

            isInvincible = true;
            invincibleTimer = timeInvincible;
            // particle
            GameObject healthDecrease =Instantiate(damagePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
            animator.SetTrigger("Hit");
            PlaySound(hitSound);
        }
        if (amount > 0)
        {
            Instantiate (healthPrefab,rigidbody2d.position, Quaternion.identity);
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);

        UIHealthBar.instance.SetValue(currentHealth / (float) maxHealth);
    }
    public void ChangeScore (int scoreAmount)
    {
        currentScore = Mathf.Clamp(currentScore + scoreAmount, 0, maxScore);
    }
    void Launch()
    {
    GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

    Projectile projectile = projectileObject.GetComponent<Projectile>();
    projectile.Launch(lookDirection, 300);

    animator.SetTrigger("Launch");
    cogs -=1;
    cogText.text = cogs.ToString();
    SetCountText();

    PlaySound(throwSound);
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    public void OnCollisionEnter2D (Collision2D collision)
{
    if (collision.collider.tag == "Ammo")
        {
            cogs += 3;
            cogText.text = cogs.ToString();
            SetCountText();
            Destroy(collision.collider.gameObject);
        }
}
    
    public void SetCountText ()
    {
        countText.text = "Robots Fixed: " + currentScore.ToString();
        cogText.text = "Cogs: " + cogs.ToString();
        if (currentScore >= 4)
        {
                gameOverText.text = "Talk to Jambi to go to level 2. Press 'R' to restart!";
                //musicSource.clip = musicClipTwo;
                //musicSource.Play();
        }
        if (level == 2)
        {
            if (currentScore >= 4)
                {
            speed = 0;
            musicSource.clip=musicClipTwo;
            musicSource.Play();
            gameOverText.text = "You Win, Game Created by James Hilley. Press 'R' to restart!";
            }
        }
    

    }

}
