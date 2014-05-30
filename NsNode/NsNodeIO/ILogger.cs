using System;
using System.Collections.Generic;
using System.Text;
using NsNodes;

namespace NsNodes
{
	public delegate void LoggerEventHandler(object sender, EventArgs<string> msg);
	public delegate void LoggerPriorityEventHandler(object sender, EventArgs<string, LogPriority> msg);

	//public class LoggerEventArgs
	//{
	//     public LoggerEventArgs(string msg, LogPriority p)
	//     {
	//          m_msg = msg;
	//          m_p = p;
	//     }
	//     public readonly string m_msg;
	//     public readonly LogPriority m_p;
	//}
	public enum LogPriority
	{
		Message,
		Warning,
		Error,
		Debug
	};
	public interface ILogger
	{
		void OnLog(object sender, EventArgs<string, LogPriority> e);
		//void OnLog(object sender, EventArgs<string> e);
		//void OnWarning(object sender, EventArgs<string> e);
		//void OnError(object sender, EventArgs<string> e);
		//void OnDebug(object sender, EventArgs<string> e);
		void OnClear(object sender, EventArgs<string> e);
		string[] Messages { get; }
		Entry[] Entries { get; }
		event EventHandler LogChanged;

	}
}
