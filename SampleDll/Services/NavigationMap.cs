using Microsoft.Extensions.Logging;
using SampleDll.Core;
using SampleDll.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SampleDll.Services
{
    public interface INavigationMap
    {
        BaseViewModel Resolve(object navigationKey);
    }

    public class NavigationMap : INavigationMap
    {
        private static readonly Dictionary<Type, NavigationPair> _types = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NavigationMap> _logger;

        public NavigationMap(IServiceProvider serviceProvider, ILogger<NavigationMap> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        static NavigationMap()
        {
            RegisterNavigation<LoginViewModel, Views.LoginView>();
            RegisterNavigation<AccountViewModel, Views.AccountView>();
            RegisterNavigation<CartViewModel, Views.CartView>();
            RegisterNavigation<ConfirmEmailViewModel, Views.ConfirmEmailView>();
            RegisterNavigation<HomeViewModel, Views.HomeView>();
            RegisterNavigation<InfoViewModel, Views.InfoView>();
            RegisterNavigation<MasterViewModel, Views.MasterView>();
            RegisterNavigation<PizzaViewModel, Views.PizzaView>();
            RegisterNavigation<RegisterViewModel, Views.RegisterView>();
            RegisterNavigation<SettingsViewModel, Views.SettingsView>();
            RegisterNavigation<SupportViewModel, Views.SupportView>();
        }

        public static void RegisterNavigation<TViewModel, TView>()
            where TViewModel : BaseViewModel
            where TView : View, new()
        {
            var viewModelType = typeof(TViewModel);
            var viewType = typeof(TView);
            var baseViewModelType = viewModelType.BaseType;

            if (baseViewModelType.IsGenericType && baseViewModelType.GetGenericTypeDefinition() == typeof(BaseViewModel<>))
            {
                var navigationViewModelType = baseViewModelType.GetGenericArguments()[0];
                _types.Add(navigationViewModelType, new NavigationPair
                {
                    View = viewType,
                    ViewModel = viewModelType,
                });
            }
            else
            {
                throw new ArgumentException($"{viewModelType.Name} должен быть унаследован от BaseViewModel<>.");
            }
        }

        public BaseViewModel Resolve(object navigationKey)
        {
            var typeKey = navigationKey.GetType();
            var pair = _types[typeKey];

            var ctor = pair.ViewModel.GetConstructors().First();
            var ctorParams = ctor.GetParameters();
            var parameters = new List<object>();

            // fetch services
            foreach (var item in ctorParams)
            {
                object? dependency = _serviceProvider.GetService(item.ParameterType);
                if (dependency == null)
                {
                    _logger.LogError($"Not found dependency service: {item.ParameterType.Name}");
                    throw new Exception($"Not found dependency service: {item.ParameterType.Name}");
                }

                parameters.Add(dependency);
            }

            var vm = (BaseViewModel)FormatterServices.GetUninitializedObject(pair.ViewModel);
            vm.ServiceProvider = _serviceProvider;
            vm.Args = navigationKey;
            vm.TypeView = pair.View;

            // call ctor
            ctor.Invoke(vm, parameters.ToArray());

            return vm;
        }

        private class NavigationPair
        {
            public required Type ViewModel { get; set; }
            public required Type View { get; set; }
        }
    }
}
