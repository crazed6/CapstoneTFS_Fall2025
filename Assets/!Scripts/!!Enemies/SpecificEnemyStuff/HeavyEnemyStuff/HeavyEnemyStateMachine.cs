

public class HeavyEnemyStateMachine
{
    private IHeavyEnemyState currentState;

    public IHeavyEnemyState GetCurrentState()
    {
        return currentState;
    }

    public void ChangeState(IHeavyEnemyState newState)
    {
        currentState?.Exit();  // Exit current state -_-
        currentState = newState;
        currentState.Enter();  // Enter new state -_-
    }

    public void Execute()
    {
        currentState?.Execute();  // Run the active state logic -_-
    }
}

public interface IHeavyEnemyState
{
    void Enter();
    void Execute();
    void Exit();
}

