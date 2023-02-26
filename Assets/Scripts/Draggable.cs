using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private Vector3 _offset;
    private Camera _camera;
    
    private void Awake()
    {
        _camera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var position = _camera.ScreenToWorldPoint(eventData.position);
        _offset = position - transform.position;
        _offset.z = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        var position = _camera.ScreenToWorldPoint(eventData.position);
        position.z = transform.position.z;
        transform.position = position - _offset;
    }
}