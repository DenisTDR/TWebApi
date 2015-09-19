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
    [TypeAliases("Category")]
    public class CategoriesController : ITApiObjectController<Category>
    {
        private readonly DbSet<Category> _categories;
        private readonly TDbContext _tDbContext;

        public CategoriesController()
        {
            _categories = (_tDbContext = new TDbContext()).Set<Category>();
        }

        [AllowGet]
        public IEnumerable<Category> Get()
        {
            return _categories;
        }

        [AllowGet]
        public object Get(int id)
        {
            var result = _categories.Find(id);
            if (result == null)
                return
                    new customApiApp_3.Responses.NotFound("No Category found with Id=" + id);
            return result;
        }

        [AllowPost]
        public object New(Category shit)
        {
            _categories.Add(shit);
            _tDbContext.SaveChanges();
            return new { Status = "new Category added", shit };
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
            var result = _categories.FirstOrDefault(x => x.Id == intVal);
            if (result == null)
                return
                    new customApiApp_3.Responses.NotFound("No Category found with Id=" + intVal);
            return result;
        }


        [AllowPost]
        public object Update(Category shit)
        {
            if (_categories.Find(shit.Id) == null)
            {
                return New(shit);
            }
            else
            {
                var oldValue = _tDbContext.Entry(_categories.Find(shit.Id));
                oldValue.CurrentValues.SetValues(shit);
                _tDbContext.SaveChanges();
                return new { Status = "Category updated", shit = _categories.Find(shit.Id) };
            }
        }

        [AllowDelete]
        public object Delete(int id)
        {
            Category oldValue;
            if ((oldValue = _categories.Find(id)) == null)
            {
                return new NotFound("Category with id=" + id + " was not found!");
            }
            else
            {
                _tDbContext.Set<Category>().Remove(oldValue);
                _tDbContext.SaveChanges();
                return new { info = "Category deleted" };
            }
        }
    }
}
