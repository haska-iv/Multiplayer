using UnityEngine;
using Mirror;
using System.Collections;

public class Player : NetworkBehaviour
{
    [Header("Links")]
    public Animator animator;
    public Rigidbody body;
    public SkinnedMeshRenderer mat;
    public TMPro.TextMeshProUGUI scoreText;

    private Transform view => View.active.transform;

    [Header("Parameters")]
    public float speed = 10f;
    public float impulseDistance = 5f;
    public float colorChangeDuration = 3f;

    [Header("Condition")]
    public bool isDashing;

    private bool setDirection;
    private bool isImmortal;
    private Vector3 direction;
    private Color baseColor;
    private Color changeColor = Color.green;
    private int currentScore;


    private void Awake()
    {
        direction = Vector3.zero;
        baseColor = mat.material.color;
    }

    private void Start()
    {
        //if (!isLocalPlayer)
        //    View.active.gameObject.SetActive(false);
        //else
        if (isLocalPlayer)
            View.active.SetTarget(transform);

        currentScore = 0;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        setDirection = !Input.anyKeyDown;
        direction = view.forward * Input.GetAxisRaw("Vertical");
        direction += view.right * Input.GetAxisRaw("Horizontal");
        direction.y = 0f;
        direction.Normalize();

        if (setDirection)
        {
            if (direction.sqrMagnitude > 0.01f)
                transform.forward = direction;
            body.drag = 0f;
        }
        else
            body.drag = 10f;

        SetRun(body.velocity.magnitude);

        if (Input.GetMouseButtonDown(0))
            isDashing = true;
    }

    private void FixedUpdate()
    {
        if (setDirection)
            body.velocity = speed * direction;

        if (isDashing)
        {
            Vector3 dashDirection = transform.forward;
            float dashDistance = 5f;
            float dashSpeed = dashDistance / 0.5f;
            body.AddForce(dashDirection * dashSpeed, ForceMode.VelocityChange);
            StartCoroutine(EndDash());
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))//tag "Player" has been set for Character in editor
        {
            var enemy = other.gameObject.GetComponent<Player>();

            if (isImmortal || isDashing)
                return;
            else if (enemy.isDashing)
            {
                
                if (isServer)
                {
                    Debug.Log("server " + enemy.name);
                    Debug.Log(this.name);
                    ChangeColorClient(changeColor, enemy);
                }
                else
                {
                    Debug.Log("not server " + enemy.name);
                    mat.material.color = changeColor;
                    isImmortal = true;
                    StartCoroutine(ChangeColor());
                }
            }
        }
    }

    [ClientRpc]
    public void ChangeColorClient(Color color, Player enemy)
    {
        enemy.currentScore++;
        enemy.scoreText.text = $"{currentScore}";
        mat.material.color = changeColor;
        isImmortal = true;
        StartCoroutine(ChangeColor());
    }

    [Command]
    public void SetRun(float run)
    {
        animator.SetFloat("Run", run);
    }

    IEnumerator ChangeColor()
    {
        yield return new WaitForSeconds(colorChangeDuration);
        isImmortal = false;
        mat.material.color = baseColor;
    }

    IEnumerator EndDash()
    {
        yield return new WaitForSeconds(0.5f); // end the dash after 0.5 seconds
        isDashing = false;
    }
}
