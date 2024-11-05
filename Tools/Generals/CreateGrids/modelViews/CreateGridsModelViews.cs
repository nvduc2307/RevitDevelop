using HcBimUtils;
using HcBimUtils.DocumentUtils;
using RevitDevelop.Tools.Generals.CreateGrids.exceptions;
using RevitDevelop.Tools.Generals.CreateGrids.models;
using RevitDevelop.Tools.Generals.CreateGrids.views;
using System.Windows;
using Utils.Messages;
using Utils.NumberUtils;

namespace RevitDevelop.Tools.Generals.CreateGrids.modelViews
{
    public partial class CreateGridsModelViews : ObservableObject
    {
        public bool IsComplete { get; set; }
        public ElementInstances ElementInstances { get; set; }
        public CreateGridsModel CreateGridsModel { get; set; }
        public CreateGridView MainView { get; set; }
        public CreateGridsModelViews()
        {
            ElementInstances = new ElementInstances();
            CreateGridsModel = new CreateGridsModel(ElementInstances);
            MainView = new CreateGridView() { DataContext = this };
        }
        [RelayCommand]
        private void CreateGridOK(object obj)
        {
            try
            {
                var gridXNames = CreateGridsModel.GridXNames.Split(' ')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
                var gridYNames = CreateGridsModel.GridYNames.Split(' ')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
                var gridXValues = CreateGridsModel.GridXValues.Split(' ')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
                var gridYValues = CreateGridsModel.GridYValues.Split(' ')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
                if (gridXNames.Count() <= 1) throw new EXCEPTION_GRID_X_NAME_LESS_THAN_MIN();
                if (gridYNames.Count() <= 1) throw new EXCEPTION_GRID_Y_NAME_LESS_THAN_MIN();
                if (gridXValues.Count() <= 1) throw new EXCEPTION_GRID_X_VALUE_LESS_THAN_MIN();
                if (gridYValues.Count() <= 1) throw new EXCEPTION_GRID_Y_VALUE_LESS_THAN_MIN();
                if (gridXNames.Count() != gridXValues.Count()) throw new EXCEPTION_GRID_X_VALUE_NOT_EQUAL_NAMES();
                if (gridYNames.Count() != gridYValues.Count()) throw new EXCEPTION_GRID_Y_VALUE_NOT_EQUAL_NAMES();
                var checkNumberX = gridXValues.CheckNumberInStrings();
                var checkNumberY = gridYValues.CheckNumberInStrings();
                if (checkNumberX) throw new EXCEPTION_NUMBER_IS_INVALID();
                if (checkNumberY) throw new EXCEPTION_NUMBER_IS_INVALID();
                var cx = 0;
                var cy = 0;
                var axR = 0.0;
                var ayR = 0.0;
                var extent = 1000.MmToFoot();
                var coordinate = new XYZ();
                var lengthGX = gridXValues.Select(x => double.Parse(x, System.Globalization.NumberStyles.Number)).Sum();
                var lengthGY = gridYValues.Select(x => double.Parse(x, System.Globalization.NumberStyles.Number)).Sum();

                using (var ts = new Transaction(AC.Document, "name transaction"))
                {
                    ts.Start();
                    //--------
                    if (ElementInstances.Grids.Count != 0)
                        AC.Document.Delete(ElementInstances.Grids.Aggregate((a, b) => a.Concat(b).ToList()).Select(x => x.Id).ToList());

                    foreach (var gx in gridXNames)
                    {
                        axR += double.Parse(gridXValues[cx], System.Globalization.NumberStyles.Number);
                        var p1 = coordinate + XYZ.BasisX * axR.MmToFoot() - XYZ.BasisY * extent;
                        var p2 = coordinate + XYZ.BasisX * axR.MmToFoot() + XYZ.BasisY * lengthGY.MmToFoot() + XYZ.BasisY * extent;
                        var gr = Grid.Create(AC.Document, Line.CreateBound(p1, p2));
                        gr.Name = gx;
                        cx++;
                    }
                    foreach (var gy in gridYNames)
                    {
                        ayR += double.Parse(gridYValues[cy], System.Globalization.NumberStyles.Number);
                        var p1 = coordinate + XYZ.BasisY * ayR.MmToFoot() - XYZ.BasisX * extent;
                        var p2 = coordinate + XYZ.BasisY * ayR.MmToFoot() + XYZ.BasisX * lengthGX.MmToFoot() + XYZ.BasisX * extent;
                        var gr = Grid.Create(AC.Document, Line.CreateBound(p1, p2));
                        gr.Name = gy;
                        cy++;
                    }
                    //--------
                    ts.Commit();
                }
                IsComplete = true;
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
                IsComplete = false;
            }
            if (obj is Window window) window.Close();
        }
        [RelayCommand]
        private void CreateGridCancel(object obj)
        {
            if (obj is Window window) window.Close();
        }
    }
}
