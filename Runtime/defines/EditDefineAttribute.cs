using System;

namespace BeatThat.Defines
{
    /// <summary>
    /// Adds a symbol to the list of precompiler defines 
    /// that show up with option to enable/disable in Window/BeatThat/Edit Defines
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class EditDefineAttribute : Attribute
    {
        public EditDefineAttribute(string symbol, string desc)
        {
            this.symbol = symbol;
            this.desc = desc;
        }

        /// <summary>
        /// the precompiler symbol/define
        /// </summary>
        public string symbol { get; private set; }

        /// <summary>
        /// Description of what defining this symbol does
        /// </summary>
        /// <value>The desc.</value>
        public string desc { get; private set; }

    }
}

