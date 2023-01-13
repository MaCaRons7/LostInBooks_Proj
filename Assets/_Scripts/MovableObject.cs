using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct InteractPoint
{
    public Transform interactPoint;
    public Transform rightHandTarget;
    public Transform leftHandTarget;
}

public class MovableObject : MonoBehaviour
{
    public InteractPoint[] interactPoints;

    public InteractPoint GetInteractPoint(Transform playerTransform)
    {
        InteractPoint interactPoint = new InteractPoint();

        float shortestDistance = float.PositiveInfinity;

        foreach(var point in interactPoints)
        {
            float distance = Vector3.Distance(point.interactPoint.position, playerTransform.position);
            if(distance < shortestDistance)
            {
                shortestDistance = distance;
                interactPoint = point;
            }
        }
        return interactPoint;
    }

} 