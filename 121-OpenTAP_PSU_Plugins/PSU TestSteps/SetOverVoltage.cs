using OpenTap;
using System;
using System.Collections.Generic;
using Creotronics.OpenTAP.Instruments.PSU.API;

namespace Creotronics.OpenTAP.Instruments.PSU.TestSteps
{
    /// <summary>
    /// This test step can be used to set the power supply over voltage protection.
    /// <br>Properties:</br>
    /// <br>    - Power supply instrument to be used</br>
    /// <br>    - Power supply channel</br>
    /// <br>    - Over voltage protection level</br>
    /// </summary>
    [Display("Set Over Voltage Protection of Channel {Channel} to {Over Voltage}", Group:"PSU",Description:"Set the power supply output over voltage protection.")]
    public class SetOverVoltage : TestStep
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
        [Display(Name:"Channel", Group:"PSU Settings", Description:"Select the power supply channel to be used.", Order:1.2)]
        [AvailableValues(nameof(AvailableChannels))]
        public UInt16 Channel 
        { 
            get => _myPsuChannel;
            set => _myPsuChannel = value; 
        }

        private double _overVoltage;
        /// <summary>
        /// The over voltage to be set.
        /// </summary>
        [Display(Group: "PSU Settings", Name: "Over Voltage", Order: 1.3)]
        [Unit("V", UseEngineeringPrefix: false)]
        public double OverVoltage 
        { 
            get => _overVoltage;
            set { if (_overVoltage != value) _overVoltage = value; }
        }

        #endregion

        public SetOverVoltage()
        {
            // Default power supply channel.
            Channel = 1;

            // Verify if the over voltage is not set outside the operating range of the used power supply.
            Rules.Add(() => OverVoltage <= MyPSU.MaxVoltage[_myPsuChannel - 1], () => "An over voltage higher than " + MyPSU.MaxVoltage[_myPsuChannel - 1] + "V for channel " + _myPsuChannel + " is not supported by " + MyPSU.Name + 
            ". Please set an over voltage between " + MyPSU.MinVoltage[_myPsuChannel - 1] + "V and " + MyPSU.MaxVoltage[_myPsuChannel - 1] + "V.", "OverVoltage");
            Rules.Add(() => OverVoltage >= MyPSU.MinVoltage[_myPsuChannel - 1], () => "An over voltage lower than " + MyPSU.MinVoltage[_myPsuChannel - 1] + "V for channel " + _myPsuChannel + " is not supported by " + MyPSU.Name + 
            ". Please set an over voltage between " + MyPSU.MinVoltage[_myPsuChannel - 1] + "V and " + MyPSU.MaxVoltage[_myPsuChannel - 1] + "V.", "OverVoltage");
        }

        public override void PrePlanRun()
        {
            base.PrePlanRun();
            // ToDo: Optionally add any setup code this step needs to run before the testplan starts
        }

        /// <summary>
        /// The actual test step. The power supply over voltage protection will be set via Scpi command.
        /// The value will be read back to verify. If successful, test step passed. If not, test fails.
        /// </summary>
        public override void Run()
        {
            // Set the over voltage protection.
            MyPSU.SetOverVoltageProtection(_overVoltage, _myPsuChannel);

            // Read the set over voltage back and verify if set correctly.
            if (MyPSU.GetOverVoltageProtection(_myPsuChannel) == _overVoltage)
            {
                Log.Info("Power supply over voltage protection of channel " + _myPsuChannel + " is set to " + _overVoltage + "V.");
                UpgradeVerdict(Verdict.Pass);
            }
            else
            {
                Log.Error("Failed to set power supply over voltage protection of channel " + _myPsuChannel + " to " + _overVoltage + "V!");
                UpgradeVerdict(Verdict.Fail);
            }

            RunChildSteps(); //If step has child steps.
        }

        public override void PostPlanRun()
        {
            // ToDo: Optionally add any cleanup code this step needs to run after the entire testplan has finished
            base.PostPlanRun();
        }
    }
}
