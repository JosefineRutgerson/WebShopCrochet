using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebShop.ViewComponents
{
    
    public class NavigationViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {

            var navigationItems = new[]
            {
                new ItemViewModel("Home", Url.Content("Home")),
                new ItemViewModel("Contact", Url.Content("Contact")),
                new ItemViewModel("About", Url.Content("Product"))
            };

            var viewModel = new ViewModel(navigationItems);

            return View(viewModel);
        }

        public class ViewModel
        {
            public IList<ItemViewModel> NavigationItems { get; }

            public ViewModel(IList<ItemViewModel> navigationItems)
            {
                NavigationItems = navigationItems;
            }
        }

        public class ItemViewModel
        {
            public string Name { get; }
            public string TargetUrl { get; }

            public ItemViewModel(string name, string targetUrl)
            {
                Name = name;
                TargetUrl = targetUrl;
            }
        }
    }

    
}
