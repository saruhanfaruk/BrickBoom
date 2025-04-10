using UnityEngine;
using System.Collections.Generic;

public class DefaultShapeFactory : IShapeFactory
{
    private readonly List<GameObject> prefabs = new List<GameObject>();
    private readonly List<Color> colors = new List<Color>();
    private readonly ShapeSpawner spawner;

    public DefaultShapeFactory(List<GameObject> prefabs, List<Color> colors, ShapeSpawner spawner)
    {
        this.prefabs = prefabs;
        this.colors = colors;
        this.spawner = spawner;
    }

    public ShapeController CreateShape(float currentScale, float targetScale)
    {
        int randomIndex = Random.Range(0, prefabs.Count);
        ShapeController controller = Object.Instantiate(prefabs[randomIndex]).GetComponent<ShapeController>();
        controller.Initialize(spawner, currentScale, targetScale);
        controller.ShapeColorApplier.SetShapeColor(colors[Random.Range(0, colors.Count)]);
        return controller;
    }
}
