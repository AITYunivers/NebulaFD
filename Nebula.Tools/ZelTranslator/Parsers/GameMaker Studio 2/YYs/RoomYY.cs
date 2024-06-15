namespace ZelTranslator_SD.Parsers.GameMakerStudio2
{
    public class RoomYY
    {
        public class RootObject
        {
            public string resourceType = "GMRoom";
            public string resourceVersion = "1.0";
            public string name { get; set; }
            public bool isDnd = false;
            public float volume = 1.0f;
            public object parentRoom = null;
            public View[] views { get; set; }
            public Layer[] layers { get; set; }
            public bool inheritLayers = false;
            public string creationCodeFile = "";
            public bool inheritCode = false;
            public InstanceCreationOrder[] instanceCreationOrder = new InstanceCreationOrder[1];
            public bool inheritCreationOrder = false;
            public object sequenceId = null;
            public RoomSettings roomSettings = new RoomSettings();
            public ViewSettings viewSettings = new ViewSettings();
            public PhysicsSettings physicsSettings = new PhysicsSettings();
            public Parent parent = new Parent();
        }

        // Views
        public class View
        {
            public bool inherit = false;
            public bool visible = false;
            public int xview = 0;
            public int yview = 0;
            public int wview { get; set; }
            public int hview { get; set; }
            public int xport = 0;
            public int yport = 0;
            public int wport { get; set; }
            public int hport { get; set; }
            public int hborder = 32;
            public int vborder = 32;
            public int hspeed = -1;
            public int vspeed = -1;
            public object objectId = null;
        }

        // Layers
        public class Layer
        {
            public string resourceType = "GMRInstanceLayer";
            public string resourceVersion = "1.0";
            public string name { get; set; }
            public Instance[] instances { get; set; }
            public bool visible { get; set; }
            public int depth = 0;
            public bool userdefinedDepth = false;
            public bool inheritLayerDepth = false;
            public bool inheritLayerSettings = false;
            public bool inheritVisibility = true;
            public bool inheritSubLayers = true;
            public int gridX = 32;
            public int gridY = 32;
            public object[] layers = new object[0];
            public bool hierarchyFrozen = false;
            public bool effectEnabled = true;
            public object effectType = null;
            public object[] properties = new object[0];
            public object spriteId = null;
            public long colour = 4278190080;
            public int x = 0;
            public int y = 0;
            public bool htiled = false;
            public bool vtiled = false;
            public float hspeed = 0.0f;
            public float vspeed = 0.0f;
            public bool stretch = false;
            public float animationFPS = 60.0f;
            public int animationSpeedType = 0;
            public bool userdefinedAnimFPS = true;
        }

        public class Instance
        {
            public string resourceType = "GMRInstance";
            public string resourceVersion = "1.0";
            public string name { get; set; }
            public object[] properties = new object[0];
            public bool isDnd = false;
            public ObjectID objectId { get; set; }
            public bool inheritCode = false;
            public bool hasCreationCode = false;
            public long colour { get; set; }
            public float rotation = 0.0f;
            public float scaleX = 1.0f;
            public float scaleY = 1.0f;
            public int imageIndex = 0;
            public float imageSpeed = 1.0f;
            public object inheritedItemId = null;
            public bool frozen = false;
            public bool ignore = false;
            public bool inheritItemSettings = false;
            public float x { get; set; }
            public float y { get; set; }
        }

        public class ObjectID
        {
            public string name { get; set; }
            public string path { get; set; }
        }

        // Instance Creation Order
        public class InstanceCreationOrder
        {
            public string name { get; set; }
            public string path { get; set; }
        }

        // Room Settings
        public class RoomSettings
        {
            public bool inheritRoomSettings = false;
            public int Width { get; set; }
            public int Height { get; set; }
            public bool persistent = false;
        }

        // View Settings
        public class ViewSettings
        {
            public bool inheritViewSettings = false;
            public bool enableViews = false;
            public bool clearViewBackground = false;
            public bool clearDisplayBuffer = true;
        }

        // Physics Settings
        public class PhysicsSettings
        {
            public bool inheritPhysicsSettings = false;
            public bool PhysicsWorld = true;
            public float PhysicsWorldGravityX = 0.0f;
            public float PhysicsWorldGravityY = 10.0f;
            public float PhysicsWorldPixToMetres = 0.1f;
        }

        // Parent
        public class Parent
        {
            public string name = "Rooms";
            public string path = "folders/Rooms.yy";
        }
    }
}
