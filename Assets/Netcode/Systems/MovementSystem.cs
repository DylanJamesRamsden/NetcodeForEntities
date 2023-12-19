using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct MovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var Builder = new EntityQueryBuilder(Allocator.Temp).WithAll<InputComponent>().WithAll<Simulate>();
        
        state.RequireForUpdate(state.GetEntityQuery(Builder));
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var MoveJob = new MovementJob
        {
            Speed = 4 * SystemAPI.Time.DeltaTime
        };


        state.Dependency = MoveJob.ScheduleParallel(state.Dependency);
        
        /*var speed = SystemAPI.Time.DeltaTime * 4;
        foreach (var (input, trans) in SystemAPI.Query<RefRO<InputComponent>, RefRW<LocalTransform>>().WithAll<Simulate>())
        {
            var moveInput = new float2(input.ValueRO.HorizontalMovement, input.ValueRO.VerticalMovement);
            moveInput = math.normalizesafe(moveInput) * speed;
            trans.ValueRW.Position += new float3(moveInput.x, 0, moveInput.y);
        }*/
    }
}

public partial struct MovementJob : IJobEntity
{
    public float Speed;

    public void Execute(InputComponent inputComponent, ref LocalTransform localTransform)
    {
        var move = new float3(inputComponent.HorizontalMovement, inputComponent.JumpMovement, inputComponent.VerticalMovement);
        // Normalize!!! No!! Seems to break the calculation, use NormalizeSafe
        move = math.normalizesafe(move) * Speed;

        localTransform.Position += new float3(move.x, move.y, move.z);
    }
}
