using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameAddtion : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out Shell flyingShell) && flyingShell.currentShellType==ShellType.WoodenArrow)
        {
            Debug.Log("更改炮弹属性");
            flyingShell.ChangeShellType(ShellType.FlameArrow);
            flyingShell.GetComponent<Renderer>().material.color = Color.red;
        }
    }
}
