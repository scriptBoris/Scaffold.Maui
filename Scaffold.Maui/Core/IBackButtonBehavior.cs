namespace ScaffoldLib.Maui.Core;

public interface IBackButtonBehavior
{
    View? LeftViewExtended(IScaffold context);
    bool? OverrideSoftwareBackButtonAction(IScaffold context);
    bool? OverrideHardwareBackButtonAction(IScaffold context);

    bool? OverrideBackButtonVisibility(IScaffold context);
    ImageSource? OverrideBackButtonIcon(IScaffold context);
}
