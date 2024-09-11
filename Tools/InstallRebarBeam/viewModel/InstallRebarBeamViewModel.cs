using HcBimUtils.DocumentUtils;
using HcBimUtils.MoreLinq;
using Newtonsoft.Json;
using RevitDevelop.Tools.InstallRebarBeam.views;
using System.IO;
using System.Windows.Controls;
using Tools.InstallRebarBeam.model;
using Utils.Directionaries;
using Utils.Messages;
using Utils.PathInWindows;
using Utils.SelectionFilterInRevit;

namespace RIMT.InstallRebarBeam.viewModel
{
    public partial class InstallRebarBeamViewModel : ObservableObject
    {
        public static string _pathSaveData = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\RIMT\\data\\";
        public InstallRebarBeamView MainView { get; set; }
        public BeamInfo BeamInfo { get; set; }
        public RebarBeamUISetting RebarBeamUISetting { get; set; }
        public BeamSetting BeamSetting { get; set; }
        public UIElement UIElement { get; set; }

        public InstallRebarBeamViewModel()
        {
            UIElement = new UIElement();
            BeamSetting = new BeamSetting(UIElement);
            RebarBeamUISetting = new RebarBeamUISetting(UIElement);
            var beam = AC.UiDoc.Selection
                .PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new GenericSelectionFilter(Autodesk.Revit.DB.BuiltInCategory.OST_StructuralFraming))
                .ToElement() as FamilyInstance;
            BeamInfo = new BeamInfo(beam, UIElement)
            {
                StartBeamSection = new StartBeamSection(RebarBeamUISetting.StartInitTopRebar, RebarBeamUISetting.StartInitBottomRebar, RebarBeamUISetting.StartInitStirrup, RebarBeamUISetting.StartInitSideBar),
                MiddleBeamSection = new MiddleBeamSection(RebarBeamUISetting.MidInitTopRebar, RebarBeamUISetting.MidInitBottomRebar, RebarBeamUISetting.MidInitStirrup, RebarBeamUISetting.MidInitSideBar),
                EndBeamSection = new EndBeamSection(RebarBeamUISetting.EndInitTopRebar, RebarBeamUISetting.EndInitBottomRebar, RebarBeamUISetting.EndInitStirrup, RebarBeamUISetting.EndInitSideBar),
                BeamSectionSelected = 0,
                BeamSetting = BeamSetting
            };
            BeamInfo.StartBeamSection.BeamInfo = BeamInfo;
            BeamInfo.MiddleBeamSection.BeamInfo = BeamInfo;
            BeamInfo.EndBeamSection.BeamInfo = BeamInfo;
            BeamSetting.BeamInfo = BeamInfo;
            RebarBeamUISetting.BeamInfo = BeamInfo;
            MainView = new InstallRebarBeamView() { DataContext = this };
        }

        [RelayCommand]
        private void ApplyDataInit()
        {
            if (RebarBeamUISetting.RebarBeamUiSettingInitSelected != null)
            {
                RebarBeamUISetting.GetInitData();
                BeamInfo.StartBeamSection = new StartBeamSection(
                    RebarBeamUISetting.StartInitTopRebar,
                    RebarBeamUISetting.StartInitBottomRebar,
                    RebarBeamUISetting.StartInitStirrup,
                    RebarBeamUISetting.StartInitSideBar);
                BeamInfo.MiddleBeamSection = new MiddleBeamSection(
                    RebarBeamUISetting.MidInitTopRebar,
                    RebarBeamUISetting.MidInitBottomRebar,
                    RebarBeamUISetting.MidInitStirrup,
                    RebarBeamUISetting.MidInitSideBar);
                BeamInfo.EndBeamSection = new EndBeamSection(
                    RebarBeamUISetting.EndInitTopRebar,
                    RebarBeamUISetting.EndInitBottomRebar,
                    RebarBeamUISetting.EndInitStirrup,
                    RebarBeamUISetting.EndInitSideBar);
                BeamInfo.BeamSectionSelected = 0;

                BeamInfo.StartBeamSection.BeamInfo = BeamInfo;
                BeamInfo.MiddleBeamSection.BeamInfo = BeamInfo;
                BeamInfo.EndBeamSection.BeamInfo = BeamInfo;
                BeamSetting.BeamInfo = BeamInfo;
                RebarBeamUISetting.BeamInfo = BeamInfo;
            }
        }
        [RelayCommand]
        private void SaveDataInit()
        {
            try
            {
                var pathData = $"{_pathSaveData}RebarBeamUISetting.json";
                pathData.CreateDirectory();
                var rebarBeamUiSettingInit = GetInitRebarBeamUiSettingInit();
                if (RebarBeamUISetting.RebarBeamUiSettingInits != null && RebarBeamUISetting.RebarBeamUiSettingInits.Count > 0)
                {
                    var index = RebarBeamUISetting.RebarBeamUiSettingInits.IndexOf(RebarBeamUISetting.RebarBeamUiSettingInitSelected);
                    var firstArr = RebarBeamUISetting.RebarBeamUiSettingInits.Slice(0, index).ToList();
                    var lastArr = RebarBeamUISetting.RebarBeamUiSettingInits.Slice(index + 1, RebarBeamUISetting.RebarBeamUiSettingInits.Count - index).ToList();
                    lastArr.Insert(0, rebarBeamUiSettingInit);
                    RebarBeamUISetting.RebarBeamUiSettingInits = firstArr.Concat(lastArr).ToList();
                    RebarBeamUISetting.RebarBeamUiSettingInitSelected = RebarBeamUISetting.RebarBeamUiSettingInits[index];
                }
                else
                {
                    RebarBeamUISetting.RebarBeamUiSettingInits = new List<RebarBeamUiSettingInit>() { rebarBeamUiSettingInit };
                    RebarBeamUISetting.RebarBeamUiSettingInitSelected = RebarBeamUISetting.RebarBeamUiSettingInits.FirstOrDefault();
                }
                var contentData = JsonConvert.SerializeObject(RebarBeamUISetting.RebarBeamUiSettingInits);
                File.WriteAllText(pathData, contentData);
            }
            catch (Exception)
            {
            }
        }

        [RelayCommand]
        private void SaveAsDataInit()
        {
            try
            {
                var pathData = $"{PathInWindow.PathData}RebarBeamUISetting.json";
                var rebarBeamUiSettingInit = GetInitRebarBeamUiSettingInit();
                var name = RebarBeamUISetting.NameRebarBeamUiSettingInit;
                if (string.IsNullOrEmpty(name)) throw new Exception("Vui long nhap ten cua type setting");
                if (RebarBeamUISetting.RebarBeamUiSettingInits.Any(x => x.nameSetting == name)) throw new Exception("Ten setting khong duoc trung nhau");
                rebarBeamUiSettingInit.nameSetting = name;
                RebarBeamUISetting.RebarBeamUiSettingInits = RebarBeamUISetting.RebarBeamUiSettingInits
                    .Concat(new List<RebarBeamUiSettingInit>() { rebarBeamUiSettingInit })
                    .ToList();
                RebarBeamUISetting.RebarBeamUiSettingInitSelected = RebarBeamUISetting.RebarBeamUiSettingInits.FirstOrDefault();
                var contentData = JsonConvert.SerializeObject(RebarBeamUISetting.RebarBeamUiSettingInits);
                File.WriteAllText(pathData, contentData);
            }
            catch (Exception ex)
            {
                IO.ShowWarning(ex.Message);
            }
        }

        private RebarBeamUiSettingInit GetInitRebarBeamUiSettingInit()
        {
            RebarBeamUiSettingInit result = null;
            try
            {
                var random = new Random();
                var rebarBeamUiSettingInit = new RebarBeamUiSettingInit();
                rebarBeamUiSettingInit.nameSetting = RebarBeamUISetting.RebarBeamUiSettingInitSelected != null
                    ? RebarBeamUISetting.RebarBeamUiSettingInitSelected.nameSetting
                    : $"Type{random.Next()}";

                //start
                rebarBeamUiSettingInit.startInitRebarClass1Top = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.StartInitRebarClass1Top.RebarBarType.Name,
                    quantity = RebarBeamUISetting.StartInitRebarClass1Top.Quantity,
                    spacing = RebarBeamUISetting.StartInitRebarClass1Top.Spacing,
                };
                rebarBeamUiSettingInit.startInitRebarClass2Top = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.StartInitRebarClass2Top.RebarBarType.Name,
                    quantity = RebarBeamUISetting.StartInitRebarClass2Top.Quantity,
                    spacing = RebarBeamUISetting.StartInitRebarClass2Top.Spacing,
                };
                rebarBeamUiSettingInit.startInitRebarClass3Top = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.StartInitRebarClass3Top.RebarBarType.Name,
                    quantity = RebarBeamUISetting.StartInitRebarClass3Top.Quantity,
                    spacing = RebarBeamUISetting.StartInitRebarClass3Top.Spacing,
                };
                rebarBeamUiSettingInit.startInitRebarClass1Bot = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.StartInitRebarClass1Bot.RebarBarType.Name,
                    quantity = RebarBeamUISetting.StartInitRebarClass1Bot.Quantity,
                    spacing = RebarBeamUISetting.StartInitRebarClass1Bot.Spacing,
                };
                rebarBeamUiSettingInit.startInitRebarClass2Bot = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.StartInitRebarClass2Bot.RebarBarType.Name,
                    quantity = RebarBeamUISetting.StartInitRebarClass2Bot.Quantity,
                    spacing = RebarBeamUISetting.StartInitRebarClass2Bot.Spacing,
                };
                rebarBeamUiSettingInit.startInitRebarClass3Bot = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.StartInitRebarClass3Bot.RebarBarType.Name,
                    quantity = RebarBeamUISetting.StartInitRebarClass3Bot.Quantity,
                    spacing = RebarBeamUISetting.StartInitRebarClass3Bot.Spacing,
                };
                rebarBeamUiSettingInit.startInitMainStirrup = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.StartInitMainStirrup.RebarBarType.Name,
                    quantity = RebarBeamUISetting.StartInitMainStirrup.Quantity,
                    spacing = RebarBeamUISetting.StartInitMainStirrup.Spacing,
                    typeStirrup = (int)(MainStirrupType)RebarBeamUISetting.StartInitMainStirrup.StirrupType,
                };
                rebarBeamUiSettingInit.startInitTieMain = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.StartInitTieMain.RebarBarType.Name,
                    quantity = RebarBeamUISetting.StartInitTieMain.Quantity,
                    spacing = RebarBeamUISetting.StartInitRebarClass1Top.Spacing,
                    typeStirrup = (int)(TieType)RebarBeamUISetting.StartInitTieMain.StirrupType,
                };
                rebarBeamUiSettingInit.startInitTieSide = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.StartInitTieSide.RebarBarType.Name,
                    quantity = RebarBeamUISetting.StartInitTieSide.Quantity,
                    spacing = RebarBeamUISetting.StartInitTieSide.Spacing,
                    typeStirrup = (int)(TieType)RebarBeamUISetting.StartInitTieSide.StirrupType,
                };
                rebarBeamUiSettingInit.startInitTieSub = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.StartInitTieSub.RebarBarType.Name,
                    quantity = RebarBeamUISetting.StartInitTieSub.Quantity,
                    spacing = RebarBeamUISetting.StartInitTieSub.Spacing,
                    typeStirrup = (int)(TieType)RebarBeamUISetting.StartInitTieSub.StirrupType,
                };
                rebarBeamUiSettingInit.startInitSideBar = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.StartInitSideBar.RebarBarType.Name,
                    quantity = RebarBeamUISetting.StartInitSideBar.Quantity,
                    spacing = RebarBeamUISetting.StartInitSideBar.Spacing,
                };
                //mid
                rebarBeamUiSettingInit.midInitRebarClass1Top = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.MidInitRebarClass1Top.RebarBarType.Name,
                    quantity = RebarBeamUISetting.MidInitRebarClass1Top.Quantity,
                    spacing = RebarBeamUISetting.MidInitRebarClass1Top.Spacing,
                };
                rebarBeamUiSettingInit.midInitRebarClass2Top = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.MidInitRebarClass2Top.RebarBarType.Name,
                    quantity = RebarBeamUISetting.MidInitRebarClass2Top.Quantity,
                    spacing = RebarBeamUISetting.MidInitRebarClass2Top.Spacing,
                };
                rebarBeamUiSettingInit.midInitRebarClass3Top = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.MidInitRebarClass3Top.RebarBarType.Name,
                    quantity = RebarBeamUISetting.MidInitRebarClass3Top.Quantity,
                    spacing = RebarBeamUISetting.MidInitRebarClass3Top.Spacing,
                };
                rebarBeamUiSettingInit.midInitRebarClass1Bot = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.MidInitRebarClass1Bot.RebarBarType.Name,
                    quantity = RebarBeamUISetting.MidInitRebarClass1Bot.Quantity,
                    spacing = RebarBeamUISetting.MidInitRebarClass1Bot.Spacing,
                };
                rebarBeamUiSettingInit.midInitRebarClass2Bot = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.MidInitRebarClass2Bot.RebarBarType.Name,
                    quantity = RebarBeamUISetting.MidInitRebarClass2Bot.Quantity,
                    spacing = RebarBeamUISetting.MidInitRebarClass2Bot.Spacing,
                };
                rebarBeamUiSettingInit.midInitRebarClass3Bot = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.MidInitRebarClass3Bot.RebarBarType.Name,
                    quantity = RebarBeamUISetting.MidInitRebarClass3Bot.Quantity,
                    spacing = RebarBeamUISetting.MidInitRebarClass3Bot.Spacing,
                };
                rebarBeamUiSettingInit.midInitMainStirrup = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.MidInitMainStirrup.RebarBarType.Name,
                    quantity = RebarBeamUISetting.MidInitMainStirrup.Quantity,
                    spacing = RebarBeamUISetting.MidInitMainStirrup.Spacing,
                    typeStirrup = (int)(MainStirrupType)RebarBeamUISetting.MidInitMainStirrup.StirrupType,
                };
                rebarBeamUiSettingInit.midInitTieMain = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.MidInitTieMain.RebarBarType.Name,
                    quantity = RebarBeamUISetting.MidInitTieMain.Quantity,
                    spacing = RebarBeamUISetting.MidInitTieMain.Spacing,
                    typeStirrup = (int)(TieType)RebarBeamUISetting.MidInitTieMain.StirrupType,
                };
                rebarBeamUiSettingInit.midInitTieSide = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.MidInitTieSide.RebarBarType.Name,
                    quantity = RebarBeamUISetting.MidInitTieSide.Quantity,
                    spacing = RebarBeamUISetting.MidInitTieSide.Spacing,
                    typeStirrup = (int)(TieType)RebarBeamUISetting.MidInitTieSide.StirrupType,
                };
                rebarBeamUiSettingInit.midInitTieSub = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.MidInitTieSub.RebarBarType.Name,
                    quantity = RebarBeamUISetting.MidInitTieSub.Quantity,
                    spacing = RebarBeamUISetting.MidInitTieSub.Spacing,
                    typeStirrup = (int)(TieType)RebarBeamUISetting.MidInitTieSub.StirrupType,
                };
                rebarBeamUiSettingInit.midInitSideBar = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.MidInitSideBar.RebarBarType.Name,
                    quantity = RebarBeamUISetting.MidInitSideBar.Quantity,
                    spacing = RebarBeamUISetting.MidInitSideBar.Spacing,
                };

                //end
                rebarBeamUiSettingInit.endInitRebarClass1Top = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.EndInitRebarClass1Top.RebarBarType.Name,
                    quantity = RebarBeamUISetting.EndInitRebarClass1Top.Quantity,
                    spacing = RebarBeamUISetting.EndInitRebarClass1Top.Spacing,
                };
                rebarBeamUiSettingInit.endInitRebarClass2Top = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.EndInitRebarClass2Top.RebarBarType.Name,
                    quantity = RebarBeamUISetting.EndInitRebarClass2Top.Quantity,
                    spacing = RebarBeamUISetting.EndInitRebarClass2Top.Spacing,
                };
                rebarBeamUiSettingInit.endInitRebarClass3Top = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.EndInitRebarClass3Top.RebarBarType.Name,
                    quantity = RebarBeamUISetting.EndInitRebarClass3Top.Quantity,
                    spacing = RebarBeamUISetting.EndInitRebarClass3Top.Spacing,
                };
                rebarBeamUiSettingInit.endInitRebarClass1Bot = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.EndInitRebarClass1Bot.RebarBarType.Name,
                    quantity = RebarBeamUISetting.EndInitRebarClass1Bot.Quantity,
                    spacing = RebarBeamUISetting.EndInitRebarClass1Bot.Spacing,
                };
                rebarBeamUiSettingInit.endInitRebarClass2Bot = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.EndInitRebarClass2Bot.RebarBarType.Name,
                    quantity = RebarBeamUISetting.EndInitRebarClass2Bot.Quantity,
                    spacing = RebarBeamUISetting.EndInitRebarClass2Bot.Spacing,
                };
                rebarBeamUiSettingInit.endInitRebarClass3Bot = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.EndInitRebarClass3Bot.RebarBarType.Name,
                    quantity = RebarBeamUISetting.EndInitRebarClass3Bot.Quantity,
                    spacing = RebarBeamUISetting.EndInitRebarClass3Bot.Spacing,
                };
                rebarBeamUiSettingInit.endInitMainStirrup = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.EndInitMainStirrup.RebarBarType.Name,
                    quantity = RebarBeamUISetting.EndInitMainStirrup.Quantity,
                    spacing = RebarBeamUISetting.EndInitMainStirrup.Spacing,
                    typeStirrup = (int)(MainStirrupType)RebarBeamUISetting.EndInitMainStirrup.StirrupType,
                };
                rebarBeamUiSettingInit.endInitTieMain = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.EndInitTieMain.RebarBarType.Name,
                    quantity = RebarBeamUISetting.EndInitTieMain.Quantity,
                    spacing = RebarBeamUISetting.EndInitTieMain.Spacing,
                    typeStirrup = (int)(TieType)RebarBeamUISetting.EndInitTieMain.StirrupType,
                };
                rebarBeamUiSettingInit.endInitTieSide = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.EndInitTieSide.RebarBarType.Name,
                    quantity = RebarBeamUISetting.EndInitTieSide.Quantity,
                    spacing = RebarBeamUISetting.EndInitTieSide.Spacing,
                    typeStirrup = (int)(TieType)RebarBeamUISetting.EndInitTieSide.StirrupType,
                };
                rebarBeamUiSettingInit.endInitTieSub = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.EndInitTieSub.RebarBarType.Name,
                    quantity = RebarBeamUISetting.EndInitTieSub.Quantity,
                    spacing = RebarBeamUISetting.EndInitTieSub.Spacing,
                    typeStirrup = (int)(TieType)RebarBeamUISetting.EndInitTieSub.StirrupType,
                };
                rebarBeamUiSettingInit.endInitSideBar = new()
                {
                    nameRebarBarTypes = RebarBeamUISetting.EndInitSideBar.RebarBarType.Name,
                    quantity = RebarBeamUISetting.EndInitSideBar.Quantity,
                    spacing = RebarBeamUISetting.EndInitSideBar.Spacing,
                };
                result = rebarBeamUiSettingInit;
            }
            catch (Exception)
            {
            }
            return result;
        }
        public void ShowUI()
        {
            MainView.Loaded += MainView_Loaded;
            MainView.ShowDialog();
        }

        private void MainView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var canvas = MainView.FindName(UIElement.CANVAS_TAG_NAME_BEAM_SECTION) as Canvas;
            UIElement.CanvasBaseSection = new Utils.canvass.CanvasPageBase(canvas);
            BeamInfo.DrawBeamInCanvas(UIElement.CanvasBaseSection, BeamInfo);
            BeamInfo.DrawRebarStirrupInCanvas(UIElement.CanvasBaseSection, BeamInfo);
            BeamInfo.DrawRebarMainInCanvas(UIElement.CanvasBaseSection, BeamInfo);
        }
    }
}
