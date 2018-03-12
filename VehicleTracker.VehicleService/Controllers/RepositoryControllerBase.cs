using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleTracker.VehicleService.Storage;

namespace VehicleTracker.VehicleService.Controllers
{
    public abstract class RepositoryControllerBase<TKey, T, TFilter> : Controller
    {
        private readonly IRepository<TKey, T, TFilter> _repository;

        protected RepositoryControllerBase(IRepository<TKey, T, TFilter> repository)
        {
            _repository = repository;
        }

        protected Task<IEnumerable<T>> List(TFilter filter)
            => _repository.List(filter);

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(TKey id)
        {
            try
            {
                var result = await _repository.Get(id);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public Task Post([FromBody]T item)
            => _repository.Create(item);

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(TKey id, [FromBody]T item)
        {
            try
            {
                await _repository.Update(id, item);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(TKey id)
        {
            try
            {
                await _repository.Delete(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
