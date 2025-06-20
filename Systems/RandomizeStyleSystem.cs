using System;
using Colossal.Entities;
using Game.Common;
using Unity.Entities;

namespace AdvancedBuildingManager.Systems
{
    public class RandomizeStyleSystem
    {
        public static void RandomizeStyle(EntityManager entityManager, Entity entity)
        {
            try
            {
                if (entityManager.TryGetComponent(entity, out PseudoRandomSeed pseudoRandomSeed))
                {
                    System.Random random = new();
                    ushort randomUShort = (ushort)random.Next(0, 65536);
                    pseudoRandomSeed.m_Seed = randomUShort;

                    entityManager.AddComponentData(entity, pseudoRandomSeed);
                    entityManager.AddComponent<Updated>(entity);
                }
            }
            catch (Exception ex)
            {
                Mod.log.Info(ex.ToString());
            }
        }
    }
}
