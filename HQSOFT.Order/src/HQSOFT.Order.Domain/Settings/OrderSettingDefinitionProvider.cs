using Volo.Abp.Settings;

namespace HQSOFT.Order.Settings;

public class OrderSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(OrderSettings.MySetting1));
    }
}
