using Autodesk.Revit.Attributes;
using HcBimUtils.DocumentUtils;
using Microsoft.Extensions.DependencyInjection;
using Nice3point.Revit.Toolkit.External;
using RIMT.Filters.FilterRebars.iservices;
using RIMT.Filters.FilterRebars.services;
using RIMT.Filters.FilterRebars.viewModels;

namespace RIMT.Filters.FilterRebars
{
    [Transaction(TransactionMode.Manual)]
    public class FilterRebarCmd : ExternalCommand
    {
        public override void Execute()
        {
            AC.GetInformation(UiDocument);
            using (var tsg = new TransactionGroup(Document, "FilterRebars"))
            {
                tsg.Start();
                try
                {
                    //--------
                    var service = new ServiceCollection();
                    service.AddSingleton<FilterRebarCmd, FilterRebarCmd>();
                    service.AddSingleton<FilterRebarsViewModel, FilterRebarsViewModel>();
                    service.AddSingleton<IFilterRebarPropertyTypeSevice, FilterRebarPropertyTypeSevice>();
                    var provider = service.BuildServiceProvider();
                    var vm = provider.GetService<FilterRebarsViewModel>();
                    vm.MainView.ShowDialog();
                    //--------
                    if (vm.IsComplete) tsg.Assimilate();
                    else tsg.RollBack();

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
