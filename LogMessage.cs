using System;

namespace AOC23
{
    public class LogMessage
    {
        public LogSeverity Severity {get; set;}
        public string Sender {get; set;}
        public string Message {get; set;}
        public Exception? Exception {get; set;}

        public LogMessage(LogSeverity severity, string sender, string message, Exception? exception=null)
        {
            Severity = severity;
            Sender = sender;
            Message = message;
            Exception = exception;
        }

        public string ToString(int padSource=20)
        {
            return $"{Severity} : {Sender} : {Message}{(Exception != null ? $"\n{Exception}" : "")}";
        }
    }
}