namespace InventoryApi.Models
{
    public class HomeDto
    {
        public string Title { get; set; }
        public string BodySw { get; set; }
        public string BodyLink { get; set; }

        public HomeDto(string title, string bodySw, string bodyLink)
        {
            Title = title;
            BodySw = bodySw;
            BodyLink = bodyLink;
        }
    }
}