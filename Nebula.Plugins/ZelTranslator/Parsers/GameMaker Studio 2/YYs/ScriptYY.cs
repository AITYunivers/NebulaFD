namespace ZelTranslator_SD.Parsers.GameMakerStudio2
{
    public class ScriptYY
    {
        public class RootObject
        {
            public string resourceType = "GMScript";
            public string resourceVersion = "1.0";
            public string name { get; set; }
            public bool isDnD = false;
            public bool isCompatibility = false;
            public Parent parent = new Parent();

        }

        public class Parent
        {
            public string name = "Scripts";
            public string path = "folders/Scripts.yy";
        }

    }
}
