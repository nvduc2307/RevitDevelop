using Autodesk.Revit.DB.Structure;
using HcBimUtils.DocumentUtils;
using Newtonsoft.Json;
using RIMT.InstallRebarBeam.viewModel;
using System.IO;
using Utils.FilterElements;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeam.model
{
    public partial class RebarBeamUISetting : ObservableObject
    {
        private string _nameRebarBeamUiSettingInit;
        private List<RebarBeamUiSettingInit> _rebarBeamUiSettingInits;
        private RebarBeamUiSettingInit _rebarBeamUiSettingInitSelected;
        public List<RebarBarType> RebarBarTypes { get; }
        public RebarClass1 StartInitRebarClass1Top { get; set; }
        public RebarClass2 StartInitRebarClass2Top { get; set; }
        public RebarClass3 StartInitRebarClass3Top { get; set; }
        public RebarClass1 StartInitRebarClass1Bot { get; set; }
        public RebarClass2 StartInitRebarClass2Bot { get; set; }
        public RebarClass3 StartInitRebarClass3Bot { get; set; }
        public MainStirrup StartInitMainStirrup { get; set; }
        public TieMain StartInitTieMain { get; set; }
        public TieSide StartInitTieSide { get; set; }
        public TieSub StartInitTieSub { get; set; }
        public SideRebar StartInitSideBar { get; set; }
        public TopRebar StartInitTopRebar { get; set; }
        public BottomRebar StartInitBottomRebar { get; set; }
        public Stirrup StartInitStirrup { get; set; }

        public RebarClass1 MidInitRebarClass1Top { get; set; }
        public RebarClass2 MidInitRebarClass2Top { get; set; }
        public RebarClass3 MidInitRebarClass3Top { get; set; }
        public RebarClass1 MidInitRebarClass1Bot { get; set; }
        public RebarClass2 MidInitRebarClass2Bot { get; set; }
        public RebarClass3 MidInitRebarClass3Bot { get; set; }
        public MainStirrup MidInitMainStirrup { get; set; }
        public TieMain MidInitTieMain { get; set; }
        public TieSide MidInitTieSide { get; set; }
        public TieSub MidInitTieSub { get; set; }
        public SideRebar MidInitSideBar { get; set; }
        public TopRebar MidInitTopRebar { get; set; }
        public BottomRebar MidInitBottomRebar { get; set; }
        public Stirrup MidInitStirrup { get; set; }
        public RebarClass1 EndInitRebarClass1Top { get; set; }
        public RebarClass2 EndInitRebarClass2Top { get; set; }
        public RebarClass3 EndInitRebarClass3Top { get; set; }
        public RebarClass1 EndInitRebarClass1Bot { get; set; }
        public RebarClass2 EndInitRebarClass2Bot { get; set; }
        public RebarClass3 EndInitRebarClass3Bot { get; set; }
        public MainStirrup EndInitMainStirrup { get; set; }
        public TieMain EndInitTieMain { get; set; }
        public TieSide EndInitTieSide { get; set; }
        public TieSub EndInitTieSub { get; set; }
        public SideRebar EndInitSideBar { get; set; }
        public TopRebar EndInitTopRebar { get; set; }
        public BottomRebar EndInitBottomRebar { get; set; }
        public Stirrup EndInitStirrup { get; set; }

        public UIElement UIElement { get; set; }
        public BeamInfo BeamInfo { get; set; }

        public List<RebarBeamUiSettingInit> RebarBeamUiSettingInits
        {
            get => _rebarBeamUiSettingInits;
            set
            {
                _rebarBeamUiSettingInits = value;
                OnPropertyChanged();
            }
        }
        public RebarBeamUiSettingInit RebarBeamUiSettingInitSelected
        {
            get => _rebarBeamUiSettingInitSelected;
            set
            {
                _rebarBeamUiSettingInitSelected = value;
                OnPropertyChanged();
            }
        }
        public string NameRebarBeamUiSettingInit
        {
            get => _nameRebarBeamUiSettingInit;
            set
            {
                _nameRebarBeamUiSettingInit = value;
                OnPropertyChanged();
            }
        }
        public RebarBeamUISetting(UIElement uIElement)
        {
            var pathData = $"{InstallRebarBeamViewModel._pathSaveData}RebarBeamUISetting.json";
            var dataContext = "[]";
            try
            {
                dataContext = File.ReadAllText(pathData);
            }
            catch (Exception)
            {
            }
            UIElement = uIElement;
            RebarBarTypes = GetRebarBarTypes();
            if (dataContext.Count() > 2 && dataContext.Contains("["))
            {
                RebarBeamUiSettingInits = JsonConvert.DeserializeObject<List<RebarBeamUiSettingInit>>(dataContext);
                RebarBeamUiSettingInitSelected = RebarBeamUiSettingInits.FirstOrDefault();
                GetInitData();
            }
            else GetInitDataNormal();
        }
        public void GetInitData()
        {

            StartInitRebarClass1Top = new RebarClass1(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.startInitRebarClass1Top.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.startInitRebarClass1Top.quantity,
                RebarBeamUiSettingInitSelected.startInitRebarClass1Top.spacing);
            StartInitRebarClass2Top = new RebarClass2(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.startInitRebarClass2Top.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.startInitRebarClass2Top.quantity,
                RebarBeamUiSettingInitSelected.startInitRebarClass2Top.spacing);
            StartInitRebarClass3Top = new RebarClass3(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.startInitRebarClass3Top.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.startInitRebarClass3Top.quantity,
                RebarBeamUiSettingInitSelected.startInitRebarClass3Top.spacing);
            StartInitRebarClass1Bot = new RebarClass1(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.startInitRebarClass1Bot.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.startInitRebarClass1Bot.quantity,
                RebarBeamUiSettingInitSelected.startInitRebarClass1Bot.spacing);
            StartInitRebarClass2Bot = new RebarClass2(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.startInitRebarClass2Bot.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.startInitRebarClass2Bot.quantity,
                RebarBeamUiSettingInitSelected.startInitRebarClass2Bot.spacing);
            StartInitRebarClass3Bot = new RebarClass3(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.startInitRebarClass3Bot.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.startInitRebarClass3Bot.quantity,
                RebarBeamUiSettingInitSelected.startInitRebarClass3Bot.spacing);
            StartInitMainStirrup = new MainStirrup(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.startInitMainStirrup.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.startInitMainStirrup.quantity,
                RebarBeamUiSettingInitSelected.startInitMainStirrup.spacing,
                Stirrup.GetMainStirrupType(RebarBeamUiSettingInitSelected.startInitMainStirrup.typeStirrup));
            StartInitTieMain = new TieMain(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.startInitTieMain.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.startInitTieMain.quantity,
                RebarBeamUiSettingInitSelected.startInitTieMain.spacing,
                Stirrup.GetTieType(RebarBeamUiSettingInitSelected.startInitTieMain.typeStirrup));
            StartInitTieSide = new TieSide(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.startInitTieSide.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.startInitTieSide.quantity,
                RebarBeamUiSettingInitSelected.startInitTieSide.spacing,
                Stirrup.GetTieType(RebarBeamUiSettingInitSelected.startInitTieSide.typeStirrup));
            StartInitTieSub = new TieSub(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.startInitTieSub.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.startInitTieSub.quantity,
                RebarBeamUiSettingInitSelected.startInitTieSub.spacing,
                Stirrup.GetTieType(RebarBeamUiSettingInitSelected.startInitTieSub.typeStirrup));
            StartInitSideBar = new SideRebar(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.startInitSideBar.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.startInitSideBar.quantity,
                RebarBeamUiSettingInitSelected.startInitSideBar.spacing);
            StartInitTopRebar = new TopRebar(StartInitRebarClass1Top, StartInitRebarClass2Top, StartInitRebarClass3Top);
            StartInitTopRebar.RebarClassSelected = 0;
            StartInitBottomRebar = new BottomRebar(StartInitRebarClass1Bot, StartInitRebarClass2Bot, StartInitRebarClass3Bot);
            StartInitBottomRebar.RebarClassSelected = 0;
            StartInitStirrup = new Stirrup(StartInitMainStirrup, StartInitTieMain, StartInitTieSide, StartInitTieSub);
            StartInitStirrup.RebarClassSelected = 0;
            StartInitStirrup.MainStirrup.IsInstall = RebarBeamUiSettingInitSelected.isInstallMainStirrupAtStartSection;

            MidInitRebarClass1Top = new RebarClass1(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.midInitRebarClass1Top.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.midInitRebarClass1Top.quantity,
                RebarBeamUiSettingInitSelected.midInitRebarClass1Top.spacing);
            MidInitRebarClass2Top = new RebarClass2(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.midInitRebarClass2Top.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.midInitRebarClass2Top.quantity,
                RebarBeamUiSettingInitSelected.midInitRebarClass2Top.spacing);
            MidInitRebarClass3Top = new RebarClass3(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.midInitRebarClass3Top.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.midInitRebarClass3Top.quantity,
                RebarBeamUiSettingInitSelected.midInitRebarClass3Top.spacing);
            MidInitRebarClass1Bot = new RebarClass1(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.midInitRebarClass1Bot.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.midInitRebarClass1Bot.quantity,
                RebarBeamUiSettingInitSelected.midInitRebarClass1Bot.spacing);
            MidInitRebarClass2Bot = new RebarClass2(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.midInitRebarClass2Bot.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.midInitRebarClass2Bot.quantity,
                RebarBeamUiSettingInitSelected.midInitRebarClass2Bot.spacing);
            MidInitRebarClass3Bot = new RebarClass3(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.midInitRebarClass3Bot.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.midInitRebarClass3Bot.quantity,
                RebarBeamUiSettingInitSelected.midInitRebarClass3Bot.spacing);
            MidInitMainStirrup = new MainStirrup(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.midInitMainStirrup.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.midInitMainStirrup.quantity,
                RebarBeamUiSettingInitSelected.midInitMainStirrup.spacing,
                Stirrup.GetMainStirrupType(RebarBeamUiSettingInitSelected.midInitMainStirrup.typeStirrup));
            MidInitTieMain = new TieMain(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.midInitTieMain.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.midInitTieMain.quantity,
                RebarBeamUiSettingInitSelected.midInitTieMain.spacing,
                Stirrup.GetTieType(RebarBeamUiSettingInitSelected.midInitTieMain.typeStirrup));
            MidInitTieSide = new TieSide(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.midInitTieSide.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.midInitTieSide.quantity,
                RebarBeamUiSettingInitSelected.midInitTieSide.spacing,
                Stirrup.GetTieType(RebarBeamUiSettingInitSelected.startInitTieSide.typeStirrup));
            MidInitTieSub = new TieSub(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.midInitTieSub.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.midInitTieSub.quantity,
                RebarBeamUiSettingInitSelected.midInitTieSub.spacing,
                Stirrup.GetTieType(RebarBeamUiSettingInitSelected.midInitTieSub.typeStirrup));
            MidInitSideBar = new SideRebar(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.midInitSideBar.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.midInitSideBar.quantity,
                RebarBeamUiSettingInitSelected.midInitSideBar.spacing);
            MidInitTopRebar = new TopRebar(MidInitRebarClass1Top, MidInitRebarClass2Top, MidInitRebarClass3Top);
            MidInitTopRebar.RebarClassSelected = 0;
            MidInitBottomRebar = new BottomRebar(MidInitRebarClass1Bot, MidInitRebarClass2Bot, MidInitRebarClass3Bot);
            MidInitBottomRebar.RebarClassSelected = 0;
            MidInitStirrup = new Stirrup(MidInitMainStirrup, MidInitTieMain, MidInitTieSide, MidInitTieSub);
            MidInitStirrup.RebarClassSelected = 0;
            MidInitStirrup.MainStirrup.IsInstall = RebarBeamUiSettingInitSelected.isInstallMainStirrupAtMidSection;

            EndInitRebarClass1Top = new RebarClass1(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.endInitRebarClass1Top.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.endInitRebarClass1Top.quantity,
                RebarBeamUiSettingInitSelected.endInitRebarClass1Top.spacing);
            EndInitRebarClass2Top = new RebarClass2(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.endInitRebarClass2Top.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.endInitRebarClass2Top.quantity,
                RebarBeamUiSettingInitSelected.endInitRebarClass2Top.spacing);
            EndInitRebarClass3Top = new RebarClass3(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.endInitRebarClass3Top.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.endInitRebarClass3Top.quantity,
                RebarBeamUiSettingInitSelected.endInitRebarClass3Top.spacing);
            EndInitRebarClass1Bot = new RebarClass1(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.endInitRebarClass1Bot.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.endInitRebarClass1Bot.quantity,
                RebarBeamUiSettingInitSelected.endInitRebarClass1Bot.spacing);
            EndInitRebarClass2Bot = new RebarClass2(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.endInitRebarClass2Bot.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.endInitRebarClass2Bot.quantity,
                RebarBeamUiSettingInitSelected.endInitRebarClass2Bot.spacing);
            EndInitRebarClass3Bot = new RebarClass3(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.endInitRebarClass3Bot.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.endInitRebarClass3Bot.quantity,
                RebarBeamUiSettingInitSelected.endInitRebarClass3Bot.spacing);
            EndInitMainStirrup = new MainStirrup(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.endInitMainStirrup.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.endInitMainStirrup.quantity,
                RebarBeamUiSettingInitSelected.endInitMainStirrup.spacing,
                Stirrup.GetMainStirrupType(RebarBeamUiSettingInitSelected.endInitMainStirrup.typeStirrup));
            EndInitTieMain = new TieMain(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.endInitTieMain.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.endInitTieMain.quantity,
                RebarBeamUiSettingInitSelected.endInitTieMain.spacing,
                Stirrup.GetTieType(RebarBeamUiSettingInitSelected.endInitTieMain.typeStirrup));
            EndInitTieSide = new TieSide(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.endInitTieSide.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.endInitTieSide.quantity,
                RebarBeamUiSettingInitSelected.endInitTieSide.spacing,
                Stirrup.GetTieType(RebarBeamUiSettingInitSelected.startInitTieSide.typeStirrup));
            EndInitTieSub = new TieSub(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.endInitTieSub.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.endInitTieSub.quantity,
                RebarBeamUiSettingInitSelected.endInitTieSub.spacing,
                Stirrup.GetTieType(RebarBeamUiSettingInitSelected.endInitTieSub.typeStirrup));
            EndInitSideBar = new SideRebar(
                RebarClass.GetRebarBarTypeFromName(RebarBarTypes, RebarBeamUiSettingInitSelected.endInitSideBar.nameRebarBarTypes),
                RebarBeamUiSettingInitSelected.endInitSideBar.quantity,
                RebarBeamUiSettingInitSelected.endInitSideBar.spacing);
            EndInitTopRebar = new TopRebar(EndInitRebarClass1Top, EndInitRebarClass2Top, EndInitRebarClass3Top);
            EndInitTopRebar.RebarClassSelected = 0;
            EndInitBottomRebar = new BottomRebar(EndInitRebarClass1Bot, EndInitRebarClass2Bot, EndInitRebarClass3Bot);
            EndInitBottomRebar.RebarClassSelected = 0;
            EndInitStirrup = new Stirrup(EndInitMainStirrup, EndInitTieMain, EndInitTieSide, EndInitTieSub);
            EndInitStirrup.RebarClassSelected = 0;
            EndInitStirrup.MainStirrup.IsInstall = RebarBeamUiSettingInitSelected.isInstallMainStirrupAtEndSection;
        }
        private void GetInitDataNormal()
        {
            StartInitRebarClass1Top = new RebarClass1(RebarBarTypes.First(), 2, 50);
            StartInitRebarClass2Top = new RebarClass2(RebarBarTypes.First(), 2, 50);
            StartInitRebarClass3Top = new RebarClass3(RebarBarTypes.First(), 2, 50);
            StartInitRebarClass1Bot = new RebarClass1(RebarBarTypes.First(), 5, 50);
            StartInitRebarClass2Bot = new RebarClass2(RebarBarTypes.First(), 2, 50);
            StartInitRebarClass3Bot = new RebarClass3(RebarBarTypes.First(), 2, 50);
            StartInitMainStirrup = new MainStirrup(RebarBarTypes.First(), 10, 200, MainStirrupType.Type1);
            StartInitTieMain = new TieMain(RebarBarTypes.First(), 10, 200, TieType.Type1);
            StartInitTieSide = new TieSide(RebarBarTypes.First(), 10, 200, TieType.Type1);
            StartInitTieSub = new TieSub(RebarBarTypes.First(), 10, 200, TieType.Type1);
            StartInitSideBar = new SideRebar(RebarBarTypes.First(), 4, 0);
            StartInitTopRebar = new TopRebar(StartInitRebarClass1Top, StartInitRebarClass2Top, StartInitRebarClass3Top);
            StartInitTopRebar.RebarClassSelected = 0;
            StartInitBottomRebar = new BottomRebar(StartInitRebarClass1Bot, StartInitRebarClass2Bot, StartInitRebarClass3Bot);
            StartInitBottomRebar.RebarClassSelected = 0;
            StartInitStirrup = new Stirrup(StartInitMainStirrup, StartInitTieMain, StartInitTieSide, StartInitTieSub);
            StartInitStirrup.RebarClassSelected = 0;
            StartInitStirrup.MainStirrup.IsInstall = true;

            MidInitRebarClass1Top = new RebarClass1(RebarBarTypes.First(), 2, 50);
            MidInitRebarClass2Top = new RebarClass2(RebarBarTypes.First(), 2, 50);
            MidInitRebarClass3Top = new RebarClass3(RebarBarTypes.First(), 2, 50);
            MidInitRebarClass1Bot = new RebarClass1(RebarBarTypes.First(), 2, 50);
            MidInitRebarClass2Bot = new RebarClass2(RebarBarTypes.First(), 2, 50);
            MidInitRebarClass3Bot = new RebarClass3(RebarBarTypes.First(), 2, 50);
            MidInitMainStirrup = new MainStirrup(RebarBarTypes.First(), 10, 200, MainStirrupType.Type1);
            MidInitTieMain = new TieMain(RebarBarTypes.First(), 10, 200, TieType.Type1);
            MidInitTieSide = new TieSide(RebarBarTypes.First(), 10, 200, TieType.Type1);
            MidInitTieSub = new TieSub(RebarBarTypes.First(), 10, 200, TieType.Type1);
            MidInitSideBar = new SideRebar(RebarBarTypes.First(), 4, 0);
            MidInitTopRebar = new TopRebar(MidInitRebarClass1Top, MidInitRebarClass2Top, MidInitRebarClass3Top);
            MidInitTopRebar.RebarClassSelected = 0;
            MidInitBottomRebar = new BottomRebar(MidInitRebarClass1Bot, MidInitRebarClass2Bot, MidInitRebarClass3Bot);
            MidInitBottomRebar.RebarClassSelected = 0;
            MidInitStirrup = new Stirrup(MidInitMainStirrup, MidInitTieMain, MidInitTieSide, MidInitTieSub);
            MidInitStirrup.RebarClassSelected = 0;
            MidInitStirrup.MainStirrup.IsInstall = true;

            EndInitRebarClass1Top = new RebarClass1(RebarBarTypes.First(), 2, 50);
            EndInitRebarClass2Top = new RebarClass2(RebarBarTypes.First(), 2, 50);
            EndInitRebarClass3Top = new RebarClass3(RebarBarTypes.First(), 2, 50);
            EndInitRebarClass1Bot = new RebarClass1(RebarBarTypes.First(), 2, 50);
            EndInitRebarClass2Bot = new RebarClass2(RebarBarTypes.First(), 2, 50);
            EndInitRebarClass3Bot = new RebarClass3(RebarBarTypes.First(), 2, 50);
            EndInitMainStirrup = new MainStirrup(RebarBarTypes.First(), 10, 200, MainStirrupType.Type1);
            EndInitTieMain = new TieMain(RebarBarTypes.First(), 10, 200, TieType.Type1);
            EndInitTieSide = new TieSide(RebarBarTypes.First(), 10, 200, TieType.Type1);
            EndInitTieSub = new TieSub(RebarBarTypes.First(), 10, 200, TieType.Type1);
            EndInitSideBar = new SideRebar(RebarBarTypes.First(), 4, 0);
            EndInitTopRebar = new TopRebar(EndInitRebarClass1Top, EndInitRebarClass2Top, EndInitRebarClass3Top);
            EndInitTopRebar.RebarClassSelected = 0;
            EndInitBottomRebar = new BottomRebar(EndInitRebarClass1Top, EndInitRebarClass2Top, EndInitRebarClass3Top);
            EndInitBottomRebar.RebarClassSelected = 0;
            EndInitStirrup = new Stirrup(EndInitMainStirrup, EndInitTieMain, EndInitTieSide, EndInitTieSub);
            EndInitStirrup.RebarClassSelected = 0;
            EndInitStirrup.MainStirrup.IsInstall = true;
        }
        private List<RebarBarType> GetRebarBarTypes()
        {
            var result = new List<RebarBarType>();
            try
            {
                result = AC.Document.GetElementsFromClass<RebarBarType>().OrderBy(x => x.Name).ToList();
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
    public class RebarBeamUiSettingInit
    {
        public string nameSetting { get; set; }
        public RebarPropertiesInit startInitRebarClass1Top { get; set; }
        public RebarPropertiesInit startInitRebarClass2Top { get; set; }
        public RebarPropertiesInit startInitRebarClass3Top { get; set; }
        public RebarPropertiesInit startInitRebarClass1Bot { get; set; }
        public RebarPropertiesInit startInitRebarClass2Bot { get; set; }
        public RebarPropertiesInit startInitRebarClass3Bot { get; set; }
        public RebarPropertiesInit startInitMainStirrup { get; set; }
        public RebarPropertiesInit startInitTieMain { get; set; }
        public RebarPropertiesInit startInitTieSide { get; set; }
        public RebarPropertiesInit startInitTieSub { get; set; }
        public RebarPropertiesInit startInitSideBar { get; set; }
        public bool isInstallMainStirrupAtStartSection { get; set; }
        public RebarPropertiesInit midInitRebarClass1Top { get; set; }
        public RebarPropertiesInit midInitRebarClass2Top { get; set; }
        public RebarPropertiesInit midInitRebarClass3Top { get; set; }
        public RebarPropertiesInit midInitRebarClass1Bot { get; set; }
        public RebarPropertiesInit midInitRebarClass2Bot { get; set; }
        public RebarPropertiesInit midInitRebarClass3Bot { get; set; }
        public RebarPropertiesInit midInitMainStirrup { get; set; }
        public RebarPropertiesInit midInitTieMain { get; set; }
        public RebarPropertiesInit midInitTieSide { get; set; }
        public RebarPropertiesInit midInitTieSub { get; set; }
        public RebarPropertiesInit midInitSideBar { get; set; }
        public bool isInstallMainStirrupAtMidSection { get; set; }
        public RebarPropertiesInit endInitRebarClass1Top { get; set; }
        public RebarPropertiesInit endInitRebarClass2Top { get; set; }
        public RebarPropertiesInit endInitRebarClass3Top { get; set; }
        public RebarPropertiesInit endInitRebarClass1Bot { get; set; }
        public RebarPropertiesInit endInitRebarClass2Bot { get; set; }
        public RebarPropertiesInit endInitRebarClass3Bot { get; set; }
        public RebarPropertiesInit endInitMainStirrup { get; set; }
        public RebarPropertiesInit endInitTieMain { get; set; }
        public RebarPropertiesInit endInitTieSide { get; set; }
        public RebarPropertiesInit endInitTieSub { get; set; }
        public RebarPropertiesInit endInitSideBar { get; set; }
        public bool isInstallMainStirrupAtEndSection { get; set; }
    }
    public class RebarPropertiesInit
    {
        public string nameRebarBarTypes { get; set; }
        public int quantity { get; set; }
        public double spacing { get; set; }
        public int typeStirrup { get; set; }
    }
}
