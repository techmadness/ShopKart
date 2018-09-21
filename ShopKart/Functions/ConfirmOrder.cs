using ShopKart.Extensions;
using ShopKart.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Linq;

namespace ShopKart.Functions
{
    public static class ConfirmOrder
    {
        [FunctionName("ConfirmOrder")]
        public static async System.Threading.Tasks.Task RunAsync([QueueTrigger("orderqueue")]
            QueueItem<Guid> queueItem,
            [Table("ordertable")] IQueryable<ExtendedTableEntity<Order>> orderTable,
            [Table("producttable")] IQueryable<ExtendedTableEntity<Product>> productTable,
            [Queue("emailqueue")] IAsyncCollector<QueueItem<EmailMessage>> emailQueue,
            TraceWriter log)
        {
            try
            {
                var orderEntity = orderTable.Where(x => x.RowKey == queueItem.Value.ToString()).FirstOrDefault();
                if (orderEntity != null)
                {
                    var order = orderEntity.FromTableEntity();
                    var productEntity = productTable.Where(x => x.RowKey == order.ProductId.ToString()).FirstOrDefault();
                    if (productEntity != null)
                    {
                        var product = productEntity.FromTableEntity();
                        var emailBody = $@"<style>body{{font-family:Segoe UI,SegoeUI,Segoe WP,Helvetica Neue,Helvetica,Tahoma,Arial,sans-serif}}</style>
                          <h3>Thank you for your order #{order.OrderId}. Your order has been confirmed.</h3>
                          <table border=""1"" celpadding=""5"">
                              <tr>
                                  <th>Product</td>
                                  <th>Price</td>
                                  <th>Quentity</td>
                                  <th>Total</td>
                              </tr>
                              <tr>
                                  <td>{product.Name}</td>
                                  <td style= ""text-align:right;"">{order.Price}</td>
                                  <td style= ""text-align:right;"">{order.Quentity}</td>
                                  <td style= ""text-align:right;"">{order.Price * order.Quentity}</td>
                              </tr>
                          </table>
                          <br/><br/>
                          Happy Shopping!";


                        await emailQueue.AddAsync(new QueueItem<EmailMessage>()
                        {
                            Value = new EmailMessage
                            {
                                From = "noreply@techmadness.com",
                                To = "hirenkagrana@techmadness.com",
                                Subject = $"Order Confirmation - #{order.OrderId}",
                                Body = emailBody
                            }
                        });
                    }
                    else
                    {
                        throw new ApplicationException($"Product ({order.ProductId}) not found.");
                    }
                }
                else
                {
                    throw new ApplicationException($"Order ({queueItem.Value}) not found.");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                throw;
            }
        }
    }
}
