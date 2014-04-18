﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="MQUTeR">
//   -
// </copyright>
// <summary>
//   Defines the Log type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TowseyLibrary
{
    using System.IO;

    using log4net;
    using log4net.Config;
    using log4net.Core;

    public static class Log
    {
        

        ////private const string MesgFormat = "{0:yyyy-MM-dd HH:mm:ss.fff}  {1}";

        private static readonly ILog Log4Net = LogManager.GetLogger(typeof(Log));

        static Log()
        {
            Verbosity = 0;
            //XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(Path.GetDirectoryName(typeof(Log).Assembly.Location), "log4net.config")));
        }

        public static int Verbosity { get; set; }


        public static void WriteLine(string format, params object[] args)
        {
            Log4Net.InfoFormat(format, args);

            //#if LOGTOCONSOLE
            //LoggedConsole.WriteLine(MesgFormat, DateTime.Now, string.Format(format, args));
            //#endif
        }

        public static void WriteIfVerbose(string format, params object[] args)
        {
            //#if LOGTOCONSOLE
            if (Verbosity > 0)
            {
                Log4Net.InfoFormat(format, args);
                //LoggedConsole.WriteLine(MesgFormat, DateTime.Now, string.Format(format, args));
            }
            //#endif
        }
    }


    
}