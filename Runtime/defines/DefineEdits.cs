using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BeatThat.Pools;

namespace BeatThat.Defines
{
    public class DefineEdits
    {
        public DefineEdits Clear()
        {
            m_defines.Clear();
            return this;
        }


        public DefineEdits AddSymbols(string defines, char sep = ';')
        {
            Merge(defines, m_defines, sep);
            return this;
        }

        public void Get(List<DefineEditData> defines, bool sort = true)
        {
            defines.AddRange(m_defines.Values);
            if (sort)
            {
                defines.Sort((x, y) => string.Compare(x.name.ToLower(), y.name.ToLower(), System.StringComparison.Ordinal));
            }
        }

        public void AddOption(string name, string desc)
        {
            name = PolishSymbol(name);

            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            DefineEditData defineEdit;
            if (!m_defines.TryGetValue(name, out defineEdit))
            {
                defineEdit = new DefineEditData
                {
                    name = name,
                    desc = desc
                };
            }

            defineEdit.desc = desc; // TODO: handle multiple descs

            m_defines[name] = defineEdit;
        }

        public void Set(string name, bool willEnable = true)
        {
            name = PolishSymbol(name);
            if(string.IsNullOrEmpty(name)) {
                return;
            }

            DefineEditData defineEdit;
            if(!m_defines.TryGetValue(name, out defineEdit)) {
                defineEdit = new DefineEditData
                {
                    name = name
                };
            }
            defineEdit.willEnable = willEnable;
            m_defines[name] = defineEdit;
        }

        public bool Contains(string symbol)
        {
            return m_defines.Keys.Any(s => s.Equals(symbol, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool AnyEdits()
        {
            return m_defines.Values.Any(s => s.enabled != s.willEnable);
        }

        public string ToSymbolString(char sep = ';', bool sort = true)
        {
            using(var symbols = ListPool<DefineEditData>.Get()) 
            using(var sb = PooledStringBuilder.Get()) {
                Get(symbols, sort);
                return symbols.Aggregate(sb.stringBuilder, (acc, cur) =>
                {
                    if (!cur.willEnable)
                    {
                        return acc;
                    }
                    if (acc.Length > 0 && acc[acc.Length - 1] != sep)
                    {
                        acc.Append(sep);
                    }
                    acc.Append(cur.name);
                    return acc;
                }).ToString();
            }
        }

        private static readonly Regex ILLEGAL_SYMBOL_CHARS = new Regex("[^A-Za-z0-9_\\-]");
        public static string PolishSymbol(string s)
        {
            return s != null ? ILLEGAL_SYMBOL_CHARS.Replace(s, "") : null;
        }


        override public string ToString()
        {
            return ToSymbolString();
        }

        public static void Merge(string defines, Dictionary<string, DefineEditData> definesDict, char sep = ';')
        {
            defines.Split(sep).Aggregate(definesDict, (acc, cur) =>
            {
                if (string.IsNullOrEmpty((cur = PolishSymbol(cur))))
                {
                    return acc;
                }

                DefineEditData curVal;
                if (acc.TryGetValue(cur, out curVal))
                {
                    curVal.enabled = true;
                }
                else
                {
                    curVal = new DefineEditData
                    {
                        name = cur,
                        enabled = true,
                        willEnable = true
                    };
                }

                acc[cur] = curVal;

                return acc;
            });
        }

        private Dictionary<string, DefineEditData> m_defines = new Dictionary<string, DefineEditData>();
    }
}
