﻿using log4net;
using log4net.Config;

namespace PerformersScene.Models.Log
{
    public static class Logger
    {
        public static ILog Log { get; } = LogManager.GetLogger("LOGGER");

        public static void InitLogger()
        {
            XmlConfigurator.Configure();
        }
    }
}
