using System.Drawing;

namespace ZelTranslator_SD.Parsers.GameMakerStudio2
{
    public class SpriteYY
    {
        public class RootObject
        {
            public string resourceType = "GMSprite";
            public string resourceVersion = "1.0";
            public string name { get; set; }
            public int bboxMode = 0;
            public int collisionKind = 1;
            public int type = 0;
            public int origin = 0;
            public bool preMultiplyAlpha = false;
            public bool edgeFiltering = false;
            public int collisionTolerance = 0;
            public float swfPrecision = 2.525f;
            public int bbox_left = 0;
            public int bbox_right { get; set; }
            public int bbox_top = 0;
            public int bbox_bottom { get; set; }
            public bool HTile = false;
            public bool VTile = false;
            public bool For3D = false;
            public bool DynamicTexturePage = false;
            public int width { get; set; }
            public int height { get; set; }
            public TextureGroupID textureGroupId = new TextureGroupID();
            public object swatchColours = null;
            public int gridX = 0;
            public int gridY = 0;
            public Frame[] frames { get; set; }
            public Sequence sequence = new Sequence();
            public Layer[] layers = new Layer[1]
            {
                new Layer()
            };
            public NineSlice nineSlice = new NineSlice();
            public Parent parent = new Parent();
        }
        
        // Texture Group IDs
        public class TextureGroupID
        {
            public string name = "Default";
            public string path = "texturegroups/Default";
        }

        // Frames
        public class Frame
        {
            public string resourceType = "GMSpriteFrame";
            public string resourceVersion = "1.1";
            public string name { get; set; }
            public int ctfhandle { get; set; }
            public int ShapeType { get; set; }
            public int FillType { get; set; }
            public Color solidColor { get; set; }
        }

        // Sequences
        public class Sequence
        {
            public string resourceType = "GMSequence";
            public string resourceVersion = "1.4";
            public string name { get; set; }
            public int timeUnits = 1;
            public int playback = 1;
            public float playbackSpeed { get; set; }
            public int playbackSpeedType = 0;
            public bool autoRecord = true;
            public float volume = 1.0f;
            public float length { get; set; }
            public Events events = new Events();
            public Moments moments = new Moments();
            public Track[] tracks = new Track[1]
            {
                new Track()
            };
            public object visibleRange = null;
            public bool lockOrigin = false;
            public bool showBackdrop = true;
            public bool showBackdropImage = false;
            public string backdropImagePath = "";
            public float backdropImageOpacity = 0.5f;
            public int backdropWidth { get; set; }
            public int backdropHeight { get; set; }
            public float backdropXOffset = 0.0f;
            public float backdropYOffset = 0.0f;
            public int xorigin { get; set; }
            public int yorigin { get; set; }
            public EventToFunction eventToFunction = new EventToFunction();
            public object eventStubScript = null;
        }

        public class Events
        {
            public object[] Keyframes = new object[0];
            public string resourceVersion = "1.0";
            public string resourceType = "KeyframeStore<MessageEventKeyframe>";
        }

        public class Moments
        {
            public object[] Keyframes = new object[0];
            public string resourceVersion = "1.0";
            public string resourceType = "KeyframeStore<MomentsEventKeyframe>";
        }

        public class Track
        {
            public string resourceType = "GMSpriteFramesTrack";
            public string resourceVersion = "1.0";
            public string name = "frames";
            public KeyFrames keyframes = new KeyFrames();
            public object spriteId = null;
            public int trackColour = 0;
            public bool inheritsTrackColour = true;
            public int builtinName = 0;
            public int traits = 0;
            public int interpolation = 1;
            public object[] tracks = new object[0];
            public object[] events = new object[0];
            public bool isCreationTrack = false;
            public object[] modifiers = new object[0];
        }

        public class KeyFrames
        {
            public KeyFrame[] Keyframes { get; set; }
            public string resourceVersion = "1.0";
            public string resourceType = "KeyframeStore<SpriteFrameKeyframe>";
        }

        public class KeyFrame
        {
            public string id { get; set; }
            public float Key = 0.0f;
            public float Length = 1.0f;
            public bool Stretch = false;
            public bool Disabled = false;
            public bool IsCreationKey = false;
            public Channels Channels = new Channels();
            public string resourceVersion = "1.0";
            public string resourceType = "Keyframe<SpriteFrameKeyframe>";
        }

        public class Channels
        {
            public _0 ZEROREPLACE = new _0();
        }

        public class _0
        {
            public Id Id = new Id();
            public string resourceVersion = "1.0";
            public string resourceType = "SpriteFrameKeyframe"; 
        }

        public class Id
        {
            public string name { get; set; }
            public string path { get; set; }
        }

        public class EventToFunction {}

        // Layers
        public class Layer
        {
            public string resourceType = "GMImageLayer";
            public string resourceVersion = "1.0";
            public string name { get; set; }
            public bool visible = true;
            public bool isLocked = false;
            public int blendMode = 0;
            public float opacity = 100.0f;
            public string displayName = "Layer 1";
        }

        // NineSlice
        public class NineSlice
        {
            public int left = 0;
            public int top = 0;
            public int right = 0;
            public int bottom = 0;
            public long[] guideColour = new long[4]
            {
                4294902015,
                4294902015,
                4294902015,
                4294902015
            };
            public int highlightColour = 1728023040;
            public int highlightStyle = 0;
            public bool enabled = false;
            public int[] tileMode = new int[5]
            {
                0,
                0,
                0,
                0,
                0
            };
            public string resourceVersion = "1.0";
            public object loadedVersion = null;
            public string resourceType = "GMNineSliceData";
        }

        // Parents
        public class Parent
        {
            public string name = "Sprites";
            public string path = "folders/Sprites.yy";
        }
    }
}
