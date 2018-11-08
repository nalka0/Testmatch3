using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace testMatch3DLL
{
    public class Joueur
    {
        private string _pseudo;
        public string Pseudo
        {
            get { return _pseudo; }
            set { _pseudo = value; }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        private int _score;
        public int Score
        {
            get { return _score; }
            set { _score = value; }
        }
        
        public Joueur(string nickname)
        {
            Pseudo = nickname;
            IsActive = false;
            Score = 0;
        }
    }
}
