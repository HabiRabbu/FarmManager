using System;
using System.Collections.Generic;

using Harvey.Data.Coffee;
using Harvey.Data.Fields;
using Harvey.Data.Implements;
using Harvey.Data.Buildings;
using Harvey.Farm.Buildings;
using Harvey.Data.Vehicles;
using Harvey.Data.Jobs;
using Harvey.Data.Workers;

[Serializable]
public class CoffeeSection
{
    public List<CoffeeCropData> Crops = new();
}

[Serializable]
public class FieldSection
{
    public List<FieldSaveData> Items = new();
}

[Serializable]
public class BuildingSection
{
    public List<HouseSaveData> Houses = new();
    public List<ShedSaveData> Sheds = new();
    public List<GarageSaveData> Garages = new();
}

[Serializable]
public class VehicleSection
{
    public List<TractorSaveData> Tractors = new();
    public List<HarvesterSaveData> Harvesters = new();
}

[Serializable]
public class ImplementSection
{
    public List<ImplementSaveData> Plows = new();
    public List<ImplementSaveData> Seeders = new();
}

[Serializable]
public class WorkerSection
{
    public List<WorkerSaveData> Workers = new();
}

[Serializable]
public class JobSection
{
    public List<ActiveJobSaveData> Active = new();
}


/* ------------------------------------------------- ------------------------------------------------- */

[Serializable]
public class TimeSection
{
    public float Hour;
}

[Serializable]
public class GameSaveData
{
    public TimeSection Time = new();
    public CoffeeSection Coffee = new();

    public FieldSection Fields = new();
    public BuildingSection Buildings = new();
    public VehicleSection Vehicles = new();
    public ImplementSection Implements = new();
    public WorkerSection Workers = new();
    public JobSection Jobs = new();


}