using DiplomaApp.Models;
using X.PagedList;

namespace DiplomaApp.ViewModels
{
    public class RequestIndex
    {
        public IPagedList<RequestViewModel> Requests { get; set; }
        public RequestIndex()
        {

        }
    }
    public class RequestViewModel
    {
        public int Id { get; set; }
        public string Theme { get; set; }
        public string Text { get; set; }
        public DateTime CreationDate { get; set; }
        public RequestStatusViewModel Status { get; set; }
        public RequestViewModel()
        {

        }
    }
    public class RequestStatusViewModel
    {
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }
        public RequestStatusViewModel()
        {

        }
    }
}
