using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheck
{
    public class ICMPHealthCheck : IHealthCheck
    {
        private readonly string host = "www.does-not-exist.com";
        private readonly int HealthyRoundtripTime = 300;

        public ICMPHealthCheck(string host, int healthyRoundtripTime)
        {
            this.host = host;
            HealthyRoundtripTime = healthyRoundtripTime;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(host);

                switch (reply.Status)
                {
                    case IPStatus.Success:
                        var msg = $"ICMP to {host} took {reply.RoundtripTime} ms.";

                        return (reply.RoundtripTime > HealthyRoundtripTime)
                            ? HealthCheckResult.Degraded(msg)
                            : HealthCheckResult.Healthy(msg);
                    default:
                        var err = $"ICMP to {host} failed: {reply.Status}";
                        return HealthCheckResult.Unhealthy(err);
                }
            }
            catch (Exception ex)
            {
                var err = $"ICMP to {host} failed: {ex.Message}";
                return HealthCheckResult.Unhealthy(err);
            }
        }
    }
}