using ShopKart.Extensions;
using ShopKart.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShopKart.Functions
{
    public static class InsertProduct
    {
        [FunctionName("InsertProduct")]
        public static async Task<HttpResponseMessage> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Product")]
            Product product,
            [Table("producttable")] IAsyncCollector<ExtendedTableEntity<Product>> productTable,
            TraceWriter log)
        {
            try
            {
                product.Id = Guid.NewGuid();
                await productTable.AddAsync(product.ToTableEntity("Product", product.Id.ToString()));
                return new HttpResponseMessage(System.Net.HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                throw;
            }
        }
    }
}
