namespace ScaffoldLib.Maui.Core;

public interface IBackButtonBehavior
{
    bool? OverrideSoftwareBackButtonAction(IScaffold context);
    bool? OverrideHardwareBackButtonAction(IScaffold context);

    bool? OverrideBackButtonVisibility(IScaffold context);
    ImageSource? OverrideBackButtonIcon(IScaffold context);
}
