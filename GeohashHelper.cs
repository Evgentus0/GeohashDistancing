using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GeohashDistancing
{
    public partial class GeohashHelper
    {
        
        private const int BITPERSITION = 52;
        private const int MINLAT = -90, MAXLAT = 90;
        private const int MINLNG = -180, MAXLNG = 180;

        #region Base geohash module

        /// <summary>
        /// Method for finding lat lng borders based on the hash and bit persition
        /// </summary>
        /// <param name="hash">int64 encoded geohash</param>
        /// <param name="bitPersition">Persition for bit geohash encoding</param>
        /// <returns>Turple of max and min values of lat lng</returns>
        public (double minLat, double maxLat, double minLng, double maxLng) GetBoxBordersLatLng(ulong hash, int bitPersition = 52)
        {
            double minLat = MINLAT, maxLat = MAXLAT,
                   minLng = MINLNG, maxLng = MAXLNG;


            for (int i = 0, latLngBitsForCheck = bitPersition / 2, currentHighOrderBit = latLngBitsForCheck * 2;
                i < latLngBitsForCheck; ++i, currentHighOrderBit = (latLngBitsForCheck - i) * 2)
            {
                ulong lngBit = (hash >> (currentHighOrderBit - 1)) & 1;
                ulong latBit = (hash >> (currentHighOrderBit - 2)) & 1;

                if (latBit == 0)
                {
                    maxLat = (maxLat + minLat) / 2;
                }
                else
                {
                    minLat = (maxLat + minLat) / 2;
                }

                if (lngBit == 0)
                {
                    maxLng = (maxLng + minLng) / 2;
                }
                else
                {
                    minLng = (maxLng + minLng) / 2;
                }
            }

            return (minLat, maxLat, minLng, maxLng);
        }

        /// <summary>
        /// Method for finding lat lng borders based on the hash
        /// </summary>
        /// <param name="hash">int64 encoded geohash</param>
        /// <returns>Turple of max and min values of lat lng</returns>
        public (double minLat, double maxLat, double minLng, double maxLng) GetBoxBordersLatLng(ulong hash)
        {
            int bitPersition = (int)(Math.Log(hash) / Math.Log(2)) + 1;
           
            return GetBoxBordersLatLng(hash, bitPersition);
        }

        /// <summary>
        /// Method for encoding lat lnt into int64 hash with spesific bit persition 
        /// </summary>
        /// <param name="lat">Lat coordinate of point</param>
        /// /// <param name="lng">Lng coordinate of point</param>
        /// <param name="bitPersition">Persition for bit geohash encoding</param>
        /// <returns>Geohash encoded into int64</returns>
        public ulong EncodeGeohash(double lat, double lng, int bitsPersition = BITPERSITION)
        {
            double minLat = MINLAT, maxLat = MAXLAT,
                   minLng = MINLNG, maxLng = MAXLNG;
            ulong result = 0;
            double midPoint;
            for (int i = 0; i < bitsPersition; ++i)
                if (i % 2 == 0)
                {                 // even bit: bisect longitude
                    midPoint = (minLng + maxLng) / 2;
                    if (lng < midPoint)
                    {
                        result <<= 1;                 // push a zero bit
                        maxLng = midPoint;            // shrink range downwards
                    }
                    else
                    {
                        result = result << 1 | 1;     // push a one bit
                        minLng = midPoint;            // shrink range upwards
                    }
                }
                else
                {                          // odd bit: bisect latitude
                     midPoint = (minLat + maxLat) / 2;
                    if (lat < midPoint)
                    {
                        result <<= 1;                 // push a zero bit
                        maxLat = midPoint;            // shrink range downwards
                    }
                    else
                    {
                        result = result << 1 | 1;     // push a one bit
                        minLat = midPoint;            // shrink range upwards
                    }
                }
           
            return result; 
        }

        #endregion

        #region Naighbor module

        /// <summary>
        /// Find the South neighbor for box with borders and specific bit percition
        /// </summary>
        /// <param name="minLat">Min Lat of the box</param>
        /// <param name="maxLat">Max Lat of the box</param>
        /// <param name="minLng">Min Lng of the box</param>
        /// <param name="maxLng">Max Lng of the box</param>
        /// <param name="bitPersition"> Persition of bits</param>
        /// <returns>int64 hash for South neighbor</returns>
        public ulong South(double minLat, double maxLat, double minLng, double maxLng, int bitPersition = BITPERSITION)
        {

            double latDiff = maxLat - minLat;
            double lat = minLat - latDiff / 2;
            double lng = (minLng + maxLng) / 2;

            if (lat < MINLAT)
            {
                lat = (MINLAT + (MINLAT - lat)) * -1;
            }

            return EncodeGeohash(lat, lng, bitPersition);
        }


        /// <summary>
        /// Find the North neighbor for box with borders and specific bit percition
        /// </summary>
        /// <param name="minLat">Min Lat of the box</param>
        /// <param name="maxLat">Max Lat of the box</param>
        /// <param name="minLng">Min Lng of the box</param>
        /// <param name="maxLng">Max Lng of the box</param>
        /// <param name="bitPersition"> Persition of bits</param>
        /// <returns>int64 hash for North neighbor</returns>
        public ulong North(double minLat, double maxLat, double minLng, double maxLng, int bitPersition = BITPERSITION)
        {

            double latDiff = maxLat - minLat;
            double lat = maxLat + latDiff / 2;
            double lng = (minLng + maxLng) / 2;

            if (lat > MAXLAT)
            {
                lat = (MAXLAT - (lat - MAXLAT)) * -1;
            }

            return EncodeGeohash(lat, lng, bitPersition);
        }


        /// <summary>
        /// Find the East neighbor for box with borders and specific bit percition
        /// </summary>
        /// <param name="minLat">Min Lat of the box</param>
        /// <param name="maxLat">Max Lat of the box</param>
        /// <param name="minLng">Min Lng of the box</param>
        /// <param name="maxLng">Max Lng of the box</param>
        /// <param name="bitPersition"> Persition of bits</param>
        /// <returns>int64 hash for East neighbor</returns>
        public ulong East(double minLat, double maxLat, double minLng, double maxLng, int bitPersition = BITPERSITION)
        {

            double lngDiff = maxLng - minLng;
            double lat = (minLat + maxLat) / 2;
            double lng = maxLng + lngDiff / 2;

            if (lng > MAXLNG)
            {
                lng = MINLNG + (lng - MAXLNG);
            }
            if (lng < MINLNG)
            {
                lng = MINLNG;
            }

            return EncodeGeohash(lat, lng, bitPersition);
        }

        /// <summary>
        /// Find the West neighbor for box with borders and specific bit percition
        /// </summary>
        /// <param name="minLat">Min Lat of the box</param>
        /// <param name="maxLat">Max Lat of the box</param>
        /// <param name="minLng">Min Lng of the box</param>
        /// <param name="maxLng">Max Lng of the box</param>
        /// <param name="bitPersition"> Persition of bits</param>
        /// <returns>int64 hash for West neighbor</returns>
        public ulong West(double minLat, double maxLat, double minLng, double maxLng, int bitPersition = BITPERSITION)
        {
            double lngDiff = maxLng - minLng;
            double lat = (minLat + maxLat) / 2;
            double lng = minLng - lngDiff / 2;
            if (lng < MINLNG)
            {
                lng = MAXLNG - (lng + MAXLNG);
            }
            if (lng > MAXLNG)
            {
                lng = MAXLNG;
            }

            return EncodeGeohash(lat, lng, bitPersition);
        }


        /// <summary>
        /// Get all neighbors of current hash
        /// </summary>
        /// <param name="hash">current box hash</param>
        /// <param name="bitPersition">Persition of bit</param>
        /// <returns>Neighbors obj with all neghbors</returns>
        public Neighbors GetAllNeighbors(ulong hash, int bitPersition = BITPERSITION)
        {
            var neighbors = new Neighbors();

            var bourders = GetBoxBordersLatLng(hash, bitPersition);

            neighbors.South = South(bourders.minLat, bourders.maxLat, bourders.minLng, bourders.maxLng, bitPersition);
            neighbors.East = East(bourders.minLat, bourders.maxLat, bourders.minLng, bourders.maxLng, bitPersition);
            neighbors.West = West(bourders.minLat, bourders.maxLat, bourders.minLng, bourders.maxLng, bitPersition);
            neighbors.North = North(bourders.minLat, bourders.maxLat, bourders.minLng, bourders.maxLng, bitPersition);

            var eastBourders = GetBoxBordersLatLng(neighbors.East, bitPersition);
            neighbors.SouthEast = South(eastBourders.minLat, eastBourders.maxLat, eastBourders.minLng, eastBourders.maxLng, bitPersition);
            neighbors.NorthEast = North(eastBourders.minLat, eastBourders.maxLat, eastBourders.minLng, eastBourders.maxLng, bitPersition);

            var westBourders = GetBoxBordersLatLng(neighbors.West, bitPersition);
            neighbors.SouthWest = South(westBourders.minLat, westBourders.maxLat, westBourders.minLng, westBourders.maxLng, bitPersition);
            neighbors.NorthWest = North(westBourders.minLat, westBourders.maxLat, westBourders.minLng, westBourders.maxLng, bitPersition);

            return neighbors;
        }

        #endregion

    }
}
