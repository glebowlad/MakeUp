using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class HandController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Links")]
    [SerializeField] private RectTransform faceZoneUI; 
    [SerializeField] private GirlController girl;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private DraggableItem eyeshadowBrush;
    [SerializeField] private DraggableItem blushBrush;
    private Vector3 handDefaultPos;
    private DraggableItem currentItem;
    private Vector3 itemDefaultPos;
    private Transform itemDefaultParent;
    private bool canDrag = false;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        handDefaultPos = rectTransform.position;
    }

    
    public void OnItemClicked(DraggableItem colorItem)
    {
        DraggableItem tool = null;
        if (colorItem.type == ItemType.Eyeshadow) tool = eyeshadowBrush;
        else if (colorItem.type == ItemType.Blushes) tool = blushBrush;
        else tool = colorItem; 

        currentItem = tool;
        itemDefaultPos=currentItem.transform.position;
        itemDefaultParent=currentItem.transform.parent;
        canDrag = false;

        Sequence takeSequence = DOTween.Sequence();
        takeSequence.Append(rectTransform.DOMove(tool.transform.position, 0.5f));
        takeSequence.AppendCallback(() =>
        {
            tool.transform.SetParent(transform); 
            tool.transform.SetSiblingIndex(1);
            tool.transform.localPosition = Vector3.zero;
        });

        // --- РАЗДЕЛЕНИЕ ЛОГИКИ ---
        if (colorItem.type == ItemType.Eyeshadow || colorItem.type == ItemType.Blushes)
        {
            takeSequence.Append(rectTransform.DOMove(colorItem.transform.position, 0.4f));

        }


        Vector3 readyPos = (faceZoneUI.position + tool.transform.position) / 2;
        takeSequence.Append(rectTransform.DOMove(readyPos, 0.5f));
        takeSequence.OnComplete(() =>canDrag = true);
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        rectTransform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        if (RectTransformUtility.RectangleContainsScreenPoint(faceZoneUI, eventData.position))
        {
            StartInteraction();
        }
    }

    public void StartInteraction()
    {
        canDrag = false;

        float radius = 80f; // Радиус "втирания" крема (настройте под себя)
        Vector3 center = rectTransform.position;

        Sequence applySequence = DOTween.Sequence();
        applySequence.Append(rectTransform.DOMove(center + new Vector3(radius, radius, 0), 0.2f));
        applySequence.Append(rectTransform.DOMove(center + new Vector3(-radius, radius, 0), 0.2f));
        applySequence.Append(rectTransform.DOMove(center + new Vector3(-radius, -radius, 0), 0.2f));
        applySequence.Append(rectTransform.DOMove(center + new Vector3(radius, -radius, 0), 0.2f));
        applySequence.OnComplete(() => {
            girl.ApplyItem(currentItem.type, null);
            ReturnItem(currentItem);
        });
    }

    private void ReturnItem(DraggableItem item)
    {
        // 1. Двигаем РУКУ (вместе с предметом внутри) к стартовой точке предмета
        rectTransform.DOMove(itemDefaultPos, 0.5f).OnComplete(() =>
        {
            // 2. Отцепляем предмет от руки и возвращаем в старый список
            item.transform.SetParent(itemDefaultParent);

            // 3. Чтобы предмет не сместился, фиксируем его позицию
             item.transform.position = itemDefaultPos;

            // 4. Возвращаем пустую руку в дефолт
            ReturnHand();
        });
    }
    private void ReturnHand()
    {
        rectTransform.DOMove(handDefaultPos, 0.6f).SetEase(Ease.OutBack);
    }
}
