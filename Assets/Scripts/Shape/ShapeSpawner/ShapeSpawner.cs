using UnityEngine;
using System.Collections.Generic;

public class ShapeSpawner : MonoBehaviour
{
    [SerializeField] private RectTransform panelArea;
    [SerializeField] private int shapeCount = 5;
    [SerializeField] private List<GameObject> shapePrefabs;
    [SerializeField] private List<Color> shapeColors;

    private readonly List<ShapeController> activeShapes = new();

    private IShapeFactory shapeFactory;
    private IShapePositioner positioner;

    private void Awake()
    {
        shapeFactory = new DefaultShapeFactory(shapePrefabs, shapeColors, this);
        positioner = new HorizontalShapePositioner();
    }

    public void SpawnShapes()
    {
        if (shapePrefabs.Count == 0) return;

        float targetScale = GridManager.Instance.ShapeScale;
        float currentScale = targetScale * 0.6f;

        float panelWidth = panelArea.rect.width;
        float panelHeight = panelArea.rect.height;

        List<Vector2> positions = positioner.CalculatePositions(panelWidth, shapeCount);

        for (int i = 0; i < shapeCount; i++)
        {
            ShapeController shape = shapeFactory.CreateShape(currentScale, targetScale);
            RectTransform rect = shape.GetComponent<RectTransform>();
            rect.SetParent(panelArea, false);
            rect.anchoredPosition = positions[i];

            activeShapes.Add(shape);
        }
    }

    public void ClearShape(ShapeController shape)
    {
        if (activeShapes.Contains(shape))
        {
            activeShapes.Remove(shape);
            Destroy(shape.gameObject);
        }

        if (activeShapes.Count == 0)
            SpawnShapes();
    }
}
