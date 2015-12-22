using System.IO;

namespace FormGenerator.Tools
{
    public interface IEditor
    {
        bool IsEmpty { get; }
        void Save(FileStream stream);
        void Clear();
        void Load(FileStream stream);
        void FormatAtStart();
        void FormatOnFly();
        void Format();
    }
}
