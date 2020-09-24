using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace IntensisKatzeService1.Models.EntityModels
{
    public  class Zaposleni
    {
        public int IDReg { get; set; }
        public string IDNo { get; set; }
        public string KorisnickoIme { get; set; }
        public DateTime TerminalskoVremeRegistracije { get; set; }
        public int Nevidljiva { get; set; }
        public int RegType { get; set; }
        public string Email { get; set; }



    }



}
