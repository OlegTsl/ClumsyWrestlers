namespace Game.Core.Bots
{
    public interface IBotBehaviorSettings
    {
        float DecisionInterval { get; }
        float PerceptionRadius { get; }
        float TargetSwitchBias { get; }
        float ArrivalDistance { get; }
        float NavigationSampleDistance { get; }
        float EdgeRecoveryDistance { get; }
        float SimpleAttackDistanceMultiplier { get; }
        float PowerAttackMinimumDistance { get; }
        float PowerAttackMaximumDistance { get; }
        float AttackCooldown { get; }
        float PowerAttackHoldDuration { get; }
        float ChaseWeight { get; }
        float SimpleAttackWeight { get; }
        float PowerAttackWeight { get; }
        float EnvironmentWeight { get; }
        float PushSearchRadius { get; }
        float PushEnemyMaximumDistance { get; }
        float PushStandOffDistance { get; }
        float PushLaneRadius { get; }
        float PushRequiredOutwardAlignment { get; }
        float JumpCooldown { get; }
        float FacingDotThreshold { get; }
        float TurnInputMagnitude { get; }
    }
}
