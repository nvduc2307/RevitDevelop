namespace Utils.SkipWarnings
{
    public class SkipNotificationRevit
    {
        public static CopyPasteOptions GetOption()
        {
            var copyOpt = new CopyPasteOptions();
            copyOpt.SetDuplicateTypeNamesHandler(new CustomCopyHandler());
            return copyOpt;
        }
    }
    public class CustomCopyHandler : IDuplicateTypeNamesHandler
    {
        public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
        {
            return DuplicateTypeAction.UseDestinationTypes;
        }
    }
}
