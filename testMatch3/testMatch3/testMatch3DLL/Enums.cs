using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testMatch3DLL
{
    public enum Couleurs
    {
        Rouge,
        Vert,
        Jaune,
        Bleu,
        Noir,
        Blanc
    }

    public enum Directions
    {
        Droite,
        Gauche,
        Haut,
        Bas
    }

    public enum Instructions
    {
        MoveServeur,
        MovePlayer,
        RegisterPlayer,
        InitializeServeur
    }
}
