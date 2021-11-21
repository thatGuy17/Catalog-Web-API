using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Dtos;
using Catalog.Models;
using Catalog.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Controllers
{
    [ApiController]
    [Route("items")]
    //[Route("[controller]")] uses controller name
    public class ItesmController : ControllerBase
    {
        private readonly IItemsRepository repository;

        public ItesmController(IItemsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetItemsAsync()
        {
            var items = (await repository.GetItemsAsync())
                        .Select(item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        //[HttpGet]
        //[Route("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemAsync([FromRoute][Required] Guid id)
        {
            var item = await repository.GetItemAsync(id);

            if (item is null)
            {
                return NotFound();
            }

            return Ok(item.AsDto());
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateItemAsync([FromBody] CreateItemDto itemDto)
        {
            Item item = new()
            {
                Id = Guid.NewGuid(),
                Name = itemDto.Name,
                Price = itemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow,
            };

            await repository.CreateItemAsync(item);

            return CreatedAtAction(nameof(GetItemAsync), new { id = item.Id }, item.AsDto());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateItemAsync([FromRoute] Guid id, [FromBody] UpdateItemDto itemDto)
        {
            var existingItem = await repository.GetItemAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            Item updatedItem = existingItem with
            {
                Name = itemDto.Name,
                Price = itemDto.Price,
            };

            await repository.UpdateItemAsync(updatedItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteItemAsync([FromRoute][Required] Guid id)
        {
            var existingItem = await repository.GetItemAsync(id);

            if (existingItem is null)
            {
                return NotFound();
            }

            await repository.DeleteItemAsync(id);

            return NoContent();
        }
    }
}