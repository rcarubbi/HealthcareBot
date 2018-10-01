using HealthCareBot.Models;
using Microsoft.ApplicationInsights;

namespace HealthCareBot.Services
{
    public static class InsightUtils
    {
        private static readonly TelemetryClient Telemetry = new TelemetryClient();

        public static void TrackEvent(MenuOptions insightEvent)
        {
            Telemetry.TrackEvent(insightEvent.ToString());
        }
    }
}