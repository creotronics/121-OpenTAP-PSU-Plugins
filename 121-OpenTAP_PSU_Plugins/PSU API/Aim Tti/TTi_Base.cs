using OpenTap;
using System.ComponentModel;
using System;
using System.Collections.Generic;

namespace Creotronics.OpenTAP.Instruments.PSU.API
{
    /// <summary>
    /// This is the base class that can be used to derive different TTi based power supply classes from.
    /// </summary>
    /// <remarks>Initial design is based on PL-P series.</remarks>
    public abstract class TTi_Base : ScpiInstrument,IPSU
    {
        #region Fields
        // Some constants that are valid for all TTi based power supplies (based on website info of 11/08/2023).
        // If needed update these constanst if new TTi models are released.

        /// <summary>
        /// The maximum voltage that can be set for TTi based power supplies.
        /// </summary>
        private readonly double _maxTTiVoltage = 300.0;
        /// <summary>
        /// The minimum voltage that can be set for TTi based power supplies.
        /// </summary>
        private readonly double _minTTiVoltage = 0.0;
        /// <summary>
        /// The maximum current that can be set for TTi based power supplies.
        /// </summary>
        private readonly double _maxTTiCurrent = 100.0;
        /// <summary>
        /// The minimum current that can be set for TTi based power supplies.
        /// </summary>
        private readonly double _minTTiCurrent = 0.0;
        /// <summary>
        /// The minimum channel count for TTi based power supplies.
        /// </summary>
        private readonly UInt16 _minTTiChannelCount = 1;
        /// <summary>
        /// The maximum channel count for TTi based power supplies.
        /// </summary>
        private readonly UInt16 _maxTTiChannelCount = 4;
        #endregion

        #region Properties
        private string _psuBrand = "THURLBY THANDAR";
        /// <summary>
        /// The brand of the power supply used. By default this is set to "THURLBY THANDAR".
        /// </summary>
        /// <remarks>
        /// The name of the brand should be equal to the manufacurers name returned by the Instrumentation Idendification SCPI command (*IDN?). 
        /// <br>Value is not case sensitive.</br>
        /// <para>See manual for more information.</para>
        /// </remarks>
        protected string PsuBrand
        {
            get => _psuBrand;
            set
            {
                if (_psuBrand != value)
                {
                    if (string.IsNullOrEmpty(_psuBrand)) throw new ArgumentOutOfRangeException(nameof(value), "Power supply brand can't be empty.");
                    _psuBrand = value;
                }
            }
        }

        private string _psuModel = "UNKNOWN";
        /// <summary>
        /// The model of the power supply used. Default is "UNKNOWN".
        /// </summary>
        /// <remarks>
        /// The model of the brand should be equal to the model returned by the Instrumentation Idendification SCPI command (*IDN?). 
        /// <br>Value is not case sensitive.</br>
        /// <para>See manual for more information.</para>
        /// </remarks>
        protected string PsuModel
        {
            get => _psuModel;
            set
            {
                if (_psuModel != value)
                {
                    if (string.IsNullOrEmpty(_psuModel)) throw new ArgumentOutOfRangeException(nameof(value), "Power supply model can't be empty.");
                    _psuModel = value;
                }
            }
        }

        private UInt16 _psuChannels = 1;
        /// <summary>
        /// The channel count of the power supply. Default is 1, max for TTi power supplies is 4.
        /// </summary>
        [Browsable(false)]
        public UInt16 Channels
        {
            get => _psuChannels;
            set
            {
                // Save the value if it is not equal to previous value and within the range of TTi based power supplies.
                if (_psuChannels != value)
                {
                    if (value < _minTTiChannelCount || value > _maxTTiChannelCount) throw new ArgumentOutOfRangeException(nameof(value),
                        "TTi power supplies have " + _minTTiChannelCount + " up to " + _maxTTiChannelCount + " channels. Channel " + value + " can't be set.");

                    _psuChannels = value;
                }
            }
        }

        private List<double> _maxVoltage = new List<double>();
        /// <summary>
        /// The maximum voltage of the TTi power supply model for each channel.
        /// </summary>
        /// <remarks>The values need to be set inline with the actual maximum voltage of the used power supply!
        /// <br>The list count must be equal to the channel count.</br></remarks>
        [Browsable(false)]
        public List<double> MaxVoltage
        {
            get => _maxVoltage;

            set
            {
                // Save the value if it is not equal to previous value and within expected limits for TTi power supplies.
                if (_maxVoltage != value)
                {
                    for (int i = 0; i < value.Count; i++)
                    {
                        if (value[i] < _minTTiVoltage || value[i] > _maxTTiVoltage) throw new ArgumentOutOfRangeException(nameof(value),
                        "Maximum voltage of TTi power supplies can't be set lower than " + _minTTiVoltage + "V or higher than " + _maxTTiVoltage + "V. Maximum voltage of " + value + "V can't be set for channel " + i + ".");
                    }

                    _maxVoltage = value;
                }
            }
        }

        private List<double> _minVoltage = new List<double>();
        /// <summary>
        /// The minimum voltage of the TTi power supply model for each channel.
        /// </summary>
        /// <remarks>The values need to be set inline with the actual minimum voltage of the used power supply!
        /// <br>The list count must be equal to the channel count.</br></remarks>
        [Browsable(false)]
        public List<double> MinVoltage
        {
            get => _minVoltage;

            set
            {
                // Save the value if it is not equal to previous value and within expected limits for TTi power supplies.
                if (_minVoltage != value)
                {
                    for (int i = 0; i < value.Count; i++)
                    {
                        if (value[i] < _minTTiVoltage || value[i] > _maxTTiVoltage) throw new ArgumentOutOfRangeException(nameof(value),
                        "Minimum voltage of TTi power supplies can't be set lower than " + _minTTiVoltage + "V or higher than " + _maxTTiVoltage + "V. Minimum voltage of " + value + "V can't be set for channel " + i + ".");
                    }

                    _minVoltage = value;
                }
            }
        }

        private List<double> _maxCurrent = new List<double>();
        /// <summary>
        /// The maximum current of the TTi power supply model for each channel.
        /// </summary>
        /// <remarks>The values need to be set inline with the actual maximum current of the used power supply!
        /// <br>The list count must be equal to the channel count.</br></remarks>
        [Browsable(false)]
        public List<double> MaxCurrent
        {
            get => _maxCurrent;

            set
            {
                // Save the value if it is not equal to previous value and within expected limits for TTi power supplies.
                if (_maxCurrent != value)
                {
                    for (int i = 0; i < value.Count; i++)
                    {
                        if (value[i] < _minTTiCurrent || value[i] > _maxTTiCurrent) throw new ArgumentOutOfRangeException(nameof(value),
                        "Maximum current of TTi power supplies can't be set lower than " + _minTTiCurrent + "A or higher than " + _maxTTiCurrent + "A. Maximum current of " + value + "A can't be set for channel " + i + ".");
                    }

                    _maxCurrent = value;
                }
            }
        }

        private List<double> _minCurrent = new List<double>();
        /// <summary>
        /// The maximum current of the TTi power supply model for each channel.
        /// </summary>
        /// <remarks>The values need to be set inline with the actual minimum current of the used power supply!
        /// <br>The list count must be equal to the channel count.</br></remarks>
        [Browsable(false)]
        public List<double> MinCurrent
        {
            get => _minCurrent;

            set
            {
                // Save the value if it is not equal to previous value and within expected limits for TTi power supplies.
                if (_minCurrent != value)
                {
                    for (int i = 0; i < value.Count; i++)
                    {
                        if (value[i] < _minTTiCurrent || value[i] > _maxTTiCurrent) throw new ArgumentOutOfRangeException(nameof(value),
                        "Maximum current of TTi power supplies can't be set lower than " + _minTTiCurrent + "A or higher than " + _maxTTiCurrent + "A. Minimum current of " + value + "A can't be set for channel " + i + ".");
                    }

                    _minCurrent = value;
                }
            }
        }

        #endregion


        protected TTi_Base()
        {
            // Name should be overwritten by the derived class defining the exact name of the TTi power supply.
            Name = "Aim TTi PSU";
        }

        public override void Open()
        {
            base.Open();

            // We'll ask the Identification of the instrument (Scpi *IDN?) and verify if the Brand and Model are as expected. 
            string psuID = IdnString;   // Get instrument identity (*IDN?).
            if (!psuID.ToLower().Contains(_psuBrand.ToLower()) && (!psuID.ToLower().Contains(_psuModel.ToLower())))
            {
                Log.Error("This instrument driver does not support the connected instrument.");
                Log.Error("Expected instrument brand " + _psuBrand + ", model " + _psuModel + ".");
                Log.Error("Connected instrument identification at address " + VisaAddress + " is: " + psuID + System.Environment.NewLine + ".");
                throw new ArgumentException("Wrong instrument type.");
            }
        }

        public override void Close()
        {
            // When the PSU is closed, we automatically switch off the power.
            SetOutputState(false);
            base.Close();
        }

        /// <summary>
        /// By default the output is switched of at the end of the test sequence (safety).
        /// If you want to keep it on at the end, set parameter to True.
        /// </summary>
        /// <param name="keepPowerOn">True: keep output power in last state, False: switch output power off.</param>
        public void Close(bool keepPowerOn)
        {
            if (!keepPowerOn)
            {
                // Set all channels off (Scpi command).
                ScpiCommand("OPALL0");
            }

            base.Close();

        }

        #region Public Methodes
        /// <inheritdoc />
        /// <exception cref="System.IO.IOException">Instrument not connected.</exception>
        public virtual string Identify()
        {
            // Send Scpi command *IDN?.
            return base.QueryIdn();
        }

        /// <inheritdoc />
        public virtual new void Reset()
        {
            // Reset power supply (Scpi command *RST).
            base.Reset();

            /* 
             * No reply is expected back.
             * We could verify if the default settings are set after the reset.
             * We'll not going to do that because it's not sure if the default is similar for all TTi power supply types.
             */
        }

        /// <inheritdoc />
        public virtual int SelfTest(out string testMessage)
        {
            /*
             * There is actually no SelfTest for TTi power supplies.
             * We'll simply set up some communication, read out some stuff and see if everything is as expected.
             */

            // Read Execution Error Register.
            string errorCode = ScpiQuery("EER?");

            // Convert the PSU response to an integer.
            int faultCode;
            if (!int.TryParse(errorCode, out faultCode)) throw new Exception("Not able to parse the Execution Error Register error code!");

            // Set the self test message based on the Execution Error Register value.
            if (faultCode == 0) testMessage = "No error detected.";
            else testMessage = "Please, verify power supply manual for error code clarification.";

            // Return the Execution Error Register value.
            return faultCode;
        }

        /// <inheritdoc />
        public virtual double GetCurrent(UInt16 psuChannel = 1)
        {
            // Check parameters.
            CheckChannelSetting(psuChannel);

            // Send Scpi command.
            string scpiResponse = ScpiQuery<string>("I" + psuChannel.ToString() + "?");

            // Response is something like 'I1 0.500'.
            // First characters should be removed because is not the voltage value.
            scpiResponse = scpiResponse.Remove(0, scpiResponse.IndexOf(" ") + 1);

            // Return the current.
            return Double.Parse(scpiResponse);
        }

        /// <inheritdoc />
        public virtual double MeasureCurrent(UInt16 psuChannel = 1)
        {
            // Check parameters.
            CheckChannelSetting(psuChannel);

            // Send Scpi command.
            string scpiResponse = ScpiQuery<string>("I" + psuChannel.ToString() + "O?");

            // Response is something like '0.512A' (yes, it is different as the I1? command).
            // The A at the end should be removed.
            scpiResponse = scpiResponse.Remove(scpiResponse.Length - 1, 1);

            // Send Scpi command.
            return Double.Parse(scpiResponse);
        }

        /// <inheritdoc />
        public virtual void SetCurrent(double currentAmps, UInt16 psuChannel = 1)
        {
            // Check parameters.
            CheckVoltageSetting(currentAmps);
            CheckChannelSetting(psuChannel);

            // Send Scpi command.
            ScpiCommand("I" + psuChannel.ToString() + " " + currentAmps + "");
        }

        /// <inheritdoc />
        public virtual void SetOverCurrentProtection(double currentAmps, UInt16 psuChannel = 1)
        {
            // Check parameters.
            CheckVoltageSetting(currentAmps);
            CheckChannelSetting(psuChannel);

            // Send Scpi command.
            ScpiCommand("OCP" + psuChannel.ToString() + " " + currentAmps + "");
        }

        /// <inheritdoc />
        public virtual double GetOverCurrentProtection(UInt16 psuChannel = 1)
        {
            // Check parameters.
            CheckChannelSetting(psuChannel);

            // Send Scpi command.
            string scpiResponse = ScpiQuery<string>("OCP" + psuChannel.ToString() + "?");

            // Response is something like 'VC1 0.500'.
            // First characters should be removed because is not the over current value.
            scpiResponse = scpiResponse.Remove(0, scpiResponse.IndexOf(" ") + 1);

            // Return the over current.
            return Double.Parse(scpiResponse);
        }

        /// <inheritdoc />
        public double GetVoltage(UInt16 psuChannel = 1)
        {
            // Check parameters.
            CheckChannelSetting(psuChannel);

            // Send Scpi command.
            string scpiResponse = ScpiQuery<string>("V" + psuChannel.ToString() + "?");

            // Response is something like 'V1 10.000'.
            // First characters should be removed because is not the voltage value.
            scpiResponse = scpiResponse.Remove(0, scpiResponse.IndexOf(" ") + 1);

            // Return the voltage.
            return Double.Parse(scpiResponse);
        }

        /// <inheritdoc />
        public double MeasureVoltage(UInt16 psuChannel = 1)
        {
            // Check parameters.
            CheckChannelSetting(psuChannel);

            // Send Scpi command.
            string scpiResponse = ScpiQuery<string>("V" + psuChannel.ToString() + "O?");

            // Response is something like '10.123V' (yes, it is different as the V1? command).
            // The V at the end should be removed.
            scpiResponse = scpiResponse.Remove(scpiResponse.Length - 1, 1);

            // Send Scpi command.
            return Double.Parse(scpiResponse);
        }

        /// <inheritdoc />
        public void SetVoltage(double voltage, UInt16 psuChannel = 1)
        {
            // Check parameters.
            CheckVoltageSetting(voltage);
            CheckChannelSetting(psuChannel);

            // Send Scpi command.
            ScpiCommand("V" + psuChannel.ToString() + " " + voltage + "");

        }

        /// <inheritdoc />
        public void SetOverVoltageProtection(double voltage, UInt16 psuChannel = 1)
        {
            // Check parameters.
            CheckVoltageSetting(voltage);
            CheckChannelSetting(psuChannel);

            // Send Scpi command.
            ScpiCommand("OVP" + psuChannel.ToString() + " " + voltage + "");
        }

        /// <inheritdoc />
        public double GetOverVoltageProtection(UInt16 psuChannel = 1)
        {
            // Check parameters.
            CheckChannelSetting(psuChannel);

            // Send Scpi command.
            string scpiResponse = ScpiQuery<string>("OVP" + psuChannel.ToString() + "?");

            // Response is something like 'VP1 10.000'.
            // First characters should be removed because is not the over voltage value.
            scpiResponse = scpiResponse.Remove(0, scpiResponse.IndexOf(" ") + 1);

            // Return the over voltage.
            return Double.Parse(scpiResponse);
        }

        /// <inheritdoc />
        public void SetOutputState(bool outputState, UInt16 psuChannel = 1)
        {
            // Check parameters.
            CheckChannelSetting(psuChannel);

            // Send Scpi command.
            ScpiCommand("OP" + psuChannel.ToString() + " " + (outputState ? "1" : "0"));
        }

        /// <inheritdoc />
        public bool GetOutputState(UInt16 psuChannel = 1)
        {
            // Check parameters.
            CheckChannelSetting(psuChannel);

            // Send Scpi command.
            string scpiResponse = ScpiQuery<string>("OP" + psuChannel.ToString() + "?");

            // If response is 1, the output is enabled.
            if (scpiResponse == "1")
                return true;
            else
                return false;
        }

        // TODO: maybe we need to add a function to read out errors from the PSU?
        /*
        * Each error message has a number; only this number is reported via the remote control interfaces. 
        * Error message numbers are not displayed but are placed in the Execution Error Register where they can be read via the remote interfaces,
        * see Status Reporting section.
        */

        // TODO: add methodes to increase/decrease voltage of PSU in steps.

        // TODO: do we need to add options for Low Range functionality?
        /*
         * The upper limit of the CURRENT control can be switched between the maximum and 500mA
         * to give finer current limit setting and measurement resolution (0.1mA up to 500mA).
         */

        // TODO: add functionality for isolated tracking?
        /* 
         * The two outputs remain electrically isolated but the Voltage controls of the Master output set an identical voltage on the Slave output.
         */

        // TODO: add method to switch all outputs on/off at the same time?

        #endregion

        #region Private Methodes
        /// <summary>
        /// Verify if the PSU channel is available. If not avaliable, an exception will be thrown.
        /// </summary>
        /// <param name="psuChannel">PSU channel to be used.</param>
        /// <exception cref="ArgumentException">Channel out of power supply range.</exception>
        private void CheckChannelSetting(UInt16 psuChannel)
        {
            if (psuChannel > _psuChannels)
                throw new ArgumentException("Trying to set channel to " + psuChannel + " which is out of limits for this power supply!", "psuChannel");
            if (psuChannel == 0)
                throw new ArgumentException("Channel can't be 0!", "psuChannel");
        }

        /// <summary>
        /// Verify if the PSU voltage is within limits. If not within limits, an exception will be thrown.
        /// </summary>
        /// <param name="voltage">The voltage to be set.</param>
        /// <param name="psuChannel">The channel for which the voltage needs to be set.</param>
        /// <exception cref="ArgumentException">Voltage level out of power supply range.</exception>
        private void CheckVoltageSetting(double voltage, UInt16 psuChannel = 1)
        {
            if (voltage > _maxVoltage[psuChannel - 1] || voltage < _minVoltage[psuChannel - 1])
                throw new ArgumentException("Trying to set voltage level of channel " + psuChannel + " to " + voltage + "V which is out of limits for this power supply!", "voltage");
        }

        /// <summary>
        /// Verify if the PSU current is within limits. If not within limits, an exception will be thrown.
        /// </summary>
        /// <param name="current">The current to be set.</param>
        /// /// <param name="psuChannel">The channel for which the current needs to be set.</param>
        /// <exception cref="ArgumentException">Current level out of power supply range.</exception>
        private void CheckCurrentSetting(double current, UInt16 psuChannel = 1)
        {
            if (current > _maxCurrent[psuChannel - 1] || current < _minCurrent[psuChannel - 1])
                throw new ArgumentException("Trying to set current level of channel " + psuChannel + " to " + current + "A which is out of limits for this power supply!", "current");
        }

        #endregion
    }
}
