using System.ComponentModel.DataAnnotations;
namespace DiplomaApp.ViewModels
{
    public class RequestCreate
    {
        [Display(Name = "Тема")]
        public string Theme { get; set; }
        [Display(Name = "Текст")]
        public string Text { get; set; }
        public RequestCreate()
        {

        }
    }
}
