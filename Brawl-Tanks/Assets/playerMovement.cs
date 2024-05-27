using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playerMovement : MonoBehaviour
{
    public enum PlayerID { Player1, Player2 }
    public PlayerID playerID;

    private float speed = 5f;
    private float rotationSpeed = 1f;
    public Rigidbody2D rb;

    public Transform firePoint;

    public bool canShoot = true;
    public int bulletCount;

    public string currentAbility = "";

    public bool canMove = true;
    public Queue<string> abilities = new Queue<string>();
    public GameObject abilityUIPrefab;  // Prefab for the ability icon
    public Transform abilityUIParent; // Parent object for the ability icons
    private List<GameObject> abilityIcons = new List<GameObject>();  // List of ability icons

    private int maxAbilities = 5;
    private int maxBullets = 7;

    public bool shieldActive = false;

    Prefabs prefabs;
    RocketController rocketController;

    GameManager gameManager;

    private KeyCode forwardKey, backwardKey, leftKey, rightKey, shootKey;

    private Camera mainCamera;
    private Vector2 screenBounds;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        AssignControls();
        firePoint = transform.Find("Turret");
        bulletCount = 0;
        prefabs = GameObject.Find("GameManager").GetComponent<Prefabs>();
        mainCamera = Camera.main;

        if (gameManager.currentModifier == 1)
        {
            speed = speed * 1.6f;
            rotationSpeed = rotationSpeed * 1.2f;
        }

        // Calculate screen bounds
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
    }

    void AssignControls()
    {
        if (playerID == PlayerID.Player1)
        {
            forwardKey = KeyCode.W;
            backwardKey = KeyCode.S;
            leftKey = KeyCode.A;
            rightKey = KeyCode.D;
            shootKey = KeyCode.Q;
        }
        else if (playerID == PlayerID.Player2)
        {
            forwardKey = KeyCode.UpArrow;
            backwardKey = KeyCode.DownArrow;
            leftKey = KeyCode.LeftArrow;
            rightKey = KeyCode.RightArrow;
            shootKey = KeyCode.Space;
        }
        if (gameManager.currentModifier == 3)
        {
            InvertControls();
        }
    }

    void InvertControls()
    {
        KeyCode temp = forwardKey;
        forwardKey = backwardKey;
        backwardKey = temp;

        temp = leftKey;
        leftKey = rightKey;
        rightKey = temp;
    }

    void Update()
    {
        bool isMoving = false;
        if (canMove)
        {
            Vector2 newPosition = rb.position;

            if (Input.GetKey(forwardKey))
            {
                newPosition += (speed / 100 * (Vector2)transform.up);
                isMoving = true;
            }
            else if (Input.GetKey(backwardKey))
            {
                newPosition -= (speed / 100 * (Vector2)transform.up);
                isMoving = true;
            }

            if (Input.GetKey(leftKey))
            {
                rb.MoveRotation(rb.rotation + 5 * rotationSpeed);
                isMoving = false;
            }
            else if (Input.GetKey(rightKey))
            {
                rb.MoveRotation(rb.rotation - 5 * rotationSpeed);
                isMoving = false;
            }

            // Check if the new position is within screen bounds
            if (SceneManager.GetActiveScene().name == "Survival")
            {
                newPosition = KeepWithinScreenBounds(newPosition);
            }

            rb.MovePosition(newPosition);

            if (Input.GetKey(shootKey) && canShoot)
            {
                if (abilities.Count <= 0 || (SceneManager.GetActiveScene().name == "Survival"))
                {
                    if (bulletCount < maxBullets || (SceneManager.GetActiveScene().name == "Survival"))
                    {
                        var bullet = Instantiate(prefabs.bulletPrefab, firePoint.position, firePoint.rotation);
                        bullet.name = name + "_bullet";
                        canShoot = false;
                        bulletCount++;
                        Debug.Log(bulletCount);
                        StartCoroutine(ShootCooldown());
                    }
                }
                else
                {
                    string nextAbility = abilities.Dequeue();  // Get the next ability
                    if (shieldActive && nextAbility == "shield")
                    {
                        abilities.Enqueue(nextAbility);
                        Debug.Log("Shield already active");
                    }
                    else
                    {
                        AbilityScript abilityScript = GameManager.instance.GetComponent<AbilityScript>();
                        abilityScript.selectAbility(gameObject, nextAbility);
                        UpdateAbilitiesUI();
                        if (nextAbility == "gatling")
                        {
                            StartCoroutine(GattlingCooldown());
                        }
                        else
                        {
                            StartCoroutine(ShootCooldown());
                        }
                    }
                }
            }
        }

        if (!isMoving)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    Vector2 KeepWithinScreenBounds(Vector2 position)
    {
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(position);

        viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.02f, 0.98f);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.03f, 0.97f);

        return mainCamera.ViewportToWorldPoint(viewportPosition);
    }

    void UpdateAbilitiesUI()
    {
        // Clear existing icons
        foreach (GameObject icon in abilityIcons)
        {
            Destroy(icon);
        }
        abilityIcons.Clear();

        // Set an initial position for the first icon
        Vector3 iconPositionOffset = new Vector3((-abilities.Count + 1) * 0.125f, 0f, 0);

        // Create new icons for each ability in the queue
        foreach (string ability in abilities)
        {
            GameObject icon = Instantiate(abilityUIPrefab, abilityUIParent);
            icon.GetComponent<Image>().sprite = getIcon(ability);

            // Set local position relative to the ability UI parent, adjusting by icon width and a fixed padding
            icon.transform.localPosition = iconPositionOffset;
            icon.transform.localScale = Vector3.one; // Ensure the icon's scale is reset to 1

            // Increment the position offset for the next icon
            iconPositionOffset.x += 0.25f; // Adjust spacing between icons (assuming each icon is about 0.5 units wide)

            abilityIcons.Add(icon);
        }
    }

    Sprite getIcon(string ability)
    {
        switch (ability)
        {
            case "shield":
                return prefabs.ShieldSprite;
            case "ray":
                return prefabs.RaySprite;
            case "frag":
                return prefabs.FragSprite;
            case "gatling":
                return prefabs.GatlingSprite;
            case "laser":
                return prefabs.LaserSprite;
            case "rc":
                return prefabs.RCSprite;
            default:
                return null;
        }
    }

    IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        canShoot = true;
    }

    IEnumerator GattlingCooldown()
    {
        yield return new WaitForSeconds(1.2f);
        canShoot = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            BulletScript bullet = other.gameObject.GetComponent<BulletScript>();
            Debug.Log(bullet.creationTime);
            Debug.Log(Time.time);
            Debug.Log(Time.time - bullet.creationTime);
            if (Time.time - bullet.creationTime > 0.02f)
            {
                Destroy(gameObject);
                GameManager.instance.PlayerDestroyed((GameManager.PlayerID)playerID);
                Destroy(other.gameObject);
                bulletCount--;
            }
        }
        if (other.gameObject.tag == "DeathRay")
        {
            Destroy(gameObject);
            GameManager.instance.PlayerDestroyed((GameManager.PlayerID)playerID);
        }
        if (other.gameObject.tag == "rcRocket")
        {
            rocketController = other.gameObject.GetComponent<RocketController>();
            if (Time.time - rocketController.creationTime > 0.02f)
            {
                Destroy(gameObject);
                GameManager.instance.PlayerDestroyed((GameManager.PlayerID)playerID);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Ability" && abilities.Count < maxAbilities)
        {
            AbilityScript abilityScript = GameManager.instance.GetComponent<AbilityScript>();
            abilityScript.doSomething(gameObject, other.gameObject);
            UpdateAbilitiesUI();
        }
    }
}
