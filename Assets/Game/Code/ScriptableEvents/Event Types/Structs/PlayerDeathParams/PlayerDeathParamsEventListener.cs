namespace Game
{
    /// <summary>
    /// Listens for Event "PlayerDeathParamsEvent" of Type "PlayerDeathParams" and invokes Unity Event Response "PlayerDeathParamsUnityEvent."
    /// </summary>
    public class PlayerDeathParamsEventListener : BaseGameEventListener<PlayerDeathParams, PlayerDeathParamsEvent, PlayerDeathParamsUnityEvent> { }
}