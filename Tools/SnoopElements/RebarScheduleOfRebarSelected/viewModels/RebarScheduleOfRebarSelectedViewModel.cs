using Autodesk.Revit.DB.Structure;
using RIMT.RebarSchedules.RebarScheduleOfRebarSelected.views;
using HcBimUtils.DocumentUtils;
using System.Windows;
using RevitDevelop.Utils.RevElements.RevRebars;

namespace RevitDevelop.Tools.SnoopElements.RebarScheduleOfRebarSelected.viewModels
{
    public partial class RebarScheduleOfRebarSelectedViewModel : ObservableObject
    {
        public RebarScheduleOfRebarSelectedView MainView { get; set; }
        public string Content { get; set; }
        public RebarScheduleOfRebarSelectedViewModel(List<Rebar> rebars)
        {
            Content = GetConten(rebars);
            MainView = new RebarScheduleOfRebarSelectedView() { DataContext = this };
        }
        private string GetConten(List<Rebar> rebars)
        {
            var content = "";
            try
            {
                var rebarT = rebars
                .Select(x => new RevRebar(x))
                .ToList();
                var rebarGs = rebarT
                    .GroupBy(x => x.Diameter)
                    .OrderBy(x => x.FirstOrDefault().Diameter)
                    .Select(x => x.ToList());
                foreach (var rg in rebarGs)
                {
                    var rebarbarType = AC.Document.GetElement((AC.Document.GetElement(new ElementId(rg.FirstOrDefault().ElementId)) as Rebar).GetTypeId()) as RebarBarType;
                    content += $"{rebarbarType.Name}: {Math.Round(rg.Sum(x => x.TotalWeight / 1000), 3)} Tons\n\r";
                }
                content += $"Total: {Math.Round(rebarT.Sum(x => x.TotalWeight / 1000), 3)} Tons";
            }
            catch (Exception)
            {
            }
            return content;
        }
        [RelayCommand]
        private void Copy()
        {
            Clipboard.SetText(Content);
            MainView.Close();
        }
        [RelayCommand]
        private void OK()
        {
            MainView.Close();
        }
    }
}
