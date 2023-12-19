using Unity.NetCode;

// IInputComponentData catches the input on every frame
// A GhostComponent indicates that this component signals a replicated entity (a ghost)
[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct InputComponent : IInputComponentData
{
    public float HorizontalMovement;
    public float VerticalMovement;
}
