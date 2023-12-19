using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[BurstCompile]
// ClientSimulation: That is us
// ThinClientSimulation: The computer pretending to be us
// We need WorldSystemFilters to tell Unity which is the Client and which is the Server and what world these systems are running
// | because we are comparing bits, its not just an OR (||)
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct EnterGameClientSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Need to hunt down the clients that have an ID but have not entered the game yet
        // Creates a temporary allocation for all entities that have a NetworkID but don't have a NetworkSteamInGame
        var Builder = new EntityQueryBuilder(Allocator.Temp).
            WithAll<NetworkId>().
            WithNone<NetworkStreamInGame>();
        
        // Adds a query that must match atleast one Entity for the System to run
        // https://docs.unity3d.com/Packages/com.unity.entities@1.0/api/Unity.Entities.ComponentSystemBase.RequireForUpdate.html
        state.RequireForUpdate(state.GetEntityQuery(Builder));
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var CommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        // Loop through all of the entities and add them to the game
        // Finds all Entities with a NetworkID comp, without a NetworkSteamInGame comp and also returns the Entity
        // WithEntityAccess gives direct access to the Entity https://docs.unity3d.com/Packages/com.unity.entities@1.0/api/Unity.Entities.QueryEnumerable-7.WithEntityAccess.html
        foreach ((var id, var entity) in SystemAPI.Query<RefRW<NetworkId>>().WithEntityAccess().
                     WithNone<NetworkStreamInGame>())
        {
            // Adds the NetworkStreamInGame to our Entity
            CommandBuffer.AddComponent<NetworkStreamInGame>(entity);
            // Creates a new entity
            Entity Req = CommandBuffer.CreateEntity();
            CommandBuffer.AddComponent<EnterGameRPC>(Req);
            // Sends a RPC
            CommandBuffer.AddComponent(Req, new SendRpcCommandRequest
            {
                TargetConnection = entity
            });
        }
        
        // Look into, but pretty sure this just ensures the code runs at the next Entity playback
        CommandBuffer.Playback(state.EntityManager);
    }
}

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct EnterGameServerSystem : ISystem
{
    public ComponentLookup<NetworkId> NetworkID;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var Builder = new EntityQueryBuilder(Allocator.Temp).
            WithAll<EnterGameRPC>().
            WithAll<ReceiveRpcCommandRequest>(); // Receives an RPC
        
        state.RequireForUpdate(state.GetEntityQuery(Builder));

        // Just caches the network ID
        NetworkID = state.GetComponentLookup<NetworkId>(true);
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var WorldName = state.WorldUnmanaged.Name;
        var CommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        //Updates the NetworkID
        NetworkID.Update(ref state);

        foreach (var (reqSRC, reqEntity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>>().
                     WithAll<EnterGameRPC>(). WithEntityAccess())
        {
            CommandBuffer.AddComponent<NetworkStreamInGame>(reqSRC.ValueRO.SourceConnection);
            var networkID = NetworkID[reqSRC.ValueRO.SourceConnection];
            
            Debug.Log($"{WorldName} connecting {networkID.Value}");
            
            CommandBuffer.DestroyEntity(reqEntity);
        }
        
        CommandBuffer.Playback(state.EntityManager);
    }
}
