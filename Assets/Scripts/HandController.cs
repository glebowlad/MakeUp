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
    private DraggableItem selectedColor;
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
        if (currentItem != null)
        {
            
            ReturnItem(() => OnItemClicked(colorItem));
            return;
        }
        selectedColor = colorItem;
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
            if (tool.TryGetComponent<Image>(out var img)) img.raycastTarget = false;

        });

        if (colorItem.type == ItemType.Eyeshadow || colorItem.type == ItemType.Blushes)
        {
            takeSequence.Append(rectTransform.DOMove(colorItem.transform.position, 0.4f));
            takeSequence.Append(rectTransform.DOShakePosition(0.4f, 15f, 10));
            takeSequence.AppendCallback(() => {
                
                Color selectedColorVal = colorItem.makeUpData.GetColor(colorItem.type, colorItem.colorIndex);
                var brushTip = currentItem.transform.GetChild(0).GetComponent<Image>();

                if (brushTip != null)
                {
                    brushTip.color = selectedColorVal;
                    brushTip.DOFade(1f, 0.1f);
                }
            });

        }

        Vector3 readyPos = (faceZoneUI.position + tool.transform.position) / 2;
        takeSequence.Append(rectTransform.DOMove(readyPos, 0.5f));
        takeSequence.OnComplete(() =>canDrag = true);
    }

    private void ReturnItem(Action onDone = null)
    {
        canDrag = false;
        DraggableItem itemToReturn = currentItem; 

        rectTransform.DOMove(itemDefaultPos, 0.5f).OnComplete(() => {
            
            itemToReturn.transform.SetParent(itemDefaultParent);
            itemToReturn.transform.position = itemDefaultPos;
            if (itemToReturn.TryGetComponent<Image>(out var img)) img.raycastTarget = true;

            currentItem = null;
            if (onDone != null) onDone.Invoke();
            else ReturnHand();
        });
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

        float radius = 80f;
        Vector3 center = rectTransform.position;

        Sequence applySequence = DOTween.Sequence();
        applySequence.Append(rectTransform.DOMove(center + new Vector3(radius, radius, 0), 0.2f));
        applySequence.Append(rectTransform.DOMove(center + new Vector3(-radius, radius, 0), 0.2f));
        applySequence.Append(rectTransform.DOMove(center + new Vector3(-radius, -radius, 0), 0.2f));
        applySequence.Append(rectTransform.DOMove(center + new Vector3(radius, -radius, 0), 0.2f));
        applySequence.OnComplete(() => {

            Sprite spriteToApply = null;
            if (selectedColor.type != ItemType.Cream && selectedColor.makeUpData != null)
                spriteToApply = selectedColor.makeUpData.GetSprite(selectedColor.type, selectedColor.colorIndex);

            girl.ApplyItem(selectedColor.type, spriteToApply);
            ReturnItem();
        });
    }

    private void ReturnHand()
    {
        if (currentItem != null && currentItem.transform.childCount > 0)
        {
            var brushTip = currentItem.transform.GetChild(0).GetComponent<Image>();
            if (brushTip != null) brushTip.color = Color.white;
        }
        rectTransform.DOMove(handDefaultPos, 0.6f).SetEase(Ease.OutBack);
    }
}
