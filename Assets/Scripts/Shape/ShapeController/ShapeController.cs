using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShapeController : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    #region Fields
    private IShapeDragHandler shapeDragHandler;
    private IShapeColorApplier shapeColorApplier;
    public IShapeColorApplier ShapeColorApplier => shapeColorApplier; 
    private IShapePreviewHandler previewHandler;
#if UNITY_EDITOR
    private IShapeEditorUtility shapeEditorUtility;
#endif

    public List<Image> shapePartImage = new List<Image>();

    public RectTransform startingCell;
    public List<Direction> shapePath = new List<Direction>();

    private ShapeSpawner shapeSpawner;

    private RectTransform rectTransform;

    #endregion

    #region Unity Methods

    public void Initialize(ShapeSpawner spawner, float defaultScale, float dragScale)
    {
        shapeSpawner = spawner;
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one * defaultScale;
        Camera uiCamera = Extensions.GetUICamera();
        shapeColorApplier = new ShapeColorApplier(shapePartImage);
        previewHandler = new ShapePreviewHandler(shapePath, startingCell, uiCamera);
        shapeDragHandler = new ShapeDragHandler(this, previewHandler, shapeColorApplier, rectTransform, Vector3.one* defaultScale,Vector3.one* dragScale, uiCamera);
    }
    private void Start()
    {
        shapeDragHandler.StartPos = rectTransform.position;
    }

    #endregion

    #region Pointer
    /// <summary>
    /// Called when the pointer is pressed down on the shape.
    /// Captures the initial drag offset and updates the shape's position.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData) => shapeDragHandler.OnPointerDown(eventData);
    /// <summary>
    /// Called when the pointer is released from the shape.
    /// Finalizes the shape's position and handles the drop logic.
    /// </summary>
    public void OnPointerUp(PointerEventData eventData) => shapeDragHandler.OnPointerUp(eventData);

    #endregion

    #region Drag Operations

    private void Update()
    {
        if(shapeDragHandler!=null)
            shapeDragHandler.Update();
    }

    #endregion

    #region Shape Placement


    /// <summary>
    /// Handles the shape when it is dropped onto the grid. Clears the shape and destroys it after placement.
    /// </summary>
    public void HandleShapeDrop()
    {
        shapeSpawner.ClearShape(this);
        Destroy(gameObject);
    }


    #endregion

    #region Editor
#if UNITY_EDITOR

    private void SetShapeEditorUtility()
    {
        if (shapeEditorUtility == null)
        {
            RectTransform parentRectTransform = GetComponent<RectTransform>();
            startingCell = transform.GetChild(0).GetComponent<RectTransform>();
            shapeEditorUtility = new ShapeEditorUtility(ref parentRectTransform, ref shapePartImage, ref shapePath, ref startingCell);
        }
    }
    
    [Button]
    public void GenerateShapePath()
    {
        SetShapeEditorUtility();
        shapeEditorUtility.GenerateShapePath();
    }
    [Button]
    public void GenerateRandomShape()
    {
        SetShapeEditorUtility();
        shapeEditorUtility.GenerateRandomShape();
    }
    [Button]
    public void SetChildrenToCenter()
    {
        SetShapeEditorUtility();
        shapeEditorUtility.GenerateRandomShape();
    }

#endif
    #endregion


}
