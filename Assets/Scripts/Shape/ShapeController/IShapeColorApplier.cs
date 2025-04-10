using UnityEngine;

public interface IShapeColorApplier 
{
    public Color CurrentShapeColor { get; }
    public void SetShapeColor(Color shapeColor);
}
