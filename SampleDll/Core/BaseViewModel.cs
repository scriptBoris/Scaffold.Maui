using Microsoft.Extensions.Logging;
using SampleDll.Services;
using ScaffoldLib.Maui;
using ScaffoldLib.Maui.Args;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleDll.Core;

public class BaseViewModel<TNavKey> : BaseViewModel where TNavKey : notnull
{
    private View? _view;

    public BaseViewModel()
    {
    }

    [NotNull]
    public required new TNavKey Args
    {
        get => (TNavKey)base.Args!;
        set => base.Args = value;
    }

    protected override View ResolveView()
    {
        if (_view == null)
        {
            try
            {
                _view = (View)Activator.CreateInstance(TypeView)!;
                _view.BindingContext = this;
            }
            catch (Exception ex)
            {
                ServiceProvider
                    .GetRequiredService<ILoggerProvider>()
                    .CreateLogger("view_builder")
                    .LogError($"Fail create view. Exception: {ex}");

                throw new Exception("Fail create view", ex);
            }
        }
        return _view;
    }
}

public abstract class BaseViewModel : BaseNotify
{
#pragma warning disable CS8618 
    public BaseViewModel()
#pragma warning restore CS8618 
    {
    }

    public required IServiceProvider ServiceProvider { get; set; }
    public required Type TypeView { get; set; }
    public object Args { get; set; }

    public bool IsBusy { get; }
    public View View => ResolveView();

    protected abstract View ResolveView();

    public Task<bool> ShowAlert(string title, string message, string ok, string cancel)
    {
        return View.GetContext()!.DisplayAlert(new CreateDisplayAlertArgs
        {
            Title = title,
            Description = message,
            Ok = ok,
            Cancel = cancel,
        });
    }

    public Task ShowError(string message)
    {
        return View.GetContext()!.DisplayAlert(new CreateDisplayAlertArgs
        {
            Title = "Error",
            Description = message,
            Ok = "OK",
            Payload = PayloadTypes.Error,
        });
    }

    public void ShowToast(string title, string message)
    {
        View.GetContext()!.Toast(new CreateToastArgs
        {
            Title = title,
            Message = message,
            ShowTime  = TimeSpan.FromSeconds(4),
        });
    }

    public Task ReplaceTo(object viewModelKey)
    {
        var vm = ServiceProvider.GetRequiredService<INavigationMap>().Resolve(viewModelKey);
        return View.GetContext()!.ReplaceView(View, vm.View);
    }

    public Task ReplaceTo(BaseViewModel viewModel)
    {
        return View.GetContext()!.ReplaceView(View, viewModel.View);
    }

    public Task GoTo(object viewModelKey)
    {
        var vm = ServiceProvider.GetRequiredService<INavigationMap>().Resolve(viewModelKey);
        return View.GetContext()!.PushAsync(vm.View);
    }

    public Task GoTo(BaseViewModel viewModel)
    {
        return View.GetContext()!.PushAsync(viewModel.View);
    }

    public Task GoBack()
    {
        return View.GetContext()!.RemoveView(View);
    }

    public Task GoBackToRoot()
    {
        return View.GetContext()!.PopToRootAsync();
    }
}
