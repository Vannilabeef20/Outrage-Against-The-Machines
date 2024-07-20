
namespace Game
{
    public interface IGameEventListener<T>
    {
        void OnEventRaised(T item);
    }
}
