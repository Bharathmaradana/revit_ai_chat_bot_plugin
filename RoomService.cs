using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary3
{
    public class RoomData
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Level { get; set; }
        public double Area { get; set; }
        public int DoorCount { get; set; }
        public int WindowCount { get; set; }
    }

    public static class RoomService
    {
        public static List<RoomData> GetRooms(UIDocument uiDoc)
        {
            var doc = uiDoc.Document;

            var rooms = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .Cast<SpatialElement>()
                .Cast<Room>()
                .ToList();

            List<RoomData> result = new List<RoomData>();

            foreach (var room in rooms)
            {
                result.Add(new RoomData
                {
                    Name = room.Name,
                    Number = room.Number,
                    Level = doc.GetElement(room.LevelId)?.Name,
                    Area = room.Area,
                    DoorCount = CountDoors(doc, room),
                    WindowCount = CountWindows(doc, room)
                });
            }

            return result;
        }

        private static int CountDoors(Document doc, Room room)
        {
            return new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Doors)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .Count(d =>
                {
                    var loc = d.Location as LocationPoint;
                    return loc != null && room.IsPointInRoom(loc.Point);
                });
        }

        private static int CountWindows(Document doc, Room room)
        {
            return new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Windows)
                .WhereElementIsNotElementType()
                .Cast<FamilyInstance>()
                .Count(w =>
                {
                    var loc = w.Location as LocationPoint;
                    return loc != null && room.IsPointInRoom(loc.Point);
                });
        }
    }
}