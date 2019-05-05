using System.Web.Mvc;
using Kendo.Mvc.ModelBinders;
using Kendo.Mvc.UI;

namespace Kendo.Mvc.UI
{
    public class CustomDataSourceRequestAttribute : DataSourceRequestAttribute
    {
        public override IModelBinder GetBinder()
        {
            return new CustomDataSourceRequestModelBinder();
        }
    }
}