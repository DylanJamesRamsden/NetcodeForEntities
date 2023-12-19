using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct Player : IComponentData
{
    [GhostField]
    public int SpookyLevel;
}
