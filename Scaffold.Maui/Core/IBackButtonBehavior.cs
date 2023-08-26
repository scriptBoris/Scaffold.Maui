namespace ScaffoldLib.Maui.Core;

public interface IBackButtonBehavior
{
    bool? OverrideSoftwareBackButtonAction(IAgent agent, IScaffold context);
    bool? OverrideHardwareBackButtonAction(IAgent agent, IScaffold context);

    bool? OverrideBackButtonVisibility(IAgent agent, IScaffold context);
    ImageSource? OverrideBackButtonIcon(IAgent agent, IScaffold context);
}
