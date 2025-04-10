using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    #region Fields
    public UIElementAdjuster uiElementAdjuster;
    public GameStarter gameStarter;
    public RectTransform gameBackground;
    public RectTransform shapeSpawnerPanel;
    public ShapeSpawner shapeSpawner;
    #endregion

    #region Unity Methods
    protected override void Awake()
    {
        base.Awake();
        uiElementAdjuster = new UIElementAdjuster(gameBackground, shapeSpawnerPanel);
        gameStarter = new GameStarter(shapeSpawner);
        uiElementAdjuster?.AdjustUIElements();
    }

    private void Start()
    {
        gameStarter?.StartGame();
    }
    #endregion
}
