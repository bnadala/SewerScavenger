using SewerScavenger.Models;

namespace SewerScavenger.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {

        public string LocationString =>Data.Location.ToString();

        public Item Data { get; set; }
        public ItemDetailViewModel(Item data = null)
        {
            Title = data?.Name;
            Data = data;
        }
    }
}
