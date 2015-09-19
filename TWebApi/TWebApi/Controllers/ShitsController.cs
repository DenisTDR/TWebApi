using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using customApiApp_3.Models;
using customApiApp_3.Responses;

namespace customApiApp_3.Controllers
{
    [TypeAliases("Shit")]
    public class ShitsController : ITApiObjectController<Shit>
    {
        private readonly DbSet<Shit> _shits;
        private readonly TDbContext _tDbContext;

        public ShitsController()
        {
            _shits = (_tDbContext = new TDbContext()).Set<Shit>();
        }

        [AllowGet]
        public IEnumerable<Shit> Get()
        {
            return _shits;
        }

        [AllowGet]
        public object Get(int id)
        {
            var result = _shits.Find(id);
            if (result == null)
                return
                    new customApiApp_3.Responses.NotFound("No Shit found with Id=" + id);
            return result;
        }

        [AllowPost]
        public object New(Shit shit)
        {
            _shits.Add(shit);
            _tDbContext.SaveChanges();
            return new { Status = "new shit added", Shit = shit };
        }
        [AllowGet]
        public object Gets(string id)
        {
            string idtmp = string.Concat(id.Where(c => "0123456789".Contains(c)));
            int intVal;
            if (!int.TryParse(idtmp, out intVal))
            {
                return new BadRequest("Invalid Id. Can't convert '" + id + "' to int.");
            }
            var result = _shits.FirstOrDefault(x => x.Id == intVal);
            if (result == null)
                return
                    new customApiApp_3.Responses.NotFound("No Shit found with Id=" + id);
            return result;
        }


        [AllowPost]
        public object Update(Shit shit)
        {
            if (_shits.Find(shit.Id) == null)
            {
                return New(shit);
            }
            else
            {
                var oldValue = _tDbContext.Entry(_shits.Find(shit.Id));
                oldValue.CurrentValues.SetValues(shit);
                _tDbContext.SaveChanges();
                return new {Status = "shit updated", shit = _shits.Find(shit.Id)};
            }
        }

        [AllowDelete]
        public object Delete(int id)
        {
            Shit oldValue;
            if ((oldValue = _shits.Find(id)) == null)
            {
                return new NotFound("Shit with id=" + id + " was not found!");
            }
            else
            {
                _shits.Remove(oldValue);
                _tDbContext.SaveChanges();
                return new {info = "shit deleted"};
            }
        }

        [AllowGet]
        public object GetPWD()
        {
            return HttpRuntime.AppDomainAppPath;
        }
    }
}
