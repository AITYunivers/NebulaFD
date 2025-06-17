namespace Nebula.Core.Data.Image
{
    public enum EImageType
    {
        BGRA8888,
        BGRA4444,
        BGRA5551,
        BGR888,
        RGBMasked, // BGR sometimes on Fusion 3?
        JPEG,
        BGR555X,
        BGR565, // Can also be 4 on Android?
        RGBA,
        ABGR,

        // Non >=MMF2 Image Types
        FromPalette
    }
}
