using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Catalog;

/// <summary>
/// Represents a product attribute search model
/// </summary>
public partial record ProductAttributeSearchModel : BaseSearchModel
{
    #region Properties

    [NopResourceDisplayName("Admin.Catalog.ProductAttribute.List.SearchProductAttributeName")]
    public string SearchProductAttributeName { get; set; }

    #endregion
}