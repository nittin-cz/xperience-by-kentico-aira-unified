﻿using System.ComponentModel.DataAnnotations;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.Workspaces;

using Kentico.Xperience.AiraUnified.Admin.InfoModels;

namespace Kentico.Xperience.AiraUnified.Admin;

/// <summary>
/// Represents a module installer for Aira Unified.
/// </summary>
internal sealed class AiraUnifiedModuleInstaller : IAiraUnifiedModuleInstaller
{
    private readonly IInfoProvider<ResourceInfo> resourceInfoProvider;


    /// <summary>
    /// Initializes a new instance of the AiraUnifiedModuleInstaller class.
    /// </summary>
    /// <param name="resourceInfoProvider">The provider for ResourceInfo objects.</param>
    public AiraUnifiedModuleInstaller(IInfoProvider<ResourceInfo> resourceInfoProvider)
        => this.resourceInfoProvider = resourceInfoProvider;


    /// <inheritdoc />
    public void Install()
    {
        var resourceInfo = InstallModule();
        InstallModuleClasses(resourceInfo);
    }


    private ResourceInfo InstallModule()
    {
        var resourceInfo = resourceInfoProvider.Get(AiraUnifiedConstants.ResourceName)
            ?? new ResourceInfo();

        resourceInfo.ResourceDisplayName = AiraUnifiedConstants.ResourceDisplayName;
        resourceInfo.ResourceName = AiraUnifiedConstants.ResourceName;
        resourceInfo.ResourceDescription = AiraUnifiedConstants.ResourceDescription;
        resourceInfo.ResourceIsInDevelopment = AiraUnifiedConstants.ResourceIsInDevelopment;
        if (resourceInfo.HasChanged)
        {
            resourceInfoProvider.Set(resourceInfo);
        }

        return resourceInfo;
    }


    private static void InstallModuleClasses(ResourceInfo resourceInfo)
    {
        InstallAiraUnifiedClass(
            resourceInfo,
            AiraUnifiedConfigurationItemInfo.TYPEINFO.ObjectClassName,
            AiraUnifiedConfigurationItemInfo.OBJECT_TYPE,
            classDisplayName: "Aira Unified Configuration Item",
            typeof(AiraUnifiedConfigurationItemInfo),
            nameof(AiraUnifiedConfigurationItemInfo.AiraUnifiedConfigurationItemId),
            [
                new FormFieldModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = WorkspaceInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraUnifiedConfigurationItemInfo.AiraUnifiedConfigurationWorkspaceName)
                }
            ]
        );

        InstallAiraUnifiedClass(
            resourceInfo,
            AiraUnifiedChatPromptGroupInfo.TYPEINFO.ObjectClassName,
            AiraUnifiedChatPromptGroupInfo.OBJECT_TYPE,
            classDisplayName: "Aira Unified Chat Prompt Group",
            typeof(AiraUnifiedChatPromptGroupInfo),
            nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupId),
            [
                new FormFieldModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = UserInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupUserId)
                },
                new FormFieldModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = AiraUnifiedChatThreadInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraUnifiedChatPromptGroupInfo.AiraUnifiedChatPromptGroupThreadId)
                }
            ]
        );

        InstallAiraUnifiedClass(
            resourceInfo,
            AiraUnifiedChatPromptInfo.TYPEINFO.ObjectClassName,
            AiraUnifiedChatPromptInfo.OBJECT_TYPE,
            classDisplayName: "Aira Unified Chat Prompt",
            typeof(AiraUnifiedChatPromptInfo),
            nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptId),
            [
                new FormFieldModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = AiraUnifiedChatPromptGroupInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptChatPromptGroupId)
                },
                new FormFieldModel
                {
                    FormFieldName = nameof(AiraUnifiedChatPromptInfo.AiraUnifiedChatPromptText),
                    FormFieldType = FieldDataType.LongText
                }
            ]
        );

        InstallAiraUnifiedClass(
            resourceInfo,
            AiraUnifiedChatMessageInfo.TYPEINFO.ObjectClassName,
            AiraUnifiedChatMessageInfo.OBJECT_TYPE,
            classDisplayName: "Aira Unified Chat Message",
            typeof(AiraUnifiedChatMessageInfo),
            nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageId),
            [
                new FormFieldModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = UserInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageUserId)
                },
                new FormFieldModel
                {
                    FormFieldName = nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageText),
                    FormFieldType = FieldDataType.LongText
                },
                new FormFieldModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = AiraUnifiedChatThreadInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraUnifiedChatMessageInfo.AiraUnifiedChatMessageThreadId)
                }
            ]
        );

        InstallAiraUnifiedClass(
            resourceInfo,
            AiraUnifiedChatThreadInfo.TYPEINFO.ObjectClassName,
            AiraUnifiedChatThreadInfo.OBJECT_TYPE,
            classDisplayName: "Aira Unified Chat Thread",
            typeof(AiraUnifiedChatThreadInfo),
            nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadId),
            [
                new FormFieldModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = UserInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadUserId)
                },
                new FormFieldModel
                {
                    ReferenceType = ObjectDependencyEnum.NotRequired,
                    ReferenceToObjectType = AiraUnifiedChatMessageInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraUnifiedChatThreadInfo.AiraUnifiedChatThreadLastMessageId)
                }
            ]
        );

        InstallAiraUnifiedClass(
            resourceInfo,
            AiraUnifiedChatSummaryInfo.TYPEINFO.ObjectClassName,
            AiraUnifiedChatSummaryInfo.OBJECT_TYPE,
            classDisplayName: "Aira Unified Chat Summary",
            typeof(AiraUnifiedChatSummaryInfo),
            nameof(AiraUnifiedChatSummaryInfo.AiraUnifiedChatSummaryId),
            [
                new FormFieldModel
                {
                    ReferenceType = ObjectDependencyEnum.Required,
                    ReferenceToObjectType = UserInfo.OBJECT_TYPE,
                    FormFieldName = nameof(AiraUnifiedChatSummaryInfo.AiraUnifiedChatSummaryUserId)
                },
                new FormFieldModel
                {
                    FormFieldType = FieldDataType.LongText,
                    FormFieldName = nameof(AiraUnifiedChatSummaryInfo.AiraUnifiedChatSummaryContent)
                }
            ]
        );
    }


    private static void InstallAiraUnifiedClass(ResourceInfo resourceInfo,
        string objectClassName,
        string objectType,
        string classDisplayName,
        Type infoType,
        string idPropertyName,
        List<FormFieldModel>? dependencies = null)
    {
        var info = DataClassInfoProvider.GetDataClassInfo(objectType) ??
            DataClassInfo.New(objectType);

        info.ClassName = objectClassName;
        info.ClassTableName = objectClassName.Replace(".", "_");
        info.ClassDisplayName = classDisplayName;
        info.ClassResourceID = resourceInfo.ResourceID;
        info.ClassType = ClassType.OTHER;

        SetFormDefinition(info, infoType, idPropertyName, dependencies);
    }


    private sealed class FormFieldModel
    {
        public ObjectDependencyEnum ReferenceType { get; set; }
        public string FormFieldName { get; set; } = string.Empty;
        public string? ReferenceToObjectType { get; set; }
        public string? FormFieldType { get; set; }
    }


    private static void SetFormDefinition(DataClassInfo info, Type infoType, string idPropertyName, List<FormFieldModel>? dependencies = null)
    {
        var formInfo = FormHelper.GetBasicFormDefinition(idPropertyName);

        formInfo = AddFormItems(formInfo, infoType, idPropertyName, dependencies);

        if (info.ClassID > 0)
        {
            var existingForm = new FormInfo(info.ClassFormDefinition);
            existingForm.CombineWithForm(formInfo, new());
            info.ClassFormDefinition = existingForm.GetXmlDefinition();
        }
        else
        {
            info.ClassFormDefinition = formInfo.GetXmlDefinition();
        }

        if (info.HasChanged)
        {
            DataClassInfoProvider.SetDataClassInfo(info);
        }
    }


    private static FormInfo AddFormItems(FormInfo formInfo, Type infoType, string idPropertyName, List<FormFieldModel>? formFieldModels = null)
    {
        var properties = infoType.GetProperties();

        foreach (var property in properties)
        {
            if (string.Equals(property.Name, idPropertyName))
            {
                continue; // Exclude Id from the loop
            }

            if (property.GetCustomAttributes(typeof(DatabaseFieldAttribute), true).FirstOrDefault() is DatabaseFieldAttribute)
            {
                var formItem = new FormFieldInfo()
                {
                    Name = property.Name,
                    Visible = true,
                    DataType = FieldDataType.Text,
                    Enabled = true,
                    AllowEmpty = !property.IsDefined(typeof(RequiredAttribute), true) // Set AllowEmpty to true if the property has the Required attribute
                };

                // Map the property type to the appropriate FieldDataType
                formItem.DataType = property.PropertyType switch
                {
                    Type t when t == typeof(string) => FieldDataType.Text,
                    Type t when t == typeof(int) => FieldDataType.Integer,
                    Type t when t == typeof(DateTime) => FieldDataType.DateTime,
                    Type t when t == typeof(Guid) => FieldDataType.Guid,
                    Type t when t == typeof(bool) => FieldDataType.Boolean,
                    _ => formItem.DataType // Default case if no match is found
                };

                if (formFieldModels is not null)
                {
                    var formFieldModel = formFieldModels.FirstOrDefault(x => x.FormFieldName == property.Name);

                    if (formFieldModel is null)
                    {
                        formInfo.AddFormItem(formItem);
                        continue;
                    }

                    if (formFieldModel.ReferenceToObjectType is not null)
                    {
                        formItem.ReferenceToObjectType = formFieldModel.ReferenceToObjectType;
                        formItem.ReferenceType = formFieldModel.ReferenceType;
                    }

                    if (formFieldModel.FormFieldType is not null)
                    {
                        formItem.DataType = formFieldModel.FormFieldType;
                    }
                }

                formInfo.AddFormItem(formItem);
            }
        }

        return formInfo;
    }
}
