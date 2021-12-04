using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MealManagementSoftwareUiLayer.ViewModel;
using MealManagementSoftwareDataLayer; 

namespace MealManagementSoftwareUiLayer.Controllers
{
    public class PaymentCardsController : Controller
    {
        private readonly MealManagementDbContext _context; 
        public PaymentCardsController(MealManagementDbContext context)
        {
            _context = context; 
        }


        // GET: Cards
        public async Task<IActionResult> Student(int? id,string message = null)
        {

            if (id == null)
            {
                return NotFound();
            }

            var card = await _context.Cards
                .Include(c => c.UserProfile)
                .FirstOrDefaultAsync(m => m.UserProfileId == id);
            if (card == null)
            {
                return NotFound();
            }

            ViewBag.Status = message;
            card.UserProfileId = id.Value;
            return View(card);
        }

        // GET: Cards
        public async Task<IActionResult> Parent(int? id, string message = null)
        {

            if (id == null)
            {
                return NotFound();
            }

            var parentDetail = await _context.ParentMapping
               .Include(c => c.StudentUserProfile)
               .FirstOrDefaultAsync(m => m.ParentId == id);

            if (parentDetail == null)
            {
                return NotFound();
            }

            var card = await _context.Cards
                .Include(c => c.UserProfile)
                .FirstOrDefaultAsync(m => m.UserProfileId == parentDetail.StudentId);
            if (card == null)
            {
                return NotFound();
            }
            ViewBag.Status = message;
            card.UserProfileId = id.Value;
            return View(card);
        }

       [Route("addstudentfund/{id}/{userProfileId}")]
        public async Task<IActionResult> AddStudentFund(int id,int userProfileId)
        {
            var card = await _context.Cards
               .FirstOrDefaultAsync(m => m.Id == id);

            if (card == null)
            {
                return NotFound();
            }

            return View(new PaymentCardViewModel()
            {
                CardId = card.Id,
                UserProfileId = userProfileId
            }) ;
        }

        [HttpPost]
        public async Task<IActionResult> AddStudentFund(PaymentCardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.CardId != 0)
            {
                var isInvalid = false;
                if (model.BankCardNumber != 11111111)
                {
                    isInvalid = true;
                    ModelState.AddModelError("error", "Enter a valid card number. example test data:11111111 "); 
                   
                }
                if (!(model.ExpiryMonth >= 01 && model.ExpiryMonth <= 12))
                {
                    isInvalid = true;
                    ModelState.AddModelError("error", "Enter a valid Expiry month. example test data:01 to 12 ");

                }
                if (!(model.ExpiryYear >= 2020 && model.ExpiryYear <= 2050))
                {
                    isInvalid = true;
                    ModelState.AddModelError("error", "Enter a valid Expiry Year. example test data:2020 to 2050 "); 
                }
                if (!(model.CVV >= 100 && model.CVV <= 999))
                {
                    isInvalid = true;
                    ModelState.AddModelError("error", "Enter a valid CVV. example test data:111 ");
                }
                if (model.Amount <= 0)
                {
                    isInvalid = true;
                    ModelState.AddModelError("error", "Amount must be greater than 0");
                }

                if (isInvalid)
                {
                    return View("AddStudentFund", model);
                }

                var card = await _context.Cards
                   .FirstOrDefaultAsync(m => m.Id == model.CardId);

                if (card == null)
                { 
                    ModelState.AddModelError("error", "Unable Add funs! Please contact admin");

                    return View("AddStudentFund", model);
                }
                card.AvailableBalance = card.AvailableBalance+ model.Amount;
                _context.Update(card);
                _context.SaveChanges();
                return RedirectToAction("Student", new { id = model.UserProfileId ,message= $"£{model.Amount} added to the fund" }); 
            }

            return View(model);
        }
        [Route("AddParentFund/{id}/{userProfileId}")]
        public async Task<IActionResult> AddParentFund(int id, int userProfileId)
        {
         
            var card = await _context.Cards
               .FirstOrDefaultAsync(m => m.Id == id);

            if (card == null)
            {
                return NotFound();
            }

            return View(new PaymentCardViewModel()
            {
                CardId = card.Id,
                UserProfileId = userProfileId
            });
        }


        [HttpPost]
        public async Task<IActionResult> AddParentFund(PaymentCardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.CardId != 0)
            {
                var isInvalid = false;
                if (model.BankCardNumber != 11111111)
                {
                    isInvalid = true;
                    ModelState.AddModelError("error", "Enter a valid card number. example test data:11111111 ");

                }
                if (!(model.ExpiryMonth >= 01 && model.ExpiryMonth <= 12))
                {
                    isInvalid = true;
                    ModelState.AddModelError("error", "Enter a valid Expiry month. example test data:01 to 12 ");

                }
                if (!(model.ExpiryYear >= 2020 && model.ExpiryYear <= 2050))
                {
                    isInvalid = true;
                    ModelState.AddModelError("error", "Enter a valid Expiry Year. example test data:2020 to 2050 ");
                }
                if (!(model.CVV >= 100 && model.CVV <= 999))
                {
                    isInvalid = true;
                    ModelState.AddModelError("error", "Enter a valid CVV. example test data:111 ");
                }
                if (model.Amount <= 0)
                {
                    isInvalid = true;
                    ModelState.AddModelError("error", "Amount must be greater than 0");
                }

                if (isInvalid)
                {
                    return View("AddStudentFund", model);
                }
                var card = await _context.Cards
                   .FirstOrDefaultAsync(m => m.Id == model.CardId);

                if (card == null)
                {
                    ModelState.AddModelError("error", "Unable Add funs! Please contact admin");

                    return View("AddStudentFund", model);
                }
                card.AvailableBalance = card.AvailableBalance+ model.Amount;
                _context.Update(card);
                _context.SaveChanges();

                return RedirectToAction("Parent", new { id = model.UserProfileId, message = $"£{model.Amount} added to the fund" });
            }

            return View(model);
        }

    }
}
