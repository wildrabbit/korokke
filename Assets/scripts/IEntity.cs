
public interface IEntity
{
    void Init(GameplayManager mgr);
    void LogicUpdate(float dt);
    void Cleanup();
    void Kill(float delay = 0.0f);
    void StartGame();
    void StopGame();
}
