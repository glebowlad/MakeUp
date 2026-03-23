using UnityEngine;

public class GirlController : MonoBehaviour
{
     private Transform acne;
     private SpriteRenderer eyeshadow;
     private SpriteRenderer lipstick;
     private SpriteRenderer blush;

    private void Awake()
    {
        acne = transform.GetChild(0);
       eyeshadow= transform.GetChild(1).GetComponent<SpriteRenderer>();
       lipstick= transform.GetChild(2).GetComponent<SpriteRenderer>();
       blush = transform.GetChild(3).GetComponent<SpriteRenderer>();
    }

    void RemoveAcne()
    {
        acne.gameObject.SetActive(false);
    }
    public void ApplyItem(ItemType type,Sprite value)
    {
        switch (type)
        {
            case ItemType.Cream: RemoveAcne(); break;
            case ItemType.Eyeshadow: eyeshadow.sprite=value; break;
            case ItemType.Lipstick: lipstick.sprite = value; ; break;
            case ItemType.Blushes: blush.sprite = value; break;
        }
    }

    public void ResetMakeup()
    {
        
        eyeshadow.sprite=null;
        lipstick.sprite=null;
        blush.sprite = null;
    }
}
