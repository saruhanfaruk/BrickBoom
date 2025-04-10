using System.Collections.Generic;
using UnityEngine;

public class HorizontalShapePositioner : IShapePositioner
{
    private const float spacing = 20f;

    public List<Vector2> CalculatePositions(float panelWidth, int count)
    {
        List<Vector2> positions = new();
        float maxSize = panelWidth / count - spacing;

        for (int i = 0; i < count; i++)
        {
            float xPos = -panelWidth / 2 + (i * (maxSize + spacing)) + maxSize / 2;
            positions.Add(new Vector2(xPos, 0));
        }

        return positions;
    }
}
