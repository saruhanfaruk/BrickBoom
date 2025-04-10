using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class AreaController : SerializedMonoBehaviour
{
    #region Fields

    private IBackgroundColorChanger backgroundColorChanger;
    private IAnimationHandler animationHandler;

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
        backgroundColorChanger = new BackgroundColorChanger(GetComponent<Image>());
        animationHandler = new AnimationHandler(backgroundColorChanger);

        defaultBackgroundColor = backgroundColorChanger.GetCurrentColor();
        currentBackgroundColor = defaultBackgroundColor;
    }
    #endregion

    #region Preview
    public void ShowPreviewShape(Color tempColor)
    {
        isShowPreview = true;
        backgroundColorChanger.ChangeColor(tempColor);
    }
    public void HidePreviewShape()
    {
        isShowPreview = false;
        backgroundColorChanger.ChangeColor(currentBackgroundColor);
    }
    #endregion
    #region Control
    public void CompletedArea(Color shapeColor)
    {
        isCompleted = true;
        currentBackgroundColor = shapeColor;
        backgroundColorChanger.ChangeColor(shapeColor);
    }
    /// <summary>
    /// Clears the completed area and resets its state.
    /// </summary>
    public void ResetArea(Color finishColor)
    {
        currentBackgroundColor = defaultBackgroundColor;
        isCompleted = false;
        isShowPreview = false;

        animationHandler.PlayResetAnimation(finishColor, defaultBackgroundColor);
    }
    #endregion

}
