using Harvey.Data.Coffee;
using Harvey.Farm.Fields;
using Harvey.Farm.VehicleScripts;
using Harvey.Farm.Workers;

public class FieldVehicleJobParams
{
    public FieldController Field;
    public JobType Task;
    public Worker AssignedWorker;
    public Vehicle Vehicle;
    public string ImplementId;
    public CoffeeCropData Crop;

    public bool IsReady =>
        Field && AssignedWorker && Vehicle &&
        (Task is not JobType.Seed || Crop != null) &&
        (Task is JobType.Harvest || !string.IsNullOrEmpty(ImplementId));
}