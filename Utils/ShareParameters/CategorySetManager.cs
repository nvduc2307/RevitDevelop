namespace Utils.Utils.ShareParameters
{
    public class CategorySetManager
    {
        private Dictionary<BindingGroups, CategorySet> definedSets = new Dictionary<BindingGroups, CategorySet>();

        public CategorySetManager(Document doc)
        {
            definedSets.Clear();

            Category categoryView = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Views);
            Category categorySheets = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Sheets);
            Category categoryProjectInformation = doc.Settings.Categories.get_Item(BuiltInCategory.OST_ProjectInformation);
            Category categoryGenericModel = doc.Settings.Categories.get_Item(BuiltInCategory.OST_GenericModel);
            Category categorySpecialityEquipment = doc.Settings.Categories.get_Item(BuiltInCategory.OST_SpecialityEquipment);
            Category categoryStructConnections = doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructConnections);
            Category categoryStructuralFraming = doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFraming);
            Category categoryAssemblies = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Assemblies);
            Category categoryWalls = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls);
            Category categoryEntourage = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Entourage);
            Category categoryFloors = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Floors);
            Category categoryColumns = doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralColumns);
            Category categoryFoundation = doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFoundation);
            Category categoryRebar = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Rebar);
            Category categoryStairs = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Stairs);

            var categories = new List<Category>
            {
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctTerminal),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_CableTrayFitting),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_CableTrayRun),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_CableTray),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Casework),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Ceilings),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Columns),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_CommunicationDevices),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Conduit),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_CableTrayFitting),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_ConduitRun),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_CurtainWallPanels),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_CurtaSystem),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DataDevices),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Doors),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctAccessory),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctFitting),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctInsulations),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctLinings),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PlaceHolderDucts),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctSystem),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctCurves),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_ElectricalCircuit),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_ElectricalEquipment),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_ElectricalFixtures),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Entourage),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_FireAlarmDevices),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_FlexDuctCurves),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_FlexPipeCurves),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Floors),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Furniture),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_GenericModel),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_HVAC_Zones),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_LightingDevices),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_LightingFixtures),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_FabricationContainment),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_FabricationDuctwork),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_FabricationHangers),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_FabricationPipework),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_MechanicalEquipment),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_NurseCallDevices),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Parking),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Parts),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeAccessory),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeFitting),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeInsulations),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PlaceHolderPipes),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeCurves),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipingSystem),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Planting),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PlumbingFixtures),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StairsRailing),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Ramps),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Roads),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Roofs),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Rooms),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_SecurityDevices),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_SpecialityEquipment),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Sprinklers),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Stairs),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFramingSystem),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralColumns),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructConnections),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFoundation),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFraming),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Windows),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Wire),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_CurtainWallMullions),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_CurtainWallPanels),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Cornices),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Gutter),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_RoofSoffit),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Fascia),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_EdgeSlab),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeCurves),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctCurves),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctAccessory),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeAccessory),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_DuctFitting),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_PipeFitting),
                doc.Settings.Categories.get_Item(BuiltInCategory.OST_Rebar),
            };



            var categorySetAll = doc.Application.Create.NewCategorySet();
            foreach (Category category in categories)
            {
                try
                {
                    if (category == null) continue;
                    categorySetAll.Insert(category);
                }
                catch
                {
                    //
                }
            }

            definedSets.Add(BindingGroups.AllModelComponents, categorySetAll);

            Category categoryRoom = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Rooms);

            var roomSet = doc.Application.Create.NewCategorySet();
            roomSet.Insert(categoryRoom);

            definedSets.Add(BindingGroups.Rooms, roomSet);



            CategorySet categorySet1 = doc.Application.Create.NewCategorySet();
            categorySet1.Insert(categoryView);
            definedSets.Add(BindingGroups.Views, categorySet1);

            CategorySet categorySet2 = doc.Application.Create.NewCategorySet();
            categorySet2.Insert(categorySheets);
            definedSets.Add(BindingGroups.Sheets, categorySet2);

            CategorySet categorySet3 = doc.Application.Create.NewCategorySet();
            categorySet3.Insert(categoryGenericModel);
            categorySet3.Insert(categorySpecialityEquipment);
            categorySet3.Insert(categoryStructConnections);
            categorySet3.Insert(categoryAssemblies);
            categorySet3.Insert(categoryEntourage);
            definedSets.Add(BindingGroups.Components, categorySet3);

            CategorySet categorySet4 = doc.Application.Create.NewCategorySet();
            categorySet4.Insert(categoryGenericModel);
            categorySet4.Insert(categorySpecialityEquipment);
            definedSets.Add(BindingGroups.GenModAndSpecialty, categorySet4);

            CategorySet categorySet5 = doc.Application.Create.NewCategorySet();
            categorySet5.Insert(categoryGenericModel);
            categorySet5.Insert(categorySpecialityEquipment);
            categorySet5.Insert(categoryStructConnections);
            definedSets.Add(BindingGroups.GenModSpecialtyAndConnections, categorySet5);

            CategorySet categorySet6 = doc.Application.Create.NewCategorySet();
            categorySet6.Insert(categoryProjectInformation);
            definedSets.Add(BindingGroups.ProjectInformation, categorySet6);

            CategorySet categorySet8 = doc.Application.Create.NewCategorySet();
            categorySet8.Insert(categoryGenericModel);
            categorySet8.Insert(categorySpecialityEquipment);
            categorySet8.Insert(categoryStructConnections);
            categorySet8.Insert(categoryStructuralFraming);
            categorySet8.Insert(categoryWalls);
            definedSets.Add(BindingGroups.AllModelComponentsAndWalls, categorySet8);

            CategorySet categorySet9 = doc.Application.Create.NewCategorySet();
            categorySet9.Insert(categoryGenericModel);
            categorySet9.Insert(categoryStructuralFraming);
            definedSets.Add(BindingGroups.StructuralFraming, categorySet9);

            CategorySet categorySet10 = doc.Application.Create.NewCategorySet();
            categorySet10.Insert(categoryAssemblies);
            definedSets.Add(BindingGroups.Assemblies, categorySet10);

            CategorySet categorySet11 = doc.Application.Create.NewCategorySet();
            categorySet11.Insert(categoryAssemblies);
            categorySet11.Insert(categoryStructuralFraming);
            categorySet11.Insert(categoryGenericModel);
            definedSets.Add(BindingGroups.AssembliesStructuralFraming, categorySet11);

            CategorySet categorySet12 = doc.Application.Create.NewCategorySet();
            categorySet12.Insert(categoryGenericModel);
            categorySet12.Insert(categorySpecialityEquipment);
            categorySet12.Insert(categoryStructConnections);
            categorySet12.Insert(categoryStructuralFraming);
            categorySet12.Insert(categoryAssemblies);
            definedSets.Add(BindingGroups.WeightPerUnit, categorySet12);

            CategorySet categorySet13 = doc.Application.Create.NewCategorySet();
            categorySet13.Insert(categorySpecialityEquipment);
            definedSets.Add(BindingGroups.SpecialtyEquipmentOnly, categorySet13);

            CategorySet categorySet14 = doc.Application.Create.NewCategorySet();
            categorySet14.Insert(categoryColumns);
            categorySet14.Insert(categoryWalls);
            categorySet14.Insert(categoryFloors);
            categorySet14.Insert(categoryFoundation);
            categorySet14.Insert(categoryStructuralFraming);
            categorySet14.Insert(categoryStairs);
            definedSets.Add(BindingGroups.FormworkElement, categorySet14);

            var setBeamSlab = doc.Application.Create.NewCategorySet();
            setBeamSlab.Insert(categoryStructuralFraming);
            setBeamSlab.Insert(categoryFloors);
            setBeamSlab.Insert(categoryStairs);
            setBeamSlab.Insert(categoryWalls);
            definedSets.Add(BindingGroups.BeamSlab, setBeamSlab);

            var setWallSlab = doc.Application.Create.NewCategorySet();
            setWallSlab.Insert(categoryWalls);
            setWallSlab.Insert(categoryFloors);
            definedSets.Add(BindingGroups.SlabWall, setWallSlab);

            var setWall = new CategorySet();
            setWall.Insert(categoryWalls);
            definedSets.Add(BindingGroups.Wall, setWall);

            var setColumn = new CategorySet();
            setColumn.Insert(categoryColumns);
            definedSets.Add(BindingGroups.Column, setColumn);

            var setRebar = new CategorySet();
            setRebar.Insert(categoryRebar);
            definedSets.Add(BindingGroups.Rebar, setRebar);

            var setViewSheet = new CategorySet();
            setViewSheet.Insert(categorySheets);
            setViewSheet.Insert(categoryView);
            definedSets.Add(BindingGroups.ViewAndSheet, setViewSheet);

            var setVFoundationColumn = new CategorySet();
            setVFoundationColumn.Insert(categoryFoundation);
            setVFoundationColumn.Insert(categoryColumns);
            definedSets.Add(BindingGroups.FoundationAndColumn, setVFoundationColumn);
        }

        public ElementBinding GetBindingForBindingGroup(Document doc, BindingGroups grp, InstType disp)
        {
            if (!definedSets.ContainsKey(grp))
                throw new Exception("defined sets does not contain key " + grp.ToString() + " Cannot continue");
            CategorySet definedSet = definedSets[grp];
            return disp == InstType.Instance
                ? doc.Application.Create.NewInstanceBinding(definedSet)
                : (ElementBinding)doc.Application.Create.NewTypeBinding(definedSet);
        }
    }
}
