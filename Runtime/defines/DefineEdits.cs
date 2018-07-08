using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BeatThat.Pools;
using UnityEngine;

namespace BeatThat.Defines
{
    public class DefineEdits
    {
        public DefineEdits Clear()
        {
            m_definesById.Clear();
            m_symbol2DefineId.Clear();
            return this;
        }

        public static string PolishSymbol(string s)
        {
            return s != null ? ILLEGAL_SYMBOL_CHARS.Replace(s, "") : null;
        }

        public DefineEdits AddDefinedSymbols(string definedSymbols, char sep = ';')
        {
            Merge(
                definedSymbols,
                m_definesById,
                m_symbol2DefineId,
                sep
            );
            return this;
        }

        public void Get(List<DefineEditData> defines, bool sort = true)
        {
            defines.AddRange(m_definesById.Values);
            if (sort)
            {
                defines.Sort((x, y) => string.Compare(x.id, y.id, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public void AddOption(string[] symbols, string desc, bool enabled = false)
        {
            AddOption(
                m_definesById, 
                m_symbol2DefineId,
                SymbolsToId(symbols), 
                symbols, 
                desc
            );
        }

        public void Set(string symbol, bool willEnable = true)
        {
            Set(
                m_definesById,
                m_symbol2DefineId,
                symbol, 
                willEnable
            );
        }

        public bool Contains(string symbol)
        {
            var id = Symbol2Id(symbol);
            return m_definesById.Keys.Any(s => s.Equals(id, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool AnyEdits()
        {
            return m_definesById.Values.Any(s => s.GetEditType() != EditType.NONE);
        }

        public string ToSymbolString(char sep = ';', bool sort = true)
        {
            using(var symbols = ListPool<DefineEditData>.Get()) 
            using(var sb = PooledStringBuilder.Get()) {
                Get(symbols, sort);
                return symbols.Aggregate(sb.stringBuilder, (acc, cur) =>
                {
                    if (!cur.willDefine)
                    {
                        return acc;
                    }
                    if (acc.Length > 0 && acc[acc.Length - 1] != sep)
                    {
                        acc.Append(sep);
                    }
                    acc.Append(cur.symbol);
                    return acc;
                }).ToString();
            }
        }

        /// <summary>
        /// Convert a group of symbols to an id string.
        /// Could do this with a hash with low probability of collisions
        /// but for now just convert the symbols into a cannonical string.
        /// </summary>
        private string SymbolsToId(string[] symbols)
        {
            using(var sorted = ArrayPool<string>.GetCopy(symbols))
            using (var sb = PooledStringBuilder.Get())
            {
                Array.Sort(sorted.array, StringComparer.InvariantCultureIgnoreCase);
                return string.Join(";", sorted.array);
            }
        }

        private string Symbol2Id(string symbol)
        {
            string id;
            return m_symbol2DefineId.TryGetValue(symbol, out id) ? id : symbol;
        }

        private static readonly Regex ILLEGAL_SYMBOL_CHARS = new Regex("[^A-Za-z0-9_\\-]");


        override public string ToString()
        {
            return ToSymbolString();
        }

        private void Merge(
            string definedSymbols, 
            Dictionary<string, DefineEditData> definesById, 
            Dictionary<string, string> symbol2DefineId,
            char sep = ';')
        {
            definedSymbols.Split(sep).Aggregate(definesById, (dDict, curSymbol) =>
            {
                try
                {
                    Set(dDict, symbol2DefineId, curSymbol, willEnable: true, isDefined: true);
                    return dDict;
                }
                catch (Exception e)
                {
#if UNITY_EDITOR || DEBUG_UNSTRIP
                    UnityEngine.Debug.LogError("Failed to set symbol '" 
                                   + curSymbol + " with error: " + e.Message);
#endif
                    return dDict;
                }
            });
        }

        private void Set(
            Dictionary<string, DefineEditData> definesById,
            Dictionary<string, string> symbol2DefineId,
            string symbol,
            bool willEnable = true,
            bool isDefined = false)
        {
            symbol = PolishSymbol(symbol);
            if (string.IsNullOrEmpty(symbol))
            {
                return;
            }

            var id = Symbol2Id(symbol);

            DefineEditData defineEdit;
            if (!definesById.TryGetValue(id, out defineEdit))
            {
                defineEdit = AddOption(definesById, 
                          symbol2DefineId, 
                          id,
                          new string[] { symbol }, 
                          desc: ""
                         );
            }

            definesById[id] = defineEdit.Select(symbol, (isDefined)).WillDefine(willEnable);;
        }

        private DefineEditData AddOption(
            Dictionary<string, DefineEditData> definesById,
            Dictionary<string, string> symbol2DefineId,
            string id, 
            string[] symbols, 
            string desc)
        {
            DefineEditData defineEdit;
            if (!definesById.TryGetValue(id, out defineEdit))
            {
                defineEdit = new DefineEditData
                {
                    id = id,
                    symbols = symbols,
                    definedSymbolIndex = -1
                };
            }

            defineEdit.desc = !string.IsNullOrEmpty(defineEdit.desc)
                ? defineEdit.desc
                : desc ?? "";
            


            definesById[id] = defineEdit;

            foreach (var s in symbols)
            {
                symbol2DefineId[s] = id;
            }

            return defineEdit;
        }

        private Dictionary<string, DefineEditData> m_definesById = new Dictionary<string, DefineEditData>();
        private Dictionary<string, string> m_symbol2DefineId = new Dictionary<string, string>();
    }
}
