using Harvey.Data.Coffee;
using Harvey.Farm.Fields;
using Harvey.Farm.Workers;

using UnityEngine;

namespace Harvey.Farm.Jobs
{
    public class FieldActionStep : IJobStep
    {
        readonly FieldTile tile;
        readonly JobType type;
        readonly CoffeeCropData crop;
        readonly Worker worker;
        readonly float actionDuration;
        float timer;

        public FieldActionStep(FieldTile tile, JobType type, CoffeeCropData crop, Worker worker)
        {
            this.tile = tile;
            this.type = type;
            this.crop = crop;
            this.worker = worker;
            this.actionDuration = worker?.Stats.GetActionDuration(type) ?? 1f; // fallback to default
        }

        public bool Tick(float dt)
        {
            timer += dt;
            if (timer < actionDuration) return false;

            switch (type)
            {
                case JobType.Plow: tile.Plow(); break;
                case JobType.Seed: tile.Seed(crop); break;
                case JobType.Harvest: tile.Harvest(); break;
            }
            tile.ClearReservation();
            return true;
        }
    }

    public class WaitStep : IJobStep
    {
        float timeLeft;
        public WaitStep(float seconds) => timeLeft = seconds;
        public bool Tick(float dt) => (timeLeft -= dt) <= 0f;
    }
}
