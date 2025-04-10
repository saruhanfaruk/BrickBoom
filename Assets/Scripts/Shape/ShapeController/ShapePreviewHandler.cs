using System.Collections.Generic;
using UnityEngine;

public class ShapePreviewHandler : IShapePreviewHandler
{
    private List<AreaController> lastPreviewAreas;
    public List<AreaController> LastPreviewAreas=> lastPreviewAreas;
    private Camera uiCamera;
    private List<Direction> shapePath = new List<Direction>();
    public RectTransform startingCell;


    public ShapePreviewHandler(List<Direction> shapePath, RectTransform startingCell, Camera uiCamera)
    {
        this.uiCamera = uiCamera;
        this.shapePath = shapePath;
        this.startingCell = startingCell;
    }

    /// <summary>
    /// Updates the preview area while dragging.
    /// </summary>
    public void UpdatePreviewArea(Color shapeColor)
    {
        List<AreaController> currentPreviewAreas = GridManager.Instance.FindValidNearestAreas(GetShapePosition(), shapePath, uiCamera);
        if (currentPreviewAreas != null)
        {
            if (lastPreviewAreas != null && lastPreviewAreas != currentPreviewAreas)
                foreach (var lastPreviewArea in lastPreviewAreas)
                    lastPreviewArea.HidePreviewShape();

            lastPreviewAreas = currentPreviewAreas;
            foreach (var previewArea in currentPreviewAreas)
                previewArea.ShowPreviewShape(shapeColor);

            foreach (var item in GridManager.Instance.GetCompletedRowsAndColumns(true))
            {
                if (!currentPreviewAreas.Contains(item))
                    currentPreviewAreas.Add(item);
                item.ShowPreviewShape(shapeColor);
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
    /// Gets the current shape position in screen coordinates.
    /// </summary>
    private Vector2 GetShapePosition()
    {
        return RectTransformUtility.WorldToScreenPoint(uiCamera, startingCell.position);
    }
}
