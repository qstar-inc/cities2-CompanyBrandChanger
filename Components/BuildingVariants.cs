using Colossal.Serialization.Entities;
using Unity.Collections;
using Unity.Entities;

namespace AdvancedBuildingManager.Components
{
    public struct BuildingVariants : IComponentData, IQueryTypeParameter, ISerializable
    {
        public void Serialize<TWriter>(TWriter writer)
            where TWriter : IWriter
        {
            writer.Write(Name.ToString());
            writer.Write(OGName.ToString());
        }

        public void Deserialize<TReader>(TReader reader)
            where TReader : IReader
        {
            reader.Read(out string name);
            reader.Read(out string ogName);
            Name = name;
            OGName = ogName;
        }

        public FixedString64Bytes Name;
        public FixedString64Bytes OGName;
    }
}
