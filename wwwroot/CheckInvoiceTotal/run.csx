#r "Newtonsoft.Json"

using System.Net;
using Newtonsoft.Json;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

    // Get request body.
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Validate the invoice.
    RootObject r = JsonConvert.DeserializeObject<RootObject>(data.ToString());
    decimal totalCost = 0M;
    decimal itemTotal = 0M;
    decimal runningTotal = 0M;

    log.Info(data.ToString());

    log.Info(String.Format("Total cost = {0}", r.Invoice.TotalCost));

    if (!Decimal.TryParse(r.Invoice.TotalCost, out totalCost))
    {
        return req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to process Invoice - cannot parse value for TotalCost.");
    }

    // Loop through the invoice items and check that the calculated cost equals the specified total cost.
    foreach (var item in r.Invoice.Items.Item)
    {
           itemTotal = 0M;
           if (!Decimal.TryParse(item.Total, out itemTotal))
           {
               return req.CreateResponse(HttpStatusCode.InternalServerError, "Failed to process invoice item - cannot parse value for Total.");
           }
           runningTotal += itemTotal;
    }

    log.Info(String.Format("Running total = {0}", runningTotal));

    if (totalCost != runningTotal)
    {
        return req.CreateResponse(HttpStatusCode.BadRequest,
            String.Format("Invoice total cost = ${0}.  Calculated total cost = ${1}", totalCost, runningTotal));
    }

    return req.CreateResponse(HttpStatusCode.OK,
            String.Format("Invoice total cost = ${0}.  Calculated total cost = ${1}", totalCost, runningTotal));
}

public class Item
{
    public string ProductCode { get; set; }
    public string ProductDescription { get; set; }
    public string Quantity { get; set; }
    public string Price { get; set; }
    public string Total { get; set; }
}

public class Items
{
    public List<Item> Item { get; set; }
}

public class Invoice
{
    public string Number { get; set; }
    public string CustomerID { get; set; }
    public string CustomerName { get; set; }
    public Items Items { get; set; }
    public string TotalCost { get; set; }
}

public class RootObject
{
    public Invoice Invoice { get; set; }
}