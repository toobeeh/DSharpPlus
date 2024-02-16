using System;
using Polly.Telemetry;

namespace DSharpPlus.Net;

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

internal class RestTelemetryListener: TelemetryListener
{
    private readonly ConcurrentDictionary<Guid, RequestTelemetry> _telemetryData = new();
    private readonly ILogger _logger;
    
    public RestTelemetryListener(ILogger logger)
    {
        _logger = logger;
    }
    
    public override void Write<TResult, TArgs>(in TelemetryEventArguments<TResult, TArgs> args)
    {
        if(args.Event.EventName != "RatelimitDelay")
        {
            return;
        }
        
        string? id = args.Context.OperationKey;
        if (id is null)
        {
            return;
        }
       
        bool isGuid = Guid.TryParse(id, out Guid guid);
        if (!isGuid)
        {
            return;
        }

        if (args.Arguments is not int delay)
        {
            return;
        }
        
        _logger.LogTrace
        (
            LoggerEvents.RatelimitDiag,
            "Received telemetry event: RatelimitDelay. Delay: {Delay}ms.",
            delay
        );
       
        if (!_telemetryData.TryGetValue(guid, out RequestTelemetry telemetry))
        {
            telemetry = new RequestTelemetry();
            telemetry.LastUpdate = DateTime.UtcNow;
            telemetry.TotalDelay += delay;
            telemetry.DelayCount = 1;
            _telemetryData.TryAdd(guid, telemetry);
            return;
        }
        
        telemetry.LastUpdate = DateTime.UtcNow;
        telemetry.TotalDelay += delay;
        telemetry.DelayCount++;
    }
}
