using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace TodoApi.Repository
{
    public class TodoRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly TodoContext _context;
        protected DbSet<T> _entities;

        public TodoRepository(TodoContext context)
        {
            this._context = context;
            this._entities = context.Set<T>();
        }
        

        public async Task DeleteAsync(T entity)
        {
             _entities.Remove(entity);
           await _context.SaveChangesAsync();
        }

        public async Task<T> GetAsync(long id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<List<T>> GetAsync()
        {
            return await _entities.ToListAsync();
        }

        public async Task<T> InsertAsync(T entity)
        {
          var newEntity = await  _entities.AddAsync(entity);
          await _context.SaveChangesAsync();

            return newEntity.Entity;
        }

        public async Task UpdateAsync(T entity)
        {
          
            if (!TodoItemExists(entity.Id))
                throw new ArgumentException($"Couldn't find matching {nameof(T)} with Id={entity.Id}");

            _context.Entry(entity).State = EntityState.Modified;


            await _context.SaveChangesAsync();
        }


        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }



   
}

       
   

