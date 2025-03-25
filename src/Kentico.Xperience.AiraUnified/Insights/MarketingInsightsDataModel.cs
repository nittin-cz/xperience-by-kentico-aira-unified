namespace Kentico.Xperience.AiraUnified.Insights
{
    public class MarketingInsightsDataModel
    {
        public ContactsSummaryModel Contacts { get; set; } = new ContactsSummaryModel();
        public List<ContactGroupModel> ContactGroups { get; set; } = new List<ContactGroupModel>();
    }
}
