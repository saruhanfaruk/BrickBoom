using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    #region Fields

    [Header("Grid Settings")]
    public RectTransform gridArea;
    public GameObject gridPrefab;
    public int gridSize = 3;

    [Range(1, 99)]
    public int paddingRatio = 50;

    

    #endregion

    #region Grid Generation

    public AreaController[,] GenerateGrid()
    {
        int cellSize = Extensions.GridCellSize;
        int cellGap = Extensions.GridCellGap;

        AreaController[,] gridCells = new AreaController[gridSize, gridSize];

        float padding = (gridArea.rect.width * (paddingRatio / 100f)) * 0.5f;
        float availableSize = gridArea.rect.width - 2 * padding;
        float totalGridSize = cellSize + (gridSize - 1) * (cellGap+ cellSize);
        float scale = availableSize / totalGridSize;
        float halfCellSize = (cellSize * scale) * 0.5f;

        float startX = -gridArea.rect.width / 2 + padding + (availableSize - totalGridSize * scale) / 2 + halfCellSize;
        float startY = gridArea.rect.height / 2 - padding - (availableSize - totalGridSize * scale) / 2 - halfCellSize;

        Camera uiCamera = Extensions.GetUICamera();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject newCell = Instantiate(gridPrefab, gridArea);
                AreaController areaController = newCell.GetComponent<AreaController>();
                RectTransform rectTransform = newCell.GetComponent<RectTransform>();

                rectTransform.localScale = Vector3.one * scale;
                float posX = startX + y * ((cellGap + cellSize) * scale);
                float posY = startY - x * ((cellGap + cellSize) * scale);
                rectTransform.anchoredPosition = new Vector2(posX, posY);

                areaController.Position = RectTransformUtility.WorldToScreenPoint(uiCamera, rectTransform.position);
                areaController.Index = new Vector2Int(x, y);
                areaController.name = $"Cell_{x}_{y}";

                gridCells[x, y] = areaController;
            }
        }

        GridManager.Instance.ShapeScale = scale;
        return gridCells;
    }

    #endregion
}
