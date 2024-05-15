namespace Nebula.Core.Utilities
{
    public interface INebulaTool
    {
        string Name { get; }
        void Execute();
    }
}
