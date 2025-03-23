using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class AreaController : SerializedMonoBehaviour
{
    #region Fields
    private Image backgroundImage;
    private Color defaultBackgroundColor;
    public Color currentBackgroundColor;

    private bool isCompleted;
    public bool IsCompleted => isCompleted;

    private bool isShowPreview;
    public bool IsShowPreview => isShowPreview;

    private Vector2 position; 
    public Vector2 Position {  get { return position; } set { position = value; } }

    private Vector2Int index;
    public Vector2Int Index { get { return index; } set { index = value; } }

    #endregion

    #region Unity Methods
    private void Awake()
    {
        position = transform.position;
        backgroundImage = GetComponent<Image>();
        defaultBackgroundColor = backgroundImage.color;
        currentBackgroundColor = defaultBackgroundColor;
    }
    #endregion

    #region Preview
    public void ShowPreviewShape(Color tempColor)
    {
        isShowPreview = true;
        backgroundImage.color = tempColor;
    }
    public void HidePreviewShape()
    {
        isShowPreview = false;
        backgroundImage.color = currentBackgroundColor;
    }
    #endregion
    #region Control
    public void CompletedArea(Color shapeColor)
    {
        isCompleted = true;
        backgroundImage.color = shapeColor;
        currentBackgroundColor = shapeColor;
    }
    /// <summary>
    /// Clears the completed area and resets its state.
    /// </summary>
    public void ResetArea(Color finishColor)
    {
        currentBackgroundColor = defaultBackgroundColor;
        isCompleted = false;
        isShowPreview = false;
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < 3; i++)
        {
            sequence.Append(backgroundImage.DOColor(Color.white, .1f));
            sequence.Append(backgroundImage.DOColor(finishColor, .1f));
        }
        sequence.Append(backgroundImage.DOColor(currentBackgroundColor, .1f));
        sequence.Play();
    }
    #endregion

    //[Button]
    //public void FillAllEdgesForTest()
    //{
    //    //for (int i = 0; i < Enum.GetNames(typeof(Direction)).Length; i++)
    //    //{
    //    //    SetEdgeActive((Direction)i);
    //    //    foreach (var neighbor in GridManager.Instance.GetNeighboringAreas(this, new List<Direction> { (Direction)i }))
    //    //        neighbor.Value.SetEdgeActive(Extensions.GetOppositeDirection(neighbor.Key));
    //    //}
    //}
}
