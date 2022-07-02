using DiplomaApp.Models;

namespace DiplomaApp.ViewModels
{
    public class RequestChangeStatus
    {
        public string Theme { get; set; }
        public string Text { get; set; }
        public int RequestStatusId { get; set; }
        public RequestChangeStatus()
        {

        }
    }
}
