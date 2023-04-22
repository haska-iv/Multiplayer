using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour //for collision testing only
{
    public Animator animator;
    public Rigidbody body;
    public SkinnedMeshRenderer mat;
    public float colorChangeDuration = 3f;

    public bool isDashing;
    private bool isImmortal;
    private Color baseColor;
    private Color changeColor = Color.green;


    private void Awake()
    {
        baseColor = mat.material.color;
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
                isImmortal = true;
                mat.material.color = changeColor;
                StartCoroutine(ChangeColor());
            }
        }
    }

    IEnumerator ChangeColor()
    {
        yield return new WaitForSeconds(colorChangeDuration);
        isImmortal = false;
        mat.material.color = baseColor;
    }
}
