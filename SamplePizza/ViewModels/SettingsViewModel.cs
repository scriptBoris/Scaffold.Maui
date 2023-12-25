using SamplePizza.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePizza.ViewModels;

public class SettingsViewModelKey
{
}

public class SettingsViewModel : BaseViewModel<SettingsViewModelKey>
{
    private AppTheme _selectedTheme;

    public SettingsViewModel()
    {
        _selectedTheme = AppInfo.Current.RequestedTheme;
    }

    public List<AppTheme> AppThemes { get; set; } = new()
    {
        AppTheme.Dark,
        AppTheme.Light,
        AppTheme.Unspecified,
    };

    public AppTheme SelectedTheme
    {
        get => _selectedTheme;
        set
        {
            _selectedTheme = value;
            App.Current!.UserAppTheme = value;
        }
    }
}
