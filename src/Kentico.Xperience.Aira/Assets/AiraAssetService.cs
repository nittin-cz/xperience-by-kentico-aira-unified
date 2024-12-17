using System.Text.Json;

using CMS.ContentEngine;
using CMS.Core;
using CMS.DataEngine;

using Kentico.Xperience.Aira.Admin;

using Microsoft.AspNetCore.Http;

using File = CMS.IO.File;
using Path = CMS.IO.Path;

namespace Kentico.Xperience.Aira.Assets;

public interface IAiraAiraAssetService
{
    Task HandleFileUpload(IFormFileCollection files, int userId);
}

internal class AiraAssetService : IAiraAiraAssetService
{
    private readonly IInfoProvider<ContentLanguageInfo> contentLanguageProvider;
    private readonly IInfoProvider<SettingsKeyInfo> settingsKeyProvider;

    public AiraAssetService(IInfoProvider<ContentLanguageInfo> contentLanguageProvider, IInfoProvider<SettingsKeyInfo> settingsKeyProvider)
    {
        this.contentLanguageProvider = contentLanguageProvider;
        this.settingsKeyProvider = settingsKeyProvider;
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

        string contentTypeName = (await DataClassInfoProvider.ProviderObject
            .Get()
            .WhereEquals(nameof(DataClassInfo.ClassGUID), contentTypeGuid)
            .GetEnumerableTypedResultAsync())
            .Single()
            .ClassName;

        string contentItemAssetColumnCodeName = contentTypeInfo["AssetFieldName"];

        string languageName = (await contentLanguageProvider
            .Get()
            .WhereEquals(nameof(ContentLanguageInfo.ContentLanguageIsDefault), true)
            .GetEnumerableTypedResultAsync())
            .First()
            .ContentLanguageName;

        foreach (var file in files)
        {
            var createContentItemParameters = new CreateContentItemParameters(contentTypeName, null, file.FileName, languageName, "KenticoDefault");

            await CreateContentAssetItem(createContentItemParameters, file, userId, contentItemAssetColumnCodeName);
        }
    }

    private async Task CreateContentAssetItem(CreateContentItemParameters createContentItemParameters, IFormFile file, int userId, string contentItemAssetColumnCodeName)
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
    }
}
