using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Ingestion.Infraestructura.Logging
{
    public class SagaLogger
    {
        private readonly ILogger _logger;
        
        public SagaLogger(ILogger logger)
        {
            _logger = logger;
        }
        
        public void LogSagaStarted(Guid sagaId, string sagaType, Dictionary<string, object> context = null)
        {
            LogSagaEvent(sagaId, sagaType, "Started", context);
        }
        
        public void LogSagaStepCompleted(Guid sagaId, string sagaType, string step, Dictionary<string, object> context = null)
        {
            LogSagaEvent(sagaId, sagaType, $"StepCompleted:{step}", context);
        }
        
        public void LogSagaStepFailed(Guid sagaId, string sagaType, string step, string error, Dictionary<string, object> context = null)
        {
            var ctx = context ?? new Dictionary<string, object>();
            ctx["error"] = error;
            LogSagaEvent(sagaId, sagaType, $"StepFailed:{step}", ctx);
        }
        
        public void LogCompensationStarted(Guid sagaId, string sagaType, string step, Dictionary<string, object> context = null)
        {
            LogSagaEvent(sagaId, sagaType, $"CompensationStarted:{step}", context);
        }
        
        public void LogCompensationCompleted(Guid sagaId, string sagaType, string step, Dictionary<string, object> context = null)
        {
            LogSagaEvent(sagaId, sagaType, $"CompensationCompleted:{step}", context);
        }
        
        public void LogSagaCompleted(Guid sagaId, string sagaType, Dictionary<string, object> context = null)
        {
            LogSagaEvent(sagaId, sagaType, "Completed", context);
        }
        
        public void LogSagaFailed(Guid sagaId, string sagaType, string error, Dictionary<string, object> context = null)
        {
            var ctx = context ?? new Dictionary<string, object>();
            ctx["error"] = error;
            LogSagaEvent(sagaId, sagaType, "Failed", ctx);
        }
        
        private void LogSagaEvent(Guid sagaId, string sagaType, string eventType, Dictionary<string, object> context)
        {
            var logContext = new Dictionary<string, object>
            {
                ["sagaId"] = sagaId,
                ["sagaType"] = sagaType,
                ["eventType"] = eventType,
                ["timestamp"] = DateTime.UtcNow
            };
            
            if (context != null)
            {
                foreach (var item in context)
                {
                    logContext[item.Key] = item.Value;
                }
            }
            
            // Estructurar el log para que sea fácilmente consultable
            _logger.LogInformation("SAGA_EVENT {SagaEvent}", System.Text.Json.JsonSerializer.Serialize(logContext));
        }
    }
}
