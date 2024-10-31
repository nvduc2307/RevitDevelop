namespace RevitDevelop.Tools.Rebars.SettingRuleRebarStandards.exceptions
{
    public class SettingRuleRebarGradesEmptyException : Exception
    {
        public const string MESSAGE_ERROR = "Grades is not empty, please create new grade.";
        public SettingRuleRebarGradesEmptyException() : base(MESSAGE_ERROR) { }
    }
}
