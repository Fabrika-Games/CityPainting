using Exoa.Designer;
using Exoa.Events;
using Lean.Common;

public class FocusOnClick : LeanSelectableBehaviour
{
    public bool follow;
    public bool focusOnFollow;

    protected override void OnSelected(LeanSelect select)
    {
        print("OnSelect " + gameObject.name);
        if (follow)
            CameraEvents.OnRequestObjectFollow?.Invoke(gameObject, focusOnFollow);
        else
            CameraEvents.OnRequestObjectFocus?.Invoke(gameObject.GetBoundsRecursive());
    }

    protected override void OnDeselected(LeanSelect select)
    {
        print("OnDeselect " + gameObject.name);
        if (follow)
            CameraEvents.OnRequestObjectFollow?.Invoke(null, focusOnFollow);
    }
}