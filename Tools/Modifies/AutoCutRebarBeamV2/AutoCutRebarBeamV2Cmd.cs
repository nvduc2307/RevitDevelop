using Autodesk.Revit.Attributes;
using HcBimUtils.DocumentUtils;
using Microsoft.Extensions.DependencyInjection;
using Nice3point.Revit.Toolkit.External;
using RevitDevelop.AutoCutRebarBeamV2.models;
using RevitDevelop.AutoCutRebarBeamV2.services;
using RevitDevelop.AutoCutRebarBeamV2.viewModels;
using RevitDevelop.Tools.Modifies.AutoCutRebarBeamV2.iservices;
using Utils.Messages;

namespace RevitDevelop.AutoCutRebarBeamV2
{
    [Transaction(TransactionMode.Manual)]
    public class AutoCutRebarBeamV2Cmd : ExternalCommand
    {
        public override void Execute()
        {
            AC.GetInformation(UiDocument);
            using (var tsg = new TransactionGroup(Document, "name transaction group"))
            {
                tsg.Start();
                try
                {
                    //--------
                    var service = new ServiceCollection();
                    service.AddSingleton<AutoCutRebarBeamV2Cmd>();
                    service.AddSingleton<IAutoCutRebarBeamV2Service, AutoCutRebarBeamV2Service>();
                    service.AddSingleton<AutoCutRebarBeamV2ViewModel>();
                    service.AddSingleton<ElementInstance, ElementInstance>();
                    service.AddSingleton<IRebarBeamAssemblyAnalysis, RebarBeamAssemblyAnalysis>();
                    var provider = service.BuildServiceProvider();
                    var vm = provider.GetService<AutoCutRebarBeamV2ViewModel>();
                    vm.MainView.ShowDialog();
                    //--------
                    tsg.Assimilate();
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
                catch (Exception ex)
                {
                    IO.ShowWarning(ex.Message);
                    tsg.RollBack();
                }
            }
        }
    }
}
