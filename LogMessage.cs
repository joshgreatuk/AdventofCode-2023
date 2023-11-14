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

        public string ToString(int severityPadSource, int senderPadSource=20)
        {
            return $"{GetPaddedString(Severity.ToString(), severityPadSource)} : {GetPaddedString(Sender, senderPadSource)} : {Message}{(Exception != null ? $"\n{Exception}" : "")}";
        }

        public string GetPaddedString(string source, int targetLength)
        {
            if (source.Length > targetLength)
            {
                return String.Join("", source.Take(targetLength));
            }
            return source.PadRight(targetLength);
        }
    }
}