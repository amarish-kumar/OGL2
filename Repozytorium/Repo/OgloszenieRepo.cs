﻿using Repozytorium.IRepo;
using Repozytorium.Models;
using System.Diagnostics;
using System.Linq;
using System;
using System.Web.Mvc;
using System.Data.Entity;
using Repozytorium.Models.Views;
using System.Collections.Generic;
using AutoMapper;

namespace Repozytorium.Repo
{
    public class OgloszenieRepo : IOgloszenieRepo
    {
        private readonly IOglContext _db;
        //private readonly IMappingService _mapper;
        public OgloszenieRepo(IOglContext db)
        {
            _db = db;
        }

        public void Aktualizuj(OgloszenieEditViewModel ogloszenieEditViewModel)
        {
            Ogloszenie ogloszenie = new Ogloszenie()
            {
                Firma = ogloszenieEditViewModel.Firma,
                Id = ogloszenieEditViewModel.IdOgloszenia,
                Tresc = ogloszenieEditViewModel.Tresc,
                Tytul = ogloszenieEditViewModel.Tytul,
                DataDodania = ogloszenieEditViewModel.DataDodania,
                UzytkownikId = ogloszenieEditViewModel.UzytkownikId,
                DataWaznosci = ogloszenieEditViewModel.DataDodania.AddDays(14),
                Zaakceptowane = ogloszenieEditViewModel.Zaakceptowane,
                MiastoId = ogloszenieEditViewModel.MiastoId,
                RodzajUmowyId = ogloszenieEditViewModel.RodzajUmowyId,
                ZarobkiOd = ogloszenieEditViewModel.ZarobkiOd,
                ZarobkiDo = ogloszenieEditViewModel.ZarobkiDo,
                Wymagania = ogloszenieEditViewModel.Wymagania
            };
            _db.Entry(ogloszenie).State = EntityState.Modified;
            //var row = _db.Ogloszenie_Kategoria.Where(p => p.KategoriaId == ogloszenieEditViewModel.KategoriaId).
            //    Where(p => p.OgloszenieId == ogloszenieEditViewModel.IdOgloszenia).FirstOrDefault();
            //if (row != null)
            //    InsertOgloszenieKategoria(Convert.ToInt32(ogloszenieEditViewModel.IdOgloszenia), ogloszenieEditViewModel.KategoriaId);
        }

        public void Dodaj(OgloszenieEditViewModel ogloszenieEditViewModel)
        {
            Ogloszenie ogloszenie = new Ogloszenie()
            {
                Firma = ogloszenieEditViewModel.Firma,
                Tresc = ogloszenieEditViewModel.Tresc,
                Tytul = ogloszenieEditViewModel.Tytul,
                DataDodania = ogloszenieEditViewModel.DataDodania,
                UzytkownikId = ogloszenieEditViewModel.UzytkownikId,
                DataWaznosci = ogloszenieEditViewModel.DataDodania.AddDays(14),
                Zaakceptowane = ogloszenieEditViewModel.Zaakceptowane,                
                MiastoId = ogloszenieEditViewModel.MiastoId,
                RodzajUmowyId = ogloszenieEditViewModel.RodzajUmowyId,
                ZarobkiOd = ogloszenieEditViewModel.ZarobkiOd,
                ZarobkiDo = ogloszenieEditViewModel.ZarobkiDo,
                Wymagania = ogloszenieEditViewModel.Wymagania
            };
            _db.Ogloszenia.Add(ogloszenie);
        }

        public void InsertOgloszenieKategoria(int kategoriaId)
        {
            var lastId = _db.Ogloszenia.Max(p => p.Id).ToString();

            Ogloszenie_Kategoria oglkat = new Ogloszenie_Kategoria()
            {
                KategoriaId = kategoriaId,
                OgloszenieId = Convert.ToInt32(lastId)
            };
            _db.Ogloszenie_Kategoria.Add(oglkat);
        }

        public List<RodzajUmowy> GetAgreementTypes()
        {
            var umowy = from o in _db.RodzajUmowy select o;
            return umowy.ToList();
        }

        public OgloszenieEditViewModel GetOgloszenieDetailsById(int id)
        {
            var ogloszenie = from o in _db.Ogloszenia.Include("Uzytkownik").Include("Miasto")
                             join k in _db.Ogloszenie_Kategoria on o.Id equals k.OgloszenieId
                             where o.Zaakceptowane == true && o.DataWaznosci > DateTime.Now
                             orderby o.DataDodania
                             select new OgloszenieEditViewModel
                             {
                                 UzytkownikId = o.UzytkownikId,
                                 Firma = o.Firma,
                                 IdOgloszenia = o.Id,
                                 Tresc = o.Tresc,
                                 Tytul = o.Tytul,
                                 MiastoId = o.MiastoId,
                                 RodzajUmowyId = o.RodzajUmowyId,
                                 KategoriaId = k.KategoriaId,
                                 ZarobkiOd = o.ZarobkiOd,
                                 ZarobkiDo = o.ZarobkiDo,
                                 Wymagania = o.Wymagania,
                                 DataDodania = o.DataDodania,
                                 Zaakceptowane = o.Zaakceptowane.HasValue ? true : false
                             };
            var x = ogloszenie.Where(p => p.IdOgloszenia == id).FirstOrDefault();
            return x;
        }

        public OgloszenieDetailsViewModel GetOgloszeniaById(int id)
        {
            var ogloszenie = from o in _db.Ogloszenia.Include("Uzytkownik").Include("Miasto")
                             where o.Zaakceptowane == true && o.DataWaznosci > DateTime.Now
                             orderby o.DataDodania
                             select new OgloszenieDetailsViewModel
                             {
                                 UzytkownikId = o.UzytkownikId,
                                 Firma = o.Uzytkownik.Firma,
                                 IdOgloszenia = o.Id,
                                 Tresc = o.Tresc,
                                 Tytul = o.Tytul,
                                 Miasto = o.Miasto.Nazwa,
                                 RodzajUmowy = o.RodzajUmowy.Nazwa,
                                 ZarobkiOd = o.ZarobkiOd,
                                 ZarobkiDo = o.ZarobkiDo,
                                 Wymagania = o.Wymagania,
                                 DataDodania = o.DataDodania
                             };
            var x = ogloszenie.Where(p => p.IdOgloszenia == id).FirstOrDefault();
            return x;
        }

        public IQueryable<OgloszenieViewModel> PobierzOgloszenia()
        {
            var ogloszeniaList = new List<OgloszenieViewModel>();
            //_db.Database.Log = message => Trace.WriteLine(message);
            //var ogloszenia = _db.Ogloszenia.AsNoTracking();
            //return ogloszenia;

            var ogloszenia = from o in _db.Ogloszenia.Include("Uzytkownik").Include("Miasto")
                             where o.Zaakceptowane == true && o.DataWaznosci > DateTime.Now 
                            orderby o.DataDodania
                             select new
                                        {
                                            UzytkownikId = o.UzytkownikId,
                                            Firma = o.Firma,
                                            IdOgloszenia = o.Id,
                                            Tytul = o.Tytul,
                                            Miasto = o.Miasto.Nazwa,
                                            RodzajUmowy = o.RodzajUmowy.Nazwa,
                                            DataDodania = o.DataDodania,
                                            Zaakceptowane = o.Zaakceptowane.HasValue ? true : false
                             };
            var x = ogloszenia.ToList();
            foreach (var ogloszenie in x)
            {
                ogloszeniaList.Add(new OgloszenieViewModel
                {
                    UzytkownikId = ogloszenie.UzytkownikId,
                    Firma = ogloszenie.Firma,
                    IdOgloszenia = ogloszenie.IdOgloszenia,
                    Tytul = ogloszenie.Tytul,
                    Miasto = ogloszenie.Miasto,
                    RodzajUmowy = ogloszenie.RodzajUmowy,
                    DataDodania = ogloszenie.DataDodania,
                    Zaakceptowane = ogloszenie.Zaakceptowane
                });
            }            
            return ogloszeniaList.AsQueryable();
        }

        //public IQueryable<OgloszenieViewModel> PobierzStrone(int? page = 1, int? pageSize = 10)
        //{
            //var ogloszenia = _db.Ogloszenia
            //    .OrderByDescending(o => o.DataDodania)
            //    .Skip((page.Value - 1) * pageSize.Value)
            //    .Take(pageSize.Value);
            //return ogloszenia;
        //    return null;
        //}

        public void SaveChanges()
        {
            _db.SaveChanges();
        }

        public void UsunOgloszenie(int id)
        {
            UsunPowiazanieOgloszenieKategoria(id);
            Ogloszenie ogloszenie = _db.Ogloszenia.Find(id);
            _db.Ogloszenia.Remove(ogloszenie);
        }

        public void UsunPowiazanieOgloszenieKategoria(int id)
        {
            var powiazaneOgloszenia =
                _db.Ogloszenie_Kategoria.Where(o => o.OgloszenieId == id);
             foreach (var ok in powiazaneOgloszenia)
            {
                _db.Ogloszenie_Kategoria.Remove(ok);
            }
        }
    }
}