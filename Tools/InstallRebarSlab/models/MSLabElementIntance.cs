using Autodesk.Revit.DB.Structure;
using HcBimUtils.DocumentUtils;
using Newtonsoft.Json;
using RevitDevelop.SettingRuleRebarStandards.models;
using RevitDevelop.Utils.NumberingRevitElements;
using Utils.Assemblies;
using Utils.canvass;
using Utils.Entities;
using Utils.FilterElements;
using Utils.RebarInRevits.Models;

namespace RevitDevelop.InstallRebarSlab.models
{
    public class MSLabElementIntance : ObservableObject
    {
        public const string REBAR_SLAB_SCHEMAL_INFO = "468f6fb2-fde4-4f2e-a8e7-ab787a89b8a3";
        public const string REBAR_SLAB_SCHEMAL_NAME = "REBAR_SLAB";
        private RebarBarTypeCustom _rebarBarTypeSelected;
        private MSlabRebarLayerUi _mSlabRebarLayerUiSelected;
        public List<ViewFamilyType> ViewFamilyTypes { get; }
        public List<Grid> Grids { get; }
        public List<AssemblyInstance> AssemblyRebars { get; }
        public List<AssemblyInfo> AssemblyRebarFloors { get; }
        public List<RebarBarTypeCustom> RebarBarTypes { get; }
        public RebarBarTypeCustom RebarBarTypeSelected
        {
            get => _rebarBarTypeSelected;
            set
            {
                _rebarBarTypeSelected = value;
                OnPropertyChanged();
            }
        }
        public List<MSlabRebarLayerUi> MSlabRebarLayerUis { get; }
        public MSlabRebarLayerUi MSlabRebarLayerUiSelected
        {
            get => _mSlabRebarLayerUiSelected;
            set
            {
                _mSlabRebarLayerUiSelected = value;
                OnPropertyChanged();
                MSlabRebarLayerUiSelectedEventAction?.Invoke();
            }
        }
        public Action MSlabRebarLayerUiSelectedEventAction { get; set; }
        public SettingRuleRebarGrade SettingRuleRebarGrade { get; }
        public SchemaInfo RebarSlabSchemaInfo { get; }
        public List<OptionNumberingTypeRebar> OptionNumberingTypeRebars { get; }
        public MSLabElementIntance()
        {
            ViewFamilyTypes = AC.Document.GetElementsFromClass<ViewFamilyType>()
                .Where(x => x.ViewFamily == ViewFamily.FloorPlan)
                .ToList();
            Grids = AC.Document.GetElementsFromClass<Grid>(false);
            AssemblyRebars = AC.Document.GetElementsFromClass<AssemblyInstance>(false)
                .Where(x => AssemblyInfo.GetAssemblyType(AC.Document, x) == AssemblyType1.Rebar)
                .ToList();
            AssemblyRebarFloors = GetAssemblyRebarFloors();
            RebarBarTypes = AC.Document.GetElementsFromClass<RebarBarType>()
                .Select(x => new RebarBarTypeCustom(x))
                .OrderBy(x => x.NameStyle)
                .ToList();
            RebarBarTypeSelected = RebarBarTypes.FirstOrDefault();
            MSlabRebarLayerUis = MSlabRebarLayerUi.GetMSlabRebarLayerUi();
            MSlabRebarLayerUiSelected = MSlabRebarLayerUis.FirstOrDefault();

            var schemaRebarNumberingInfo = new SchemaInfo(
                    ElementInstance.SCHEMAL_INFO_RULE_DEVELOP_LAP,
                    ElementInstance.REBAR_RULE_DEVELOP_LAP,
                    new SchemaField());

            var rebarInfos = SchemaInfo.Read(schemaRebarNumberingInfo.SchemaBase, AC.Document.ProjectInformation, schemaRebarNumberingInfo.SchemaField.Name);
            SettingRuleRebarGrade = JsonConvert.DeserializeObject<SettingRuleRebarGrade>(rebarInfos);

            RebarSlabSchemaInfo = new SchemaInfo(REBAR_SLAB_SCHEMAL_INFO, REBAR_SLAB_SCHEMAL_NAME, new SchemaField());

            OptionNumberingTypeRebars = new List<OptionNumberingTypeRebar>()
            {
                OptionNumberingTypeRebar.Prefix,
                OptionNumberingTypeRebar.Length,
                OptionNumberingTypeRebar.Diameter,
                OptionNumberingTypeRebar.RebarShape,
                OptionNumberingTypeRebar.StartThread,
                OptionNumberingTypeRebar.EndThread,
            };
        }
        private List<AssemblyInfo> GetAssemblyRebarFloors()
        {
            return AssemblyInfo.GetRebarAssemblyInstanceFormRebarHostCategory(
                AC.Document,
                AssemblyRebars,
                RebarHostCategory.Floor,
                out List<Rebar> rebarFalseHost)
                .Select(x => new AssemblyInfo(AC.UiDoc, x))
                .ToList();
        }
        public static void MSlabRebarLayerUiSelectedEventActionF(List<MSlab> mSlabs, MSlabRebarLayerType mSlabRebarLayerType, CanvasPageBase canvasPageBase, XYZ rCenter, InstallRebarSlabModel installRebarSlabModel)
        {
            foreach (var mSl in mSlabs)
            {
                mSl.MSlabRebar.RebarLayerActive =
                MSlabRebarLayerUi.GetMSlabRebarLayer(mSlabRebarLayerType, mSl.MSlabRebar);
                var rebarLayer = mSl.MSlabRebar.RebarLayerActive;
                var layerActive = MSlabRebarLayerUi.GetMSlabRebarLayer(mSlabRebarLayerType, mSl.MSlabRebar);
                layerActive.SpacingEventAction = () =>
                {
                    MSlabRebarLayer.SpacingEventActionF(layerActive, mSlabs, canvasPageBase, rCenter, OptionStyleInstanceInCanvas.OPTION_REBAR, installRebarSlabModel);
                };

                foreach (var item in mSl.MSlabRebar.TopX.RebarsInCanvas)
                {
                    canvasPageBase.Parent.Children.Remove(item);
                }
                foreach (var item in mSl.MSlabRebar.TopY.RebarsInCanvas)
                {
                    canvasPageBase.Parent.Children.Remove(item);
                }
                foreach (var item in mSl.MSlabRebar.BotX.RebarsInCanvas)
                {
                    canvasPageBase.Parent.Children.Remove(item);
                }
                foreach (var item in mSl.MSlabRebar.BotY.RebarsInCanvas)
                {
                    canvasPageBase.Parent.Children.Remove(item);
                }

                layerActive.RebarsInCanvas = MSlabRebarLayer.DrawRebarLayerInCanvas(layerActive, mSlabs, OptionStyleInstanceInCanvas.OPTION_REBAR, rCenter, canvasPageBase, installRebarSlabModel);

            }
        }
    }
}
