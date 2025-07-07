namespace Harvey.Farm.UI.Radial
{
    public interface IRadialProvider
    {
        System.Collections.Generic.IReadOnlyList<RadialMenuItem> BuildRadialItems();
    }
}
