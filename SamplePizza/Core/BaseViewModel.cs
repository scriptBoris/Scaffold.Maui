using ScaffoldLib.Maui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplePizza.Core;

public class BaseViewModel<TView> : BaseViewModel 
    where TView : View, new()
{
    private TView? _view;

    public BaseViewModel()
    {
    }

    public new TView View => (TView)ResolveView();

    protected override View ResolveView()
    {
        if (_view == null)
        {
            _view = new TView();
            _view.BindingContext = this;
        }
        return _view;
    }
}

public abstract class BaseViewModel : BaseNotify
{
    public BaseViewModel()
    {
    }

    public bool IsBusy { get; }
    public View View => ResolveView();

    protected abstract View ResolveView();

    public Task<bool> ShowAlert(string title, string message, string ok, string cancel)
    {
        return View.GetContext()!.DisplayAlert(title, message, ok, cancel);
    }

    public Task ShowError(string message)
    {
        return View.GetContext()!.DisplayAlert("Error", message, "OK");
    }

    public void ShowToast(string title, string message)
    {
        View.GetContext()!.Toast(title, message, TimeSpan.FromSeconds(4));
    }

    public Task ReplaceTo(BaseViewModel viewModel)
    {
        return View.GetContext()!.ReplaceView(View, viewModel.View);
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
