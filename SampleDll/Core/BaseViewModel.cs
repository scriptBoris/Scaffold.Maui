﻿using Microsoft.Extensions.Logging;
using SampleDll.Services;
using ScaffoldLib.Maui;
using ScaffoldLib.Maui.Args;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                _view = (View)Activator.CreateInstance(TypeView)!;
                _view.BindingContext = this;

                stopwatch.Stop();

                var elapsedTime = stopwatch.Elapsed;
                Console.WriteLine($"Прошло времени: {elapsedTime}");

                long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"Прошло времени в миллисекундах: {elapsedMilliseconds}");

                long elapsedTicks = stopwatch.ElapsedTicks;
                Console.WriteLine($"Прошло времени в тиках: {elapsedTicks}");

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

    public Task ShowAlert(string title, string message, string ok)
    {
        return View.GetContext()!.DisplayAlert(new DisplayAlertArgs
        {
            Title = title,
            Description = message,
            Ok = ok,
        });
    }

    public Task<bool> ShowAlert(string title, string message, string ok, string cancel)
    {
        return View.GetContext()!.DisplayAlert(new DisplayAlertArgs2
        {
            Title = title,
            Description = message,
            Ok = ok,
            Cancel = cancel,
        });
    }

    public Task ShowError(string message)
    {
        return View.GetContext()!.DisplayAlert(new DisplayAlertArgs
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

    public Task<BaseViewModel> GoTo(object viewModelKey)
    {
        var vm = ServiceProvider.GetRequiredService<INavigationMap>().Resolve(viewModelKey);
        return View
            .GetContext()!
            .PushAsync(vm.View)
            .ContinueWith(x => vm);
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
