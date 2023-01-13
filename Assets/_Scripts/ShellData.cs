using UnityEngine;

[CreateAssetMenu(fileName = "NewShellData",menuName = "MyData/ShellData")]
public class ShellData:ScriptableObject
{
    public float interval;
    public float liveRange;
    public float damage;
    public int perShellNum;
    public int price;
}
