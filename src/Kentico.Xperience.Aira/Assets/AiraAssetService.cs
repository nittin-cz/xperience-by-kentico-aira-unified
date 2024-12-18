using System.Text.Json;

using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;

using Kentico.Xperience.Aira.Admin;
using Kentico.Xperience.Aira.Admin.InfoModels;

using Microsoft.AspNetCore.Http;

using File = CMS.IO.File;
using Path = CMS.IO.Path;

namespace Kentico.Xperience.Aira.Assets;

public interface IAiraAiraAssetService
{
    Task HandleFileUpload(IFormFileCollection files, int userId);
    Task GetUsersUploadedAssets(int userId);
}

internal class AiraAssetService : IAiraAiraAssetService
{
    private readonly IInfoProvider<ContentLanguageInfo> contentLanguageProvider;
    private readonly IInfoProvider<SettingsKeyInfo> settingsKeyProvider;
    private readonly IInfoProvider<AiraChatContentItemAssetReferenceInfo> airaChatContentItemAssetReferenceProvider;

    public AiraAssetService(IInfoProvider<ContentLanguageInfo> contentLanguageProvider,
        IInfoProvider<SettingsKeyInfo> settingsKeyProvider,
        IInfoProvider<AiraChatContentItemAssetReferenceInfo> airaChatContentItemAssetReferenceProvider)
    {
        this.contentLanguageProvider = contentLanguageProvider;
        this.settingsKeyProvider = settingsKeyProvider;
        this.airaChatContentItemAssetReferenceProvider = airaChatContentItemAssetReferenceProvider;
    }

    public async Task GetUsersUploadedAssets(int userId)
    {
        var contentItemAssetReferences = (await airaChatContentItemAssetReferenceProvider
            .Get()
            .Source(x => x.InnerJoin<DataClassInfo>(
                nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceContentTypeDataClassInfoID),
                nameof(DataClassInfo.ClassID)
            ))
            .WhereEquals(nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceUserID), userId)
            .Columns(nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceContentItemID),
                nameof(DataClassInfo.ClassName),
                nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceContentTypeAssetFieldName),
                nameof(AiraChatContentItemAssetReferenceInfo.AiraChatContentItemAssetReferenceUploadTime))
            .GetDataContainerResultAsync())
            .GroupBy(x => x[nameof(DataClassInfo.ClassName)] as string);

        //var builder = new ContentItemQueryBuilder();
        //builder.ForContentTypes(parameters => parameters
        //    .OfContentType(contentItemAssetReferences
        //    .Select(x => x.Key)
        //    .ToArray()
        //    )
        //).Parameters(parameters =>
        //{
        //    parameters.Where(where => where.WhereEquals())
        //});
    }

    public async Task HandleFileUpload(IFormFileCollection files, int userId)
    {
        var massAssetUploadConfiguration = (await settingsKeyProvider
            .Get()
            .WhereEquals(nameof(SettingsKeyInfo.KeyName), AiraConstants.MassAssetUploadConfigurationKey)
            .GetEnumerableTypedResultAsync())
            .First();

        var contentTypeInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(massAssetUploadConfiguration.KeyValue) ??
            throw new InvalidOperationException("No content type is configured for mass upload.");

        var contentTypeGuid = Guid.Parse(contentTypeInfo["ContentTypeGuid"]);

        var contentType = (await DataClassInfoProvider.ProviderObject
            .Get()
            .WhereEquals(nameof(DataClassInfo.ClassGUID), contentTypeGuid)
            .GetEnumerableTypedResultAsync())
            .Single();

        string contentItemAssetColumnCodeName = contentTypeInfo["AssetFieldName"];

        string languageName = (await contentLanguageProvider
            .Get()
            .WhereEquals(nameof(ContentLanguageInfo.ContentLanguageIsDefault), true)
            .GetEnumerableTypedResultAsync())
            .First()
            .ContentLanguageName;

        foreach (var file in files)
        {
            var createContentItemParameters = new CreateContentItemParameters(contentType.ClassName, null, file.FileName, languageName, "KenticoDefault");

            int contentItemId = await CreateContentAssetItem(createContentItemParameters, file, userId, contentItemAssetColumnCodeName);
            CreateContentAssetItemChatReference(userId, contentType.ClassID, contentItemAssetColumnCodeName, contentItemId);
        }
    }

    /// <summary>
    /// ID of newly created content item assset.
    /// </summary>
    /// <param name="createContentItemParameters"></param>
    /// <param name="file"></param>
    /// <param name="userId"></param>
    /// <param name="contentItemAssetColumnCodeName"></param>
    /// <returns></returns>
    private async Task<int> CreateContentAssetItem(CreateContentItemParameters createContentItemParameters, IFormFile file, int userId, string contentItemAssetColumnCodeName)
    {
        var contentItemManager = Service.Resolve<IContentItemManagerFactory>().Create(userId);

        var tempDirectory = Directory.CreateTempSubdirectory();

        string tempFilePath = Path.Combine(tempDirectory.FullName, file.FileName);
        using var fileStream = File.Create(tempFilePath);
        await file.CopyToAsync(fileStream);

        fileStream.Seek(0, SeekOrigin.Begin);

        var assetMetadata = new ContentItemAssetMetadata()
        {
            Extension = Path.GetExtension(tempFilePath),
            Identifier = Guid.NewGuid(),
            LastModified = DateTime.Now,
            Name = Path.GetFileName(tempFilePath),
            Size = fileStream.Length
        };

        var fileSource = new ContentItemAssetStreamSource((CancellationToken cancellationToken) => Task.FromResult<Stream>(fileStream));
        var assetMetadataWithSource = new ContentItemAssetMetadataWithSource(fileSource, assetMetadata);

        var itemData = new ContentItemData(new Dictionary<string, object>{
            { contentItemAssetColumnCodeName, assetMetadataWithSource }
        });

        int contentItemId = await contentItemManager.Create(createContentItemParameters, itemData);
        _ = await contentItemManager.TryPublish(contentItemId, createContentItemParameters.LanguageName);

        File.Delete(tempFilePath);
        tempDirectory.Delete(true);

        return contentItemId;
    }

    private void CreateContentAssetItemChatReference(int userId, int classId, string fieldName, int contentItemId)
    {
        var referenceItem = new AiraChatContentItemAssetReferenceInfo
        {
            AiraChatContentItemAssetReferenceUserID = userId,
            AiraChatContentItemAssetReferenceContentTypeDataClassInfoID = classId,
            AiraChatContentItemAssetReferenceContentTypeAssetFieldName = fieldName,
            AiraChatContentItemAssetReferenceUploadTime = DateTime.Now,
            AiraChatContentItemAssetReferenceContentItemID = contentItemId
        };

        airaChatContentItemAssetReferenceProvider.Set(referenceItem);
    }
}
