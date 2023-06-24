using System;
using System.Diagnostics;

namespace OmniCache
{
    public interface IDebugLogger
    {
        void Log(string text);
    }

    public enum DebugLogSource
    {
        None,
        LocalCache,
        Redis,
        Database
    }

    public class DebugLogger
    {
        protected static IDebugLogger _Logger { get; set; }
        public static List<string> Log = new List<string>();
        public static bool KeepData { get; set; }

        protected static Dictionary<DebugLogSource, bool> DebugOn = new Dictionary<DebugLogSource, bool>();

        private DebugLogger()
        {
        }

        public static void ClearLogData()
        {
            Log.Clear();
        }

        public static void SetLogger(IDebugLogger logger)
        {
            _Logger = logger;
        }

        public static void WriteLine(string message)
        {
            if (_Logger != null)
            {
                _Logger.Log(message);
            }

            if (KeepData)
            {
                Log.Add(message);
            }
        }

        public static void SetDebugOn(DebugLogSource source)
        {
            DebugOn[source] = true;
        }

        public static void Debug(DebugLogSource source, string text, List<string> keys, object obj)
        {
            Debug(source, text, $"[{string.Join(",", keys)}]", obj);
        }

        public static void Debug(DebugLogSource source, string text, string key, object obj)
        {

            if (!DebugOn.ContainsKey(source))
            {
                return;
            }
            WriteLine($"{source.ToString()}| {text}: {key} -> {(obj == null ? "NULL" : ToString(obj))}");
        }

        public static void Debug(DebugLogSource source, string text, List<string> keys)
        {
            Debug(source, text, $"[{string.Join(",", keys)}]");
        }

        public static void Debug(DebugLogSource source, string text, string key)
        {
            Debug(source, $"{text} -> {key}");
        }

        public static void Debug(DebugLogSource source, string text)
        {
            if (!DebugOn.ContainsKey(source))
            {
                return;
            }
            WriteLine($"{source.ToString()}| {text}");
        }

        private static string ToString(object obj)
        {
            if (obj == null)
            {
                return "NULL";
            }
            if (obj.GetType() == typeof(List<string>))
            {
                List<string> list = (List<string>)obj;
                string text = string.Join(",", list);
                return "[" + text + "]";
            }

            return obj.ToString();
        }



    }
}

