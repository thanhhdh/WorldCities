﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldCitiesAPI.Data;
using WorldCitiesAPI.Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace WorldCitiesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cities
        [HttpGet]
        public async Task<ActionResult<ApiResult<CityDTO>>> GetCities(
            int pageIndex = 0, int pageSize = 10, string? sortColumn = null, string? sortOrder = null, string? filterColumn = null, string? filterQuery = null)
        {
            return await ApiResult<CityDTO>.CreateAsync(_context.Cities.AsNoTracking()
                .Select(c => new CityDTO()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Lat = c.Lat,
                    Lon = c.Lon,
                    CountryId = c.Country!.Id,
                    CountryName = c.Country!.Name
                })
                , pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<City>> GetCity(int id)
        {
          if (_context.Cities == null)
          {
              return NotFound();
          }
            var city = await _context.Cities.FindAsync(id);

            if (city == null)
            {
                return NotFound();
            }

            return city;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "RegisteredUser")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(int id, City city)
        {
            if (id != city.Id)
            {
                return BadRequest();
            }

            _context.Entry(city).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CityExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Cities
        [Authorize(Roles = "RegisteredUser")]
        [HttpPost]
        public async Task<ActionResult<City>> PostCity(City city)
        {
            if(city != null)
            {
                _context.Cities.Add(city);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCity", new { id = city.Id }, city);
            } else
            {
                return BadRequest("Giá trị City không được để trống. Không thể thêm vào cơ sở dữ liệu.");
            }
        }

        // DELETE: api/Cities/5
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            if (_context.Cities == null)
            {
                return NotFound();
            }
            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CityExists(int id)
        {
            return (_context.Cities?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost]
        [Route("IsDupeCity")]
        public bool IsDupeCity(City city)
        {
            return _context.Cities.Any(
                e => e.Name == city.Name 
                && e.Lat == city.Lat 
                && e.Lon == city.Lon 
                && e.CountryId == city.CountryId 
                && e.Id != city.Id);
        }
    }
}
