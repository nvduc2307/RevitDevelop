using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;
using UIFramework;

namespace RevitDevelop.Utils.RevUtils
{
    public static class RevUtil
    {
        public static Button GetButton(this string nameButton)
        {
            Button result = null;
            try
            {
                var mainWindow = MainWindow.getMainWnd();
                var dcm = mainWindow.FindChildrenByType<StackPanel>();
                var btns = mainWindow.FindChildrenByType<Button>();
                result = btns
                    .FirstOrDefault(b => b.Name == "mRibbonPanelButton" && AutomationProperties.GetName(b) == nameButton);
            }
            catch (Exception)
            {
            }
            return result;
        }
        public static List<T> FindChildrenByType<T>(this DependencyObject parent) where T : DependencyObject
        {
            var result = new List<T>();
            if (parent == null) return result;
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t) result.Add(t);
                result.AddRange(FindChildrenByType<T>(child));
            }
            return result;
        }
    }
}
