using System;

namespace BeatThat.Defines
{
    /// <summary>
    /// Adds a symbol to the list of precompiler defines 
    /// that show up with option to enable/disable in Window/BeatThat/Edit Defines.
    /// 
    /// You can also specify an array of symbols. 
    /// These will be displayed as (radio) options.
    /// 
    /// The motivating example is a group of defines that change which server 
    /// e.g. ENV_DEV, ENV_DEV_LOCAL, ENV_PRODUCTION
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class EditDefineAttribute : Attribute
    {
        public EditDefineAttribute(string symbol, string desc) 
            : this(new string[] { symbol }, desc) {}

        public EditDefineAttribute(string[] symbols, string desc)
        {
            this.symbols = symbols;
            this.desc = desc;
        }

        /// <summary>
        /// The precompiler symbol/define or, if multiple, the first
        /// </summary>
        public string symbol 
        {
            get {
                return this.symbols != null 
                           && this.symbols.Length > 0 ?
                           this.symbols[0] : null;
            }
        }

        /// <summary>
        /// Description of what defining this symbol does
        /// </summary>
        /// <value>The desc.</value>
        public string desc { get; private set; }

        public string[] symbols { get; private set; }

    }
}

