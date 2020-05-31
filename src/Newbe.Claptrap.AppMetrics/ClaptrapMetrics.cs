using App.Metrics;
using App.Metrics.Timer;

namespace Newbe.Claptrap.AppMetrics
{
    public static class ClaptrapMetrics
    {
        public static IMetricsRoot MetricsRoot { get; set; } = null!;

        #region EventHandler

        private static readonly TimerOptions EventHandlerTimer = new TimerOptions
        {
            Name = "Claptrap Event Handling Timer",
            MeasurementUnit = Unit.Events,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        private static readonly string[] EventHandlingKeys =
        {
            "ClaptrapTypeCode",
            "EventTypeCode"
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity">current id, id of event is master id</param>
        /// <param name="event"></param>
        /// <returns></returns>
        public static TimerContext MeasureEventHandling(
            IClaptrapIdentity identity,
            IEvent @event)
        {
            var values = new[]
            {
                identity.TypeCode,
                @event.EventTypeCode,
            };
            var metricTags = new MetricTags(EventHandlingKeys, values);
            return MetricsRoot.Measure.Timer.Time(EventHandlerTimer, metricTags);
        }

        #endregion

        #region Activation

        private static readonly TimerOptions ActivationTimer = new TimerOptions
        {
            Name = "Claptrap Activation Timer",
            MeasurementUnit = Unit.Calls,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        private static readonly string[] ActivationKeys =
        {
            "ClaptrapTypeCode",
        };

        public static TimerContext MeasureActivation(
            IClaptrapIdentity identity)
        {
            var values = new[]
            {
                identity.TypeCode,
            };
            var metricTags = new MetricTags(ActivationKeys, values);
            return MetricsRoot.Measure.Timer.Time(ActivationTimer, metricTags);
        }

        #endregion

        #region Deactivation

        private static readonly TimerOptions DeactivationTimer = new TimerOptions
        {
            Name = "Claptrap Deactivation Timer",
            MeasurementUnit = Unit.Calls,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        private static readonly string[] DeactivationKeys =
        {
            "ClaptrapTypeCode",
        };

        public static TimerContext MeasureDeactivation(
            IClaptrapIdentity identity)
        {
            var values = new[]
            {
                identity.TypeCode,
            };
            var metricTags = new MetricTags(DeactivationKeys, values);
            return MetricsRoot.Measure.Timer.Time(DeactivationTimer, metricTags);
        }

        #endregion

        #region EventSaver

        private static readonly TimerOptions EventSaverTimer = new TimerOptions
        {
            Name = "Claptrap EventSaver Timer",
            MeasurementUnit = Unit.Events,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        private static readonly string[] EventSaverKeys =
        {
            "ClaptrapTypeCode"
        };

        public static TimerContext MeasureEventSaver(
            IClaptrapIdentity identity)
        {
            var values = new[]
            {
                identity.TypeCode,
            };
            var metricTags = new MetricTags(EventSaverKeys, values);
            return MetricsRoot.Measure.Timer.Time(EventSaverTimer, metricTags);
        }

        #endregion

        #region EventLoader

        private static readonly TimerOptions EventLoaderTimer = new TimerOptions
        {
            Name = "Claptrap EventLoader Timer",
            MeasurementUnit = Unit.Calls,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        private static readonly string[] EventLoaderKeys =
        {
            "ClaptrapTypeCode"
        };

        public static TimerContext MeasureEventLoader(
            IClaptrapIdentity identity)
        {
            var values = new[]
            {
                identity.TypeCode,
            };
            var metricTags = new MetricTags(EventLoaderKeys, values);
            return MetricsRoot.Measure.Timer.Time(EventLoaderTimer, metricTags);
        }

        #endregion

        #region StateSaver

        private static readonly TimerOptions StateSaverTimer = new TimerOptions
        {
            Name = "Claptrap StateSaver Timer",
            MeasurementUnit = Unit.Calls,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        private static readonly string[] StateSaverKeys =
        {
            "ClaptrapTypeCode"
        };

        public static TimerContext MeasureStateSaver(
            IClaptrapIdentity identity)
        {
            var values = new[]
            {
                identity.TypeCode,
            };
            var metricTags = new MetricTags(StateSaverKeys, values);
            return MetricsRoot.Measure.Timer.Time(StateSaverTimer, metricTags);
        }

        #endregion

        #region StateLoader

        private static readonly TimerOptions StateLoaderTimer = new TimerOptions
        {
            Name = "Claptrap StateLoader Timer",
            MeasurementUnit = Unit.Calls,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        private static readonly string[] StateLoaderKeys =
        {
            "ClaptrapTypeCode"
        };

        public static TimerContext MeasureStateLoader(
            IClaptrapIdentity identity)
        {
            var values = new[]
            {
                identity.TypeCode,
            };
            var metricTags = new MetricTags(StateLoaderKeys, values);
            return MetricsRoot.Measure.Timer.Time(StateLoaderTimer, metricTags);
        }

        #endregion

        #region EventLoaderMigration

        private static readonly TimerOptions EventLoaderMigrationTimer = new TimerOptions
        {
            Name = "Claptrap EventLoader Migration Timer",
            MeasurementUnit = Unit.Calls,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        private static readonly string[] EventLoaderMigrationKeys =
        {
            "ClaptrapTypeCode"
        };

        public static TimerContext MeasureEventLoaderMigration(
            IClaptrapIdentity identity)
        {
            var values = new[]
            {
                identity.TypeCode,
            };
            var metricTags = new MetricTags(EventLoaderMigrationKeys, values);
            return MetricsRoot.Measure.Timer.Time(EventLoaderMigrationTimer, metricTags);
        }

        #endregion

        #region EventSaverMigration

        private static readonly TimerOptions EventSaverMigrationTimer = new TimerOptions
        {
            Name = "Claptrap EventSaver Migration Timer",
            MeasurementUnit = Unit.Calls,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        private static readonly string[] EventSaverMigrationKeys =
        {
            "ClaptrapTypeCode"
        };

        public static TimerContext MeasureEventSaverMigration(
            IClaptrapIdentity identity)
        {
            var values = new[]
            {
                identity.TypeCode,
            };
            var metricTags = new MetricTags(EventSaverMigrationKeys, values);
            return MetricsRoot.Measure.Timer.Time(EventSaverMigrationTimer, metricTags);
        }

        #endregion

        #region StateLoaderMigration

        private static readonly TimerOptions StateLoaderMigrationTimer = new TimerOptions
        {
            Name = "Claptrap StateLoader Migration Timer",
            MeasurementUnit = Unit.Calls,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        private static readonly string[] StateLoaderMigrationKeys =
        {
            "ClaptrapTypeCode"
        };

        public static TimerContext MeasureStateLoaderMigration(
            IClaptrapIdentity identity)
        {
            var values = new[]
            {
                identity.TypeCode,
            };
            var metricTags = new MetricTags(StateLoaderMigrationKeys, values);
            return MetricsRoot.Measure.Timer.Time(StateLoaderMigrationTimer, metricTags);
        }

        #endregion

        #region StateSaverMigration

        private static readonly TimerOptions StateSaverMigrationTimer = new TimerOptions
        {
            Name = "Claptrap StateSaver Migration Timer",
            MeasurementUnit = Unit.Calls,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        private static readonly string[] StateSaverMigrationKeys =
        {
            "ClaptrapTypeCode"
        };

        public static TimerContext MeasureStateSaverMigration(
            IClaptrapIdentity identity)
        {
            var values = new[]
            {
                identity.TypeCode,
            };
            var metricTags = new MetricTags(StateSaverMigrationKeys, values);
            return MetricsRoot.Measure.Timer.Time(StateSaverMigrationTimer, metricTags);
        }

        #endregion
    }
}