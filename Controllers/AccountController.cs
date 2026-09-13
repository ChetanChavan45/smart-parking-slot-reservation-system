using Microsoft.AspNetCore.Mvc;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;
using PametniParkingSistem.Services.Interfaces;
using PametniParkingSistem.ViewModels.Account;

namespace PametniParkingSistem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result =
                await _accountService.RegisterAsync(model);

            if (!result.Succeeded)
            {
                AddErrorsToModelState(result.Errors);
                return View(model);
            }

            return RedirectByRole(result.Korisnik!);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result =
                await _accountService.LoginAsync(model);

            if (!result.Succeeded)
            {
                AddErrorsToModelState(result.Errors);
                return View(model);
            }

            return RedirectByRole(result.Korisnik!);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();

            return RedirectToAction(
                "Index",
                "Home"
            );
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectByRole(
            Korisnik korisnik)
        {
            return korisnik.Uloga switch
            {
                Uloga.Administrator =>
                    RedirectToAction("Index", "Korisnik"),

                Uloga.Operater =>
                    RedirectToAction("Index", "ParkingMjesto"),

                _ =>
                    RedirectToAction("Index", "Home")
            };
        }

        private void AddErrorsToModelState(
            IEnumerable<string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error
                );
            }
        }
    }
}