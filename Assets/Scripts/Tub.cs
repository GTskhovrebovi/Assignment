using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Tub : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private List<SpriteRenderer> spriteRenderers;
    [SerializeField] private Collider2D interactionCollider;
    [SerializeField] private GameObject splashEffect;
    
    private readonly List<Duck> _ducksInside = new();
    private State _currentState;

    public Transform Center => center;
    
    private void Awake()
    {
        _currentState = State.Normal;
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.3f);
    }

    public void Initialize(Color color)
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = color;
        }
    }
    
    private void UpdateState()
    {
        switch (_currentState)
        {
            case State.Normal:
                if (_ducksInside.Count > 0)
                    SetState(State.Hovered);
                break;
            case State.Hovered:
                if (_ducksInside.Count == 0)
                    SetState(State.Normal);
                break;
        }
    }

    private void SetState(State state)
    {
        _currentState = state;
        switch (state)
        {
            case State.Normal:
                transform.DOScale(Vector3.one, 0.2f);
                break;
            case State.Hovered:
                transform.DOScale(Vector3.one * 1.1f, 0.2f);
                break;
        }
    }
    
    public void SetInteractable(bool interactable)
    {
        interactionCollider.enabled = interactable;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Duck>(out var duck))
        {
            _ducksInside.Add(duck);
        }
        
        UpdateState();
    }
    

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Duck>(out var duck))
        {
            _ducksInside.Remove(duck);
        }

        UpdateState();
    }

    public void Splash()
    {
        splashEffect.gameObject.SetActive(true);
    }
    
    private enum State
    {
        Normal,
        Hovered
    }
}