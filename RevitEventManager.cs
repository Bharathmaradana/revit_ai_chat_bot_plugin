using Autodesk.Revit.UI;

namespace ClassLibrary3
{
    public static class RevitEventManager
    {
        public static ExternalEvent ExternalEvent;
        public static RevitEventHandler Handler;

        public static void Initialize(UIDocument uiDoc)
        {
            Handler = new RevitEventHandler();
            Handler.UiDoc = uiDoc;

            ExternalEvent = ExternalEvent.Create(Handler);
        }
    }
}