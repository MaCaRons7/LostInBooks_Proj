using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public int hP;
    public int coin;
    public Vector3 moveSpeedTime;
    public float attackDamage;
    public float attackRange;
    public float attackCD;
    public float skillCD;
    protected Vector3 moveSpeed;
    private float nowSkillCD;
    private enum DestroyType { }

    public virtual void UseSkill() { }

    private void Start()
    {
        float x = 0, y = 0, z = 0;
        if (moveSpeedTime.x != 0) x = 1.0f / moveSpeedTime.x;
        if (moveSpeedTime.y != 0) y = 1.0f / moveSpeedTime.y;
        if (moveSpeedTime.z != 0) z = 1.0f / moveSpeedTime.z;
        moveSpeed = new Vector3(x, y, z);
    }

    private void Update()
    {
        Move();
        nowSkillCD += Time.deltaTime;
        if (nowSkillCD >= skillCD)
        {
            UseSkill();
            nowSkillCD = 0;
        }
    }

    public void GetHurt(int damage)
    {
        hP -= damage;
        if (hP <= 0) Dead();
    }

    private void Move()
    {
        transform.position = transform.position + moveSpeed * Time.deltaTime;
    }

    private void AttackWall()
    {

    }

    private void AttackTurret()
    {

    }

    private void EnterCastle()
    {

    }

    private void Dead()
    {
        Destroy(gameObject);
    }
}
