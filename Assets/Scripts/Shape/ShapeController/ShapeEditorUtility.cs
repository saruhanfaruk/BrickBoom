#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShapeEditorUtility : IShapeEditorUtility
{
    #region Field
    List<Image> shapePartImage = new List<Image>();
    List<Direction> shapePath = new List<Direction>();
    RectTransform startingCell = new RectTransform();
    RectTransform shapeRectTransform = new RectTransform();
    #endregion

    #region Unity Methods
    public ShapeEditorUtility(ref RectTransform shapeRectTransform, ref List<Image> shapePartImage, ref List<Direction> shapePath, ref RectTransform startingCell)
    {
        this.shapePartImage = shapePartImage;
        this.shapePath = shapePath;
        this.startingCell = startingCell;
        this.shapeRectTransform = shapeRectTransform;
    }
    #endregion

    #region Public Methods
    public void GenerateShapePath()
    {
        shapePartImage.Clear();
        shapePath.Clear();

        if (startingCell == null)
        {
            Debug.Log("Starting cell was not assigned");
            return;
        }

        RectTransform previousChild = startingCell;

        shapePartImage.Add(previousChild.GetComponent<Image>());

        for (int i = 1; i < shapeRectTransform.childCount; i++)
        {
            RectTransform currentChild = shapeRectTransform.GetChild(i) as RectTransform;
            shapePartImage.Add(currentChild.GetComponent<Image>());

            Vector2 diff = currentChild.anchoredPosition - previousChild.anchoredPosition;
            AddDirectionsFromDifference(diff);

            previousChild = currentChild;
        }
        AdjustRectTransformSize(200);
    }


    public void GenerateRandomShape()
    {
        List<RectTransform> children = new List<RectTransform>();
        foreach (RectTransform child in shapeRectTransform)
            children.Add(child.GetComponent<RectTransform>());

        Vector2 currentPosition = Vector2.zero;
        Vector2 totalPosition = Vector2.zero;

        HashSet<Vector2> usedPositions = new HashSet<Vector2>
        {
            currentPosition
        };

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

    public void SetChildrenToCenter()
    {
        List<RectTransform> children = new List<RectTransform>();
        foreach (RectTransform child in shapeRectTransform)
            children.Add(child.GetComponent<RectTransform>());

        Vector2 totalPosition = Vector2.zero;

        foreach (var child in children)
            totalPosition += child.anchoredPosition;

        Vector2 offset = totalPosition / children.Count;
        foreach (var child in children)
            child.anchoredPosition -= offset;
        GenerateShapePath();
    }
    #endregion

    #region Private Methods

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

    private void AdjustRectTransformSize(float padding)
    {
        List<RectTransform> childRects = new List<RectTransform>();

        foreach (var item in shapePartImage)
            childRects.Add(item.rectTransform);

        float minWidth = childRects.Min(r => r.anchoredPosition3D.x);
        float maxWidth = childRects.Max(r => r.anchoredPosition3D.x);
        float minHeight = childRects.Min(r => r.anchoredPosition3D.y);
        float maxHeight = childRects.Max(r => r.anchoredPosition3D.y);
        float width = maxWidth - minWidth + padding + Extensions.GridCellSize;
        float height = maxHeight - minHeight + padding + Extensions.GridCellSize;

        shapeRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        shapeRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    #endregion
}
#endif