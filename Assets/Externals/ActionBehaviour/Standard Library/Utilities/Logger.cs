/********************************************************************
	created:	2014/01/13
	filename:	Logger.cs
	author:		Benjamin
	purpose:	[]
*********************************************************************/
#if (DEV_BUILD || DEBUG_ON)
#define CONDITIONAL_LOG		// 데브 빌드는 로그 보여줘
#endif

#if (UNITY_EDITOR || UNITY_STANDALONE)
#define FILE_LOG			// 컴파일 옵션에서 조절 하게 하자.
#endif

using UnityEngine;
using System;
using System.Diagnostics;

namespace Common.Utilities
{
	using System.IO;

    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARN,
        ERROR,
        FATAL,
        PACKET, // 이렇게 넣으면 흠...
    }

	// 로그만 따로 정리해서 보여주는거 필요해서 만들었다.
	public static class Logger
	{

		// 로그 레벨
		static LogMask captureLogMask = new LogMask {
			debug = true,
			info = true,
			warning = true,
			error = true,
			packet = true
		};

		// 디렉토리 경로
		static string path = @"./logs";
		static string file = @"logger.log";
		static string fullname = @"./logs/logger.log";

		static string FileName
		{
			get { return file; }
			set
			{
				// 파일이 변경 되었을때만 변경 하자.
                if (value != file && isInitialized) {
					file = value;
					fullname = string.Format("{0}\\{1}", path, file);
					_openLogFile();
				}
			}
		}


		// 초기화 한번만 하도록 하자... static 이니까..
		static bool isInitialized = false;
		static bool isUseLogCapture = false;
		public static void Init(string _path = "logs", string _FileName = "logger.log" ,string _header = ">")
		{
			if (!isInitialized) {
				isInitialized = true;
				if (isUseLogCapture)
                    Application.logMessageReceived += CaptureLog;
			}

            switch(Application.platform) {
				case RuntimePlatform.WindowsPlayer:
				case RuntimePlatform.WindowsEditor:
				//case RuntimePlatform.OSXPlayer:
				case RuntimePlatform.OSXEditor:
                    IsEnableFileWrite = true;
					break;
			}

            // Settup
            path = _path;
            FileName = _FileName;

		}

		static bool IsEnableFileWrite = false;
		// 로그파일 생성
		static bool _openLogFile()
		{
            if (!IsEnableFileWrite)
                return false;

			// 폴더 생성 처리
			if (!string.IsNullOrEmpty(path) && !System.IO.Directory.Exists(path)) {
				System.IO.Directory.CreateDirectory(path);
			}

			// 파일 열어 보자.
			try {

				// 사용되는 중이면 닫아부러
				if (null != fs) {
					stream_writer = null;
					fs.Close();
					fs = null;
				}

				// 다시 열어서 사용 해야지
				fs = new FileStream(fullname, FileMode.Create);
				if (null != fs) {
					stream_writer = new StreamWriter(fs);
					if (null != stream_writer)
						stream_writer.AutoFlush = true;
					stream_writer.WriteLine("------------------ Log start ------------------");
					return true;
				}
			}
			catch (System.Exception ex1) {
				IsEnableFileWrite = false;
				UnityEngine.Debug.LogWarning(ex1); // 동일한 파일을 열었을때는 무시해버리자.
				//UnityEngine.Debug.LogException(ex1); // exception 처리 해버리니 주석해야 해
				if (null != fs) {
					fs.Close();
					fs = null;
				}
			}

			return false;
		}
		static FileStream fs = null;
		static StreamWriter stream_writer = null;


		// 로그 파일 첫줄을 긋기 위한것
		public static bool isEnableAssert = true;

        [Conditional("CONDITIONAL_LOG")]
        public static void Log(LogLevel level,object log)
        {
            WriteFileLog(log.ToString(), level);
            UnityEngine.Debug.Log(log);
        }


		[Conditional("CONDITIONAL_LOG")]
		public static void Info(ColorType color, object log)
		{
			WriteFileLog(log.ToString(), LogLevel.INFO);
			UnityEngine.Debug.Log(string.Format("<color={0}>{1}</color>", color.ToString(), log));
		}

		[Conditional("CONDITIONAL_LOG")]
		public static void Info(object log)
		{
			WriteFileLog(log.ToString(), LogLevel.INFO);
			UnityEngine.Debug.Log(log);
		}

		[Conditional("CONDITIONAL_LOG")]
		public static void Debug(object log)
		{
			WriteFileLog(log.ToString(), LogLevel.DEBUG);
			UnityEngine.Debug.Log(log);
		}
		[Conditional("CONDITIONAL_LOG")]
		public static void DebugFormat(ColorType color, string logString)
		{
			WriteFileLog(logString, LogLevel.DEBUG);
			UnityEngine.Debug.Log(string.Format("<color={0}>{1}</color>", color.ToString(), logString));
		}
		[Conditional("CONDITIONAL_LOG")]
		public static void DebugFormat(ColorType color, string szFormat, params object[] p)
		{
			WriteFileLog(string.Format(szFormat, p), LogLevel.DEBUG);
			UnityEngine.Debug.Log(string.Format("<color={0}>{1}</color>", color.ToString(), string.Format(szFormat, p)));
		}

		[Conditional("CONDITIONAL_LOG")]
		public static void DebugFormat(string szFormat, params object[] p)
		{
			WriteFileLog(string.Format(szFormat, p), LogLevel.DEBUG);
			UnityEngine.Debug.Log(string.Format(szFormat, p));
		}

		[Conditional("CONDITIONAL_LOG")]
		public static void InfoFormat(ColorType color, string szFormat, params object[] p)
		{
			WriteFileLog(string.Format(szFormat, p), LogLevel.INFO);
			UnityEngine.Debug.Log(string.Format("<color={0}>{1}</color>", color.ToString(), string.Format(szFormat, p)));
		}

		[Conditional("CONDITIONAL_LOG")]
		public static void InfoFormat(string szFormat, params object[] p)
		{
			WriteFileLog(string.Format(szFormat, p), LogLevel.INFO);
			UnityEngine.Debug.Log(string.Format(szFormat, p));
		}

		[Conditional("CONDITIONAL_LOG")]
		public static void LogError(string szFormat, params object[] p)
		{
			WriteFileLog(GetErrorString(StackTracer(), szFormat, p), LogLevel.ERROR);
			UnityEngine.Debug.LogError(string.Format(szFormat, p));
		}

		[Conditional("CONDITIONAL_LOG")]
		public static void LogFatal(string szFormat, params object[] p)
		{
			WriteFileLog(GetErrorString(StackTracer(),szFormat,p), LogLevel.FATAL);
			UnityEngine.Debug.LogError(string.Format(szFormat, p));
		}

		[Conditional("CONDITIONAL_LOG")]
		public static void LogWarning(string szFormat, params object[] p)
		{
			WriteFileLog(GetErrorString(StackTracer(), szFormat, p), LogLevel.WARN);
			UnityEngine.Debug.LogWarning(string.Format(szFormat, p));
		}
/*
		[Conditional("CONDITIONAL_LOG")]
		public static void LogPacket(PacketID packetID, string szFormat = null, params object[] p)
		{
			if (null != szFormat)
				WriteFileLog(string.Format("{0} - ",packetID) + string.Format(szFormat, p),LogLevel.PACKET);
			else
				WriteFileLog(string.Format("{0} - ", packetID), LogLevel.PACKET);
		}
*/
		[Conditional("CONDITIONAL_LOG")]
		public static void LogToFile(string szFormat, params object[] p)
		{
			WriteFileLog(string.Format(szFormat, p), LogLevel.INFO);
		}
		[Conditional("CONDITIONAL_LOG")]
		public static void Assert(bool condition, string szFormat, params object[] p)
		{
			// 어썰트 사용하지 않을때... 봇용
			if (!isEnableAssert)
				return;
			if (condition)
				return;
			else {
				LogError(string.Format(szFormat, p));
			}
		}

#if ( UNITY_EDITOR || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 )
		static Func<string> StackTracer = delegate() {
			// @bluegol 20140205 Unity는 \r만 찍네. 미쳤어;;;
			// IndexOf( '\n' ) doesn't work but IndexOf("\n" ) does...
			var all = UnityEngine.StackTraceUtility.ExtractStackTrace();
			var all2 = all.Substring(all.IndexOf("\n") + 1);
			return all2.Substring(all2.IndexOf("\n") + 1);
		};
#else
		static public Func<string> StackTracer = delegate() {
			var fr = new StackFrame( 2, true );
			return ( new StackTrace( fr ) ).ToString();
		};
#endif

		static System.Text.StringBuilder sb_ = new System.Text.StringBuilder();

		static string GetErrorString(string stack_trace, string more_info, params object[] p)
		{
			if (!string.IsNullOrEmpty(more_info))
				sb_.Append(" ").AppendFormat(more_info, p);
			sb_.AppendLine();
			sb_.Append(stack_trace);
			string error_string = sb_.ToString();
			sb_.Remove(0, sb_.Length);

			return error_string;
		}

		// 파일 로그 남기기
		[Conditional("FILE_LOG")]
		static void WriteFileLog(string logMsg, LogLevel level = LogLevel.DEBUG)
		{
            if (!IsEnableFileWrite)
                return;
			// 필터 거치자
			if (!captureLogMask.Filter(level))
				return;

			if (string.IsNullOrEmpty(logMsg))
				return;

            if (null == fs) {
                if (!_openLogFile())
                    return;
            }

			try
			{
				if (!IsEnableFileWrite || null == stream_writer)
					return;

				// NowDate.ToString("yyyy-MM-dd HH:mm:ss") + ":" + NowDate.Millisecond.ToString("000");
				stream_writer.WriteLine(string.Format("{0} {1} - {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), level, logMsg));
				// 임시로 시간을 뺏다..
				//stream_writer.WriteLine(string.Format("{0} {1} - {2}", "\t", level, logMsg));
				//writer.Close();
			}
			catch (System.Exception ex1) {
				UnityEngine.Debug.LogException(ex1); // exception 처리 해버리니 주석해야 해
				IsEnableFileWrite = false;
			}

		}

		/// <summary>
		/// 로그 Mask 처리용
		/// </summary>
		[Serializable]
		public class LogMask
		{
			public bool debug;
			public bool info;
			public bool warning;
			public bool error;
			public bool packet;

			public LogMask() { }

			public LogMask(bool debug, bool info, bool warning, bool error,bool packet)
			{
				this.debug = debug;
				this.info = info;
				this.warning = warning;
				this.error = error;
				this.packet = packet;
			}

			public bool Filter(LogType type)
			{
				switch (type) {
				case LogType.Log:
					return info;
				case LogType.Warning:
					return warning;
				case LogType.Assert:
				case LogType.Exception:
				case LogType.Error:
					return error;
				}

				return true;
			}

			// 필터
			public bool Filter(LogLevel type)
			{
				switch (type) {
					case LogLevel.DEBUG:
						return debug;
					case LogLevel.INFO:
						return info;
					case LogLevel.WARN:
						return warning;
					case LogLevel.ERROR:
						return error;
					case LogLevel.FATAL:
						return error;
					case LogLevel.PACKET:
						return packet;
				}

				return true;
			}
		}

		static void CaptureLog(string log, string stacktrace, LogType type)
		{
			if (!captureLogMask.Filter(type))
				return;

			switch (type) {
			case LogType.Log:
				//WriteFileLog(GetErrorString(stacktrace, log), ErrorLevel.INFO);
				break;
			case LogType.Warning:
				//WriteFileLog(GetErrorString(stacktrace, log), ErrorLevel.WARN);
				break;
			case LogType.Assert:
			case LogType.Error:
				//WriteFileLog(GetErrorString(stacktrace, log), ErrorLevel.ERROR);
				break;
			case LogType.Exception:
				WriteFileLog(GetErrorString(stacktrace, log), LogLevel.FATAL);
				break;
			}
		}
	}
}
