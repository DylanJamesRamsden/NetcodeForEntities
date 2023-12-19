using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerMono : MonoBehaviour
{
    public int SpookyLevel;
}

public class PlayerBaker : Baker<PlayerMono>
{
    public override void Bake(PlayerMono authoring)
    {
        Player player = default;
        player.SpookyLevel = authoring.SpookyLevel;
        AddComponent(player);
    }
}
