﻿namespace Kentico.Xperience.AiraUnified.Insights.Models;

/// <summary>
/// Contact group insights model.
/// </summary>
internal sealed class ContactGroupInsightsModel
{
    /// <summary>
    /// Contact group id.
    /// </summary>
    public int Id { get; set; }


    /// <summary>
    /// Contact group name.
    /// </summary>
    public string Name { get; set; } = string.Empty;


    /// <summary>
    /// Contact group dynamic condition.
    /// </summary>
    public string Conditions { get; set; } = string.Empty;


    /// <summary>
    /// Contact group dynamic count.
    /// </summary>
    public int Count { get; set; }
}
