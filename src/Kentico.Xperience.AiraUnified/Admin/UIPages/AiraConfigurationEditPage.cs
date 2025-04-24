using CMS.DataEngine;
using CMS.Membership;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using Kentico.Xperience.AiraUnified.Admin.InfoModels;
using Kentico.Xperience.AiraUnified.Admin.UIPages;

[assembly: UIPage(
    parentType: typeof(AiraUnifiedApplicationPage),
    uiPageType: typeof(AiraUnifiedConfigurationEditPage),
    slug: "configuration",
    name: "{$AiraUnifiedConfigurationEditPage.Name$}",
    templateName: TemplateNames.EDIT,
    order: UIPageOrder.NoOrder)]

namespace Kentico.Xperience.AiraUnified.Admin.UIPages;

[UIEvaluatePermission(SystemPermissions.UPDATE)]
internal sealed class AiraUnifiedConfigurationEditPage : ModelEditPage<AiraUnifiedConfigurationModel>
{
    private AiraUnifiedConfigurationModel? model = null;
    private readonly IAiraUnifiedConfigurationService airaUnifiedConfigurationService;
    private readonly IInfoProvider<AiraUnifiedConfigurationItemInfo> airaUnifiedConfigurationProvider;

    private const string AiraUnifiedConfigurationUpdated = "Aira unified configuration updated.";
    private const string AiraUnifiedConfigurationNotUpdated = "Could not update aira unified configuration.";

    public AiraUnifiedConfigurationEditPage(Xperience.Admin.Base.Forms.Internal.IFormItemCollectionProvider formItemCollectionProvider,
        IFormDataBinder formDataBinder,
        IAiraUnifiedConfigurationService airaUnifiedConfigurationService,
        IInfoProvider<AiraUnifiedConfigurationItemInfo> airaUnifiedConfigurationProvider)
        : base(formItemCollectionProvider, formDataBinder)
    {
        this.airaUnifiedConfigurationService = airaUnifiedConfigurationService;
        this.airaUnifiedConfigurationProvider = airaUnifiedConfigurationProvider;
    }


    protected override AiraUnifiedConfigurationModel Model
    {
        get
        {
            var configurationInfo = airaUnifiedConfigurationProvider.Get().TopN(2).GetEnumerableTypedResult().SingleOrDefault();

            model ??= new AiraUnifiedConfigurationModel(configurationInfo ?? new());

            return model;
        }
    }


    protected override async Task<ICommandResponse> ProcessFormData(AiraUnifiedConfigurationModel model, ICollection<IFormItem> formItems)
    {
        if (!model.RelativePathBase.IsValidSubPath())
        {
            return ResponseFrom(new FormSubmissionResult(
                FormSubmissionStatus.ValidationFailure
            )).AddErrorMessage($"{model.RelativePathBase} is not a valid path.");
        }

        var result = await airaUnifiedConfigurationService.TrySaveOrUpdateConfiguration(model);

        var response = ResponseFrom(new FormSubmissionResult(
            result
            ? FormSubmissionStatus.ValidationSuccess
            : FormSubmissionStatus.ValidationFailure
        ));

        if (result)
        {
            response.AddSuccessMessage(AiraUnifiedConfigurationUpdated);
        }
        else
        {
            response.AddErrorMessage(AiraUnifiedConfigurationNotUpdated);
        }

        return response;
    }
}
