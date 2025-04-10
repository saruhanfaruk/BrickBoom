#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IShapeEditorUtility
{
    void GenerateShapePath();
    void GenerateRandomShape();
    void SetChildrenToCenter();
}
#endif
