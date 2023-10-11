namespace SapphireD.Core.Utilities
{
    public interface SapDPlugin
    {
        string Name { get; }
        void Execute();
    }
}
