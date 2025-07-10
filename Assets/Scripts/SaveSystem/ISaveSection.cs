namespace Harvey.SaveSystem
{
    public interface ISaveSection
    {
        string SectionName { get; }        // e.g. "Coffee", "Vehicles"
        object CaptureState();               // returns a POCO or struct
        void RestoreState(object state);   // given that POCO back
    }
}
