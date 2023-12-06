using OpenTap;
using System;
using System.Collections.Generic;

namespace Creotronics.OpenTAP.Instruments.PSU.API
{
    /// <summary>
    /// Interface for OpenTap Power Supply Plugins.
    /// </summary>
    public interface IPSU : IInstrument
    {
        #region Properties

        /// <summary>
        /// Get the power supply channel count.
        /// </summary>
        UInt16 Channels { get; }

        /// <summary>
        /// Get the power supply maximum voltage that can be set.
        /// </summary>
        List<double> MaxVoltage { get; }

        /// <summary>
        /// Get the power supply minimum voltage that can be set.
        /// </summary>
        List<double> MinVoltage { get; }

        /// <summary>
        /// Get the power supply maximum current that can be set.
        /// </summary>
        List<double> MaxCurrent { get; }

        /// <summary>
        /// Get the power supply minimum current that can be set.
        /// </summary>
        List<double> MinCurrent { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Get the power supply identification information.
        /// </summary>
        /// <returns>Instrument identification.</returns>
        string Identify();

        /// <summary>
        /// Reset the instrument to the default settings.
        /// </summary>
        /// <remarks>Check power supply datasheet for the default setting values.</remarks>
        void Reset();

        /// <summary>
        /// Self test of instrument.
        /// </summary>
        /// <param name="testMessage">Self test message.</param>
        /// <returns>Self test status code (0: pass, other: fault code).</returns>
        int SelfTest(out string testMessage);

        /// <summary>
        /// The set output current. 
        /// </summary>
        /// <returns>Output current (unit = A).</returns>
        /// <param name="psuChannel">The instrument channel (optional).</param>
        double GetCurrent(UInt16 psuChannel = 1);

        /// <summary>
        /// The actual PSU output current
        /// </summary>
        /// <returns>Actual output current (unit = A).(</returns>
        /// <param name="psuChannel">The instrument channel (optional).</param>
        double MeasureCurrent(UInt16 psuChannel = 1);

        /// <summary>
        /// Set the PSU output current.
        /// </summary>
        /// <param name="currentAmps">Current (unit = A).</param>
        /// <param name="psuChannel">The instrument channel (optional).</param>
        void SetCurrent(double currentAmps, UInt16 psuChannel = 1);

        /// <summary>
        /// Set the over current protection of the PSU. If this is reached, PSU is disabled automatically.
        /// </summary>
        /// <param name="currentAmps">>Over current (unit = A).</param>
        /// <param name="psuChannel">The instrument channel (optional).</param>
        void SetOverCurrentProtection(double currentAmps, UInt16 psuChannel = 1);

        /// <summary>
        /// Get the over current protection of the PSU.
        /// </summary>
        /// <param name="psuChannel">The instrument channel (optional).</param>
        /// <returns>Over current protection level set (unit = A).</returns>
        double GetOverCurrentProtection(UInt16 psuChannel = 1);

        /// <summary>
        /// The set output voltage.
        /// </summary>
        /// <returns>Output voltage (unit = V).</returns>
        /// <param name="psuChannel">The instrument channel (optional).</param>
        double GetVoltage(UInt16 psuChannel = 1);

        /// <summary>
        /// The actual PSU output voltage
        /// </summary>
        /// <returns>Actual output voltage (unit = V).(</returns>
        /// <param name="psuChannel">The instrument channel (optional).</param>
        double MeasureVoltage(UInt16 psuChannel = 1);

        /// <summary>
        /// Set the PSU output voltage.
        /// </summary>
        /// <param name="voltage">Output voltage to be set (unit = V).</param>
        /// <param name="psuChannel">The instrument channel (optional).</param>
        void SetVoltage(double voltage, UInt16 psuChannel = 1);

        /// <summary>
        /// Set the over voltage protection of the PSU. If this is reached, PSU is disabled automatically.
        /// </summary>
        /// <param name="currentAmps">>Overvoltage (unit = V).</param>
        /// <param name="psuChannel">The instrument channel (optional).</param>
        void SetOverVoltageProtection(double voltage, UInt16 psuChannel = 1);

        /// <summary>
        /// Get  the over voltage protection of the PSU.
        /// </summary>
        /// <param name="psuChannel">The instrument channel (optional).</param>
        /// <returns>Over voltage level set (unit = V).</returns>
        double GetOverVoltageProtection(UInt16 psuChannel = 1);

        /// <summary>
        /// Set the output state of the PSU.
        /// </summary>
        /// <param name="outputState">True: on, False: off.</param>
        /// <param name="psuChannel">The instrument channel (optional).</param>
        void SetOutputState(bool outputState, UInt16 psuChannel = 1);

        /// <summary>
        /// Read the output state of the PSU.
        /// </summary>
        /// <param name="psuChannel">The instrument channel (optional).</param>
        /// <returns></returns>
        bool GetOutputState(UInt16 psuChannel = 1);

        // TODO: add functions for increasing/decreasing voltage or current in steps.

        #endregion
    }
}
