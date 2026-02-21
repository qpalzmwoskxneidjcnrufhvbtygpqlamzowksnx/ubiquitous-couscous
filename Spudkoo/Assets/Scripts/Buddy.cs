using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Buddy : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //This script will be for the AI buddy character.
    //They are made of a UI image since we'll need it to be on screen and interactable. 
    //They will randomly wander around the screen, and they can also be dragged by the user to a specific location.

    [Header("Physics")]
    [SerializeField] private float gravity = 400f;

    [Header("Wandering")]
    [SerializeField] private float wanderSpeed = 100f;
    [SerializeField] private float targetReachedThreshold = 5f;
    [SerializeField] private float minWaitTime = 1f;
    [SerializeField] private float maxWaitTime = 4f;
    [Tooltip("When false, Buddy will not wander (e.g. during chat or other activities).")]
    [SerializeField] private bool canWander = true;

    [Header("Squish Effect")]
    [SerializeField] private float squishStretch = 1.15f;
    [SerializeField] private float squishCompress = 0.85f;
    [SerializeField] private float squishRecoveryDuration = 0.25f;

    private RectTransform _rectTransform;
    private RectTransform _parentRect;
    private Vector2 _targetPosition;
    private bool _hasValidTarget;
    private bool _isWaiting;
    private float _waitTimer;
    private float _squishTimer;
    private Vector2 _moveDirection;

    private float _velocityY;
    private bool _isGrounded;
    private bool _isDragged;
    private float _floorY;
    private float _groundY;
    private float _halfHeight;
    private float _halfWidth;

    private List<UICollidable> _collidablesCache = new List<UICollidable>();
    private Vector3[] _cornerBuffer = new Vector3[4];

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _parentRect = _rectTransform.parent as RectTransform;

        if (_parentRect == null)
        {
            Debug.LogWarning("Buddy: Parent must have a RectTransform for bounds. Wandering disabled.");
            canWander = false;
        }

        Rect buddyRect = _rectTransform.rect;
        _halfWidth = buddyRect.width * 0.5f;
        _halfHeight = buddyRect.height * 0.5f;
        UpdateFloorY();
        RefreshCollidables();
    }

    private void Start()
    {
        _isGrounded = true;
        _groundY = _rectTransform.anchoredPosition.y;
        if (canWander)
        {
            PickNewTarget();
        }
    }

    private void Update()
    {
        if (_isDragged)
            return;

        float dt = Time.deltaTime;
        Vector2 pos = _rectTransform.anchoredPosition;

        if (!_isGrounded)
        {
            _velocityY -= gravity * dt;
            float newY = pos.y + _velocityY * dt;

            float? landY = GetLandingY(pos.x, pos.y, newY);
            if (landY.HasValue)
            {
                pos.y = landY.Value;
                _groundY = landY.Value;
                _velocityY = 0f;
                _isGrounded = true;
                if (canWander)
                    PickNewTarget();
            }
            else
            {
                pos.y = newY;
            }

            _rectTransform.anchoredPosition = pos;
            return;
        }

        if (!canWander)
            return;

        if (_isWaiting)
        {
            _waitTimer -= dt;
            if (_waitTimer <= 0f)
            {
                _isWaiting = false;
                PickNewTarget();
            }
            return;
        }

        if (!_hasValidTarget)
            return;

        // Squish effect: ease from stretched back to normal as we move
        if (_squishTimer > 0f)
        {
            _squishTimer -= dt;
            float t = 1f - Mathf.Clamp01(_squishTimer / squishRecoveryDuration);
            t = t * t * (3f - 2f * t);
            float stretch = Mathf.Lerp(squishStretch, 1f, t);
            float compress = Mathf.Lerp(squishCompress, 1f, t);
            bool stretchX = Mathf.Abs(_moveDirection.x) >= Mathf.Abs(_moveDirection.y);
            _rectTransform.localScale = new Vector3(
                stretchX ? stretch : compress,
                stretchX ? compress : stretch,
                1f
            );
        }

        pos = _rectTransform.anchoredPosition;
        Vector2 newPos = Vector2.MoveTowards(pos, _targetPosition, wanderSpeed * dt);
        _rectTransform.anchoredPosition = newPos;

        // Check if we've walked off the edge - apply gravity if no longer supported
        if (!IsSupportedAtPosition(newPos.x, newPos.y))
        {
            _isGrounded = false;
            _hasValidTarget = false;
            _rectTransform.localScale = Vector3.one;
            return;
        }

        if (Vector2.Distance(newPos, _targetPosition) < targetReachedThreshold)
        {
            _rectTransform.localScale = Vector3.one;
            _isWaiting = true;
            _waitTimer = Random.Range(minWaitTime, maxWaitTime);
            _hasValidTarget = false;
        }
    }

    private void UpdateFloorY()
    {
        if (_parentRect == null) return;
        _floorY = _parentRect.rect.yMin + _halfHeight;
    }

    private void RefreshCollidables()
    {
        _collidablesCache.Clear();
        _collidablesCache.AddRange(FindObjectsByType<UICollidable>(FindObjectsSortMode.None));
    }

    /// <summary>
    /// Call if you add UICollidable elements at runtime.
    /// </summary>
    public void RefreshCollidablesCache()
    {
        RefreshCollidables();
    }

    private float? GetLandingY(float posX, float currentY, float newY)
    {
        float buddyBottom = newY - _halfHeight;
        float buddyLeft = posX - _halfWidth;
        float buddyRight = posX + _halfWidth;

        // Floor: bottom of parent rect
        float floorSurfaceY = _parentRect.rect.yMin;
        if (buddyBottom <= floorSurfaceY)
            return _floorY;

        float? highestLanding = null;
        foreach (var col in _collidablesCache)
        {
            if (col == null)
                continue;
            if (col.transform == transform)
                continue;

            Rect colRect = GetCollidableRectInParentSpace(col);
            float colTop = colRect.yMax;
            float colLeft = colRect.xMin;
            float colRight = colRect.xMax;

            bool overlapsX = buddyLeft < colRight && buddyRight > colLeft;
            bool wasAbove = (currentY - _halfHeight) >= colTop;
            bool wouldLand = buddyBottom <= colTop;

            if (overlapsX && wasAbove && wouldLand)
            {
                float landY = colTop + _halfHeight;
                if (!highestLanding.HasValue || landY > highestLanding.Value)
                    highestLanding = landY;
            }
        }

        return highestLanding;
    }

    private const float SupportTolerance = 2f;

    private bool IsSupportedAtPosition(float posX, float posY)
    {
        float buddyBottom = posY - _halfHeight;
        float buddyLeft = posX - _halfWidth;
        float buddyRight = posX + _halfWidth;

        // Floor spans full width - if we're at floor level, we're supported
        float floorSurfaceY = _parentRect.rect.yMin;
        if (buddyBottom <= floorSurfaceY + SupportTolerance)
            return true;

        // Check collidables: is our bottom resting on a platform we overlap?
        foreach (var col in _collidablesCache)
        {
            if (col == null || col.transform == transform)
                continue;

            Rect colRect = GetCollidableRectInParentSpace(col);
            float colTop = colRect.yMax;
            float colLeft = colRect.xMin;
            float colRight = colRect.xMax;

            bool overlapsX = buddyLeft < colRight && buddyRight > colLeft;
            bool restingOnTop = Mathf.Abs(buddyBottom - colTop) <= SupportTolerance;

            if (overlapsX && restingOnTop)
                return true;
        }

        return false;
    }

    private Rect GetCollidableRectInParentSpace(UICollidable col)
    {
        col.RectTransform.GetWorldCorners(_cornerBuffer);
        Vector2 bl = _parentRect.InverseTransformPoint(_cornerBuffer[0]);
        Vector2 tl = _parentRect.InverseTransformPoint(_cornerBuffer[1]);
        Vector2 tr = _parentRect.InverseTransformPoint(_cornerBuffer[2]);
        Vector2 br = _parentRect.InverseTransformPoint(_cornerBuffer[3]);
        float minX = Mathf.Min(bl.x, tl.x, tr.x, br.x);
        float maxX = Mathf.Max(bl.x, tl.x, tr.x, br.x);
        float minY = Mathf.Min(bl.y, tl.y, tr.y, br.y);
        float maxY = Mathf.Max(bl.y, tl.y, tr.y, br.y);
        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }

    private void PickNewTarget()
    {
        if (_parentRect == null)
            return;

        Rect parentRect = _parentRect.rect;

        // Wander only along the bottom: X varies, Y stays at ground level
        float minX = parentRect.xMin + _halfWidth;
        float maxX = parentRect.xMax - _halfWidth;

        if (minX >= maxX)
        {
            _hasValidTarget = false;
            return;
        }

        _targetPosition = new Vector2(
            Random.Range(minX, maxX),
            _groundY
        );
        _hasValidTarget = true;

        // Direction for squish effect (stretch in movement direction)
        Vector2 currentPos = _rectTransform.anchoredPosition;
        _moveDirection = (_targetPosition - currentPos).normalized;
        if (_moveDirection.sqrMagnitude < 0.01f)
            _moveDirection = Vector2.right; // fallback if already at target

        _squishTimer = squishRecoveryDuration;
    }

    /// <summary>
    /// Call when drag starts (from drag handler). Stops physics and wandering.
    /// </summary>
    public void OnDragStart()
    {
        _isDragged = true;
    }

    /// <summary>
    /// Call when drag ends (from drag handler). Buddy will fall if released above ground.
    /// </summary>
    public void OnDragEnd()
    {
        _isDragged = false;
        _isGrounded = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        OnDragStart();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragged) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint);
        _rectTransform.anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnDragEnd();
    }

    /// <summary>
    /// Call this to enable/disable wandering (e.g. when chat is open or during other activities).
    /// </summary>
    public void SetCanWander(bool value)
    {
        canWander = value;
        if (canWander && !_hasValidTarget && !_isWaiting)
        {
            PickNewTarget();
        }
    }
}
