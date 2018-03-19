using Microsoft.AspNetCore.Mvc;
using PrimeCheckerComponent.Services;

namespace PrimeCheckerComponent.Controllers
{
    [Route("api/[controller]")]
    public class PrimeCheckerController : Controller
    {
        [HttpGet]
        [Route("IsPrime")]
        public bool IsPrime(long numberToCheck)
        {
            return PrimeCheckerService.IsPrime(numberToCheck);
        }

        [HttpGet]
        [Route("NumberOfPrimesInRange")]
        public int NumberOfPrimesInRange(long start, long end)
        {
            return PrimeCheckerService.CountPrimes(start, end);
        }
    }
}
