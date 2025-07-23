using UnityEngine;

public abstract class TutorialCondition : ScriptableObject, ITutorialCondition
{
    public virtual void Initialize() { }
    public abstract bool IsSatisfied();
}
