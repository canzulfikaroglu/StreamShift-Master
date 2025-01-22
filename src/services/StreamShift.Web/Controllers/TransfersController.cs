using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StreamShift.Domain.DatabaseEnums;
using StreamShift.Domain.Entities;
using StreamShift.Infrastructure.Services.TransferService.Abstract;
using StreamShift.Infrastructure.Context;
using StreamShift.Infrastructure.Repository;

namespace StreamShift.Web.Controllers
{
    [Authorize]
    public class TransfersController : Controller
    {
        private readonly AppDb _context;
        private readonly ITransferService _transferService;
        public TransfersController(AppDb context, ITransferService transferService)
        {
            _context = context;
            _transferService = transferService;
        }

        // GET: Transfer
        public async Task<IActionResult> Index()
        {
            var userId = User.GetIdClaim();
            return View(await _context.Transfers.Where(x => x.AppUserId == userId).ToListAsync());
        }

        // GET: Transfer/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transfer = await _context.Transfers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transfer == null)
            {
                return NotFound();
            }

            return View(transfer);
        }

        // GET: Transfer/Create
        public IActionResult Create()
        {
            ViewBag.eDatabaseList = Enum.GetValues(typeof(eDatabase))
                .Cast<eDatabase>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString()
                })
                .ToList();
            return View();
        }

        // POST: Transfer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transfer transfer)
        {
            transfer.AppUserId = User.GetIdClaim();
            transfer.IsFinished = false;
            if (ModelState.IsValid)
            {
                _context.Transfers.Add(transfer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.eDatabaseList = Enum.GetValues(typeof(eDatabase))
                .Cast<eDatabase>()
                .Select(e => new SelectListItem
                {
                    Value = e.ToString(),
                    Text = e.ToString()
                })
                .ToList();
            return View(transfer);
        }

        // GET: Transfer/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var AppUserId = User.GetIdClaim();
            var transfer = await _context.Transfers.FirstOrDefaultAsync(a => a.AppUserId == AppUserId && a.Id == id);
            if (transfer == null)
            {
                return NotFound();
            }
            return View(transfer);
        }

        // POST: Transfer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Transfer transfer)
        {
            if (id != transfer.Id)
            {
                return NotFound();
            }
            var old = await _context.Transfers.FirstOrDefaultAsync(x => x.Id == id);
            if (old != null)
            {
                var AppUserId = User.GetIdClaim();
                if (old.AppUserId != AppUserId)
                {
                    return NotFound();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transfer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransferExists(transfer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(transfer);
        }

        // GET: Transfer/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transfer = await _context.Transfers
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transfer == null)
            {
                return NotFound();
            }

            var AppUserId = User.GetIdClaim();

            if (transfer.AppUserId != AppUserId)
            {
                return NotFound();
            }
            //buraya İdye göre bir delete sorgusu yazdıran bir fonksiyon çağıralacak aldığı id değerine göre silecek : delete 
            return View(transfer);
        }

        // POST: Transfer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var transfer = await _context.Transfers.FindAsync(id);
            var AppUserId = User.GetIdClaim();
            if (transfer.AppUserId != AppUserId)
            {
                return NotFound();
            }
            _context.Transfers.Remove(transfer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransferExists(string id)
        {
            var AppUserId = User.GetIdClaim();
            return _context.Transfers.Any(e => e.Id == id && e.AppUserId == AppUserId);
        }

        //POST: Transfer/Start

        public async Task<IActionResult> Start(string id)
        {
            var transfer = await _context.Transfers.FirstOrDefaultAsync(m => m.Id == id);
            if (transfer == null)
            {
                return NotFound("Transfer record not found.");
            }

            try
            {
                await _transferService.TransferDatabase(
                        transfer.SourceConnectionString,
                        transfer.SourceDatabase,
                        transfer.DestinationConnectionString,
                        transfer.DestinationDatabase);

                transfer.IsFinished = true;
                await _context.SaveChangesAsync();
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

}