using UnityEngine;

public class GirlController : MonoBehaviour
{
     private Transform _acne;
     private SpriteRenderer _eyeshadow;
     private SpriteRenderer _lipstick;
     private SpriteRenderer _blush;

    private void Awake()
    {
        _acne = transform.GetChild(0);
       _eyeshadow= transform.GetChild(1).GetComponent<SpriteRenderer>();
       _lipstick= transform.GetChild(2).GetComponent<SpriteRenderer>();
       _blush = transform.GetChild(3).GetComponent<SpriteRenderer>();
    }

    void RemoveAcne()
    {
        _acne.gameObject.SetActive(false);
    }
    public void ApplyItem(ItemType type,Sprite value)
    {
        switch (type)
        {
            case ItemType.Cream: RemoveAcne(); break;
            case ItemType.Eyeshadow: _eyeshadow.sprite=value; break;
            case ItemType.Lipstick: _lipstick.sprite = value; ; break;
            case ItemType.Blushes: _blush.sprite = value; break;
        }
    }

    public void ResetMakeup()
    {
        
        _eyeshadow.sprite=null;
        _lipstick.sprite=null;
        _blush.sprite = null;
    }
}
