using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace testMatch3DLL
{
    public class Gemme
    {
        private static Random rnjesus = new Random();
        private Couleurs _couleur;
        public Couleurs Couleur
        {
            get { return _couleur; }
            private set { _couleur = value; }
        }

        public Gemme()
        {
            Couleur = donnerCouleurAléatoire();
        }

        public Gemme(Gemme copiedGem)
        {
            Couleur = copiedGem.Couleur;
        }

        private Couleurs donnerCouleurAléatoire()
        {
            return (Couleurs)rnjesus.Next(0, 6);
        }
    }
}