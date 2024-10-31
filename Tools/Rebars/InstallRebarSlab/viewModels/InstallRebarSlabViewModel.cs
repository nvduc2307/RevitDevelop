using HcBimUtils.DocumentUtils;
using RevitDevelop.Tools.InstallRebarSlab.views;
using RevitDevelop.Tools.Rebars.InstallRebarSlab.models;
using RevitDevelop.Utils.NumberingRevitElements;
using RIMT.SettingRuleRebarStandards.viewModels;
using System.Windows.Controls;
using Utils.canvass;
using Utils.SkipWarnings;

namespace RevitDevelop.InstallRebarSlab.viewModels
{
    public partial class InstallRebarSlabViewModel : ObservableObject
    {
        public InstallRebarSlabViewMain MainView { get; private set; }
        public CanvasPageBase CanvasPageBase { get; private set; }
        public CanvasPageBase CanvasPageBaseCoordinate { get; private set; }
        public MSLabElementIntance MSLabElementIntance { get; private set; }
        public InstallRebarSlabModel InstallRebarSlabModel { get; private set; }
        public InstallRebarSlabViewModel(MSLabElementIntance mSLabElementIntance, List<MSlab> mSlab)
        {
            MSLabElementIntance = mSLabElementIntance;
            MSLabElementIntance.MSlabRebarLayerUiSelectedEventAction = () =>
            {
                MSLabElementIntance.MSlabRebarLayerUiSelectedEventActionF(
                    InstallRebarSlabModel.MSlabs,
                    MSLabElementIntance.MSlabRebarLayerUiSelected.MSlabRebarLayerType,
                    CanvasPageBase,
                    InstallRebarSlabModel.MSlabCenter,
                    InstallRebarSlabModel);
            };
            InstallRebarSlabModel = new InstallRebarSlabModel(mSLabElementIntance, mSlab);
            MainView = new InstallRebarSlabViewMain() { DataContext = this };
            MainView.Loaded += MainView_Loaded;
        }

        [RelayCommand]
        private void SettingAnchor()
        {
            try
            {
                var vm = new SettingLapLengthAndDevelopRebarRuleViewModel();
                vm.MainView.ShowDialog();
            }
            catch (Exception)
            {
            }
        }
        [RelayCommand]
        private void CreateRebarSlab()
        {
            MainView.Close();
            using (var ts = new Transaction(AC.Document, "name transaction"))
            {
                ts.SkipAllWarnings();
                ts.Start();
                //--------
                foreach (var mslab in InstallRebarSlabModel.MSlabs)
                {
                    foreach (var r in mslab.MSlabRebar.TopX.MSlabRebars)
                    {
                        r.CreateRebar();
                        mslab.MSlabRebar.AllRebars.Add(r.RebarNumbering);
                    }
                    foreach (var r in mslab.MSlabRebar.TopY.MSlabRebars)
                    {
                        r.CreateRebar();
                        mslab.MSlabRebar.AllRebars.Add(r.RebarNumbering);
                    }
                    foreach (var r in mslab.MSlabRebar.BotX.MSlabRebars)
                    {
                        r.CreateRebar();
                        mslab.MSlabRebar.AllRebars.Add(r.RebarNumbering);
                    }
                    foreach (var r in mslab.MSlabRebar.BotY.MSlabRebars)
                    {
                        r.CreateRebar();
                        mslab.MSlabRebar.AllRebars.Add(r.RebarNumbering);
                    }
                    //set solid
                    //numbering rebar
                    NumberingRevitRebar.Numbering(mslab.MSlabRebar.AllRebars, MSLabElementIntance.OptionNumberingTypeRebars, MSLabElementIntance.RebarSlabSchemaInfo);
                    //create assembly rebar
                    var rebarAssebly = AssemblyInstance.Create(
                        AC.Document,
                        mslab.MSlabRebar.AllRebars.Select(x => new ElementId(int.Parse(x.ElementId.ToString()))).ToList(),
                        Category.GetCategory(AC.Document, BuiltInCategory.OST_Rebar).Id);
                }
                //--------
                ts.Commit();
            }
        }

        [RelayCommand]
        private void PrevAngle()
        {
            InstallRebarSlabModel.MSlabCoordinateAxis.Angle -= 15;
        }
        [RelayCommand]
        private void NextAngle()
        {
            InstallRebarSlabModel.MSlabCoordinateAxis.Angle += 15;
        }
        private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var canvas = MainView.FindName("CanvasSlabPlan") as Canvas;
            CanvasPageBase = new CanvasPageBase(canvas);
            var rbox = InstallRebarSlabModel.MSlabBox;
            var rBoxCross = rbox.Min.DistanceTo(rbox.Max);
            CanvasPageBase.Scale = CanvasPageBase.RatioScale * CanvasPageBase.DistanceCrossScreen / rBoxCross;
            DrawOnUI(CanvasPageBase);
            InstallRebarSlabModel.MSlabCoordinateAxis.AngleChangeEvent = () =>
            {
                MSlabCoordinateAxis.AngleChangeEventAction(
                    CanvasPageBase,
                    InstallRebarSlabModel.MSlabCoordinateAxis.VtXBase,
                    InstallRebarSlabModel.MSlabCoordinateAxis.VtYBase,
                    InstallRebarSlabModel.MSlabCoordinateAxis.Angle,
                    InstallRebarSlabModel.MSlabCenter,
                    out XYZ vtX,
                    out XYZ vtY);
                InstallRebarSlabModel.MSlabCoordinateAxis.VtX = vtX;
                InstallRebarSlabModel.MSlabCoordinateAxis.VtY = vtY;
                foreach (var msl in InstallRebarSlabModel.MSlabs)
                {
                    msl.MSlabRebar.VTX = vtX;
                    msl.MSlabRebar.VTY = vtY;

                    msl.MSlabRebar.TopX.NormalEventAction = () =>
                    {
                        MSlabRebarLayer.NormalEventActionF(
                            msl.MSlabRebar.TopX,
                            InstallRebarSlabModel.MSlabs,
                            CanvasPageBase,
                            InstallRebarSlabModel.MSlabCenter,
                            InstallRebarSlabModel);
                    };
                    msl.MSlabRebar.TopY.NormalEventAction = () =>
                    {
                        MSlabRebarLayer.NormalEventActionF(
                            msl.MSlabRebar.TopY,
                            InstallRebarSlabModel.MSlabs,
                            CanvasPageBase,
                            InstallRebarSlabModel.MSlabCenter,
                            InstallRebarSlabModel);
                    };
                    msl.MSlabRebar.BotX.NormalEventAction = () =>
                    {
                        MSlabRebarLayer.NormalEventActionF(
                            msl.MSlabRebar.BotX,
                            InstallRebarSlabModel.MSlabs,
                            CanvasPageBase,
                            InstallRebarSlabModel.MSlabCenter,
                            InstallRebarSlabModel);
                    };
                    msl.MSlabRebar.BotY.NormalEventAction = () =>
                    {
                        MSlabRebarLayer.NormalEventActionF(
                            msl.MSlabRebar.BotY,
                            InstallRebarSlabModel.MSlabs,
                            CanvasPageBase,
                            InstallRebarSlabModel.MSlabCenter,
                            InstallRebarSlabModel);
                    };

                    msl.MSlabRebar.TopX.Normal = vtY;
                    msl.MSlabRebar.TopY.Normal = vtX;
                    msl.MSlabRebar.BotX.Normal = vtY;
                    msl.MSlabRebar.BotY.Normal = vtX;

                    msl.MSlabRebar.TopX.Direction = vtX;
                    msl.MSlabRebar.TopY.Direction = vtY;
                    msl.MSlabRebar.BotX.Direction = vtX;
                    msl.MSlabRebar.BotY.Direction = vtY;

                    MSlabRebarLayer.NormalEventActionF(
                            msl.MSlabRebar.TopX,
                            InstallRebarSlabModel.MSlabs,
                            CanvasPageBase,
                            InstallRebarSlabModel.MSlabCenter,
                            InstallRebarSlabModel);

                    MSlabRebarLayer.NormalEventActionF(
                            msl.MSlabRebar.TopY,
                            InstallRebarSlabModel.MSlabs,
                            CanvasPageBase,
                            InstallRebarSlabModel.MSlabCenter,
                            InstallRebarSlabModel);

                    MSlabRebarLayer.NormalEventActionF(
                            msl.MSlabRebar.BotX,
                            InstallRebarSlabModel.MSlabs,
                            CanvasPageBase,
                            InstallRebarSlabModel.MSlabCenter,
                            InstallRebarSlabModel);

                    MSlabRebarLayer.NormalEventActionF(
                            msl.MSlabRebar.BotY,
                            InstallRebarSlabModel.MSlabs,
                            CanvasPageBase,
                            InstallRebarSlabModel.MSlabCenter,
                            InstallRebarSlabModel);

                    MSlabRebarLayer.SpacingEventActionF(
                            msl.MSlabRebar.RebarLayerActive,
                            InstallRebarSlabModel.MSlabs,
                            CanvasPageBase,
                            InstallRebarSlabModel.MSlabCenter,
                            OptionStyleInstanceInCanvas.OPTION_REBAR,
                            InstallRebarSlabModel);

                }
            };

        }
        private void DrawOnUI(CanvasPageBase canvasPageBase)
        {
            //vẽ sàn trên canvas
            foreach (var mSlab in InstallRebarSlabModel.MSlabs)
            {
                var slab = new InstanceInCanvasPolygon(canvasPageBase, OptionStyleInstanceInCanvas.OPTION_CONCRETE, mSlab.PointsSlabOnFloorPlan.Select(x => x.ConvertPointRToC(InstallRebarSlabModel.MSlabCenter, canvasPageBase)));
                mSlab.SlabInCanvas = slab.UIElement as System.Windows.Shapes.Polygon;
                mSlab.SlabInCanvas.MouseLeftButtonDown += (o, e) => { MSlab.ActionElementSelected(mSlab, InstallRebarSlabModel.MSlabs, InstallRebarSlabModel); };
                slab.DrawInCanvas();
            }
            //vẽ lỗ mở trên canvas
            foreach (var mSlab in InstallRebarSlabModel.MSlabs)
            {
                foreach (var plg in mSlab.MSlabOpenings)
                {
                    var opening = new InstanceInCanvasPolygon(canvasPageBase, OptionStyleInstanceInCanvas.OPTION_OPENING, plg.Points.Select(x => x.ConvertPointRToC(InstallRebarSlabModel.MSlabCenter, canvasPageBase)));
                    mSlab.OpeningsInCanvas.Add(opening.UIElement as System.Windows.Shapes.Polygon);
                    opening.DrawInCanvas();
                }
            }
            //vẽ các đối tượng xung quanh sàn
            foreach (var neighborhood in InstallRebarSlabModel.MSlabElementNeighborhoods)
            {
                var slab = new InstanceInCanvasPolygon(canvasPageBase, OptionStyleInstanceInCanvas.OPTION_CONCRETE, neighborhood.PointsOnFloorPlan.Select(x => x.ConvertPointRToC(InstallRebarSlabModel.MSlabCenter, canvasPageBase)));
                neighborhood.UIElement = slab.UIElement as System.Windows.Shapes.Polygon;
                neighborhood.EventArgsSelector = () => { MSlabElementNeighborhood.MSlabElementNeighborhoodEventArgsSelector(InstallRebarSlabModel.MSlabElementNeighborhoods, neighborhood); };
                neighborhood.UIElement.MouseLeftButtonDown += (o, e) => { neighborhood.ActionElementSelected(); };
                slab.DrawInCanvas();
            }
            //vẽ lỗ mở của các đối tượng xung quanh trên canvas
            foreach (var ele in InstallRebarSlabModel.MSlabElementNeighborhoods)
            {
                foreach (var plg in ele.Openings)
                {
                    var opening = new InstanceInCanvasPolygon(canvasPageBase, OptionStyleInstanceInCanvas.OPTION_OPENING, plg.Points.Select(x => x.ConvertPointRToC(InstallRebarSlabModel.MSlabCenter, canvasPageBase)));
                    //ele.OpeningsInCanvas.Add(opening.UIElement as System.Windows.Shapes.Polygon);
                    opening.DrawInCanvas();
                }
            }
            //vẽ thép sàn 
            foreach (var mSlab in InstallRebarSlabModel.MSlabs)
            {
                var rebarLayer = mSlab.MSlabRebar.RebarLayerActive;
                rebarLayer.SpacingEventAction = () =>
                {
                    MSlabRebarLayer.SpacingEventActionF(
                        rebarLayer, InstallRebarSlabModel.MSlabs,
                        canvasPageBase, InstallRebarSlabModel.MSlabCenter,
                        OptionStyleInstanceInCanvas.OPTION_REBAR, InstallRebarSlabModel);
                };
                mSlab.MSlabRebar.TopX.RebarsInCanvas = MSlabRebarLayer.DrawRebarLayerInCanvas(
                    mSlab.MSlabRebar.TopX, InstallRebarSlabModel.MSlabs,
                    OptionStyleInstanceInCanvas.OPTION_REBAR, InstallRebarSlabModel.MSlabCenter,
                    canvasPageBase, InstallRebarSlabModel);
            }
        }
    }
}
