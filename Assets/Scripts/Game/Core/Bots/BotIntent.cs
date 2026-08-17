namespace Game.Core.Bots
{
    public enum BotIntent : byte
    {
        Idle,
        Recover,
        ChaseEnemy,
        SimpleAttack,
        PowerAttack,
        PushEnvironment
    }
}
