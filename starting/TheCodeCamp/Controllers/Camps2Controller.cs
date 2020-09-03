using AutoMapper;
using Microsoft.Web.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
    [ApiVersion("2.0")]
    [RoutePrefix("api/v{version:apiVersion}/camps")]
    public class Camps2Controller : ApiController
    {
        private readonly ICampRepository repository;
        private readonly IMapper mapper;

        public Camps2Controller(ICampRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        //[Route()]
        [Route]
        public async Task<IHttpActionResult> Get(bool includeTalks = false)
        {
            try
            {
                var result = await repository.GetAllCampsAsync(includeTalks);

                // Mapping
                var mappedResult = mapper.Map<IEnumerable<CampModel>>(result);

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                // TODO Add Logging
                return InternalServerError(ex);
            }
        }

        [Route("{moniker}", Name = "GetCamp20")]
        public async Task<IHttpActionResult> Get(string moniker, bool includeTalks = false)
        {
            try
            {
                var result = await repository.GetCampAsync(moniker, includeTalks);

                if (result == null)
                    return NotFound();

                return Ok(new { success = true, camp = mapper.Map<CampModel>(result) });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("searchByDate/{eventDate:datetime}")]
        [HttpGet]
        public async Task<IHttpActionResult> SearchByEventDate(DateTime eventDate, bool includeTalks = false)
        {
            try
            {
                var result = await repository.GetAllCampsByEventDate(eventDate, includeTalks);

                return Ok(mapper.Map<CampModel[]>(result));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route]
        public async Task<IHttpActionResult> Post(CampModel model)
        {
            try
            {
                if (await repository.GetCampAsync(model.Moniker) != null)
                {
                    ModelState.AddModelError("Moniker", "Moniker in use");
                }

                if (ModelState.IsValid)
                {
                    var camp = mapper.Map<Camp>(model);

                    repository.AddCamp(camp);

                    if (await repository.SaveChangesAsync())
                    {
                        var newModel = mapper.Map<CampModel>(camp);

                        return CreatedAtRoute("GetCamp", new { moniker = newModel.Moniker }, newModel);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return BadRequest(ModelState);
        }

        [Route("{moniker}")]
        public async Task<IHttpActionResult> Put(string moniker, CampModel model)
        {
            try
            {
                var camp = await repository.GetCampAsync(moniker);

                if (camp == null)
                    return NotFound();

                mapper.Map(model, camp);

                if (await repository.SaveChangesAsync())
                {
                    return Ok(mapper.Map<CampModel>(camp));
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("{moniker}")]
        public async Task<IHttpActionResult> Delete(string moniker)
        {
            try
            {
                var camp = await repository.GetCampAsync(moniker);

                if (camp == null)
                    return NotFound();

                repository.DeleteCamp(camp);

                if (await repository.SaveChangesAsync())
                {
                    return Ok();
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}