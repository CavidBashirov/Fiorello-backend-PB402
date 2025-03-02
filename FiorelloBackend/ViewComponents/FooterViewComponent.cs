using FiorelloBackend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FiorelloBackend.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly ISettingService _settingService;

        public FooterViewComponent(ISettingService settingService)
        {
            _settingService = settingService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> datas = await _settingService.GetAllAsync();
            return View(datas);
        }
    }
}
