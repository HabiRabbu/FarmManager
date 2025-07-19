using System.Threading.Tasks;

namespace Harvey.SaveSystem
{
    public interface ISaveSection
    {
        int LoadPriority { get; }
        void Capture(GameSaveData root);
        Task Restore(GameSaveData root);
    }
}
