using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Cream, Lipstick, Eyeshadow, Blushes }
public class DraggableItem : MonoBehaviour
{
    public ItemType type;
    public MakeUpData makeUpData;
    public int colorIndex;
    private Vector3 startPosition;
    private void Awake()
    {
        startPosition = transform.position;
 
    }
}
