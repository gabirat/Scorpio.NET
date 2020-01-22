using Autofac;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scorpio.Messaging.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scorpio.Api.HostedServices
{
    public abstract class HostedServiceBase : IHostedService, IDisposable
    {
        protected abstract TimeSpan TaskPeriod { get; }
        protected abstract TimeSpan TaskTimeout { get; }
        protected ILifetimeScope Autofac { get; }
        protected Timer Timer;
        protected CancellationTokenSource Cts;
        protected CancellationToken CancellationToken => Cts.Token;

        private ILogger<HostedServiceBase> _logger;
        protected ILogger<HostedServiceBase> Logger => _logger ??= Autofac.Resolve<ILogger<HostedServiceBase>>();

        private IEventBus _eventBus;
        protected IEventBus EventBus => _eventBus ??= Autofac.Resolve<IEventBus>();

        protected HostedServiceBase(ILifetimeScope autofac)
        {
            Autofac = autofac ?? throw new ArgumentNullException(nameof(autofac));
        }

        public virtual Task StartAsync(CancellationToken stoppingToken)
        {
            ValidateTimeout();
            Logger.LogInformation($"{GetType().Name} running.");

            Cts = new CancellationTokenSource();
            Cts.CancelAfter(TaskTimeout);
            Logger.LogInformation($"{GetType().Name} timeout is configured to: {TaskTimeout.ToString()}");

            Timer = new Timer(DoWork, null, TimeSpan.Zero, TaskPeriod);

            return Task.CompletedTask;
        }

        private void ValidateTimeout()
        {
            if (TaskPeriod <= TaskTimeout)
            {
                var msg = $"{nameof(TaskPeriod)} should be shorten than {nameof(TaskTimeout)}";
                throw new InvalidOperationException(msg);
            }
        }

        /// <summary>
        /// Worker function. Called periodically with TimerPeriod.
        /// </summary>
        /// <param name="state"></param>
        protected abstract void DoWork(object state);

        public virtual Task StopAsync(CancellationToken stoppingToken)
        {
            Logger.LogInformation($"{GetType().Name} is stopping.");

            Timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public virtual void Dispose()
        {
            Timer?.Dispose();
            Cts?.Dispose();
        }
    }
}