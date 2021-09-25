using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CelestialObjectController(ApplicationDbContext ctx)
        {
            _context = ctx;

        }
        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var Co = _context.CelestialObjects.Find(id);
            if (Co == null)
                return NotFound();
            Co.Satellites = _context.CelestialObjects.Where(op => op.OrbitedObjectId == id).ToList();
            return Ok(Co);

        }
        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var Co = _context.CelestialObjects.Where(op => op.Name == name).ToList();
            if (!Co.Any())
                return NotFound();
            foreach (var obj in Co)
            {
                obj.Satellites = _context.CelestialObjects.Where(op => op.OrbitedObjectId == obj.Id).ToList();
            }
            return Ok(Co);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var Co = _context.CelestialObjects.ToList();

            foreach (var obj in Co)
            {
                obj.Satellites = _context.CelestialObjects.Where(op => op.OrbitedObjectId == obj.Id).ToList();
            }
            return Ok(Co);

        }
        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject CelestialObject)
        {
            if (ModelState.IsValid)
            {
                _context.CelestialObjects.Add(CelestialObject);
                _context.SaveChanges();
            }
            else
            {
                return BadRequest();
            }
            return CreatedAtRoute("GetById", new { id = CelestialObject.Id }, CelestialObject);

        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject CelestialObject)
        {
            var Co = _context.CelestialObjects.Find(id);
            if (Co == null)
                return NotFound();
            Co.Name = CelestialObject.Name;
            Co.OrbitedObjectId = CelestialObject.OrbitedObjectId;
            Co.OrbitalPeriod = CelestialObject.OrbitalPeriod;
            _context.CelestialObjects.Update(Co);
            _context.SaveChanges();

            return NoContent();
        }
        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var Co = _context.CelestialObjects.Find(id);
            if (Co == null)
                return NotFound();
            Co.Name = name;
            _context.CelestialObjects.Update(Co);
            _context.SaveChanges();
            return NoContent();

        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {

            var co = _context.CelestialObjects.Where(op => op.Id == id || op.OrbitedObjectId == id);

            if (!co.Any())
                return NotFound();

            _context.CelestialObjects.RemoveRange(co);
            _context.SaveChanges();
            return NoContent();
        }

    }
}
