using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnerMono : MonoBehaviour
{
    public GameObject EntityToTrack;
}

public class SpawnerBaker : Baker<SpawnerMono>
{
    public override void Bake(SpawnerMono authoring)
    {
        Spawner spawner = default;
        spawner.EntityToTrack = GetEntity(authoring.EntityToTrack, TransformUsageFlags.Dynamic);
        AddComponent(spawner);
    }
}
