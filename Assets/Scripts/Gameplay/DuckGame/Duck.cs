using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace Gameplay.DuckGame
{
    public class Duck : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Tub targetTub;
        [SerializeField] private Transform defaultPosition;
        [SerializeField] private Animator animator;
        [SerializeField] private Collider2D interactionCollider;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer failSpriteRenderer;
        [SerializeField] private AudioSource audioSource;
    
        [Header("Sorting")]
        [SerializeField] private SortingGroup sortingGroup;
        [SerializeField] private int defaultSortingOrder;
        [SerializeField] private int hoveringSortingOrder;
    
        [Header ("VFX")]
        [SerializeField] private GameObject glow;
        [SerializeField] private SpriteRenderer shadow;
        [SerializeField] private GameObject grabVFX;
        [SerializeField] private GameObject landVFX;

        private readonly List<Tub> _tubsInside = new();

        public event Action OnJumpInTubStart;
        public event Action OnJumpInTubFinish;
        public event Action OnTriedToJumpInWrongTub;
        public event Action OnDuckGrab;
    
        private static readonly int Swimming = Animator.StringToHash("Swimming");

        public void Initialize(Color color)
        {
            spriteRenderer.color = color;
            sortingGroup.sortingOrder = defaultSortingOrder;
        }

        private void SetState(State state)
        {
            switch (state)
            {
                case State.Free:
                    animator.speed = 1;
                    animator.enabled = true;
                    grabVFX.gameObject.SetActive(false);
                    transform.DOScale(Vector3.one, 0.2f);
                    sortingGroup.sortingOrder = defaultSortingOrder;
                    EnableShadow();
                    break;
                case State.Grabbed:
                    OnDuckGrab?.Invoke();
                    animator.speed = 0;
                    animator.enabled = false;
                    grabVFX.gameObject.SetActive(true);
                    sortingGroup.sortingOrder = hoveringSortingOrder;
                    transform.DOScale(Vector3.one * 1.1f, 0.2f);
                    DisableShadow();
                    break;
                case State.Animation:
                    animator.speed = 0;
                    animator.enabled = false;
                    grabVFX.gameObject.SetActive(false);
                    transform.DOScale(Vector3.one, 0.2f);
                    break;
            }
        }

        private void AnimateFailColor()
        {
            failSpriteRenderer.DOFade(1, 0.2f).OnComplete(() =>
            {
                failSpriteRenderer.DOFade(0, 0.8f);
            });
        }
    
        public void SetGlowActive(bool active)
        {
            glow.SetActive(active);
        }
    
        private void EnableShadow()
        {
            shadow.DOFade(1, 0.2f);
        }
    
        private void DisableShadow()
        {
            shadow.DOFade(0, 0.2f);
        }

        public void SetInteractable(bool interactable)
        {
            interactionCollider.enabled = interactable;
        }
    
        public void Quack()
        {
            audioSource.Play();
        }
    
        public void OnPointerDown(PointerEventData eventData)
        {
            SetState(State.Grabbed);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_tubsInside.Count == 0)
            {
                JumpToDefaultPosition();
            }
            else
            {
                if (_tubsInside.Contains(targetTub))
                {
                    JumpInCorrectTub();
                }
                else
                {
                    JumpInIncorrectTub();
                }
            }
        }
    
        private void JumpInCorrectTub()
        {
            OnJumpInTubStart?.Invoke();
            JumpToPosition(targetTub.Center.position, 
                () => { OnJumpInTubFinish?.Invoke(); },
                () => { sortingGroup.sortingOrder = defaultSortingOrder; });
        }
    
        private void JumpInIncorrectTub()
        {
            OnTriedToJumpInWrongTub?.Invoke();
            AnimateFailColor();
            JumpToPosition(defaultPosition.position, () => {landVFX.gameObject.SetActive(true);});
        }
    
        private void JumpToDefaultPosition()
        {
            OnTriedToJumpInWrongTub?.Invoke();
            JumpToPosition(defaultPosition.position, () => {landVFX.gameObject.SetActive(true);});
        }

        private void JumpToPosition(Vector3 position, Action finishCallback = null, Action halfwayCallback = null)
        {
            SetInteractable(false);
            DisableShadow();
            SetState(State.Animation);
        
            var bezierPathStartPoint = transform.position;
            var bezierPathStartControlPoint = transform.position;
            var bezierPathEndPoint = position;
            var bezierPathEndControlPoint = position + new Vector3(0, 4, 0);
        
            var bezierFactor = 0f;
            var halfwayCallbackInvoked = false;
            DOTween.To(()=> bezierFactor, x=> bezierFactor = x, 1f, 0.5f)
                .OnUpdate(() =>
                {
                    var positionOnCurve = DOCurve.CubicBezier.GetPointOnSegment
                    (
                        bezierPathStartPoint,
                        bezierPathStartControlPoint,
                        bezierPathEndPoint,
                        bezierPathEndControlPoint,
                        bezierFactor);
                    transform.position = positionOnCurve;
                
                    if (!halfwayCallbackInvoked && bezierFactor > 0.5f)
                    {
                        halfwayCallback?.Invoke();
                        halfwayCallbackInvoked = true;
                    }
                })
                .OnComplete(() =>
                {
                    SetState(State.Free);
                    SetInteractable(true);
                    finishCallback?.Invoke();
                });
        }
    
        public void SwimToPosition(Vector3 position, Action finishCallback)
        {
            animator.SetBool(Swimming, true);
            SetState(State.Animation);
        
            transform.DOMove(position, 3).OnComplete(() =>
            {
                animator.SetBool(Swimming, false);
                SetState(State.Free);
                finishCallback?.Invoke();
            });
        }
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Tub>(out var tub))
            {
                _tubsInside.Add(tub);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<Tub>(out var tub))
            {
                _tubsInside.Remove(tub);
            }
        }

        private enum State
        {
            Animation,
            Free,
            Grabbed
        }
    }
}