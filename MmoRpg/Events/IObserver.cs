namespace MmoRpg.Events
{
    public interface IObserver
    {
        void Update(UnitEvent unitEvent);
    }
}