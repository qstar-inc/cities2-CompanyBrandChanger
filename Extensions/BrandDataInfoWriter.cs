using Colossal.UI.Binding;
using CompanyBrandChanger.Systems;

namespace CompanyBrandChanger.Extensions
{
    public static class BrandDataInfoJsonWriterExtensions
    {
        public static void Write(this IJsonWriter writer, BrandDataInfo value)
        {
            writer.TypeBegin(typeof(BrandDataInfo).FullName);

            writer.PropertyName("Name");
            writer.Write(value.Name);

            writer.PropertyName("PrefabName");
            writer.Write(value.PrefabName);

            writer.PropertyName("Color1");
            writer.Write(value.Color1);

            writer.PropertyName("Color2");
            writer.Write(value.Color2);

            writer.PropertyName("Color3");
            writer.Write(value.Color3);

            writer.PropertyName("Entity");
            writer.Write(value.Entity);

            writer.PropertyName("Icon");
            writer.Write(value.Icon);

            writer.PropertyName("Companies");
            writer.Write(value.Companies);

            writer.TypeEnd();
        }

        public static void Write(this IJsonWriter writer, BrandDataInfo[] array)
        {
            writer.ArrayBegin(array.Length);
            foreach (var item in array)
                Write(writer, item);
            writer.ArrayEnd();
        }
    }
}
