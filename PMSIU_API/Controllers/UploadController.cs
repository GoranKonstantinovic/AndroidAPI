using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace PMSIU_API.Controllers
{
    public class UploadController : ApiController
    {
        [HttpPost]
        public IHttpActionResult UploadImage()
        {
            try
            {


                var file = HttpContext.Current.Request.Files.Count > 0 ?
                    HttpContext.Current.Request.Files[0] : null;
                string fileName = "";
                if (file != null && file.ContentLength > 0)
                {
                    var ext = Path.GetExtension(file.FileName);
                    fileName = Guid.NewGuid().ToString() + ext;

                    var path = Path.Combine(
                        HttpContext.Current.Server.MapPath("~/uploads"),
                        fileName
                    );

                    file.SaveAs(path);
                }

                if (file != null)
                {
                    return Ok("http://gogikole-001-site1.gtempurl.com/uploads/" + fileName);

                }
                else
                {
                    return BadRequest();
                }

            }

            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }
}
