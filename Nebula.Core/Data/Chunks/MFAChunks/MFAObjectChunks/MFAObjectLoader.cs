using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Memory;
using System.Drawing;

namespace Nebula.Core.Data.Chunks.MFAChunks.MFAObjectChunks
{
    public class MFAObjectLoader : Chunk
    {
        public BitDict ObjectFlags = new BitDict( // Object Flags
            "DisplayInFront",         // Display In Front (Unused?)
            "Background",             // Background
            "SaveBackground",         // Save Background
            "RunBeforeFadeIn",        // Run Before Fade In
            "HasMovements",           // Has Movements
            "HasAnimations",          // Has Animations
            "TabStop",                // Tab Stop Focus
            "WindowProcess",          // Is Window Process
            "HasAlterables",          // Has Alterable Values, Strings, and Flags
            "HasSprites",             // Uses Images
            "InternalSaveBackground", // Interal Save Background
            "DontFollowFrame",        // Follow the frame Disabled
            "DisplayAsBackground",    // Display as background
            "DontDestroyIfTooFar",    // Destroy object if too far from frame Disabled
            "DontInactivateIfTooFar", // Inactivate if too far from window: No
            "InactivateIfTooFar",     // Inactivate if too far from window: Yes
            "HasText",                // Uses Text
            "CreateAtStart",          // Create at start
            "CCNCheck", "",           // Only on for CCNs
            "DontResetFrameDuration"  // Do not reset current frame duration when the animation is modified
        );

        public BitDict NewObjectFlags = new BitDict( // New Object Flags
            "DontSaveBackground",     // Save background Disabled
            "WipeWithColor",          // Wipe with color
            "DontUseFineDetection",   // Use fine detection Disabled / Collision with Box 
            "VisibleAtStart",         // Visible at start
            "SolidObstacle",          // Obstacle Type: Obstacle
            "PlatformObstacle",       // Obstacle Type: Platform
            "LadderObstacle",         // Obstacle Type: Ladder
            "AutomaticRotations",     // Automatic Rotations Enabled
            "InitializeFlags"         // Initialize Flags
        );

        public Color Background = Color.White;
        public short[] Qualifiers = new short[8];
        public ObjectAlterableValues AlterableValues = new();
        public ObjectAlterableStrings AlterableStrings = new();
        public ObjectMovements Movements = new();
        public ObjectBehaviours Behaviours = new();
        public TransitionChunk TransitionIn = new();
        public TransitionChunk TransitionOut = new();

        public MFAObjectLoader()
        {
            ChunkName = "MFAObjectLoader";
        }

        public override void ReadCCN(ByteReader reader, params object[] extraInfo)
        {

        }

        public override void ReadMFA(ByteReader reader, params object[] extraInfo)
        {
            ObjectFlags.Value = reader.ReadUInt();
            NewObjectFlags.Value = reader.ReadUInt();
            Background = reader.ReadColor();
            for (int i = 0; i < Qualifiers.Length; i++)
                Qualifiers[i] = reader.ReadShort();
            reader.Skip(2);

            AlterableValues.ReadMFA(reader);
            AlterableStrings.ReadMFA(reader);
            Movements.ReadMFA(reader);
            Behaviours.ReadMFA(reader);

            if (reader.ReadByte() == 1)
                TransitionIn.ReadMFA(reader);

            if (reader.ReadByte() == 1)
                TransitionOut.ReadMFA(reader);
        }

        public override void WriteCCN(ByteWriter writer, params object[] extraInfo)
        {

        }

        public override void WriteMFA(ByteWriter writer, params object[] extraInfo)
        {
            writer.WriteUInt(ObjectFlags.Value);
            writer.WriteUInt(NewObjectFlags.Value);
            writer.WriteColor(Background);
            foreach (short qualifier in Qualifiers)
                writer.WriteShort(qualifier);
            writer.WriteShort(-1);

            AlterableValues.WriteMFA(writer);
            AlterableStrings.WriteMFA(writer);
            Movements.WriteMFA(writer);
            Behaviours.WriteMFA(writer);

            writer.WriteByte(string.IsNullOrEmpty(TransitionIn.ModuleName) ? (byte)0 : (byte)1);
            if (!string.IsNullOrEmpty(TransitionIn.ModuleName))
                TransitionIn.WriteMFA(writer);

            writer.WriteByte(string.IsNullOrEmpty(TransitionOut.ModuleName) ? (byte)0 : (byte)1);
            if (!string.IsNullOrEmpty(TransitionOut.ModuleName))
                TransitionOut.WriteMFA(writer);
        }
    }
}
