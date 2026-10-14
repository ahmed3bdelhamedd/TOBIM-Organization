using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TobimSystem.Data;
using TobimSystem.Models;

namespace TobimSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly TobimDbContext _context;

        public SubscriptionsController(TobimDbContext context)
        {
            _context = context;
        }

        // جلب كل الاشتراكات
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions()
        {
            return await _context.Subscriptions.ToListAsync();
        }

        // إضافة اشتراك جديد
        [HttpPost]
        public async Task<ActionResult<Subscription>> CreateSubscription([FromBody] Subscription subscription)
        {
            if (subscription.PaidAmount >= subscription.TotalPrice)
            {
                subscription.Status = "Full Pay";
            }
            else
            {
                subscription.Status = "Rest";
            }

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubscriptions), new { id = subscription.Id }, subscription);
        }
    }
}
