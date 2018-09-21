using ShopKart.Models;
using Newtonsoft.Json;

namespace ShopKart.Extensions
{
    public static class Extensions
    {
        public static ExtendedTableEntity<T> ToTableEntity<T>(this T obj, string partitionKey, string rowKey)
        {
            return new ExtendedTableEntity<T>
            {
                PartitionKey = partitionKey,
                RowKey = rowKey,
                Value = JsonConvert.SerializeObject(obj)
            };
        }

        public static T FromTableEntity<T>(this ExtendedTableEntity<T> obj)
        {
            return JsonConvert.DeserializeObject<T>(obj.Value);
        }
    }
}
