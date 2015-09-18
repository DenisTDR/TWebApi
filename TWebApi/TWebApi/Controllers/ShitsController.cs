using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using customApiApp_3.Models;
using customApiApp_3.Responses;

namespace customApiApp_3.Controllers
{
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

        [AllowGet]
        public object Fuck(string id)
        {
            return "fuck cu str=" + id;
        }

        [AllowPost]
        public object New(Shit shit)
        {
            _shits.Add(shit);
            _tDbContext.SaveChangesAsync();
            return shit;
        }
        /*public ShitsController()
        {
            shits = new List<Shit>
            {
                new Shit
                {
                    Id = 22,
                    Name = "nume1",
                    Description = "desc3213"
                },
                new Shit
                {
                    Id = 51,
                    Name = "numeleee",
                    Description = "grrfds dsf sdgsdg"
                },
                new Shit
                {
                    Id = 76,
                    Name = "bdgb dgdg sg",
                    Description = "sfs dfh dfhd fdgwegdj ng hkrsdtgzbd vgv fgv555"
                }
            };
        }*/
        public string One(string iemu, string def = "dfdf")
        {
            return "test 666";
        }
    }
}
