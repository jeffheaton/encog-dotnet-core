using System;
using System.Collections.Generic;

namespace EncogCmd
{
    public class ParseCommand
    {
        private readonly IList<String> _args;
        private readonly String _command;
        private readonly IDictionary<String, String> _settings;

        public ParseCommand(String[] args)
        {
            _args = new List<String>();
            _settings = new Dictionary<string, string>();

            foreach (string t1 in args)
            {
                String t = t1.Trim();

                if (t[0] == '-')
                {
                    int idx = t.IndexOf(':');

                    if (idx == -1)
                    {
                        String name = t.Substring(0, idx).Trim().ToLower();
                        String value = t.Substring(idx + 1).Trim();
                        _settings[name] = value;
                    }
                    else
                    {
                        String name = t.Substring(0, idx).Trim();
                        _settings[name] = "t";
                    }
                }
                else
                {
                    if (_command == null)
                    {
                        _command = t.ToLower();
                    }
                    else
                    {
                        _args.Add(t);
                    }
                }
            }
        }

        public IList<String> Args
        {
            get { return _args; }
        }

        public IDictionary<String, String> Settings
        {
            get { return _settings; }
        }

        public String Command
        {
            get { return _command; }
        }

        public bool PromptBoolean(string name, bool d)
        {
            String n = name.ToLower();
            if (_settings.ContainsKey(n))
            {
                return char.ToLower(_settings[n][0]) == 't';
            }

            Console.Write("Enter value for [" + name + "] (default="+d+"): ");
            String str = Console.ReadLine().Trim().ToLower();
            if (str.Length == 0)
                return d;
            else
                return str[0] == 't';
        }

        internal int PromptInteger(string name, int d)
        {
            String n = name.ToLower();
            if (_settings.ContainsKey(n))
            {
                return int.Parse(_settings[n]);
            }

            Console.Write("Enter value for [" + name + "] (default="+d+"): ");
            String str = Console.ReadLine().Trim().ToLower();
            if (str.Length == 0)
                return d;
            else
                return int.Parse(str);
        }

        public string PromptString(string name, String d)
        {
            String n = name.ToLower();
            if (_settings.ContainsKey(n))
            {
                return _settings[n];
            }

            Console.Write("Enter value for [" + name + "] (default=" + d + "): ");
            String result = Console.ReadLine().Trim().ToLower();

            if (result.Length == 0)
                result = d;

            return result;
        }

        internal double PromptDouble(string name, double d)
        {
            String n = name.ToLower();
            if (_settings.ContainsKey(n))
            {
                return int.Parse(_settings[n]);
            }

            Console.Write("Enter value for [" + name + "] (default=" + d + "): ");
            String str = Console.ReadLine().Trim().ToLower();
            if (str.Length == 0)
                return d;
            else
                return double.Parse(str);
        }
    }
}