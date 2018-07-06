namespace BeatThat.Defines
{
    public enum EditType
    {
        NONE = 0,
        WILL_ADD = 1,
        WILL_REMOVE = 2
    }

    public struct DefineEditData
    {
        public string name;
        public string desc;
        public bool enabled;
        public bool willEnable;
        public bool showDetails;

        public EditType GetEditType()
        {
            return this.enabled == this.willEnable ?
                       EditType.NONE : this.willEnable ?
                       EditType.WILL_ADD : EditType.WILL_REMOVE;
        }

        public DefineEditData ShowDetails(bool show)
        {
            return new DefineEditData
            {
                name = this.name,
                desc = this.desc,
                enabled = this.enabled,
                willEnable = this.enabled,
                showDetails = show
            };
        }

        public DefineEditData WillEnable(bool e)
        {
            return new DefineEditData
            {
                name = this.name,
                desc = this.desc,
                enabled = this.enabled,
                willEnable = e,
                showDetails = this.showDetails
            };
        }
    }

}
