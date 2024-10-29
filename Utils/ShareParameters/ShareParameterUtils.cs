using HcBimUtils.DocumentUtils;
using System.IO;
using Utils.Messages;

namespace Utils.Utils.ShareParameters
{
    public static class ShareParameterUtils
    {
        public static void ProjectParametersAdd(this Document document, Autodesk.Revit.ApplicationServices.Application application, string path, List<ParamDefinition> paramCreationDataList)
        {
            try
            {
                string parametersFilename = application.SharedParametersFilename;
                application.SharedParametersFilename = path;
                Dictionary<SharedParamGroup, Definitions> groupDefinitions = GetSharedParamFileGroupDefinitions(application.OpenSharedParameterFile(), SharedParamGroup.General);
                List<string> existingParams = new List<string>();
                existingParams = new FilteredElementCollector(document)
                    .OfClass(typeof(SharedParameterElement))
                    .Select(x => x.Name)
                    .Distinct()
                    .ToList();

                CategorySetManager setManager = new CategorySetManager(document);

                foreach (var paramCreationData in paramCreationDataList)
                {
                    paramCreationData.CreateParameter(document, existingParams, setManager, groupDefinitions);
                }

                application.SharedParametersFilename = parametersFilename;
            }
            catch (Exception)
            {
                IO.ShowWarning("fail");
            }
        }

        private static Dictionary<SharedParamGroup, Definitions> GetSharedParamFileGroupDefinitions(DefinitionFile definitionFile, SharedParamGroup sharedParamGroup)
        {
            Dictionary<SharedParamGroup, Definitions> dictionary = new Dictionary<SharedParamGroup, Definitions>
            {
                {sharedParamGroup, definitionFile.Groups.get_Item(sharedParamGroup.ToString()).Definitions}
            };
            return dictionary;
        }

        public static bool AreParametersShareInProject(IList<string> parameters)
        {
            List<string> parameterShares = new List<string>();
            DefinitionBindingMapIterator definitionBindingMapIterator = AC.Document.ParameterBindings.ForwardIterator();
            definitionBindingMapIterator.Reset();
            while (definitionBindingMapIterator.MoveNext())
            {
                Definition definition = definitionBindingMapIterator.Key;
                if (definition != null)
                {
                    parameterShares.Add(definition.Name);
                }
            }

            foreach (string parameter in parameters)
            {
                if (!parameterShares.Contains(parameter))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool AreParametersInProject(Document document, IList<string> nameParameters, bool isShare)
        {
            IEnumerable<ParameterElement> allParameters = new FilteredElementCollector(document).OfClass(typeof(ParameterElement)).Cast<ParameterElement>();
            List<string> nameParametersProject = new List<string>();
            foreach (ParameterElement parameterElement in allParameters)
            {
                if (isShare)
                {
                    if (parameterElement is SharedParameterElement)
                    {
                        nameParametersProject.Add(parameterElement.GetDefinition().Name);
                    }
                }
                else
                {
                    if (!(parameterElement is SharedParameterElement))
                    {
                        nameParametersProject.Add(parameterElement.GetDefinition().Name);
                    }
                }
            }
            bool result = true;
            foreach (string nameParameter in nameParameters)
            {
                result = result && nameParametersProject.Contains(nameParameter);
            }
            return result;
        }

        public static DefinitionFile GetSharedParamDefinitionFile(Document document, string fileShareName)
        {
            DefinitionFile definitionFile = document.Application.OpenSharedParameterFile();
            if (definitionFile != null)
            {
                string fileShareNameInProject = Path.GetFileNameWithoutExtension(definitionFile.Filename);
                if (fileShareNameInProject == fileShareName)
                {
                    return definitionFile;
                }
            }
            return null;
        }
#if R17 || R18 || R19 || R20 || R21
      public static List<UniqueParameter> CreateParameterUseShare(Document document, string nameFileShareParameter, IList<ParameterInformation> parameterInformations, ParameterType parameterType, IList<BuiltInCategory> builtInCategories, BuiltInParameterGroup builtInParameterGroup, CustomGroupParameters customGroupParameters, bool instance)
      {
         string tempFile = Path.Combine(Path.GetTempPath(), $"{nameFileShareParameter}.txt");
         if (File.Exists(tempFile))
         {
            File.Delete(tempFile);
         }
         using (FileStream fileStream = File.Create(tempFile)) { fileStream.Close(); };
         document.Application.SharedParametersFilename = tempFile;

         CategorySet categorySet = new CategorySet();
         foreach (BuiltInCategory builtInCategory in builtInCategories)
         {
            Category category = Category.GetCategory(document, builtInCategory);
            categorySet.Insert(category);
         }

         DefinitionGroup definitionGroup = document.Application.OpenSharedParameterFile().Groups.Create(customGroupParameters.ToString());

         Binding binding = document.Application.Create.NewTypeBinding(categorySet);
         if (instance) binding = document.Application.Create.NewInstanceBinding(categorySet);

         BindingMap bindingMap = document.ParameterBindings;

         List<UniqueParameter> uniqueParameters = new List<UniqueParameter>();
         foreach (ParameterInformation parameterInformation in parameterInformations)
         {
            ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(parameterInformation.Name, parameterType) { UserModifiable = !parameterInformation.ReadOnly, Description = parameterInformation.Description, Visible = parameterInformation.Visible };
            ExternalDefinition externalDefinition = definitionGroup.Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;
            uniqueParameters.Add(new UniqueParameter() { ParameterInformation = parameterInformation, Guid = externalDefinition.GUID });
            bindingMap.Insert(externalDefinition, binding, builtInParameterGroup);
         }
         return uniqueParameters;
      }
#endif

#if R22 || R23 || R24 || R25 || R26
      public static List<UniqueParameter> CreateParameterUseShare(
         Document document, 
         string nameFileShareParameter, 
         IList<ParameterInformation> parameterInformations, 
         ForgeTypeId forgeTypeId, 
         IList<BuiltInCategory> builtInCategories, 
         BuiltInParameterGroup builtInParameterGroup, 
         CustomGroupParameters customGroupParameters, 
         bool instance)
      {
         string tempFile = Path.Combine(Path.GetTempPath(), $"{nameFileShareParameter}.txt");
         if (File.Exists(tempFile))
         {
            File.Delete(tempFile);
         }
         using (FileStream fileStream = File.Create(tempFile)) { fileStream.Close(); };
         document.Application.SharedParametersFilename = tempFile;

         CategorySet categorySet = new CategorySet();
         foreach (BuiltInCategory builtInCategory in builtInCategories)
         {
            Category category = Category.GetCategory(document, builtInCategory);
            categorySet.Insert(category);
         }

         DefinitionGroup definitionGroup = document.Application.OpenSharedParameterFile().Groups.Create(customGroupParameters.ToString());

         Binding binding = document.Application.Create.NewTypeBinding(categorySet);
         if (instance) binding = document.Application.Create.NewInstanceBinding(categorySet);

         BindingMap bindingMap = document.ParameterBindings;

         List<UniqueParameter> uniqueParameters = new List<UniqueParameter>();
         foreach (ParameterInformation parameterInformation in parameterInformations)
         {
            ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(parameterInformation.Name, forgeTypeId) { UserModifiable = !parameterInformation.ReadOnly, Description = parameterInformation.Description, Visible = parameterInformation.Visible };
            ExternalDefinition externalDefinition = definitionGroup.Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;
            uniqueParameters.Add(new UniqueParameter() { ParameterInformation = parameterInformation, Guid = externalDefinition.GUID });
            bindingMap.Insert(externalDefinition, binding, builtInParameterGroup);
         }
         return uniqueParameters;
      }
#endif
    }
}
