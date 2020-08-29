using System;
using System.Threading.Tasks;

using Xpandables.Net.Events;
using Xpandables.Net.Extensions;

namespace Xpandables.Samples.Business.Services
{
    public sealed class HttpLogginService : ILogger
    {
        private readonly Serilog.ILogger _logger;

        public HttpLogginService(Serilog.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnEntryLogAsync(object sender, object argument)
        {
            _logger.Information("Entering the {Handler} with {Query}", sender.GetType().Name, argument.ToJsonString());
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task OnExceptionLogAsync(object sender, object argument, Exception exception)
        {
            _logger.Error(exception, "Exception {Handler} with {Message}", sender.GetType().Name, exception.Message);
            await Task.CompletedTask.ConfigureAwait(false);
        }

        public async Task OnExitLogAsync(object sender, object argument, object result = default)
        {
            _logger.Information("Exiting the {Handler} with {Token}", sender.GetType().Name, result);
            await Task.CompletedTask.ConfigureAwait(false);
        }
    }
}
