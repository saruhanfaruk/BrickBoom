using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    #region Fields
    public GridGenerator gridGenerator;
    float shapeScale;
    public float ShapeScale {  get { return shapeScale; } set { shapeScale = value; } }
    private AreaController[,] gridCells;

    [HideInInspector]
    public List<Direction> allDirections = new();
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++)
            allDirections.Add((Direction)i);
    }
    #endregion

    #region Grid Operations
    public void CreateGrid()
    {
        gridCells = gridGenerator.GenerateGrid();
    }

    /// <summary>
    /// Finds the nearest valid areas based on the given shape position and movement path.
    /// </summary>
    public List<AreaController> FindValidNearestAreas(Vector3 shapePosition,List<Direction> shapePath,Camera uiCamera)
    {
        float maxDistance = Vector2.Distance(gridCells[0, 0].Position, gridCells[0, 1].Position);
        AreaController nearestArea = null;
        float nearestDistance = Mathf.Infinity;

        for (int x = 0; x < gridGenerator.gridSize; x++)
        {
            for (int y = 0; y < gridGenerator.gridSize; y++)
            {
                AreaController currentArea = gridCells[x, y];
                float distanceToStart = Vector2.Distance(shapePosition, currentArea.Position);
                if (distanceToStart < nearestDistance)
                {
                    nearestDistance = distanceToStart;
                    nearestArea = currentArea;
                }
            }
        }
        if (nearestArea == null)
            return null;

        if (Vector2.Distance(nearestArea.Position, shapePosition) > maxDistance)
            return null;

        if (!nearestArea.IsCompleted)
            return GetValidPathAreas(nearestArea, shapePath);
        else
            return null;
    }

    /// <summary>
    /// Retrieves a list of valid areas following the specified path from a starting area.
    /// </summary>
    public List<AreaController> GetValidPathAreas(AreaController selectArea, List<Direction> shapePath)
    {
        List<AreaController> validAreas = new List<AreaController>() { selectArea };
        foreach (var pathDirection in shapePath)
        {
            Vector2Int nextAreaIndex = GetNextGridIndex(selectArea.Index, pathDirection);
            if(IsValidGridIndex(nextAreaIndex))
            {
                if (!gridCells[nextAreaIndex.x, nextAreaIndex.y].IsCompleted)
                {
                    selectArea = gridCells[nextAreaIndex.x, nextAreaIndex.y];
                    validAreas.Add(selectArea);
                }
                else
                    return null;
            }
            else
                return null;

        }
        return validAreas;
    }
    /// <summary>
    /// Calculates the next grid index based on the current position and movement direction.
    /// </summary>
    public Vector2Int GetNextGridIndex(Vector2Int index, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                index.x--;
                break;
            case Direction.Down:
                index.x++;
                break;
            case Direction.Left:
                index.y--;
                break;
            case Direction.Right:
                index.y++;
                break;
            case Direction.TopRight:
                index.x--;
                index.y++;
                break;
            case Direction.TopLeft:
                index.x--;
                index.y--;
                break;
            case Direction.BottomRight:
                index.x++;
                index.y++;
                break;
            case Direction.BottomLeft:
                index.x++;
                index.y--;
                break;
            default:
                break;
        }
        return index;
    }


    /// <summary>
    /// Finds the nearest area to the given position.
    /// </summary>
    /// <param name="shapePosition">The position to check.</param>
    /// <returns>The nearest AreaController.</returns>
    public AreaController FindNearestArea(Vector3 shapePosition)
    {
        AreaController nearestArea = null;
        float nearestDistance = Mathf.Infinity;
        for (int x = 0; x < gridGenerator.gridSize; x++)
        {
            for (int y = 0; y < gridGenerator.gridSize; y++)
            {
                AreaController currentArea = gridCells[x, y];
                float distanceToStart = Vector2.Distance(shapePosition, currentArea.Position);
                if (distanceToStart < nearestDistance)
                {
                    nearestDistance = distanceToStart;
                    nearestArea = currentArea;
                }
            }
        }
        return nearestArea;
    }

    #endregion
    #region Grid Completion Check

    /// <summary>
    /// Checks for completely filled rows and columns based on the given criteria
    /// and returns a list of completed AreaController objects.
    /// </summary>
    /// <param name="checkPreview">
    /// If true, both IsCompleted and isShowPreview conditions must be met.
    /// If false, only IsCompleted is checked.
    /// </param>
    /// <returns>A list of AreaController objects in completed rows and columns.</returns>
    public List<AreaController> GetCompletedRowsAndColumns(bool checkPreview = false)
    {
        List<AreaController> completedAreas = new List<AreaController>();

        completedAreas.AddRange(GetCompletedRows(checkPreview));
        completedAreas.AddRange(GetCompletedColumns(checkPreview));

        return completedAreas;
    }

    /// <summary>
    /// Checks for completely filled rows based on the given criteria and returns 
    /// a list of AreaController objects in those rows.
    /// </summary>
    /// <param name="checkPreview">
    /// If true, both IsCompleted and isShowPreview conditions must be met.
    /// If false, only IsCompleted is checked.
    /// </param>
    /// <returns>A list of AreaController objects in completed rows.</returns>
    private List<AreaController> GetCompletedRows(bool checkPreview)
    {
        List<AreaController> completedAreas = new List<AreaController>();
        int gridSize = gridGenerator.gridSize;

        for (int x = 0; x < gridSize; x++)
        {
            bool isRowCompleted = true;

            for (int y = 0; y < gridSize; y++)
            {
                if (!gridCells[x, y].IsCompleted && (!checkPreview || !gridCells[x, y].IsShowPreview))
                {
                    isRowCompleted = false;
                    break;
                }
            }

            if (isRowCompleted)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    completedAreas.Add(gridCells[x, y]);
                }
            }
        }

        return completedAreas;
    }

    /// <summary>
    /// Checks for completely filled columns based on the given criteria and returns 
    /// a list of AreaController objects in those columns.
    /// </summary>
    /// <param name="checkPreview">
    /// If true, both IsCompleted and isShowPreview conditions must be met.
    /// If false, only IsCompleted is checked.
    /// </param>
    /// <returns>A list of AreaController objects in completed columns.</returns>
    private List<AreaController> GetCompletedColumns(bool checkPreview)
    {
        List<AreaController> completedAreas = new List<AreaController>();
        int gridSize = gridGenerator.gridSize;

        for (int y = 0; y < gridSize; y++)
        {
            bool isColumnCompleted = true;

            for (int x = 0; x < gridSize; x++)
            {
                if (!gridCells[x, y].IsCompleted && (!checkPreview || !gridCells[x, y].IsShowPreview))
                {
                    isColumnCompleted = false;
                    break;
                }
            }

            if (isColumnCompleted)
            {
                for (int x = 0; x < gridSize; x++)
                {
                    completedAreas.Add(gridCells[x, y]);
                }
            }
        }

        return completedAreas;
    }



    #endregion
    #region Utility Methods

    /// <summary>
    /// Checks if the given grid index is within valid bounds.
    /// </summary>
    /// <param name="index">The grid index to check.</param>
    /// <returns>True if the index is valid, otherwise false.</returns>
    private bool IsValidGridIndex(Vector2Int index)
    {
        return index.x >= 0 && index.x < gridGenerator.gridSize &&
               index.y >= 0 && index.y < gridGenerator.gridSize;
    }

    #endregion
}
