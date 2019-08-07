using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure.ToolHelper
{
    public class LogUtils
    {
        //可以声明多个日志对象
        public static ILog log = LogManager.GetLogger(typeof(LogUtils));

		#region 01-初始化Log4net的配置
		/// <summary>
		/// 初始化Log4net的配置
		/// xml文件一定要改为嵌入的资源
		/// </summary>
		public static void InitLog4Net()
        {
            //Assembly assembly = Assembly.GetExecutingAssembly();
            //var xml = assembly.GetManifestResourceStream("Ypf.Utils.log4net.xml");
            //log4net.Config.XmlConfigurator.Configure(xml);
            log4net.Config.XmlConfigurator.Configure();
        }
        #endregion

        /************************* 五种不同日志级别 *******************************/
        //FATAL(致命错误) > ERROR（一般错误） > WARN（警告） > INFO（一般信息） > DEBUG（调试信息）

        #region 01-DEBUG（调试信息）
        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="msg">日志信息</param>
        public static void Debug(string msg)
        {
			Console.WriteLine($"Debug@{msg}");
			//log.Debug(msg);
			ThreadPool.QueueUserWorkItem(new WaitCallback(log.Debug), msg);
		
        }
        /// <summary>
        /// Debug
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="exception">错误信息</param>
        public static void Debug(string msg, Exception exception)
        {
			Console.WriteLine($"Debug@{msg}");
			Console.WriteLine($"Exception@{exception.Message}");
			log.Debug(msg, exception);
        }

        #endregion

        #region 02-INFO（一般信息）
        /// <summary>
        /// Info
        /// </summary>
        /// <param name="msg">日志信息</param>
        public static void Info(string msg)
        {
			Console.WriteLine($"Info@{msg}");
			ThreadPool.QueueUserWorkItem(new WaitCallback(log.Info), msg);
			
        }
        /// <summary>
        /// Info
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="exception">错误信息</param>
        public static void Info(string msg, Exception exception)
        {
			Console.WriteLine($"Info@{msg}");
			Console.WriteLine($"Exception@{exception.Message}");
			log.Info(msg, exception);
        }
        #endregion

        #region 03-WARN（警告）
        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="msg">日志信息</param>
        public static void Warn(string msg)
        {
			Console.WriteLine($"Warn@{msg}");
			ThreadPool.QueueUserWorkItem(new WaitCallback(log.Warn), msg);
        }
        /// <summary>
        /// Warn
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="exception">错误信息</param>
        public static void Warn(string msg, Exception exception)
        {
			Console.WriteLine($"Warn@{msg}");
			Console.WriteLine($"Exception@{exception.Message}");
			log.Warn(msg, exception);
        }
        #endregion

        #region 04-ERROR（一般错误）
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="msg">日志信息</param>
        public static void Error(string msg)
        {
			Console.WriteLine($"Error@{msg}");
			ThreadPool.QueueUserWorkItem(new WaitCallback(log.Error), msg);
        }
        /// <summary>
        /// Error
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="exception">错误信息</param>
        public static void Error(string msg, Exception exception)
        {
			Console.WriteLine($"Error@{msg}");
			Console.WriteLine($"Exception@{exception.Message}");
			log.Error(msg, exception);
        }
        #endregion

        #region 05-FATAL(致命错误)
        /// <summary>
        /// Fatal
        /// </summary>
        /// <param name="msg">日志信息</param>
        public static void Fatal(string msg)
        {
			Console.WriteLine($"Fatal@{msg}");
			ThreadPool.QueueUserWorkItem(new WaitCallback(log.Fatal), msg);
        }
        /// <summary>
        /// Fatal
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="exception">错误信息</param>
        public static void Fatal(string msg, Exception exception)
        {
			Console.WriteLine($"Fatal@{msg}");
			Console.WriteLine($"Exception@{exception.Message}");
			log.Fatal(msg, exception);
        }
        #endregion
    }
}
