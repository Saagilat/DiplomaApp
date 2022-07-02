using System.ComponentModel.DataAnnotations;

namespace DiplomaApp.Models
{
    public class Request
    {
        public int Id { get; set; }
        [Display(Name = "Тема заявки")]
        public string Theme { get; set; }
        [Display(Name = "Текст заявки")]
        public string Text { get; set; }
        [Display(Name = "Дата создания")]
        public DateTime CreationDate { get; set; }
        public List<RequestRequestStatus> RequestStatuses { get; set; }
        public Request()
        {

        }
    }
    public class RequestRequestStatus
    {
        public Request Request { get; set; }
        public int RequestId { get; set; }
        public RequestStatus RequestStatus { get; set; }
        public int RequestStatusId { get; set; }
        public DateTime CreationDate { get; set; }
        public RequestRequestStatus()
        {

        }
    }
    public class RequestStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<RequestRequestStatus> RequestStatuses { get; set; }
        public RequestStatus()
        {

        }
    }
}
