using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;

    [SerializeField] private float speed = 3;
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private SpriteRenderer spriter;
    [SerializeField] private Animator anim;

    private void FixedUpdate()
    {
        // 1. 힘을 준다.
        //rigid.AddForce(inputVec);

        // 2. 속도 제어
        //rigid.velocity = inputVec;

        // 3. 위치 이동
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    private void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    private void LateUpdate()
    {
        anim.SetFloat("Speed", inputVec.magnitude);


        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0; 
        }
    }
}
