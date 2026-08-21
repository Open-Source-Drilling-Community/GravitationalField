using OSDC.DotnetLibraries.Drilling.DrillingProperties;
using OSDC.DotnetLibraries.General.DataManagement;
using System;
using System.ComponentModel.DataAnnotations;

namespace NORCE.Drilling.GravitationalField.Model
{
    /// <summary>
    /// a base class other classes may derive from
    /// </summary>
    public class GravitationalData
    {
        /// <summary>
        /// WGS84 geodetic latitude in radians (SI).
        /// </summary>
        [Range(-1.5707963267948966, 1.5707963267948966, ErrorMessage = "Latitude must be expressed in SI radians between -pi/2 and pi/2.")]
        public double Latitude { get; set; }
        /// <summary>
        /// WGS84 geodetic longitude in radians (SI).
        /// </summary>
        [Range(-3.1415926535897931, 3.1415926535897931, ErrorMessage = "Longitude must be expressed in SI radians between -pi and pi.")]
        public double Longitude { get; set; }
        /// <summary>
        /// True vertical depth below the WGS84 ellipsoid in metres (SI), positive downward.
        /// </summary>
        public double Depth { get; set; }
        /// <summary>
        /// Calculated easterly component of gravitational acceleration in metres per second squared (SI).
        /// </summary>
        public double? GravityIntensityX { get; set; }
        /// <summary>
        /// Calculated northerly component of gravitational acceleration in metres per second squared (SI).
        /// </summary>
        public double? GravityIntensityY { get; set; }
        /// <summary>
        /// Calculated upward component of gravitational acceleration in metres per second squared (SI), normally negative.
        /// </summary>
        public double? GravityIntensityZ { get; set; }


        /// <summary>
        /// default constructor required for JSON serialization
        /// </summary>
        public GravitationalData() : base()
        {
        }
    }
}
