using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType { Cream, Lipstick, Eyeshadow, Blushes, Sponge }
public class DraggableItem : MonoBehaviour
{
    public ItemType type;
    public Sprite brushColorSprite; 
    public Vector3 startPosition;
    private void Awake()
    {
        startPosition = transform.position;
 
    }
}
