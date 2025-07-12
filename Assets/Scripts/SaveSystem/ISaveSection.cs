namespace Harvey.SaveSystem
{
    public interface ISaveSection
    {
        string SectionName { get; }        // e.g. "Coffee", "Vehicles"
        string CaptureJson();
        void RestoreJson(string json);
    }
}
