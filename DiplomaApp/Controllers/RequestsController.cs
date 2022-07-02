using DiplomaApp.Core;
using DiplomaApp.Data;
using DiplomaApp.Models;
using DiplomaApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace DiplomaApp.Controllers
{
    [Authorize]
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RequestController
        public ActionResult Index(int page = 1)
        {
            if (_context == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Marketplace'  is null.");
            }
            
            if (_context.Request.Any())
            {
                var requests = _context.Request.AsEnumerable().OrderByDescending(c => c.CreationDate);
                RequestIndex viewModel = new RequestIndex();
                List<RequestViewModel> requestsViewModel = new List<RequestViewModel>();
                foreach (var request in requests)
                {
                    var lastStatus = _context.RequestRequestStatus.AsEnumerable().Where(c => c.RequestId == request.Id).MaxBy(c => c.CreationDate);
                    var status = _context.RequestStatus.Find(lastStatus.RequestStatusId);
                    requestsViewModel.Add(new RequestViewModel()
                    {
                        Id = request.Id,
                        CreationDate = request.CreationDate,
                        Status = new RequestStatusViewModel() { Name = status.Name, CreationDate = lastStatus.CreationDate },
                        Text = request.Text,
                        Theme = request.Theme
                    });
                }
                viewModel.Requests = requestsViewModel.ToPagedList(page, Constants.recordsPerPage);
                return View(viewModel);
            }
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestCreate requestCreate)
        {
            if (ModelState.IsValid)
            {
                Request request = new Request();
                request.CreationDate = DateTime.UtcNow;
                request.Theme = requestCreate.Theme;
                request.Text = requestCreate.Text;
                _context.Add(request);
                await _context.SaveChangesAsync();
                _context.RequestRequestStatus.Add(new RequestRequestStatus() { RequestId = request.Id, RequestStatusId = 1, CreationDate = request.CreationDate });
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View();
        }
        [Authorize(Roles = Constants.administrator)]
        public IActionResult ChangeStatus(int id)
        {
            var request = _context.Request.FirstOrDefault(c => c.Id == id);
            if (request == null)
            {
                return NotFound();
            }
            RequestChangeStatus viewModel = new RequestChangeStatus();
            var lastRequestStatus = _context.RequestRequestStatus.AsEnumerable().Where(c => c.RequestId == id).MaxBy(c => c.CreationDate);
            viewModel.RequestStatusId = _context.RequestStatus.FirstOrDefault(c => c.Id == lastRequestStatus.RequestStatusId).Id;
            viewModel.Theme = request.Theme;
            viewModel.Text = request.Text;
            ViewData["RequestStatuses"] = new SelectList(_context.RequestStatus, "Id", "Name", viewModel.RequestStatusId);
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.administrator)]
        public async Task<IActionResult> ChangeStatus(int id, RequestChangeStatus requestChangeStatus)
        {
            if (ModelState.IsValid)
            {
                var request = _context.Request.Find(id);
                _context.RequestRequestStatus.Add(new RequestRequestStatus() { RequestId = request.Id, RequestStatusId = requestChangeStatus.RequestStatusId, CreationDate = DateTime.UtcNow });
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(requestChangeStatus);
        }
    }
}
