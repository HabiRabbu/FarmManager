using UnityEngine.Events;

public interface IFieldVehiclePage
{
    void Enter(FieldVehicleJobParams model);

    void Exit();

    UnityEvent OnChanged { get; }
}
