using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public int hP;
    public int coin;
    public float moveSpeed;
    public float attackDamage;
    public float attackRange;
    public float attackCD;
    public float skillCD;
    private float nowSkillCD;
    private enum DestroyType { }

    public virtual void UseSkill() { }

    private void Update()
    {
        Move();
        nowSkillCD += Time.deltaTime;
        if (nowSkillCD >= skillCD)
        {
            UseSkill();
            nowSkillCD = 0;
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
        transform.position = new Vector3(transform.position.x - moveSpeed * Time.deltaTime, 
                                               transform.position.y, transform.position.z);
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
