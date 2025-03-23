using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShapeController : MonoBehaviour, /*IDragHandler,*/ IPointerDownHandler,IPointerUpHandler
{
    #region Fields
    bool isDragging;
    private float defaultScale;
    private float clickScale;

    public List<Image> shapePartImage = new List<Image>();
    private Color currentShapeColor;

    public RectTransform startingCell;
    public List<Direction> shapePath = new List<Direction>();

    private ShapeSpawner shapeSpawner;
    public ShapeSpawner ShapeSpawner { get { return shapeSpawner; } set { shapeSpawner = value; } }

    private RectTransform rectTransform;
    private Camera uiCamera;
    private Vector2 dragOffset;
    private Vector2 startingOffSet;
    private List<AreaController> lastPreviewAreas;
    private Vector3 startPos;

    #endregion

    #region Unity Methods

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        uiCamera = UIManager.Instance.uiCamera;
        startPos = rectTransform.position;
        startingOffSet = new Vector2(0, 1);
    }

    #endregion

    #region Pointer
    /// <summary>
    /// Called when the pointer is pressed down on the shape.
    /// Captures the initial drag offset and updates the shape's position.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOScale(clickScale, .1f);
        isDragging = true;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, uiCamera, out Vector3 worldPoint);
        dragOffset = (Vector2)rectTransform.position - (Vector2)worldPoint;
        rectTransform.position = worldPoint + (Vector3)dragOffset + (Vector3)startingOffSet;
    }
    /// <summary>
    /// Called when the pointer is released from the shape.
    /// Finalizes the shape's position and handles the drop logic.
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, uiCamera, out Vector3 worldPoint))
        {
            rectTransform.position = worldPoint + (Vector3)dragOffset + (Vector3)startingOffSet;
            HandleDrop();
        }
    }

    #endregion

    #region Drag Operations

    ///// <summary>
    ///// Handles the dragging logic.
    ///// </summary>
    //public void OnDrag(PointerEventData eventData)
    //{
    //    if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, uiCamera, out Vector3 worldPoint))
    //    {
    //        rectTransform.position = worldPoint + (Vector3)dragOffset + (Vector3)startingOffSet;
    //        UpdatePreviewArea();
    //    }
    //}

    private void Update()
    {
        if (isDragging)
        {
            Vector2 mousePosition = Input.mousePosition;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, mousePosition, uiCamera, out Vector3 worldPoint))
            {
                rectTransform.position = worldPoint + (Vector3)dragOffset + (Vector3)startingOffSet;
                UpdatePreviewArea();
            }
        }
    }

    #endregion

    #region Shape Placement

    public void SetShapeColor(Color shapeColor)
    {
        currentShapeColor = shapeColor;
        foreach (var item in shapePartImage)
            item.color = currentShapeColor;
    }

    /// <summary>
    /// Updates the preview area while dragging.
    /// </summary>
    private void UpdatePreviewArea()
    {
        List<AreaController> currentPreviewAreas = GridManager.Instance.FindValidNearestAreas(GetShapePosition(), shapePath, uiCamera);
        if (currentPreviewAreas != null)
        {
            if (lastPreviewAreas != null && lastPreviewAreas != currentPreviewAreas)
                foreach (var lastPreviewArea in lastPreviewAreas)
                    lastPreviewArea.HidePreviewShape();

            lastPreviewAreas = currentPreviewAreas;
            foreach (var previewArea in currentPreviewAreas)
                previewArea.ShowPreviewShape(currentShapeColor);

            foreach (var item in GridManager.Instance.GetCompletedRowsAndColumns(true))
            {
                if(!currentPreviewAreas.Contains(item))
                    currentPreviewAreas.Add(item);
                item.ShowPreviewShape(currentShapeColor);
            }

        }
        else
        {
            if (lastPreviewAreas != null)
            {
                foreach (var lastPreviewArea in lastPreviewAreas)
                    lastPreviewArea.HidePreviewShape();
                lastPreviewAreas = null;
            }
        }
    }

    /// <summary>
    /// Handles shape placement when dropped.
    /// </summary>
    private void HandleDrop()
    {
        if (lastPreviewAreas != null)
        {
            foreach (var item in lastPreviewAreas)
                item.CompletedArea(currentShapeColor);
            shapeSpawner.ClearShapes(this);
            Destroy(gameObject);
        }
        else
        {
            transform.DOScale(defaultScale, .1f);
            rectTransform.DOMove(startPos, .1f);
        }
        foreach (var item in GridManager.Instance.GetCompletedRowsAndColumns())
            item.ResetArea(currentShapeColor);
    }


    #endregion

    #region Utility Methods
    public void SetScale(float defaultScale, float clickScale)
    {
        this.defaultScale = defaultScale;
        this.clickScale = clickScale;
    }

    /// <summary>
    /// Gets the current shape position in screen coordinates.
    /// </summary>
    public Vector2 GetShapePosition()
    {
        return RectTransformUtility.WorldToScreenPoint(uiCamera, startingCell.position);
    }

    #endregion

    #region Editor
#if UNITY_EDITOR
    [Button]
    public void GenerateShapePath()
    {
        shapePartImage.Clear();
        shapePath.Clear();

        startingCell = transform.GetChild(0).GetComponent<RectTransform>();

        if (startingCell == null )
        {
            Debug.Log("Starting cell was not assigned");
            return;
        }

        RectTransform previousChild = startingCell;

        shapePartImage.Add(previousChild.GetComponent<Image>());

        for (int i = 1; i < transform.childCount; i++)
        {
            RectTransform currentChild = transform.GetChild(i) as RectTransform;
            shapePartImage.Add(currentChild.GetComponent<Image>());

            Vector2 diff = currentChild.anchoredPosition - previousChild.anchoredPosition;
            AddDirectionsFromDifference(diff);

            previousChild = currentChild;
        }
        AdjustRectTransformSize(200);
    }

    private void AddDirectionsFromDifference(Vector2 diff)
    {
        Direction direction = Direction.Up;

        float roundedX = Mathf.Round(diff.x * 100f) / 100f;
        float roundedY = Mathf.Round(diff.y * 100f) / 100f;

        if (roundedX > 0)
        {
            if (roundedY > 0)
                direction = Direction.TopRight;
            else if (roundedY < 0)
                direction = Direction.BottomRight;
            else
                direction = Direction.Right;
        }
        else if (roundedX < 0)
        {
            if (roundedY > 0)
                direction = Direction.TopLeft;
            else if (roundedY < 0)
                direction = Direction.BottomLeft;
            else
                direction = Direction.Left;
        }
        else
        {
            if (roundedY > 0)
                direction = Direction.Up;
            else if (roundedY < 0)
                direction = Direction.Down;
        }
        shapePath.Add(direction);
    }

    [Button]
    public void GenerateRandomShape()
    {
        List<RectTransform> children = new List<RectTransform>();
        foreach (Transform child in transform)
        {
            children.Add(child.GetComponent<RectTransform>());
        }

        Vector2 currentPosition = Vector2.zero;
        Vector2 totalPosition = Vector2.zero;

        HashSet<Vector2> usedPositions = new HashSet<Vector2>();
        usedPositions.Add(currentPosition);

        for (int i = 0; i < children.Count; i++)
        {
            if (i > 0)
            {
                bool positionFound = false;

                while (!positionFound)
                {
                    int moveType = Random.Range(0, 3);

                    int directionX = Random.Range(0, 2) == 0 ? 1 : -1;
                    int directionY = Random.Range(0, 2) == 0 ? 1 : -1;

                    if (moveType == 0)
                    {
                        currentPosition += new Vector2(directionX * 256, 0);
                    }
                    else if (moveType == 1)
                    {
                        currentPosition += new Vector2(0, directionY * 256);
                    }
                    else if (moveType == 2)
                    {
                        currentPosition += new Vector2(directionX * 256, directionY * 256);
                    }

                    if (!usedPositions.Contains(currentPosition))
                    {
                        positionFound = true;
                        usedPositions.Add(currentPosition);
                    }
                    else
                    {
                        currentPosition = Vector2.zero;
                    }
                }
            }

            children[i].anchoredPosition = currentPosition;
            totalPosition += currentPosition;
        }

        Vector2 offset = totalPosition / children.Count;

        foreach (var child in children)
        {
            child.anchoredPosition -= offset;
        }
        GenerateShapePath();
    }
    [Button]
    public void SetChildrenToCenter()
    {
        List<RectTransform> children = new List<RectTransform>();
        foreach (Transform child in transform)
        {
            children.Add(child.GetComponent<RectTransform>());
        }

        Vector2 totalPosition = Vector2.zero;

        foreach (var child in children)
        {
            totalPosition += child.anchoredPosition;
        }

        Vector2 offset = totalPosition / children.Count;

        foreach (var child in children)
        {
            child.anchoredPosition -= offset;
        }
        GenerateShapePath();
    }

    public void AdjustRectTransformSize(float padding)
    {
        RectTransform parentRectTransform = GetComponent<RectTransform>();
        List<RectTransform> childRects = new List<RectTransform>();

        foreach (var item in shapePartImage)
            childRects.Add(item.rectTransform);

        float minWidth = childRects.Min(r => r.anchoredPosition3D.x); 
        float maxWidth = childRects.Max(r => r.anchoredPosition3D.x); 
        float minHeight = childRects.Min(r => r.anchoredPosition3D.y); 
        float maxHeight = childRects.Max(r => r.anchoredPosition3D.y); 
        float width = maxWidth - minWidth + padding + Extensions.GridCellSize;
        float height = maxHeight - minHeight + padding + Extensions.GridCellSize;

        parentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        parentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

#endif
    #endregion


}
