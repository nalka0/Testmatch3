using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace testMatch3DLL
{
    public class Plateau
    {
        public Gemme[][] gemmes;
        private static List<Vector2> toutesPosSuppr = new List<Vector2>();
        public static readonly int columnCount = 8;
        public static readonly int rowCount = 8;

        //testé et fonctionnel
        internal void supprimerGemmes(Joueur activePlayer)
        {
            trierListeSuppression();
            nettoyerListeSuppression();
            foreach (Vector2 element in toutesPosSuppr)
            {
                gemmes[(int)element.X][(int)element.Y] = null;
                if (activePlayer != null)
                    activePlayer.Score++;
            }
            int x = rowCount - 1;
            int y = columnCount - 1;
            while (x > -1)
            {
                while (y > -1)
                {
                    if (gemmes[x][y] == null)
                        faireDescendre(new Vector2(x, y));
                    else
                        y--;
                }
                x--;
                y = columnCount - 1;
            }
        }

        internal void faireDescendre(Vector2 gemmeMorte)
        {
            while (gemmeMorte.X > 0)
            {
                gemmes[(int)gemmeMorte.X][(int)gemmeMorte.Y] = gemmes[(int)gemmeMorte.X - 1][(int)gemmeMorte.Y];
                gemmeMorte.X--;
            }
            gemmes[0][(int)gemmeMorte.Y] = new Gemme();
        }

        //testé et fonctionnel
        public void initialiser()
        {
            int i = 0;
            int j = 0;
            gemmes = new Gemme[rowCount][];
            foreach (Gemme[] element in gemmes)
            {
                gemmes[i] = new Gemme[columnCount];
                foreach (Gemme item in gemmes[i])
                {
                    gemmes[i][j] = new Gemme();
                    j++;
                }
                i++;
                j = 0;
            }
        }

        //testé et fonctionnel
        private static void trierListeSuppression()
        {
            int futurePosition = 0;
            List<Vector2> resultat = new List<Vector2>();
            foreach (Vector2 element in toutesPosSuppr)
            {
                if (resultat.Count == 0)
                {
                    resultat.Add(element);
                    continue;
                }
                foreach (Vector2 item in resultat)
                {
                    if (element.X < item.X || (element.X == item.X && element.Y < item.Y))
                    {
                        resultat.Insert(futurePosition, element);
                        break;
                    }
                    futurePosition++;
                    if (resultat.Count() <= futurePosition)
                    {
                        resultat.Add(element);
                        break;
                    }
                }
                futurePosition = 0;
            }
            toutesPosSuppr = resultat;
        }

        //testé et fonctionnel
        public  void faireMatch(Joueur activePlayer)
        {
            bool alreadyPassed = false;
            do
            {
                toutesPosSuppr.Clear();
                int ligne = 0;
                int colonne = 0;
                List<Vector2> posLigneSuppr = new List<Vector2>();
                List<Vector2> posColonneSuppr = new List<Vector2>();
                while (ligne < rowCount)
                {
                    foreach (Gemme item in gemmes[ligne])
                    {
                        posLigneSuppr.Clear();
                        if (compterGemmesEgales(ligne, colonne, Directions.Droite, item.Couleur, posLigneSuppr) + compterGemmesEgales(ligne, colonne, Directions.Gauche, item.Couleur, posLigneSuppr) + 1 > 2)
                        {
                            foreach (Vector2 element in posLigneSuppr)
                            {
                                toutesPosSuppr.Add(element);
                            }
                        }
                        posColonneSuppr.Clear();
                        if (compterGemmesEgales(ligne, colonne, Directions.Bas, item.Couleur, posColonneSuppr) + compterGemmesEgales(ligne, colonne, Directions.Haut, item.Couleur, posColonneSuppr) + 1 > 2)
                        {
                            foreach (Vector2 element in posColonneSuppr)
                            {
                                toutesPosSuppr.Add(element);
                            }
                        }
                        colonne++;
                    }
                    colonne = 0;
                    ligne++;
                }
                if (!alreadyPassed && toutesPosSuppr.Count == 0)
                {
                    throw new customException();
                }
                supprimerGemmes(activePlayer);
                alreadyPassed = true;
            } while (toutesPosSuppr.Count > 0);
        }

        //testé et fonctionnel (essayer en enlevant ref)
        private int compterGemmesEgales(int ligne, int colonne, Directions direction, Couleurs matchingColor, List<Vector2> positions)
        {
            int ret = -1;
            while (ligne > -1 && ligne < rowCount && colonne > -1 && colonne < columnCount && gemmes[ligne][colonne].Couleur == matchingColor)
            {
                ret++;
                if (ret != 0)
                    positions.Add(new Vector2(ligne, colonne));
                if (direction == Directions.Gauche)
                    colonne--;
                else if (direction == Directions.Droite)
                    colonne++;
                else if (direction == Directions.Haut)
                    ligne--;
                else if (direction == Directions.Bas)
                    ligne++;
            }
            return ret;
        }

        private static void nettoyerListeSuppression()
        {
            List<Vector2> propre = new List<Vector2>();
            foreach (Vector2 element in toutesPosSuppr)
            {
                if (!propre.Contains(element))
                    propre.Add(element);
            }
            toutesPosSuppr = propre;
        }
    }
}