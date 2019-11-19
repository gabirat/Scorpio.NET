using Scorpio.Api.DataAccess;
using Scorpio.Api.Models;

namespace Scorpio.Api.Controllers
{
    public class ConfigurationController : CrudController<IUiConfigurationRepository, UiConfiguration>
    {
        public ConfigurationController(IUiConfigurationRepository uiConfigurationRepository) : base(uiConfigurationRepository)
        {
        }
    }
}