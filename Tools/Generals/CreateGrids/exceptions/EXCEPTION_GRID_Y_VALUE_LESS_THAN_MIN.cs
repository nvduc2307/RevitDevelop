namespace RevitDevelop.Tools.Generals.CreateGrids.exceptions
{
    public class EXCEPTION_GRID_Y_VALUE_LESS_THAN_MIN : Exception
    {
        public const string MESSAGE_ERROR = "EXCEPTION_GRID_Y_VALUE_LESS_THAN_MIN";
        public EXCEPTION_GRID_Y_VALUE_LESS_THAN_MIN() : base(MESSAGE_ERROR) { }
    }
}
