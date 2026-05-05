using Autodesk.Revit.UI;
using System;

namespace ClassLibrary3
{
    public class RevitEventHandler : IExternalEventHandler
    {
        public RevitRequest Request;
        public UIDocument UiDoc;

        public void Execute(UIApplication app)
        {
            try
            {
                if (Request == null)
                {
                    Logger.Log("Request is null" + Request);
                    return;
                }

                if (UiDoc == null)
                {
                    Request.Result = "ERROR: UiDoc is null";
                    return;
                }

                Logger.Log("Handler Triggered");
                Logger.Log("Query: " + Request.Query);
                Logger.Log("UiDoc NULL? " + (UiDoc == null));
                string input = Request.Query.ToLower();
                    
                try
                {
                    if (input.Contains("room"))
                    {
                        var rooms = RoomService.GetRooms(UiDoc);
                        Request.Result = Newtonsoft.Json.JsonConvert.SerializeObject(rooms);
                    }
                    else if (input.Contains("door"))
                    {
                        Request.Result = RevitDataService.GetDoorsJson(UiDoc);
                    }
                    else
                    {
                        Request.Result = "NO_MATCH"; // 🔥 never "{}"
                    }
                }
                catch (Exception ex)
                {
                    Request.Result = "ERROR: " + ex.ToString();
                }
            }
            catch (Exception ex)
            {
                Request.Result = "ERROR: " + ex.Message;
            }
        }

        public string GetName() => "Revit AI External Event";
    }
}