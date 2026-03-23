using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="MakeUpData", menuName ="MakeUp/MakeUpData")]
public class MakeUpData : ScriptableObject
{
    [Header("EyeShadows")]
    [SerializeField] private Sprite[] eyeShadows;
    [SerializeField] private Color[] eyeShadowColors;
    [Header("Lipstick")]
    [SerializeField] private Sprite[] lipsticks;
    [Header("Blushes")]
    [SerializeField] private Sprite[] blushes;
    [SerializeField] private Color[] blushColors;


    public Sprite GetSprite(ItemType type, int index)
    {
        switch (type)
        {
            case ItemType.Eyeshadow: return eyeShadows[index];
            case ItemType.Lipstick: return lipsticks[index];
            case ItemType.Blushes: return blushes[index];
            default: return null;
        }
    }
    public Color GetColor(ItemType type, int index)
    {
        if (type == ItemType.Eyeshadow) return eyeShadowColors[index];
        if (type == ItemType.Blushes) return blushColors[index];
        return Color.white;
    }
}
