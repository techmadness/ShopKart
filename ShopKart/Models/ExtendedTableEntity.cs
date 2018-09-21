using Microsoft.WindowsAzure.Storage.Table;

namespace ShopKart.Models
{
    public class ExtendedTableEntity<T> : TableEntity
    {
        public string Value { get; set; }
    }
}
