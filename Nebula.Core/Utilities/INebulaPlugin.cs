namespace Nebula.Core.Utilities
{
    public interface INebulaPlugin
    {
        string Name { get; }
        void Execute();
    }
}
