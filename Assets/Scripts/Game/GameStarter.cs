public class GameStarter : IGameStarter
{
    private ShapeSpawner shapeSpawner;

    public GameStarter(ShapeSpawner shapeSpawner)
    {
        this.shapeSpawner = shapeSpawner;
    }

    public void StartGame()
    {
        if (GridManager.Instance != null)
            GridManager.Instance.CreateGrid();

        shapeSpawner?.SpawnShapes();
    }
}