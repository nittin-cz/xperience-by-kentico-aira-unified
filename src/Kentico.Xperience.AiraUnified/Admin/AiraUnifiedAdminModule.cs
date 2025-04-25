﻿using CMS;
using CMS.Core;

using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.AiraUnified.Admin;

[assembly: RegisterModule(typeof(AiraUnifiedAdminModule))]

namespace Kentico.Xperience.AiraUnified.Admin;

/// <summary>
/// Manages administration features and integration.
/// </summary>
internal sealed class AiraUnifiedAdminModule() : AdminModule(nameof(AiraUnifiedAdminModule))
{
    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit(parameters);

        RegisterClientModule("kentico", "xperience-integrations-aira-unified");
    }
}
