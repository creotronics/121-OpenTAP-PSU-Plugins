using OpenTap;
using System;
using System.Collections.Generic;
using Creotronics.OpenTAP.Instruments.PSU.API;

namespace Creotronics.OpenTAP.Instruments.PSU.TestSteps
{
    /// <summary>
    /// This test step can be used to get the power supply voltage.
    /// <br>Properties:</br>
    /// <br>    - Power supply instrument to be used</br>
    /// <br>    - Power supply channel</br>
    /// <br>    - Check the value?</br>
    /// <br>    - Expected voltage level</br>
    /// <br>    - Accepted deviation (in %)</br>
    /// </summary>
    [Display("Get Voltage of Channel {Channel}", Group: "PSU", Description: "Get the power supply output voltage.")]
    public class GetVoltage : TestStep
    {
        #region Settings
        /// <summary>
        /// The power supply to be used for this test step.
        /// </summary>
        [Display(Group: "PSU Settings", Name: "PSU", Order: 1.1)]
        public IPSU MyPSU { get; set; }

        private UInt16 _myPsuChannel;
        /// <summary>
        /// List of power supply channels that can be used.
        /// </summary>
        public List<UInt16> AvailableChannels
        {
            // Get the channel cound supported by the power supply and make a selection list. 
            get
            {
                List<UInt16> channels = new List<UInt16>();

                for (UInt16 i = 0; i < MyPSU.Channels; i++)
                {
                    channels.Add((ushort)(i + 1));
                }

                return channels;
            }
        }

        /// <summary>
        /// The power supply channel to be used for the test step.
        /// </summary>
        [Display(Name: "Channel", Group: "PSU Settings", Description: "Select the power supply channel to be used.", Order: 1.2)]
        [AvailableValues(nameof(AvailableChannels))]
        public UInt16 Channel
        {
            get => _myPsuChannel;
            set => _myPsuChannel = value;
        }

        private bool _limitCheck;
        /// <summary>
        /// Enable if the read out voltage must be verified.
        /// </summary>
        [Display(Group: "Level Checking", Name: "Level Check", Order: 2.1)]
        public bool LimitCheckEnabled
        {
            get => _limitCheck; 
            set => _limitCheck = value; 
        }

        private double _voltageLevel;
        /// <summary>
        /// The voltage level expected.
        /// </summary>
        [Display(Group: "Level Checking", Name: "Level", Order: 2.2,
            Description: "The expected voltage level to be read back.")]
        [EnabledIf("LimitCheckEnabled", true, HideIfDisabled = true)]
        [Unit("V", UseEngineeringPrefix: false)]
        public double VoltageLevel 
        { 
            get => _voltageLevel;
            set => _voltageLevel = value;
        }

        private UInt16 _voltageDeviation;
        /// <summary>
        /// The voltage level deviation accepted.
        /// </summary>
        [Display(Group: "Level Checking", Name: "Deviation", Order: 2.3,
            Description: "The accepted deviation (in %).")]
        [EnabledIf("LimitCheckEnabled", true, HideIfDisabled = true)]
        [Unit("%", UseEngineeringPrefix: false)]
        public UInt16 VoltageDeviation 
        { 
            get => _voltageDeviation; 
            set => _voltageDeviation = value; 
        }

        #endregion

        public GetVoltage()
        {
            // Default power supply channel.
            Channel = 1;

            // Check if deviation is between 0 and 100 %
            Rules.Add(() => VoltageDeviation >= 0 && VoltageDeviation <= 100, "The read out voltage level deviation should be between 0 and 100%.", "VoltageDeviation");
        }

        public override void PrePlanRun()
        {
            base.PrePlanRun();
            // ToDo: Optionally add any setup code this step needs to run before the testplan starts
        }

        /// <summary>
        /// The actual test step. The power supply voltage will be read via Scpi command.
        /// If value is within expected limits, test passes. If not, test fails.
        /// </summary>
        public override void Run()
        {
            // Get the voltage.
            double readVoltage = MyPSU.MeasureVoltage(_myPsuChannel);

            if (_limitCheck)
            {
                // Read out voltage level needs to be verified.

                // Calculate limits
                double minLevel = (_voltageLevel * (100 - _voltageDeviation) / 100);
                double maxLevel = (_voltageLevel * (100 + _voltageDeviation) / 100);

                if (readVoltage < maxLevel && readVoltage > minLevel)
                {
                    // Value is within limits
                    Log.Info("Power supply voltage of channel " + _myPsuChannel + " is " + readVoltage + "V. Voltage is within expected limits of " + _voltageLevel + "V +/- " + _voltageDeviation + "% (" + minLevel + "V - " + maxLevel + "V).");
                    UpgradeVerdict(Verdict.Pass);
                }
                else
                {
                    // Value is not within limits. Test fails.
                    Log.Error("Power supply voltage of channel " + _myPsuChannel + " is " + readVoltage + "V. This is not within expected limits of " + _voltageLevel + "V +/- " + _voltageDeviation + "% (" + minLevel + "V - " + maxLevel + "V).");
                    
                    // If the output is disabled it's logical that the test will fail (this command reads the voltage at the output of the PSU).
                    // Warn the user about this.
                    if (!MyPSU.GetOutputState(_myPsuChannel))
                    {
                        Log.Warning("The output of channel " + _myPsuChannel + " is not enabled! Verify if this is as expected.");
                    }

                    UpgradeVerdict(Verdict.Fail);
                }
            }
            else
            {
                // No need to verify the value. Test will pass.
                Log.Info("Power supply voltage of channel " + _myPsuChannel + " is " + readVoltage + "V.");
                UpgradeVerdict(Verdict.Pass);
            }

            //Results.Publish("PSU voltage of channel " + _myPsuChannel, new { Voltage = readVoltage });

            RunChildSteps(); //If step has child steps.
        }

        public override void PostPlanRun()
        {
            // ToDo: Optionally add any cleanup code this step needs to run after the entire testplan has finished
            base.PostPlanRun();
        }
    }
}
