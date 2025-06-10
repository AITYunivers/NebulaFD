namespace Nebula.Core.CTF25.MFA.Frame.Chunk
{
    internal enum EFrameChunks : ushort
    {
        LAST                    = 0x00,
        FRAME_RECT              = 0x21,
        FRAME_DEMO_PATH         = 0x22,
        FRAME_SEED              = 0x23,
        FRAME_LAYER_EFFECTS     = 0x25,
        FRAME_MOVEMENT_TIMER    = 0x27,
        FRAME_EFFECTS           = 0x28,
        FRAME_INCLUDE           = 0x31,
    }
}
