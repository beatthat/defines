using System;
using UnityEngine;

namespace BeatThat.Defines
{
    public enum EditType
    {
        NONE = 0,
        WILL_ADD = 1,
        WILL_REMOVE = 2,
        WILL_CHANGE_SELECTION = 3
    }

    [Serializable]
    public struct DefineEditData
    {
        public string id;
        public string[] symbols;
        public int willDefineSymbolIndex;
        public int definedSymbolIndex;
        public string desc;
        public bool willDefine;
        public bool showDetails;


        public bool isDefined { 
            get { return this.definedSymbolIndex >= 0; }
        }

        public string symbol { 
            get {
                return this.symbols != null
                           && this.symbols.Length > this.willDefineSymbolIndex
                           && this.willDefineSymbolIndex >= 0 ?
                           this.symbols[this.willDefineSymbolIndex] : null;
            } 
        }

        public int symbolCount {
            get {
                return this.symbols != null ? this.symbols.Length : 0;
            }
        }

        public EditType GetEditType()
        {
            if(this.isDefined == this.willDefine) {
                return (this.willDefine && this.definedSymbolIndex != this.willDefineSymbolIndex) ?
                    EditType.WILL_CHANGE_SELECTION : EditType.NONE; 
            }

            if(!this.willDefine) {
                return EditType.WILL_REMOVE;
            }

            return EditType.WILL_ADD;
        }

        public DefineEditData Select(string symbol, bool isDefined = false)
        {
            var d = this;

            d.willDefineSymbolIndex = this.symbolCount > 1 ?
                Mathf.Clamp(Array.FindIndex(this.symbols, s => s == symbol), 0, this.symbolCount - 1)
                : 0;

            if(isDefined) {
                d.definedSymbolIndex = d.willDefineSymbolIndex;
            }
            
            return d;
        }

        public DefineEditData ShowDetails(bool show)
        {
            var d = this;
            d.showDetails = show;
            return d;
        }

        public DefineEditData WillDefine(bool wd)
        {
            var d = this;
            d.willDefine = wd;
            return d;
        }
    }

}
