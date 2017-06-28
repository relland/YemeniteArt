using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Linq.Expressions;

namespace Nop.Plugin.Widgets.AllInOne.Models
{
    public class AllInOneValidator : BaseNopValidator<AllInOneModel>
    {
        public AllInOneValidator(ILocalizationService localizationService)
        {
            base.RuleFor<string>((AllInOneModel x) => x.Name).NotNull<AllInOneModel, string>().WithMessage<AllInOneModel, string>(localizationService.GetResource("Plugins.Widgets.AllInOne.Fields.Name.Required"));
            base.RuleFor<string>((AllInOneModel x) => x.WidgetZone).NotNull<AllInOneModel, string>().WithMessage<AllInOneModel, string>(localizationService.GetResource("Plugins.Widgets.AllInOne.Fields.WidgetZone.Required"));
        }
    }
}