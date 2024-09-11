namespace Utils.Utils.ShareParameters
{
    public class ParamDefinition
    {
        public string ParameterName;
        public BindingGroups BindingGroup;
        public InstType Disposition;
        public SharedParamGroup SharedParamFileGroup;

#if R20 || R21 || R22 || R23
        public BuiltInParameterGroup ParameterGroup { set; get; }
#else
        public BuiltInParameterGroup ParameterGroup { set; get; }
#endif

#if R20 || R21 || R22 || R23
        public ParamDefinition(
          string name,
          BindingGroups group,
          InstType typeOrInstance,
          SharedParamGroup groupInSharedParamsFile)
        {
            ParameterName = name;
            BindingGroup = group;
            Disposition = typeOrInstance;
            SharedParamFileGroup = groupInSharedParamsFile;
        }
#else
        public ParamDefinition(
          string name,
          BindingGroups group,
          InstType typeOrInstance,
          SharedParamGroup groupInSharedParamsFile)
        {
            ParameterName = name;
            BindingGroup = group;
            Disposition = typeOrInstance;
            SharedParamFileGroup = groupInSharedParamsFile;
        }
#endif

        public bool CreateParameter(Document doc, List<string> existingParams, CategorySetManager setManager,
        Dictionary<SharedParamGroup, Definitions> sharedDefs)
        {
            ElementBinding bindingForBindingGroup = setManager.GetBindingForBindingGroup(doc, BindingGroup, Disposition);

            if (sharedDefs.ContainsKey(SharedParamFileGroup) && sharedDefs[SharedParamFileGroup] != null)
            {
                Definition definition = sharedDefs[SharedParamFileGroup].get_Item(ParameterName) as ExternalDefinition;
                if (definition != null)
                {
                    try
                    {
                        var rt = doc.ParameterBindings.Insert(definition, bindingForBindingGroup, ParameterGroup);
                        if (!rt)
                        {
                            var a = doc.ParameterBindings.ReInsert(definition, bindingForBindingGroup, ParameterGroup);
                            existingParams.Add(ParameterName);
                        }
                        existingParams.Add(ParameterName);
                    }
                    catch
                    {
                        doc.ParameterBindings.ReInsert(definition, bindingForBindingGroup, ParameterGroup);
                        existingParams.Add(ParameterName);
                    }

                    return true;
                }
            }
            return false;
        }
    }
    public enum BindingGroups
    {
        ViewAndSheet,
        Views,
        Sheets,
        Components,
        AllModelComponents,
        Rooms,
        ProjectInformation,
        StructuralFraming,
        Assemblies,
        AssembliesStructuralFraming,
        WeightPerUnit,
        GenModAndSpecialty,
        AllModelComponentsAndWalls,
        GenModSpecialtyAndConnections,
        SpecialtyEquipmentOnly,
        EntourageOnly,
        FormworkElement,
        BeamSlab,
        SlabWall, Wall,
        Column,
        Rebar, FoundationAndColumn,
    }

    public enum InstType
    {
        Type,
        Instance,
    }

    public enum SharedParamGroup
    {
        General
    }
}
