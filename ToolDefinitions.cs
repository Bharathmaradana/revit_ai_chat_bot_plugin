namespace ClassLibrary3
{
    public static class ToolDefinitions
    {
        public static string GetTools()
        {
            return @"
            [
              {
                ""name"": ""get_rooms"",
                ""description"": ""Get all rooms in the model""
              },
              {
                ""name"": ""get_doors"",
                ""description"": ""Get all doors in the model""
              }
            ]";
        }
    }
}