﻿using System.Data.Entity;
using System.Web.Mvc;
using Repozytorium.Repo;
using Repozytorium.Models;
using Repozytorium.IRepo;
using Repozytorium.Models.Views;
using System.Linq;
using System;
using PagedList;
using System.Net;

namespace OGL2.Controllers
{
    public class KategoriaController : Controller
    {
        private readonly IKategoriaRepo _repo;
        private readonly IOgloszenieRepo _oglRepo;
        public KategoriaController(IKategoriaRepo repo, IOgloszenieRepo oglRepo)
        {
            _repo = repo;
            _oglRepo = oglRepo;
        }

        // GET
        public ActionResult Dodaj()
        {
            return View();
        }

        // POST
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Dodaj(Kategoria kategoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repo.Dodaj(kategoria);
                    _repo.SaveChanges();
                    return RedirectToAction("Sukces");
                }
                catch (Exception)
                {
                    return View(kategoria);
                }
            }
            return View(kategoria);
        }

        public ActionResult Sukces()
        {
            return View();
        }

        // GET: Kategoria
        public ActionResult Index(int? page, string sortOrder)
        {
            int currentPage = page ?? 1;
            int naStronie = 10;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NazwaSort = sortOrder == "NazwaAsc" ? "Nazwa" : "NazwaAsc";
            ViewBag.OpisSort = sortOrder == "OpisAsc" ? "Opis" : "OpisAsc";
            ViewBag.IloscOfertSort = sortOrder == "IloscOfertAsc" ? "IloscOfert" : "IloscOfertAsc";

            var kategorie = _repo.PobierzKategorie();

            switch (sortOrder)
            {
                case "Nazwa":
                    kategorie = kategorie.OrderByDescending(s => s.Nazwa);
                    break;
                case "NazwaAsc":
                    kategorie = kategorie.OrderBy(s => s.Nazwa);
                    break;

                case "Opis":
                    kategorie = kategorie.OrderByDescending(s => s.Nazwa);
                    break;
                case "OpisAsc":
                    kategorie = kategorie.OrderBy(s => s.Opis);
                    break;

                case "IloscOfert": 
                    kategorie = kategorie.OrderByDescending(s => s.LiczbaOfert);
                    break;
                case "IloscOfertAsc":
                    kategorie = kategorie.OrderBy(s => s.LiczbaOfert);
                    break;
            }
            return View(kategorie.ToPagedList<KategoriaViewModel>(currentPage, naStronie));
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OgloszenieDetailsViewModel ogloszenie = _oglRepo.GetOgloszeniaById((int)id);
            if (ogloszenie == null)
            {
                return HttpNotFound();
            }
            return View(ogloszenie);
        }

        public ActionResult PokazOgloszenia(int id, int? page, string sortOrder)
        {
            int currentPage = page ?? 1;
            int naStronie = 12;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdOgloszenia = sortOrder == "IdOgloszenia" ? "IdOgloszeniaAsc" : "IdOgloszenia";
            ViewBag.DataDodaniaSort = sortOrder == "DataDodania" ? "DataDodaniaAsc" : "DataDodania";
            ViewBag.TytulSort = sortOrder == "TytulAsc" ? "Tytul" : "TytulAsc";
            ViewBag.MiastoSort = sortOrder == "MiastoAsc" ? "Miasto" : "MiastoAsc";
            ViewBag.RodzajUmowySort = sortOrder == "RodzajUmowyAsc" ? "RodzajUmowy" : "RodzajUmowyAsc";
            var ogloszenia = _repo.PobierzOgloszeniaZKategorii(id);
            switch (sortOrder)
            {
                case "IdOgloszenia":
                    ogloszenia = ogloszenia.OrderByDescending(s => s.IdOgloszenia);
                    break;
                case "IdOgloszeniaAsc":
                    ogloszenia = ogloszenia.OrderBy(s => s.IdOgloszenia);
                    break;

                case "RodzajUmowy":
                    ogloszenia = ogloszenia.OrderByDescending(s => s.RodzajUmowy);
                    break;
                case "RodzajUmowyAsc":
                    ogloszenia = ogloszenia.OrderBy(s => s.RodzajUmowy);
                    break;

                case "Miasto":
                    ogloszenia = ogloszenia.OrderByDescending(s => s.Miasto);
                    break;
                case "MiastoAsc":
                    ogloszenia = ogloszenia.OrderBy(s => s.Miasto);
                    break;

                case "DataDodania":
                    ogloszenia = ogloszenia.OrderByDescending(s => s.DataDodania);
                    break;
                case "DataDodaniaAsc":
                    ogloszenia = ogloszenia.OrderBy(s => s.DataDodania);
                    break;

                case "Tytul":
                    ogloszenia = ogloszenia.OrderByDescending(s => s.Tytul);
                    break;
                case "TytulAsc":
                    ogloszenia = ogloszenia.OrderBy(s => s.Tytul);
                    break;

                default:  // id descending
                    ogloszenia = ogloszenia.OrderByDescending(s => s.DataDodania);
                    break;
            }
            return View(ogloszenia.ToPagedList<OgloszeniaZKategoriiViewModels>(currentPage, naStronie));
        }

        [Route("JSON")]
        public ActionResult KategorieWJason()
        {
            var kategorie = _repo.PobierzKategorie().AsNoTracking();
            return Json(kategorie, JsonRequestBehavior.AllowGet);
        }

        // ------------------------- EDIT -------------------------------------
        // GET
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kategoria kategoria = _repo.GetKategoriaById((int)id);

            if (kategoria == null)
            {
                return HttpNotFound();
            }
            else if (!(User.IsInRole("Admin")))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(kategoria);
        }


        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(Kategoria kategoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // ogloszenie.UzytkownikId = "ffgfs";
                    _repo.Aktualizuj(kategoria);
                    _repo.SaveChanges();
                }
                catch (Exception)
                {
                    ViewBag.Blad = true;
                    return View(kategoria);
                }
            }
            ViewBag.Blad = false;
            return View(kategoria);
        }

        // GET: Kategoria
        public ActionResult IndexCV(int? page, string sortOrder)
        {
            int currentPage = page ?? 1;
            int naStronie = 10;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NazwaSort = sortOrder == "NazwaAsc" ? "Nazwa" : "NazwaAsc";
            ViewBag.OpisSort = sortOrder == "OpisAsc" ? "Opis" : "OpisAsc";
            ViewBag.IloscOfertSort = sortOrder == "IloscOfertAsc" ? "IloscOfert" : "IloscOfertAsc";

            var kategorie = _repo.PobierzKategorieCV();

            switch (sortOrder)
            {
                case "Nazwa":
                    kategorie = kategorie.OrderByDescending(s => s.Nazwa);
                    break;
                case "NazwaAsc":
                    kategorie = kategorie.OrderBy(s => s.Nazwa);
                    break;

                case "Opis":
                    kategorie = kategorie.OrderByDescending(s => s.Nazwa);
                    break;
                case "OpisAsc":
                    kategorie = kategorie.OrderBy(s => s.Opis);
                    break;

                case "IloscOfert":
                    kategorie = kategorie.OrderByDescending(s => s.LiczbaOfert);
                    break;
                case "IloscOfertAsc":
                    kategorie = kategorie.OrderBy(s => s.LiczbaOfert);
                    break;
            }            
            return View(kategorie.ToPagedList<KategoriaCVViewModel>(currentPage, naStronie));
        }

        public ActionResult PokazCV(int id, int? page, string sortOrder)
        {
            int currentPage = page ?? 1;
            int naStronie = 12;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdOgloszenia = sortOrder == "IdOgloszenia" ? "IdOgloszeniaAsc" : "IdOgloszenia";
            ViewBag.DataDodaniaSort = sortOrder == "DataDodania" ? "DataDodaniaAsc" : "DataDodania";
            ViewBag.TytulSort = sortOrder == "TytulAsc" ? "Tytul" : "TytulAsc";
            ViewBag.MiastoSort = sortOrder == "MiastoAsc" ? "Miasto" : "MiastoAsc";
            ViewBag.RodzajUmowySort = sortOrder == "ImieAsc" ? "Imie" : "ImieAsc";
            var cv = _repo.PobierzCVZKategorii(id);
            switch (sortOrder)
            {
                case "IdOgloszenia":
                    cv = cv.OrderByDescending(s => s.IdCV);
                    break;
                case "IdOgloszeniaAsc":
                    cv = cv.OrderBy(s => s.IdCV);
                    break;

                case "Imie":
                    cv = cv.OrderByDescending(s => s.Nazwisko);
                    break;
                case "ImieAsc":
                    cv = cv.OrderBy(s => s.Nazwisko);
                    break;

                case "Miasto":
                    cv = cv.OrderByDescending(s => s.Miasto);
                    break;
                case "MiastoAsc":
                    cv = cv.OrderBy(s => s.Miasto);
                    break;

                case "DataDodania":
                    cv = cv.OrderByDescending(s => s.DataDodania);
                    break;
                case "DataDodaniaAsc":
                    cv = cv.OrderBy(s => s.DataDodania);
                    break;

                case "Tytul":
                    cv = cv.OrderByDescending(s => s.Tytul);
                    break;
                case "TytulAsc":
                    cv = cv.OrderBy(s => s.Tytul);
                    break;

                default:  // id descending
                    cv = cv.OrderByDescending(s => s.DataDodania);
                    break;
            }
            return View(cv.ToPagedList<CVZKategoriiViewModels>(currentPage, naStronie));
        }
    }
}
