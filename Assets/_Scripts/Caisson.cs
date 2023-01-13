using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caisson : MonoBehaviour
{
    public ShellType caissonShellType;
    public int savingNum = 10 ;
    private ShellData currentShell
    {
        get
        {
            switch (caissonShellType)
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

    public bool TakeOut()
    {
        if (savingNum > 0)
        {
            savingNum--;
            return true;
        }
        else
        {
            Destroy(this.gameObject);
            return false;
        }
    }

    public void PutIn()
    {
        savingNum++;
    }
}
