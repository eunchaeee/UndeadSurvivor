using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float curHealth;
    public float maxHealth;
    public RuntimeAnimatorController[] animatorControllers;
    public Rigidbody2D target;

    bool isLive;

    Rigidbody2D rigid;
    Collider2D coll;
    Animator animator;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;

    private Vector2 dir;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        wait = new WaitForFixedUpdate();
    }

    private void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        animator.SetBool("Dead", false);
        curHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        if (!isLive || animator.GetCurrentAnimatorStateInfo(0).IsName("Hit")) return;

        dir = target.position - rigid.position;
        Vector2 nextVec = dir.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (!isLive) return;
        spriter.flipX = dir.x < 0;
    }

    public void Init(SpawnData data)
    {
        animator.runtimeAnimatorController = animatorControllers[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        curHealth = data.health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet") || !isLive)
        {
            curHealth -= collision.GetComponent<Bullet>().damage;
            StartCoroutine(KnockBack());
        }

        if (curHealth < 0)
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            animator.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp();
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    IEnumerator KnockBack()
    {
        yield return wait;  // 하나의 물리 프레임을 딜레이
        Vector3 playerpos = GameManager.instance.player.transform.position;
        Vector3 dir = transform.position - playerpos;
        rigid.AddForce(dir.normalized * 3, ForceMode2D.Impulse);
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }
}
