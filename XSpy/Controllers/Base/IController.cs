using XSpy.Database.Entities;

namespace XSpy.Controllers.Base
{
    public interface IController
    {
        public User LoggedUser { get; set; }

    }
}