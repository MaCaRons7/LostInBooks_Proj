using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFireable
{
    public void SetFire();
}

public class BaseTurret : MonoBehaviour,IFireable
{


    public Transform firePos;
    public GameObject loadingShell;
    public float loadingTime;
    public ShellType shellType = ShellType.WoodenArrow;
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

            return null;
        }
    }
    
    private int HP;
    private int shellNum;

    [ContextMenu("Fire")]
    public void SetFire()
    {
        StartCoroutine(Fire());
    }
    
    IEnumerator Fire()
    {
        int nowTime = 0;
        while (nowTime<currentShell.perShellNum)
        {
            var shell = Instantiate(loadingShell, firePos.position, Quaternion.identity);
            shell.GetComponent<Shell>().ChangeShellType(shellType);
            nowTime++;
            yield return new WaitForSeconds(currentShell.interval);
        }

    }

    void ChangeToShell(ShellType targetShellType)
    {
        shellType = targetShellType;
    }
    
    
    

}
