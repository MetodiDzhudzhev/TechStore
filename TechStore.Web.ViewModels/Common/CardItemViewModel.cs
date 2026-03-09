namespace TechStore.Web.ViewModels.Common
{
    public class CardItemViewModel
    {
        public string Name { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public string DetailsUrl { get; set; } = null!;
        public string EditUrl { get; set; } = null!;
        public string DeleteUrl { get; set; } = null!;
        public bool ShowName { get; set; }
    }
}
