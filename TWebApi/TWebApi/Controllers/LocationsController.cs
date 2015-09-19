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
    class LocationsController : ITApiObjectController<Location>
    {
        private readonly DbSet<Location> _locations;
        private readonly TDbContext _tDbContext;

        public LocationsController()
        {
            _locations = (_tDbContext = new TDbContext()).Set<Location>();
        }

        [AllowGet]
        public IEnumerable<Location> Get()
        {
            return _locations;
        }

        [AllowGet]
        public object Get(int id)
        {
            var result = _locations.Find(id);
            if (result == null)
                return
                    new customApiApp_3.Responses.NotFound("No Location found with Id=" + id);
            return result;
        }

        [AllowPost]
        public object New(Location location)
        {
            _locations.Add(location);
            _tDbContext.SaveChangesAsync();
            return new { Status = "new location added", location };
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
            var result = _locations.FirstOrDefault(x => x.Id == intVal);
            if (result == null)
                return
                    new customApiApp_3.Responses.NotFound("No Location found with Id=" + intVal);
            return result;
        }


        [AllowPost]
        public object Update(Location location)
        {
            if (_locations.Find(location.Id) == null)
            {
                return New(location);
            }
            else
            {
                var oldValue = _tDbContext.Entry(_locations.Find(location.Id));
                oldValue.CurrentValues.SetValues(location);
                _tDbContext.SaveChanges();
                return new { Status = "Location updated", shit = _locations.Find(location.Id) };
            }
        }

        [AllowDelete]
        public object Delete(int id)
        {
            Location oldValue;
            if ((oldValue = _locations.Find(id)) == null)
            {
                return new NotFound("Location with id=" + id + " was not found!");
            }
            else
            {
                _tDbContext.Set<Location>().Remove(oldValue);
                _tDbContext.SaveChanges();
                return new { info = "location deleted" };
            }
        }
    }
}
