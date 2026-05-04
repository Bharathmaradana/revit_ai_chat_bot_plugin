//using Autodesk.Revit.Attributes;
//using Autodesk.Revit.DB;
//using Autodesk.Revit.UI;
//using ChatUI;
//using Newtonsoft.Json;
//using System;
//using System.Linq;
//using System.Reflection;
//using System.Windows.Interop;

//namespace ClassLibrary3
//{
//    // 🔹 App Entry (Ribbon)
//    public class App : IExternalApplication
//    {
//        public Result OnStartup(UIControlledApplication app)
//        {
//            string tabName = "ChatBot";

//            try { app.CreateRibbonTab(tabName); } catch { }

//            RibbonPanel panel = app.CreateRibbonPanel(tabName, "My Panel");

//            string path = Assembly.GetExecutingAssembly().Location;

//            PushButtonData button = new PushButtonData(
//                "ChatButton",
//                "AI Assistant",
//                path,
//                "ClassLibrary3.Command"
//            );

//            panel.AddItem(button);

//            return Result.Succeeded;
//        }

//        public Result OnShutdown(UIControlledApplication app)
//        {
//            return Result.Succeeded;
//        }
//    }

//    // 🔥 Main Command
//    [Transaction(TransactionMode.Manual)]
//    public class Command : IExternalCommand
//    {
//        public Result Execute(
//            ExternalCommandData commandData,
//            ref string message,
//            ElementSet elements)
//        {
//            try
//            {
//                UIDocument uiDoc = commandData.Application.ActiveUIDocument;

//                MainWindow window = new MainWindow();

//                // Attach to Revit window
//                IntPtr revitHandle = commandData.Application.MainWindowHandle;
//                WindowInteropHelper helper = new WindowInteropHelper(window);
//                helper.Owner = revitHandle;

//                if (window.ShowDialog() == true)
//                {
//                    string input = window.UserInput;

//                    // 🔥 STEP 1: Extract Revit data (SAFE - main thread)
//                    string data = "";

//                    if (input.ToLower().Contains("room"))
//                    {
//                        var rooms = RoomService.GetRooms(uiDoc);
//                        data = Newtonsoft.Json.JsonConvert.SerializeObject(rooms);
//                    }
//                    else if (input.ToLower().Contains("door"))
//                    {
//                        data = RevitDataService.GetDoorsJson(uiDoc);
//                    }

//                    // 🔥 STEP 2: Run ChatGPT in background (NO FREEZE)
//                    System.Threading.Tasks.Task.Run(async () =>
//                    {
//                        string response = await ChatProcessor.ProcessAsync(input, data);

//                        // 🔥 STEP 3: Back to UI thread
//                        window.Dispatcher.Invoke(() =>
//                        {
//                            TaskDialog.Show("AI Response", response);
//                        });
//                    });
//                }

//                return Result.Succeeded;
//            }
//            catch (Exception ex)
//            {
//                TaskDialog.Show("Error", ex.ToString());
//                return Result.Failed;
//            }
//        }
//    }
//    // 🔹 Door Data Service
//    public static class RevitDataService
//    {
//        public static string GetDoorsJson(UIDocument uiDoc)
//        {
//            Document doc = uiDoc.Document;

//            var doors = new FilteredElementCollector(doc)
//                .OfCategory(BuiltInCategory.OST_Doors)
//                .WhereElementIsNotElementType()
//                .Cast<FamilyInstance>()
//                .Select(d => new
//                {
//                    Id = d.Id.Value,
//                    Name = d.Name,
//                    Level = doc.GetElement(d.LevelId)?.Name,
//                    Width = d.Symbol.LookupParameter("Width")?.AsDouble(),
//                    Height = d.Symbol.LookupParameter("Height")?.AsDouble()
//                })
//                .ToList();

//            return JsonConvert.SerializeObject(doors, Formatting.Indented);
//        }
//    }
//}


using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ChatUI;
using System;
using System.Reflection;
using System.Windows.Interop;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace ClassLibrary3
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication app)
        {
            string tabName = "ChatBot";

            try { app.CreateRibbonTab(tabName); } catch { }

            RibbonPanel panel = app.CreateRibbonPanel(tabName, "My Panel");

            string path = Assembly.GetExecutingAssembly().Location;

            PushButtonData button = new PushButtonData(
                "ChatButton",
                "AI Assistant",
                path,
                "ClassLibrary3.Command"
            );

            panel.AddItem(button);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                UIDocument uiDoc = commandData.Application.ActiveUIDocument;

                MainWindow window = new MainWindow();

                // 🔥 Inject processor (IMPORTANT)
                MainWindow.ProcessMessage = async (input) =>
                {
                    Logger.Log("User Input: " + input);

                    // Step 1: Extract data (safe - main thread)
                    string data = "";

                    if (input.ToLower().Contains("room"))
                    {
                        var rooms = RoomService.GetRooms(uiDoc);
                        data = Newtonsoft.Json.JsonConvert.SerializeObject(rooms);
                    }
                    else if (input.ToLower().Contains("door"))
                    {
                        data = RevitDataService.GetDoorsJson(uiDoc);
                    }

                    // Step 2: Call LLM
                    return await ChatProcessor.ProcessAsync(input, data);
                };

                // Attach to Revit
                IntPtr revitHandle = commandData.Application.MainWindowHandle;
                WindowInteropHelper helper = new WindowInteropHelper(window);
                helper.Owner = revitHandle;

                window.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.ToString());
                return Result.Failed;
            }
        }
    }

    public static class RevitDataService
    {
        public static string GetDoorsJson(UIDocument uiDoc)
        {
            Document doc = uiDoc.Document;

            var doors = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .Select(d => new
                {
                    Id = d.Id.Value,
                    Name = d.Name,
                    Level = doc.GetElement(d.LevelId)?.Name,
                    Width = d.Symbol.LookupParameter("Width")?.AsDouble(),
                    Height = d.Symbol.LookupParameter("Height")?.AsDouble()
                })
                .ToList();

            return JsonConvert.SerializeObject(doors, Formatting.Indented);
        }
    }
    }