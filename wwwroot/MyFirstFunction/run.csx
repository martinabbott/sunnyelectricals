#r "Newtonsoft.Json"
#load "..\Shared\common.csx"

using System;

public static void Run(TimerInfo myTimer, out string outputQueueItem, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");    
    
    var msg = new Message
    {
      msg = "From trigger",
      msgtime = DateTime.UtcNow
    };
    
    outputQueueItem = Newtonsoft.Json.JsonConvert.SerializeObject(msg);
}