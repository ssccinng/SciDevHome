using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SciDevHome.Server.Mediator.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        //public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        //{
        //    _logger.LogInformation($"Handling {typeof(TRequest).Name}");
        //    var response = await next();
        //    return response;
        //}

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Handling {typeof(TRequest).Name}");
                _logger.LogDebug("request data {data}", request);
                var response = await next();
                _logger.LogDebug("response data {data}", response);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured while handling {request}", typeof(TRequest).Name);
                _logger.LogError(ex.Message);
            }
            return default;

        }
    }
}
