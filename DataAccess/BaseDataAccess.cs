using log4net;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DataAccess
{
    public class BaseDataAccess
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static IConfigurationRoot _configuration;
        public static IConfigurationRoot configuration
        {
            get
            {
                try
                {
                    if (_configuration == null)
                    {
                        //retrieve the configuration file.
                        //load the configuration and return it!
                        var configurationBuilder = new ConfigurationBuilder();
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
                        configurationBuilder.AddJsonFile(path, false);

                        _configuration = configurationBuilder.Build();

                    }
                    else
                    {
                        return _configuration;
                    }
                }
                catch (Exception e)
                {
                    log.Error("An error occurred while reading the configuration file.", e);
                }

                return _configuration;
            }
        }
    }
}
