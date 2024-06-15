namespace ZelTranslator_SD.Parsers.GameMakerStudio2
{
    public class PathYY
    {
        public class RootObject
        {
            public string resourceType = "GMPath";
            public string resourceVersion = "1.0";
            public string name { get; set; }
            public int kind = 0;
            public int precision = 4;
            public bool closed = false;
            public Point[] points { get; set; }
            public Parent parent = new Parent();
        }

        // Points
        public class Point
        {
            public float speed { get; set; }
            public float x { get; set; }
            public float y { get; set; }
        }

        // Parent
        public class Parent
        {
            public string name = "Paths";
            public string path = "folders/Paths.yy";
        }
    }
}
