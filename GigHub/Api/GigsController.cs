using GigHub.Contracts;
using GigHub.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GigHub.Api
{
    [Route("api/[controller]")]
    public class GigsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDataProtector _gigIdDataProtector;

        public GigsController(IDataProtectionProvider dataProtectionProvider, 
            DataProtectionPurposeStrings dataProtectionPurposeStrings, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _gigIdDataProtector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.GigIdRouteValue);
        }
       
        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var gigId = Convert.ToInt32(_gigIdDataProtector.Unprotect(id));

            var gig = _unitOfWork.Gigs.FindGigById(gigId);
            gig.IsCanceled = true;

            _unitOfWork.Complete();

            return Ok();
        }
    }
}
