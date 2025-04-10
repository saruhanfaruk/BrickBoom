using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeColorApplier : IShapeColorApplier
{
    private Color currentShapeColor;
    public Color CurrentShapeColor => currentShapeColor;
    
    private List<Image> shapePartImage = new List<Image>();

    public ShapeColorApplier(List<Image> shapePartImage)
    {
        this.shapePartImage = shapePartImage;
    }

    public void SetShapeColor(Color shapeColor)
    {
        currentShapeColor = shapeColor;
        foreach (var item in shapePartImage)
            item.color = currentShapeColor;
    }
}
