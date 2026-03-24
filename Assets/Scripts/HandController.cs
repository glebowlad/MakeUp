using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

public class HandController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [Header("Links")]
    [SerializeField] private RectTransform _faceZoneUI; 
    [SerializeField] private GirlController _girl;
    [SerializeField] private DraggableItem _eyeshadowBrush;
    [SerializeField] private DraggableItem _blushBrush;

    private RectTransform _rectTransform;
    private Transform _itemDefaultParent;
    private DraggableItem _currentItem;
    private DraggableItem _selectedColor;
    private Vector3 _handDefaultPos;
    private Vector3 _itemDefaultPos;
    private bool _canDrag = false;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _handDefaultPos = _rectTransform.position;
    }

    
    public void OnItemClicked(DraggableItem colorItem)
    {
        if (_currentItem != null)
        {

            ReturnItem(() => OnItemClicked(colorItem));
            return;
        }

        PrepareItemData(colorItem);

        MakeSequence(colorItem);
    }

    private void MakeSequence(DraggableItem colorItem)
    {
        Sequence takeSequence = DOTween.Sequence();

        takeSequence.Append(_rectTransform.DOMove(_itemDefaultPos, 0.5f));
        takeSequence.AppendCallback(() => AttachHand(_currentItem));


        if (colorItem.type == ItemType.Eyeshadow || colorItem.type == ItemType.Blushes)
        {
            takeSequence.Append(_rectTransform.DOMove(colorItem.transform.position, 0.4f));
            takeSequence.Append(_rectTransform.DOShakePosition(0.4f, 15f, 10));
            takeSequence.AppendCallback(() => PaintBrush(colorItem));


        }

        Vector3 readyPos = (_faceZoneUI.position + _itemDefaultPos) / 2;
        takeSequence.Append(_rectTransform.DOMove(readyPos, 0.5f));
        takeSequence.OnComplete(() => _canDrag = true);
    }

    private void PrepareItemData(DraggableItem colorItem)
    {
        _selectedColor = colorItem;
        _canDrag = false;
        _currentItem = colorItem.type switch
        {
            ItemType.Eyeshadow => _eyeshadowBrush,
            ItemType.Blushes => _blushBrush,
            _ => colorItem
        };

        _itemDefaultPos = _currentItem.transform.position;
        _itemDefaultParent = _currentItem.transform.parent;
    }

    private void PaintBrush(DraggableItem colorItem)
    {
        Color selectedColorVal = colorItem.makeUpData.GetColor(colorItem.type, colorItem.colorIndex);
        var brushTip = _currentItem.transform.GetChild(0).GetComponent<Image>();

        if (brushTip != null)
        {
            brushTip.color = selectedColorVal;
            brushTip.DOFade(1f, 0.1f);
        }
    }

    private void AttachHand(DraggableItem tool)
    {
        tool.transform.SetParent(transform);
        tool.transform.SetSiblingIndex(1);
        tool.transform.localPosition = Vector3.zero;
        if (tool.TryGetComponent<Image>(out var img)) img.raycastTarget = false;
    }

    private void ReturnItem(Action onDone = null)
    {
        _canDrag = false;
        DraggableItem itemToReturn = _currentItem; 

        _rectTransform.DOMove(_itemDefaultPos, 0.5f).OnComplete(() => {
            
            itemToReturn.transform.SetParent(_itemDefaultParent);
            itemToReturn.transform.position = _itemDefaultPos;
            if (itemToReturn.TryGetComponent<Image>(out var img)) img.raycastTarget = true;

            _currentItem = null;
            if (onDone != null) onDone.Invoke();
            else ReturnHand();
        });
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_canDrag) return;
        _rectTransform.position = eventData.position;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_canDrag) return;
        if (RectTransformUtility.RectangleContainsScreenPoint(_faceZoneUI, eventData.position))
        {
            StartInteraction();
        }
    }

    public void StartInteraction()
    {
        _canDrag = false;

        float radius = 80f;
        Vector3 center = _rectTransform.position;

        Sequence applySequence = DOTween.Sequence();
        applySequence.Append(_rectTransform.DOMove(center + new Vector3(radius, radius, 0), 0.2f));
        applySequence.Append(_rectTransform.DOMove(center + new Vector3(-radius, radius, 0), 0.2f));
        applySequence.Append(_rectTransform.DOMove(center + new Vector3(-radius, -radius, 0), 0.2f));
        applySequence.Append(_rectTransform.DOMove(center + new Vector3(radius, -radius, 0), 0.2f));
        applySequence.OnComplete(() => {

            Sprite spriteToApply = null;
            if (_selectedColor.type != ItemType.Cream && _selectedColor.makeUpData != null)
                spriteToApply = _selectedColor.makeUpData.GetSprite(_selectedColor.type, _selectedColor.colorIndex);

            _girl.ApplyItem(_selectedColor.type, spriteToApply);
            ReturnItem();
        });
    }

    private void ReturnHand()
    {
        if (_currentItem != null && _currentItem.transform.childCount > 0)
        {
            var brushTip = _currentItem.transform.GetChild(0).GetComponent<Image>();
            if (brushTip != null) brushTip.color = Color.white;
        }
        _rectTransform.DOMove(_handDefaultPos, 0.6f).SetEase(Ease.OutBack);
    }
}
