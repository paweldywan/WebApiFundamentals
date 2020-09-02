using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TheCodeCamp.Data;

namespace TheCodeCamp.Controllers
{
    public class CampsController : ApiController
    {
        private readonly ICampRepository repository;

        public CampsController(ICampRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var result = await repository.GetAllCampsAsync();

                return Ok(result);
            }
            catch(Exception ex)
            {
                // TODO Add Logging
                return InternalServerError(ex);
            }
        }
    }
}