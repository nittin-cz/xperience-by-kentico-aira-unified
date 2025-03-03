using System.ComponentModel.DataAnnotations;
using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;

using Kentico.Xperience.AiraUnified.Admin.InfoModels;


[assembly: RegisterObjectType(typeof(AiraUnifiedChatThreadInfo), AiraUnifiedChatThreadInfo.OBJECT_TYPE)]

namespace Kentico.Xperience.AiraUnified.Admin.InfoModels;

/// <summary>
/// Data container class for <see cref="AiraUnifiedChatThreadInfo"/>.
/// </summary>
public class AiraUnifiedChatThreadInfo : AbstractInfo<AiraUnifiedChatThreadInfo, IInfoProvider<AiraUnifiedChatThreadInfo>>
{
    /// <summary>
    /// Object type.
    /// </summary>
    public const string OBJECT_TYPE = "kenticoairaunified.airaunifiedchatthread";


    /// <summary>
    /// Type information.
    /// </summary>
    public static readonly ObjectTypeInfo TYPEINFO = new(typeof(IInfoProvider<AiraUnifiedChatThreadInfo>), OBJECT_TYPE, "KenticoAiraUnified.AiraUnifiedChatThread", nameof(AiraUnifiedChatThreadId), null, nameof(AiraUnifiedChatThreadGuid), null, null, null, null, null)
    {
        TouchCacheDependencies = true,
        DependsOn =
        [
            new(nameof(AiraUnifiedChatThreadUserId), UserInfo.OBJECT_TYPE, ObjectDependencyEnum.Required),
            new(nameof(AiraUnifiedChatThreadLastMessageId), AiraUnifiedChatMessageInfo.OBJECT_TYPE, ObjectDependencyEnum.NotRequired)
        ],
        ContinuousIntegrationSettings =
        {
            Enabled = false
        }
    };


    /// <summary>
    /// Chat thread id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraUnifiedChatThreadId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraUnifiedChatThreadId)), 0);
        set => SetValue(nameof(AiraUnifiedChatThreadId), value);
    }


    /// <summary>
    /// Admin application user id.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual int AiraUnifiedChatThreadUserId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraUnifiedChatThreadUserId)), 0);
        set => SetValue(nameof(AiraUnifiedChatThreadUserId), value);
    }


    /// <summary>
    /// Chat thread guid.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual Guid AiraUnifiedChatThreadGuid
    {
        get => ValidationHelper.GetGuid(GetValue(nameof(AiraUnifiedChatThreadGuid)), Guid.Empty);
        set => SetValue(nameof(AiraUnifiedChatThreadGuid), value);
    }


    /// <summary>
    /// Chat thread name.
    /// </summary>
    [DatabaseField]
    [Required]
    public virtual string AiraUnifiedChatThreadName
    {
        get => ValidationHelper.GetString(GetValue(nameof(AiraUnifiedChatThreadName)), string.Empty);
        set => SetValue(nameof(AiraUnifiedChatThreadName), value);
    }


    /// <summary>
    /// Is chat thread the latest used.
    /// </summary>
    [DatabaseField]
    public bool AiraUnifiedChatThreadIsLatest
    {
        get => ValidationHelper.GetBoolean(GetValue(nameof(AiraUnifiedChatThreadIsLatest)), false);
        set => SetValue(nameof(AiraUnifiedChatThreadIsLatest), value);
    }


    /// <summary>
    /// Last chat message in this thread.
    /// </summary>
    [DatabaseField]
    public int AiraUnifiedChatThreadLastMessageId
    {
        get => ValidationHelper.GetInteger(GetValue(nameof(AiraUnifiedChatThreadLastMessageId)), 0);
        set => SetValue(nameof(AiraUnifiedChatThreadLastMessageId), value);
    }


    /// <summary>
    /// Deletes the object using appropriate provider.
    /// </summary>
    protected override void DeleteObject() => Provider.Delete(this);


    /// <summary>
    /// Updates the object using appropriate provider.
    /// </summary>
    protected override void SetObject() => Provider.Set(this);


    public AiraUnifiedChatThreadInfo()
        : base(TYPEINFO)
    {
    }


    public AiraUnifiedChatThreadInfo(DataRow dr)
        : base(TYPEINFO, dr)
    {
    }
}
