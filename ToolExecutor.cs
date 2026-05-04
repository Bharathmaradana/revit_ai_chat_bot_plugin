using Autodesk.Revit.UI;
using Newtonsoft.Json;

namespace ClassLibrary3
{
    public static class ToolExecutor
    {
        public static string Execute(string toolName, UIDocument uiDoc)
        {
            if (toolName == "get_rooms")
            {
                var rooms = RoomService.GetRooms(uiDoc);
                return JsonConvert.SerializeObject(rooms);
            }

            if (toolName == "get_doors")
            {
                return RevitDataService.GetDoorsJson(uiDoc);
            }

            return "{}";
        }
    }
}