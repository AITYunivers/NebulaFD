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

    // Whoops I'm retarded, I started writing CCN shit.
    /*internal enum EFrameChunks : ushort
    {
        FRAME_HEADER,
        FRAME_NAME,
        FRAME_PASSWORD,
        FRAME_PALETTE,
        FRAME_INSTANCES,
        FRAME_FADE_IN,
        FRAME_FADE_OUT,
        FRAME_TRANSITION_IN,
        FRAME_TRANSITION_OUT,
        FRAME_EVENTS,
        FRAME_PLAY_HEADER,
        FRAME_EXTRA_ITEMS,
        FRAME_EXTRA_INSTANCES,
        FRAME_LAYERS,
        FRAME_RECT,
        FRAME_DEMO_PATH,
        FRAME_SEED,
        FRAME_LAYER_EFFECTS,
        FRAME_BLURAY_OPTIONS,
        FRAME_MOVEMENT_TIMER,
        FRAME_MOSAIC_TABLE,
        FRAME_EFFECTS,
        FRAME_RUNTIME_OPTIONS,
        FRAME_WUA_OPTIONS,
        FRAME_HANDLE
    }*/
}
