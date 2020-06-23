using System;
using System.Linq;
using AutoMapper;
using InventoryApi.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using InventoryApi.Repositories;
using System.Collections.Generic;
using InventoryApi.Entities;
using InventoryApi.Models;
using InventoryApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;

namespace InventoryApi.v1.Controllers
{

    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")]
    public class InventoriesController : Controller
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IUrlHelper _urlHelper;
        private readonly IMapper _mapper;

        private IHostingEnvironment _env;

        public InventoriesController(
            IUrlHelper urlHelper,
            IInventoryRepository inventoryRepository,
            IMapper mapper,
            IHostingEnvironment env)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
            _urlHelper = urlHelper;
            _env = env;
        }


      
        [Route("/", Name = nameof(Home))]
        [HttpGet]
        public ActionResult Home()
        {
            
            var link =  _urlHelper.Link(nameof(Home), null) + "swagger\n";
            var bodyLink = _urlHelper.Link(nameof(Home), null) +  "api/v1/inventories\n";
            var authLink = _urlHelper.Link(nameof(Home), null) +  "api/v1/users/authenticate\n";
            var usersLink = _urlHelper.Link(nameof(Home), null) +  "api/v1/users\n";
            var response = link + bodyLink;
            
            var final = "";
            if (response.Contains("https"))
            {
                final = response;
            }
            else
            {
                final = response.Replace("http", "https");
            }

            // var webRoot = _env.ContentRootPath;
	
            // var fileContent=System.IO.File.ReadAllText(webRoot + "/View/index.html");
           DateTime localDate = DateTime.Now;
            int year = localDate.Year;
            ViewData["link"] = link;
            ViewData["bodyLink"] = bodyLink;
             ViewData["authLink"] = authLink;
            ViewData["usersLink"] = usersLink;
            ViewData["year"] = year;
            return View("index.html");
           
        }


        [HttpGet(Name = nameof(GetInventories))]
        public ActionResult GetInventories(ApiVersion version, [FromQuery] QueryParameters queryParameters)
        {
            List<InventoryEntity> inventoryItems = _inventoryRepository.GetAll(queryParameters).ToList();

            var allItemCount = _inventoryRepository.Count();

            var paginationMetadata = new
            {
                totalCount = allItemCount,
                pageSize = queryParameters.PageCount,
                currentPage = queryParameters.Page,
                totalPages = queryParameters.GetTotalPages(allItemCount)
            };

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));

            var links = CreateLinksForCollection(queryParameters, allItemCount, version);

            var toReturn = inventoryItems.Select(x => ExpandSingleInventoryItem(x, version));

            return Ok(new
            {
                value = toReturn,
                links = links
            });
        }

        [HttpGet]
        [Route("{id:int}", Name = nameof(GetSingleInventory))]
        public ActionResult GetSingleInventory(ApiVersion version, int id)
        {
            InventoryEntity inventoryItem = _inventoryRepository.GetSingle(id);

            if (inventoryItem == null)
            {
                return NotFound();
            }

            return Ok(ExpandSingleInventoryItem(inventoryItem, version));
        }

        [HttpPost(Name = nameof(AddInventory))]
        public ActionResult<InventoryDto> AddInventory(ApiVersion version, [FromBody] InventoryCreateDto inventoryCreateDto)
        {
            if (inventoryCreateDto == null)
            {
                return BadRequest();
            }

            InventoryEntity toAdd = _mapper.Map<InventoryEntity>(inventoryCreateDto);

            _inventoryRepository.Add(toAdd);

            if (!_inventoryRepository.Save())
            {
                throw new Exception("Creating a inventory item failed on save.");
            }

            InventoryEntity newInventoryItem = _inventoryRepository.GetSingle(toAdd.Id);

            return CreatedAtRoute(nameof(GetSingleInventory), new { version = version.ToString(), id = newInventoryItem.Id },
                _mapper.Map<InventoryDto>(newInventoryItem));
        }

        [HttpPatch("{id:int}", Name = nameof(PartiallyUpdateInventory))]
        public ActionResult<InventoryDto> PartiallyUpdateInventory(int id, [FromBody] JsonPatchDocument<InventoryUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            InventoryEntity existingEntity = _inventoryRepository.GetSingle(id);

            if (existingEntity == null)
            {
                return NotFound();
            }

            InventoryUpdateDto inventoryUpdateDto = _mapper.Map<InventoryUpdateDto>(existingEntity);
            patchDoc.ApplyTo(inventoryUpdateDto);

            TryValidateModel(inventoryUpdateDto);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(inventoryUpdateDto, existingEntity);
            InventoryEntity updated = _inventoryRepository.Update(id, existingEntity);

            if (!_inventoryRepository.Save())
            {
                throw new Exception("Updating a inventory item failed on save.");
            }

            return Ok(_mapper.Map<InventoryDto>(updated));
        }

        [HttpDelete]
        [Route("{id:int}", Name = nameof(RemoveInventory))]
        public ActionResult RemoveInventory(int id)
        {
            InventoryEntity inventoryItem = _inventoryRepository.GetSingle(id);

            if (inventoryItem == null)
            {
                return NotFound();
            }

            _inventoryRepository.Delete(id);

            if (!_inventoryRepository.Save())
            {
                throw new Exception("Deleting a inventory item failed on save.");
            }

            return NoContent();
        }

        [HttpPut]
        [Route("{id:int}", Name = nameof(UpdateInventory))]
        public ActionResult<InventoryDto> UpdateInventory(int id, [FromBody]InventoryUpdateDto inventoryUpdateDto)
        {
            if (inventoryUpdateDto == null)
            {
                return BadRequest();
            }

            var existingInventoryItem = _inventoryRepository.GetSingle(id);

            if (existingInventoryItem == null)
            {
                return NotFound();
            }

            _mapper.Map(inventoryUpdateDto, existingInventoryItem);

            _inventoryRepository.Update(id, existingInventoryItem);

            if (!_inventoryRepository.Save())
            {
                throw new Exception("Updating a inventory item failed on save.");
            }

            return Ok(_mapper.Map<InventoryDto>(existingInventoryItem));
        }

        [HttpGet("GetRandomItem", Name = nameof(GetRandomItem))]
        public ActionResult GetRandomItem()
        {
            ICollection<InventoryEntity> inventoryItems = _inventoryRepository.GetRandomMeal();

            IEnumerable<InventoryDto> dtos = inventoryItems
                .Select(x => _mapper.Map<InventoryDto>(x));

            var links = new List<LinkDto>();

            // self 
            links.Add(new LinkDto(_urlHelper.Link(nameof(GetRandomItem), null), "self", "GET"));

            return Ok(new
            {
                value = dtos,
                links = links
            });
        }

        private List<LinkDto> CreateLinksForCollection(QueryParameters queryParameters, int totalCount, ApiVersion version)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(new LinkDto(_urlHelper.Link(nameof(GetInventories), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.Page,
                orderby = queryParameters.OrderBy
            }), "self", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetInventories), new
            {
                pagecount = queryParameters.PageCount,
                page = 1,
                orderby = queryParameters.OrderBy
            }), "first", "GET"));

            links.Add(new LinkDto(_urlHelper.Link(nameof(GetInventories), new
            {
                pagecount = queryParameters.PageCount,
                page = queryParameters.GetTotalPages(totalCount),
                orderby = queryParameters.OrderBy
            }), "last", "GET"));

            if (queryParameters.HasNext(totalCount))
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetInventories), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page + 1,
                    orderby = queryParameters.OrderBy
                }), "next", "GET"));
            }

            if (queryParameters.HasPrevious())
            {
                links.Add(new LinkDto(_urlHelper.Link(nameof(GetInventories), new
                {
                    pagecount = queryParameters.PageCount,
                    page = queryParameters.Page - 1,
                    orderby = queryParameters.OrderBy
                }), "previous", "GET"));
            }

            var posturl = _urlHelper.Link(nameof(AddInventory), new { version = version.ToString() });

            links.Add(
               new LinkDto(posturl,
               "create_inventory",
               "POST"));

            return links;
        }

        private dynamic ExpandSingleInventoryItem(InventoryEntity inventoryItem, ApiVersion version)
        {
            var links = GetLinks(inventoryItem.Id, version);
            InventoryDto item = _mapper.Map<InventoryDto>(inventoryItem);

            var resourceToReturn = item.ToDynamic() as IDictionary<string, object>;
            resourceToReturn.Add("links", links);

            return resourceToReturn;
        }

        private IEnumerable<LinkDto> GetLinks(int id, ApiVersion version)
        {
            var links = new List<LinkDto>();

            var getLink = _urlHelper.Link(nameof(GetSingleInventory), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(getLink, "self", "GET"));

            var deleteLink = _urlHelper.Link(nameof(RemoveInventory), new { version = version.ToString(), id = id });

            links.Add(
              new LinkDto(deleteLink,
              "delete_inventory",
              "DELETE"));

            var createLink = _urlHelper.Link(nameof(AddInventory), new { version = version.ToString() });

            links.Add(
              new LinkDto(createLink,
              "create_inventory",
              "POST"));

            var updateLink = _urlHelper.Link(nameof(UpdateInventory), new { version = version.ToString(), id = id });

            links.Add(
               new LinkDto(updateLink,
               "update_inventory",
               "PUT"));

            return links;
        }
    }
}
