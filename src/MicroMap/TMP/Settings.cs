using MicroMap.Diagnostics;
using MicroMap.Mapper;
using System;

namespace MicroMap
{
    public interface ISettings
    {
        /// <summary>
        /// Gets the loggerfactory that containes all loggers that are defined in the configuraiton additionaly to the loggers added per instance of the settings
        /// </summary>
        ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Gets or sets how restrictive the mapper handles errors
        /// </summary>
        RestrictiveMode RestrictiveMappingMode { get; set; }

        ///// <summary>
        ///// Gets or sets the depth of information that is logged
        ///// </summary>
        //LogDebth LogLevel { get; set; }

        ///// <summary>
        ///// Adds a logwriter to the factory
        ///// </summary>
        ///// <param name="logger">The logwriter</param>
        //void AddLogWriter(ILogWriter logger);
    }

    public class Settings : ISettings
    {
        private readonly Lazy<ILoggerFactory> _loggerFactory;

        public Settings()
        {
            RestrictiveMappingMode = RestrictiveMode.Log;
            //LogLevel = LogDebth.Extended;

            // initialize the loggerfactory
            _loggerFactory = new Lazy<ILoggerFactory>(() =>
            {
                // copy all loggers from the Configuration
                var factory = new LoggerFactory();
                //Configuration().Loggers.ForEach(l => factory.AddWriter(l.GetType().Name, l));
                return factory;
            });
        }

        /// <summary>
        /// Gets the loggerfactory that containes all loggers that are defined in the configuraiton additionaly to the loggers added per instance of the settings
        /// </summary>
        public ILoggerFactory LoggerFactory => _loggerFactory.Value;

        /// <summary>
        /// Gets or sets how restrictive the mapper handles errors
        /// </summary>
        public RestrictiveMode RestrictiveMappingMode { get; set; }
    }
}
