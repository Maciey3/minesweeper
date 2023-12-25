using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweeperWinForms
{
    internal class Tile
    {
        public int? Value;
        public int PositionX, PositionY;
        public bool Clicked;
        public bool Marked;
        public Tile(int? value, int positionX, int positionY, bool clicked, bool marked = false) { 
            Value = value;
            PositionX = positionX;
            PositionY = positionY;
            Clicked = clicked;
            Marked = marked;
        }
    }
}
