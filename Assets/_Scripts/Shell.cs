using System;
using System.Collections;
using System.Collections.Generic;
using OpenCover.Framework.Model;
using UnityEngine;


public enum ShellType
{
    WoodenArrow,
    Stone,
    FlameArrow,
    NoneType
}

public class Shell : MonoBehaviour
{
    public Vector3 direction = Vector3.forward;
    public float flyingSpeed = 0.01f;
    
    [SerializeField]
    private ShellType shellType = ShellType.WoodenArrow;


    public ShellType currentShellType => shellType;
    private ShellData currentShell
    {
        get
        {
            switch (shellType)
            {
                case ShellType.Stone:
                {
                    return (ShellData)Resources.Load("ShellDataFold/StoneData");
                }
                case ShellType.FlameArrow:
                {
                    return (ShellData)Resources.Load("ShellDataFold/FlameArrowData");
                }
                case ShellType.WoodenArrow:
                {
                    return (ShellData)Resources.Load("ShellDataFold/WoodenArrowData");
                }
                default:
                {
                    return null;
                }
            }
            
        }
    }


    public void FixedUpdate()
    {
        transform.Translate(direction * (flyingSpeed * Time.fixedDeltaTime));
        // GetComponent<Rigidbody>().velocity = direction * (flyingSpeed * Time.fixedTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer==LayerMask.NameToLayer("Enemy"))
        Destroy(this.gameObject);
    }

    public void ForPickUp()
    {
        
    }

    public void ForLoading()
    {
        
    }

    public void ChangeShellType(ShellType targetShellType)
    {
        shellType = targetShellType;
    }

}
