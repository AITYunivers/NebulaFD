namespace FusionRipper.Core.Utilities
{
    public interface IFRipPlugin
    {
        string Name { get; }
        void Execute();
    }
}
