using System;
using System.Collections.Generic;
using System.Text;

namespace GeohashDistancing
{
    public class Neighbors
    {
        public ulong North { get; set; }

        public ulong South { get; set; }

        public ulong West { get; set; }

        public ulong East { get; set; }

        public ulong NorthEast { get; set; }

        public ulong NorthWest { get; set; }

        public ulong SouthEast { get; set; }

        public ulong SouthWest { get; set; }

        public Neighbors()
        {

        }

        public Neighbors(ulong north, ulong east, ulong west, ulong south, ulong northEast,
            ulong northWest, ulong southEast, ulong southWest)
        {
            North = north;
            South = south;
            West = west;
            East = east;
            NorthEast = northEast;
            NorthWest = northWest;
            SouthEast = southEast;
            SouthWest = southWest;
        }

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Neighbors neighbors = (Neighbors)obj;
                return (this.East == neighbors.East) && (this.South == neighbors.South) 
                    && (this.West == neighbors.West) && (this.North == neighbors.North)
                    && (this.NorthEast == neighbors.NorthEast) && (this.NorthWest == neighbors.NorthWest)
                    && (this.SouthEast == neighbors.SouthEast) && (this.SouthWest == neighbors.SouthWest);
            }
        }

        public override int GetHashCode()
        {
            return (int)((East ^ West + SouthWest ^ SouthEast + NorthEast ^ NorthWest + North^South) % int.MaxValue);
        }
    }
}
