using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using Input = UnityEngine.Input;

// Only update if there are Input Components
[UpdateInGroup((typeof(GhostInputSystemGroup)))]
public partial struct InputSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Spawner>();
        state.RequireForUpdate<InputComponent>();
        state.RequireForUpdate<NetworkId>();
    }
    
    public void OnDestroy(ref SystemState state)
    {
    }
    
    public void OnUpdate(ref SystemState state)
    {
        bool left = Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.D);
        bool forward = Input.GetKey(KeyCode.W);
        bool back = Input.GetKey(KeyCode.S);
        bool jumpUp = Input.GetKey(KeyCode.Space);
        bool jumpDown = Input.GetKey(KeyCode.LeftShift);

        // GhostOwnerIsLocal makes sure that the Entity is being controlled and is owned locally
        foreach (var inputComponent in SystemAPI.Query<RefRW<InputComponent>>().WithAll<GhostOwnerIsLocal>())
        {
            // Sets the inputComponent to default values
            inputComponent.ValueRW = default;

            if (left)
                inputComponent.ValueRW.HorizontalMovement -= 1;
            if (right)
                inputComponent.ValueRW.HorizontalMovement += 1;
            if (forward)
                inputComponent.ValueRW.VerticalMovement += 1;
            if (back)
                inputComponent.ValueRW.VerticalMovement -= 1;
            if (jumpUp)
                inputComponent.ValueRW.JumpMovement += 1;
            if (jumpDown)
                inputComponent.ValueRW.JumpMovement -= 1;
        }
    }
}
