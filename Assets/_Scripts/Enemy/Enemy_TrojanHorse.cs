using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_TrojanHorse : BaseEnemy
{
    public override void UseSkill()
    {
        base.UseSkill();
        GameObject x;
        if (Random.Range(0, 10) == 9)
        {
            x = Instantiate(EnemySpawner.instance.enemys[(int)EnemySpawner.EnemyType.Enemy_Sheild]);
            x.transform.position = transform.position + moveSpeed.normalized;
        }
        else
        {
            x = Instantiate(EnemySpawner.instance.enemys[(int)EnemySpawner.EnemyType.Enemy_Normal]);
            x.transform.position = transform.position + moveSpeed.normalized;
        }
    }
}
