using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshBaker : MonoBehaviour
{
    public NavMeshSurface Surface2D;
    public IEnumerator GenerateMesh()
    {
        if (Surface2D.useGeometry == NavMeshCollectGeometry.PhysicsColliders)
        {
            yield return new WaitForFixedUpdate();
        }
        Surface2D.BuildNavMesh();
        yield return null;
    }
}