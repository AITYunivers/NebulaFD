namespace Nebula.Core.Data.Image
{
    [Flags]
    public enum EImageFlags
    {
        RLE     = 0b00000001,
        RLEW    = 0b00000010,
        RLET    = 0b00000100,
        LZX     = 0b00001000,
        Alpha   = 0b00010000,
        ACE     = 0b00100000,
        Mac     = 0b01000000,
        RGBA    = 0b10000000
    }
}
