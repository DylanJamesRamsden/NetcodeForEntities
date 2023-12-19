using Unity.Entities;
using UnityEngine;

public class InputMono : MonoBehaviour
{
    
}

public class InputBaker : Baker<InputMono>
{
    public override void Bake(InputMono authoring)
    {
        AddComponent<InputComponent>();
    }
}
