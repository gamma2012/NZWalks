using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NZWalksAPI.CustomActionFilters;
using NZWalksAPI.Data;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.DTO;
using NZWalksAPI.Repositories;
using System.Collections.Generic;
using System.Text.Json;

namespace NZWalksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {

        public readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalksDbContext dbContext,
            IRegionRepository regionRepository, 
            IMapper mapper,
            ILogger<RegionsController> logger)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                //throw new Exception("this is a custom exception");
                logger.LogInformation("Get all action method was invoked");

                //var regions = await dbContext.Regions.ToListAsync();
                var regions = await regionRepository.GetAllAsync();

                //var regionsDto = new List<RegionDto>();
                //foreach (var region in regions)
                //{
                //    regionsDto.Add(new RegionDto()
                //    {
                //        Id = region.Id,
                //        Code = region.Code,
                //        Name = region.Name,
                //        RegionImageUrl = region.RegionImageUrl,
                //    });
                //}

                logger.LogInformation($"Finished GetAllRegions request with data: {JsonSerializer.Serialize(regions)}");

                var regionsDto = mapper.Map<List<RegionDto>>(regions);

                return Ok(regionsDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
            
        }

        [HttpGet]
        [Route("{Id}")]
        [Authorize(Roles = "Reader")]
        public async Task <IActionResult> GetById(Guid Id)
        {
            //var region = dbContext.Regions.FirstOrDefault(x => x.Id == Id);

            //var region = await dbContext.Regions.FindAsync(Id);
            var region = await regionRepository.GetByIdAsync(Id);

            if (region == null)
            {
                return NotFound();
            }

            //var regionDto = new RegionDto 
            //{
            //    Id = region.Id,
            //    Code = region.Code,
            //    Name = region.Name,
            //    RegionImageUrl = region.RegionImageUrl,
            //};

            var regionDto = mapper.Map<RegionDto>(region);

            return Ok(regionDto);
        }

        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            //if(ModelState.IsValid)
            //{
                //Map or convert DTO to domain model
                //var regionDomainModel = new Region
                //{
                //    Code = addRegionRequestDto.Code,
                //    Name = addRegionRequestDto.Name,
                //    RegionImageUrl = addRegionRequestDto.RegionImageUrl,
                //};
                var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

                //Use domain model to create Region
                //await dbContext.Regions.AddAsync(regionDomainModel);
                //await dbContext.SaveChangesAsync();

                await regionRepository.CreateAsync(regionDomainModel);

                //Map to domain model back to DTO
                //var regionDto = new RegionDto
                //{
                //    Id = regionDomainModel.Id,
                //    Code = regionDomainModel.Code,
                //    Name = regionDomainModel.Name,
                //    RegionImageUrl = regionDomainModel.RegionImageUrl,
                //};

                var regionDto = mapper.Map<RegionDto>(regionDomainModel);

                return CreatedAtAction(nameof(GetById), new { Id = regionDto.Id }, regionDto);
            //}
            //else
            //{
            //    return BadRequest(ModelState);
            //}
            
        }

        [HttpPut]
        [Route("{Id}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid Id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            //if (ModelState.IsValid)
            //{
                //Map DTO to Domain Model
                //var regionDomainModel = new Region
                //{
                //    Code = updateRegionRequestDto.Code,
                //    Name = updateRegionRequestDto.Name,
                //    RegionImageUrl = updateRegionRequestDto.RegionImageUrl,
                //};

                var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

                //var region = dbContext.Regions.FirstOrDefault(x => x.Id == Id);
                //var regionDomainModel = await dbContext.Regions.FindAsync(Id);
                regionDomainModel = await regionRepository.UpdateAsync(Id, regionDomainModel);

                if (regionDomainModel == null)
                {
                    return NotFound();
                }

                //var regionDto = new RegionDto
                //{
                //    Id = regionDomainModel.Id,
                //    Code = regionDomainModel.Code,
                //    Name = regionDomainModel.Name,
                //    RegionImageUrl = regionDomainModel.RegionImageUrl,
                //};

                var regionDto = mapper.Map<RegionDto>(regionDomainModel);

                return Ok(regionDto);
            //}
            //else
            //{
            //    return BadRequest(ModelState);
            //}
            
        }

        [HttpDelete]
        [Route("{Id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid Id)
        {
            //var region = dbContext.Regions.FirstOrDefault(x => x.Id == Id);

            //var regionDomainModel = await dbContext.Regions.FindAsync(Id);
            var regionDomainModel = await regionRepository.DeleteAsync(Id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            //Return deleted region back
            //Map domain model to regionDto
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl,
            //};

            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);
        }
    }
}
