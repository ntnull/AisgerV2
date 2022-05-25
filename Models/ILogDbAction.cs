namespace Aisger.Models
{
    public interface ILogDbAction
    {
        void Run();
        void SaveEvents();
    }
}