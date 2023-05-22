
public interface TransitionListener
{

    void OnPortalEnter(Transition transitioning);

    void OnPortalTransitioning(Transition transitioning);

    void OnPortalExit(Transition transitioning);
    void OnPortalCrossed(Transition transitioning);

}
