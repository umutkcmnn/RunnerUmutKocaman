using UnityEngine;
using DG.Tweening;
using System;

public enum Positions
{
    onLeft,
    onMiddle,
    onRight
}

public class PlayerController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] Rigidbody rb;
    [SerializeField] public Animator myAnim;
    [SerializeField] AudioClip bonusSound, CoinSound, DeathSound, MagnetCoinSound, ShieldSound, WinSound;
    [SerializeField] AudioSource PlayerSound;
    [SerializeField] GameObject coinCollectedVFX, deathVFX, healthDeclineVFX, magnetVFX, wallBreakVFX, ShieldVFX;

    [Header("Settings")]
    [SerializeField] float speed;
    [SerializeField] float shift = 2;
    [SerializeField] bool isLeft, isMiddle, isRight;

    public bool isDead;
    public bool isStart;

    [SerializeField] public int score;
    [SerializeField] public float floatScore;
    [SerializeField] public float passedTime;

    [System.NonSerialized] public Positions positions = Positions.onMiddle;

    public bool is2XActive, isShieldActive, isMagnetActive;
    [SerializeField] public int Health;

    float beforeSpeed;
    bool isMove;

    // Zýplama
    bool isJumping = false;
    float jumpTimer = 0f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float jumpDuration = 0.5f;
    float initialY;

    // Dokunma kontrolü için
    Vector2 touchStartPos;
    bool swipeDetected = false;

    void Start()
    {
        isMiddle = true;
        initialY = transform.position.y;
    }

    void Update()
    {
        passedTime += Time.deltaTime;
        moveCharacter();
    }

    void moveCharacter()
    {
        if (!isStart || isDead) return;

        if (is2XActive)
            floatScore += Time.deltaTime;

        floatScore += Time.deltaTime;
        if (floatScore > 1)
        {
            score += 1;
            floatScore = 0;
        }

        if (passedTime > 10)
        {
            speed += 0.3f;
            passedTime = 0;
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Giriþ kontrolü: PC ve Mobil
        if ((Input.GetKeyDown(KeyCode.A) || IsLeftSideTouched()) && transform.position.x > -0.5f && !isMove)
        {
            transform.DOMoveX(transform.position.x - shift, 0.5f).SetEase(Ease.Linear).OnComplete(isMoveToFalse);
            isMove = true;
        }
        else if ((Input.GetKeyDown(KeyCode.D) || IsRightSideTouched()) && transform.position.x < 0.5f && !isMove)
        {
            transform.DOMoveX(transform.position.x + shift, 0.5f).SetEase(Ease.Linear).OnComplete(isMoveToFalse);
            isMove = true;
        }

        // Zýplama tetikleme
        if ((Input.GetKeyDown(KeyCode.Space) || IsSwipeUp()) && !isJumping)
        {
            isJumping = true;
            jumpTimer = 0f;
            myAnim.SetTrigger("JumpTrigger");
        }

        if (isJumping)
        {
            jumpTimer += Time.deltaTime;
            float percent = jumpTimer / jumpDuration;
            float yOffset = Mathf.Sin(percent * Mathf.PI) * jumpHeight;

            Vector3 pos = transform.position;
            pos.y = initialY + yOffset;
            transform.position = pos;

            if (jumpTimer >= jumpDuration)
            {
                isJumping = false;
                transform.position = new Vector3(transform.position.x, initialY, transform.position.z);
                myAnim.SetBool("Jump", false);
            }
        }
    }

    void isMoveToFalse()
    {
        isMove = false;
    }

    #region Dokunma Giriþleri
    bool IsLeftSideTouched()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && touch.position.x < Screen.width / 2)
                return true;
        }
        return false;
    }

    bool IsRightSideTouched()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && touch.position.x >= Screen.width / 2)
                return true;
        }
        return false;
    }

    bool IsSwipeUp()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
                swipeDetected = true;
            }
            else if (touch.phase == TouchPhase.Ended && swipeDetected)
            {
                Vector2 swipeDelta = touch.position - touchStartPos;
                swipeDetected = false;

                if (swipeDelta.y > 100 && Mathf.Abs(swipeDelta.x) < 100) // Yukarý kaydýrma
                {
                    return true;
                }
            }
        }
        return false;
    }
    #endregion

    #region Çarpýþma ve Toplama Kodlarý (Deðiþmedi)

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("obstacle"))
        {
            int damage = other.gameObject.GetComponent<Obstacle>().damage;

            if (isShieldActive)
            {
                Destroy(other.gameObject);
                isShieldActive = false;
                GameObject vfx = Instantiate(wallBreakVFX, other.transform.position, Quaternion.identity);
                Destroy(vfx, 1f);
            }
            else
            {
                CheckHealth(damage, other.gameObject);
            }
        }
    }

    private void CheckHealth(int damage, GameObject other)
    {
        Health -= damage;
        if (Health <= 0)
        {
            myAnim.SetBool("Death", true);
            PlayerSound.PlayOneShot(DeathSound);
            GameObject vfx = Instantiate(deathVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 1f);
            isDead = true;
        }
        else
        {
            Destroy(other);
            Instantiate(wallBreakVFX, other.transform.position, Quaternion.identity);
            GameObject healthvfx = Instantiate(healthDeclineVFX, transform.position, Quaternion.identity, this.transform);
            Destroy(healthvfx, 1f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("collectable"))
        {
            Collectables collectables = other.GetComponent<Collectables>();
            switch (collectables.collectablesEnum)
            {
                case CollectablesEnum.Coin:
                    AddScrore(collectables.toBeAddedScore);
                    break;
                case CollectablesEnum.Shield:
                    ActivateShield();
                    break;
                case CollectablesEnum.Health:
                    AddHealth(collectables.TobeAddedHealth);
                    break;
                case CollectablesEnum.Score2X:
                    ActivateBonus();
                    break;
                case CollectablesEnum.SpeedUp:
                    AddSpeed(collectables.toBeAddedSpeed);
                    break;
                case CollectablesEnum.Magnet:
                    ActivateMagnet();
                    break;
            }

            Destroy(other.gameObject);
        }
    }

    private void AddSpeed(int toBeAddedSpeed)
    {
        beforeSpeed = speed;
        speed += toBeAddedSpeed;
        Invoke("BackToOrijinalSpeed", 5f);
    }

    void BackToOrijinalSpeed()
    {
        speed = beforeSpeed;
    }

    void AddScrore(int TobeAddedScore)
    {
        if (isMagnetActive)
        {
            PlayerSound.clip = MagnetCoinSound;
            PlayerSound.Play();
        }
        else
        {
            PlayerSound.clip = CoinSound;
            PlayerSound.Play();
        }

        GameObject vfx = Instantiate(coinCollectedVFX, transform.position + new Vector3(0, 1, 0), Quaternion.identity, this.transform);
        Destroy(vfx, 1f);

        if (is2XActive)
        {
            TobeAddedScore *= 2;
        }

        score += TobeAddedScore;
    }

    void ActivateShield()
    {
        isShieldActive = true;
        PlayerSound.PlayOneShot(ShieldSound);
        GameObject vfx = Instantiate(ShieldVFX, transform.position, Quaternion.identity, this.transform);
        Destroy(vfx, 5f);
        Invoke("DeactivateShield", 5f);
    }

    void DeactivateShield()
    {
        isShieldActive = false;
    }

    void AddHealth(int ToBeAddedHealth)
    {
        Health += ToBeAddedHealth;
        if (Health <= 0)
        {
            myAnim.SetBool("Death", true);
            isDead = true;
        }
    }

    void ActivateBonus()
    {
        is2XActive = true;
        AudioSource.PlayClipAtPoint(bonusSound, transform.position);
        Invoke("DeActivateBonus", 5f);
    }

    void DeActivateBonus()
    {
        is2XActive = false;
    }

    void ActivateMagnet()
    {
        isMagnetActive = true;
        GameObject vfx = Instantiate(magnetVFX, transform.position + new Vector3(0, 1, 0), Quaternion.identity, this.transform);
        Destroy(vfx, 5f);
        Invoke("DeActivateMagnet", 5f);
    }

    void DeActivateMagnet()
    {
        isMagnetActive = false;
    }

    #endregion
}
