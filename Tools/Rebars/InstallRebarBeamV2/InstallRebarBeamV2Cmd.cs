using Autodesk.Revit.Attributes;
using HcBimUtils.DocumentUtils;
using Microsoft.Extensions.DependencyInjection;
using Nice3point.Revit.Toolkit.External;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.iservices;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.middleWares;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.models;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.service;
using RevitDevelop.Tools.Rebars.InstallRebarBeamV2.viewModels;

namespace RevitDevelop.Tools.Rebars.InstallRebarBeamV2
{
    [Transaction(TransactionMode.Manual)]
    public class InstallRebarBeamV2Cmd : ExternalCommand
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
                    service.AddSingleton<InstallRebarBeamV2Cmd>();
                    service.AddSingleton<InstallRebarBeamV2ViewModel>();
                    service.AddSingleton<InstallRebarBeamV2MiddleWare>();
                    service.AddSingleton<IDrawRebarBeamInCanvasSerice, DrawRebarBeamInCanvasSerice>();
                    service.AddSingleton<IRebarBeamTypeService, RebarBeamTypeService>();
                    service.AddSingleton<IInstallRebarBeamInModelService, InstallRebarBeamInModelService>();
                    var provider = service.BuildServiceProvider();
                    var installRebarBeamV2ViewModel = provider.GetService<InstallRebarBeamV2ViewModel>();
                    installRebarBeamV2ViewModel.MainView.ShowDialog();
                    //--------
                    tsg.Assimilate();
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
                catch (Exception)
                {
                    tsg.RollBack();
                }
            }
        }
    }
}
