using ShopKart.Extensions;
using ShopKart.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Net;
using System.Net.Http;

namespace ShopKart.Functions
{
    public static class AcceptOrder
    {
        [FunctionName("AcceptOrder")]
        public static async System.Threading.Tasks.Task<HttpResponseMessage> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Order")]
            Order order,
            [Table("ordertable")] IAsyncCollector<ExtendedTableEntity<Order>> orderTable,
            [Queue("orderqueue")] IAsyncCollector<QueueItem<Guid>> orderQueue,
            TraceWriter log)
        {
            try
            {
                order.OrderId = Guid.NewGuid();
                await orderTable.AddAsync(order.ToTableEntity("order", order.OrderId.ToString()));
                await orderQueue.AddAsync(new QueueItem<Guid> { Value = order.OrderId });

                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                throw;
            }
        }
    }
}
