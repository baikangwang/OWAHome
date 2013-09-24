using log4net;

namespace Orange.OWA.Core
{
    public static class Log
    {
        private static readonly ILog _log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void Info(string message)
        {
            _log.Info(message);
        }

        public static void InfoFormat(string format, params object[] args)
        {
            _log.InfoFormat(format, args);
        }

        public static void Debug(string message)
        {
            _log.Debug(message);
        }

        public static void DebugFormat(string format, params object[] args)
        {
            _log.DebugFormat(format, args);
        }

        public static void Error(string message)
        {
            _log.Error(message);
        }

        public static void ErrorFormat(string format,params object[] args)
        {
            _log.ErrorFormat(format,args);
        }
    }
}
