using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Cream, Lipstick, Eyeshadow, Blushes }
public class DraggableItem : MonoBehaviour
{
    public ItemType type;
    public MakeUpData makeUpData;
    public int colorIndex;
    private Vector3 _startPosition;
    private void Awake()
    {
        _startPosition = transform.position;
 
    }
}
