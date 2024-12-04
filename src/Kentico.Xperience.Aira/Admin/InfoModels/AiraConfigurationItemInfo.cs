using System.Data;
using System.Runtime.Serialization;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;

using Kentico.Xperience.Aira.Admin.InfoModels;

[assembly: RegisterObjectType(typeof(AiraConfigurationItemInfo), AiraConfigurationItemInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.Aira.Admin.InfoModels;

/// <summary>
/// Data container class for <see cref="AiraConfigurationItemInfo"/>.
/// </summary>
[Serializable]
public partial class AiraConfigurationItemInfo : AbstractInfo<AiraConfigurationItemInfo, IInfoProvider<AiraConfigurationItemInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoaira.airaconfigurationitem";

    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraConfigurationItemInfo>), OBJECT_TYPE, "KenticoAira.AiraConfigurationItem", nameof(AiraConfigurationItemId), null, nameof(AiraConfigurationItemGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        ContinuousIntegrationSettings =
        {
            Enabled = true
        }
    };


    [DatabaseField]
    public virtual int AiraConfigurationItemId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraConfigurationItemId)), 0);
        set => SetValue(nameof(AiraConfigurationItemId), value);
    }


    public virtual Guid AiraConfigurationItemGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraConfigurationItemGuid)), default);
        set => SetValue(nameof(AiraConfigurationItemGuid), value);
    }


    public virtual string AiraConfigurationItemAiraPathBase
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraConfigurationItemAiraPathBase)), string.Empty);
        set => SetValue(nameof(AiraConfigurationItemAiraPathBase), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    protected AiraConfigurationItemInfo(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }


    public AiraConfigurationItemInfo()
        : base(TYPEINFO)
    {
    }


    public AiraConfigurationItemInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
