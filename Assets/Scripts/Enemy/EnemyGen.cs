using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Squad
{
    public List<GameObject> units;
}

[Serializable]
public class Level
{
    public List<Squad> squads;
}

public class EnemyGen : MonoBehaviour
{
    public List<Level> squadLayouts;
    public GameObject container;

    public IEnumerator GenerateEnemies()
    {
        Level level = squadLayouts[LevelManager.Level];

        int squadNum = 1;
        foreach (var squad in level.squads)
        {
            Vector2 pos = new(Random.Range(-50, 50), Random.Range(-50, 50));
            while (Physics2D.OverlapPoint(pos, LayerMask.GetMask("Enemy", "Terrain")))
            {
                pos = new(Random.Range(-50, 50), Random.Range(-50, 50));
                yield return null;
            }
            foreach (var unit in squad.units)
            {
                Vector2 offset = new(Random.Range(-5, 5), Random.Range(-5, 5));
                while (Physics2D.OverlapPoint(pos + offset, LayerMask.GetMask("Enemy", "Terrain")))
                {
                    offset = new(Random.Range(-5, 5), Random.Range(-5, 5));
                    yield return null;
                }
                GameObject obj = Instantiate(unit, pos + offset, Quaternion.identity, container.transform);
                obj.GetComponent<Enemy>().squad = squadNum;
            }

            squadNum++;
        }
    }
}
