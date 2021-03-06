﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Repozytorium.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Repozytorium.Models
{
    // You can add profile data for the user by adding more properties to your Uzytkownik class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class Uzytkownik : IdentityUser
    {
        public Uzytkownik()
        {
            this.Ogloszenia = new HashSet<Ogloszenie>();
            this.CVs = new HashSet<CV>();
            this.Wiadomosci = new HashSet<Wiadomosc>();
        }
        //public int Id { get; set; }
        // klucz podstawowy odziedziczony po klasie IdentityUser       
        public string Imie { get; set; }
        public string Nazwisko { get; set; }
        public int? Wiek { get; set; }
        public string Firma { get; set; }
        public DateTime? DataUrodzenia { get; set; }
        public string Ulica { get; set; }
        public string NumerDomu { get; set; }
        public string NazwaMiasta { get; set; }
        public string Kod { get; set; }
        public string Telefon { get; set; }
        public string Description { get; set; }
        public int MiastoId { get; set; }

        public virtual Miasto Miasto { get; set; }

        #region dodatkowe pole notmapped
        [NotMapped]
        [Display(Name = "Pan/Pan:")]
        public string PelneNazwisko
        {
            get { return Imie + ' ' + Nazwisko; }
        }
        #endregion

        public virtual ICollection<Ogloszenie> Ogloszenia { get; private set; }
        public virtual ICollection<CV> CVs { get; private set; }
        public virtual ICollection<Wiadomosc> Wiadomosci { get; private set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<Uzytkownik> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}