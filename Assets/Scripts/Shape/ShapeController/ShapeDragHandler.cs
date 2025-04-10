using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShapeDragHandler : IShapeDragHandler
{
    private Vector3 startPos;
    public Vector3 StartPos { get { return startPos; } set { startPos = value; } }

    private bool isDragging;
    private RectTransform rectTransform;
    private Vector3 defaultScale;
    private Vector3 dragScale;
    private Camera uiCamera;
    private Vector3 dragOffset;
    private Vector3 startingOffSet = new Vector2(0, 1);
    private IShapePreviewHandler previewHandler;
    private IShapeColorApplier applier;
    private ShapeController shapeController;

    public ShapeDragHandler(ShapeController shapeController, IShapePreviewHandler previewHandler, IShapeColorApplier applier,RectTransform rectTransform,Vector3 defaultScale, Vector3 dragScale, Camera uiCamera) 
    { 
        this.shapeController = shapeController;
        this.previewHandler = previewHandler;
        this.applier = applier;
        this.rectTransform = rectTransform;
        this.defaultScale = defaultScale;
        this.dragScale = dragScale;
        this.uiCamera = uiCamera;

    }

    /// <summary>
    /// Called when the pointer is pressed down on the shape.
    /// Captures the initial drag offset and updates the shape's position.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.DOScale(dragScale, .1f);
        isDragging = true;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, eventData.position, uiCamera, out Vector3 worldPoint);
        dragOffset = (Vector2)rectTransform.position - (Vector2)worldPoint;
        rectTransform.position = worldPoint + (Vector3)dragOffset + startingOffSet;
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

    public void Update()
    {
        if (isDragging)
        {
            Vector2 mousePosition = Input.mousePosition;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, mousePosition, uiCamera, out Vector3 worldPoint))
            {
                rectTransform.position = worldPoint + (Vector3)dragOffset + (Vector3)startingOffSet;
                previewHandler.UpdatePreviewArea(applier.CurrentShapeColor);
            }
        }
    }
    /// <summary>
    /// Handles shape placement when dropped.
    /// </summary>
    private void HandleDrop()
    {
        if (previewHandler.LastPreviewAreas != null)
        {
            foreach (var item in previewHandler.LastPreviewAreas)
                item.CompletedArea(applier.CurrentShapeColor);
            shapeController.HandleShapeDrop();
        }
        else
        {
            rectTransform.DOScale(defaultScale, .1f);
            rectTransform.DOMove(startPos, .1f);
        }
        foreach (var item in GridManager.Instance.GetCompletedRowsAndColumns())
            item.ResetArea(applier.CurrentShapeColor);
    }
}
