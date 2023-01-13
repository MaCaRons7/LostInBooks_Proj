using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{

    [Header("检测可移动物体的层级")]
    public LayerMask movableLayer;

    [Header("交互的检测距离")]
    public float checkDistance = 1f;

    [Header("交互的角度限制，玩家前方与障碍物法线之间的夹角大于此值时不可交互")]
    public float interactAngle = 45f;
    
    [Header("玩家可以推动的物体的最低高度")]
    public float movableObjectHeight = 0.8f;
    
    float climbDistance;  //攀爬的实际距离，距离墙壁小于这个距离才会攀爬
    Vector3 climbHitNormal;   //墙壁法线
    public Vector3 ClimbHitNormal { get => climbHitNormal; }


    private void Start()
    {

    }

    public T InteractObjectCheck<T>(Vector3 originPos,Transform playerTransform, Vector3 inputDirection)
    {
        Debug.DrawRay(originPos, playerTransform.forward,Color.green,2f);
        if(Physics.Raycast(originPos, playerTransform.forward, out RaycastHit hit, checkDistance, movableLayer))
        {
            if(Vector3.Angle(-hit.normal, playerTransform.forward) > interactAngle || Vector3.Angle(-hit.normal, inputDirection) > interactAngle)
            {
                return default(T);
            }

            T res;
            if(hit.collider.TryGetComponent<T>(out res))
            {
                return res;
            }
        }
        return default(T);
    }

    public MovableObject MovableObjectCheck(Transform playerTransform, Vector3 inputDirection)
    {
        if(Physics.Raycast(playerTransform.position + Vector3.up * movableObjectHeight, playerTransform.forward, out RaycastHit hit, checkDistance, movableLayer))
        {
            climbHitNormal = hit.normal;
            if(Vector3.Angle(-climbHitNormal, playerTransform.forward) > interactAngle || Vector3.Angle(-climbHitNormal, inputDirection) > interactAngle)
            {
                return null;
            }
            MovableObject movableObject;
            if(hit.collider.TryGetComponent<MovableObject>(out movableObject))
            {
                return movableObject;
            }
        }
        return null;
    }
} 