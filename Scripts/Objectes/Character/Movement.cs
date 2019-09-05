using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    protected Animator animator;
    protected Character character;
    protected Health myHealth;
    protected Attack myAttack;
    public Vector2 lastDirection;

    int wallLayerNum = 0;

    public float movingSpeed; // 이동속도 설정

    bool isIninity = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        character = GetComponent<Character>();
        myHealth = GetComponent<Health>();
        myAttack = GetComponent<Attack>();
    }

    private void Start()
    {
        wallLayerNum += 1 << LayerMask.NameToLayer("wall");

        if (SaveSystemInfinity.Instance != null)
            isIninity = true;
    }

    protected void move()
    {
        Vector2 movingPosition = myAttack.targetObject.transform.position;
        Vector2 myPosition = this.transform.position;
        Vector2 init = new Vector3(0, 0); // 빽업시 지우면됨

        // 죽었으면 움직이지않는다.
        if (!myHealth.IsAlive)
            return;

        // 공격중이면 움직이지 않는다.
        if (myAttack.isAttacking)
            return;

        // 타겟이 없거나 비활성화 상태거나 거리가 멀면 직진한다.
        if (myAttack.targetObject == null || !myAttack.targetObject.activeInHierarchy ||
            Vector3.Distance(gameObject.transform.position, myAttack.targetObject.transform.position) >= 150)
        {
            character.CorrectSpriteDirection(GameManager.Instance.gameObject);
            animator.SetInteger("AnimState", 1);
            // 빽업시 ((init - myPosition).normalized) 삭제 후, left로 수정하면됨

            if(isIninity)
            {
                transform.Translate(((init - myPosition).normalized) *
                                (Time.deltaTime * GameManager.Instance.GameSpeed) * (movingSpeed * character.characterInfo.multipleMovingSpeed));
                lastDirection = (init - myPosition).normalized;
            }
            else
            {
                transform.Translate(Vector2.left *
                                (Time.deltaTime * GameManager.Instance.GameSpeed) * (movingSpeed * character.characterInfo.multipleMovingSpeed));
                lastDirection = Vector2.left;
            }

            if (lastDirection.x < 0)
                character.spriteRenderer.flipX = true;
            else if (lastDirection.x > 0)
                character.spriteRenderer.flipX = false;

            return;
        }

        // 공격범위 안에 들어오지 않았을때 이동한다.
        if (!myAttack.attackEllipse.InEllipse(myAttack.targetObject.transform))
        {
            Vector2 normMovingPosition = (movingPosition - myPosition).normalized;

            this.transform.Translate(normMovingPosition *
                                (Time.deltaTime * GameManager.Instance.GameSpeed) * (movingSpeed * character.characterInfo.multipleMovingSpeed));

            lastDirection = normMovingPosition;

            animator.SetInteger("AnimState", 1);
        }
        else
        {
            animator.SetInteger("AnimState", 0);
        }

        character.CorrectSpriteDirection(myAttack.targetObject);

        if (lastDirection.x < 0)
            character.spriteRenderer.flipX = true;
        else if(lastDirection.x > 0)
            character.spriteRenderer.flipX = false;

    } // 이동함수

    void Update()
    {
        // 일시정지 상태일때 동작시키지 않음
        if (GameManager.Instance.GameSpeed == 0)
            return;

        move(); // 적이 내 공격 사거리 안에 들어올 때 까지 이동
    }
}
