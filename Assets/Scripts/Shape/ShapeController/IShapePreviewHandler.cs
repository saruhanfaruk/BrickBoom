using System.Collections.Generic;
using UnityEngine;

public interface IShapePreviewHandler 
{
    List<AreaController> LastPreviewAreas { get; }

    void UpdatePreviewArea(Color shapeColor);
}
