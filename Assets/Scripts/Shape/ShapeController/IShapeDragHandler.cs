using UnityEngine;
using UnityEngine.EventSystems;

public interface IShapeDragHandler 
{
    Vector3 StartPos { get; set; }

    void OnPointerDown(PointerEventData eventData);
    void OnPointerUp(PointerEventData eventData);
    void Update();

}
