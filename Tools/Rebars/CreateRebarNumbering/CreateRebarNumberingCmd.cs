using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Structure;
using HcBimUtils.DocumentUtils;
using Newtonsoft.Json;
using Nice3point.Revit.Toolkit.External;
using RevitDevelop.Utils.RevElementNumberings;
using RevitDevelop.Utils.RevElements.RevRebars;
using System.Diagnostics;
using Utils.Entities;
using Utils.FilterElements;

namespace RevitDevelop.Tools.Rebars.CreateRebarNumbering
{
    [Transaction(TransactionMode.Manual)]
    public class CreateRebarNumberingCmd : ExternalCommand
    {
        public const string SCHEMAL_INFO_NUMBERING_REBAR = "aa29b7c0-7095-424f-a2bf-a0250c750923";
        public const string REBAR_POSITION_NUMBER_INFO = "RebarPositionNumberInfo";
        public override void Execute()
        {
            AC.GetInformation(UiDocument);
            using (var tsg = new TransactionGroup(Document, "Already Numbering"))
            {
                tsg.Start();
                try
                {
                    var w = new Stopwatch();
                    w.Start();
                    var schemaRebarNumberingInfo = new SchemaInfo(SCHEMAL_INFO_NUMBERING_REBAR, REBAR_POSITION_NUMBER_INFO, new SchemaField());
                    //--------
                    var optionsNumbering = new List<OptionRebarNumbering>() {
                        OptionRebarNumbering.Prefix,
                        OptionRebarNumbering.GroupElevation,
                        OptionRebarNumbering.Zone,
                        OptionRebarNumbering.Length,
                        OptionRebarNumbering.Diameter,
                        OptionRebarNumbering.RebarShape,
                        OptionRebarNumbering.StartThread,
                        OptionRebarNumbering.EndThread,
                    };

                    var rebars = Document.GetElementsFromClass<Rebar>(false);
                    var rebarInfos = rebars
                        .Select(x => SchemaInfo.Read(schemaRebarNumberingInfo.SchemaBase, x, schemaRebarNumberingInfo.SchemaField.Name))
                        .Select(x => JsonConvert.DeserializeObject<RevRebar>(x))
                        .ToList();
                    using (var ts = new Transaction(Document, "name transaction"))
                    {
                        ts.Start();
                        //--------
                        RevRebarUtils.Numbering(rebarInfos, optionsNumbering, schemaRebarNumberingInfo);
                        //--------
                        ts.Commit();
                    }
                    //--------
                    tsg.Assimilate();
                    w.Stop();
                    //Debug.Write(w.ElapsedMilliseconds);
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
