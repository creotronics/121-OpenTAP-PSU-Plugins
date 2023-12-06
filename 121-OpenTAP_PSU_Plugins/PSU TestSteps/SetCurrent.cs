using OpenTap;
using System;
using System.Collections.Generic;
using Creotronics.OpenTAP.Instruments.PSU.API;

namespace Creotronics.OpenTAP.Instruments.PSU.TestSteps
{
    /// <summary>
    /// This test step can be used to set the power supply current.
    /// <br>Properties:</br>
    /// <br>    - Power supply instrument to be used</br>
    /// <br>    - Power supply channel</br>
    /// <br>    - Current level</br>
    /// </summary>
    [Display("Set Current of Channel {Channel} to {Current}", Group:"PSU",Description:"Set the power supply output current.")]
    public class SetCurrent : TestStep
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

        private double _current;
        /// <summary>
        /// The current to be set.
        /// </summary>
        [Display(Group: "PSU Settings", Name: "Current", Order: 1.3)]
        [Unit("A", UseEngineeringPrefix: false)]
        public double Current 
        { 
            get => _current;
            set { if (_current != value) _current = value; }
        }

        #endregion

        public SetCurrent()
        {
            // Default power supply channel and current.
            Channel = 1;
            Current = 0.1;

            // Verify if the current is not set outside the operating range of the power supply.
            Rules.Add(() => Current <= MyPSU.MaxCurrent[_myPsuChannel - 1], () => "A current higher than " + MyPSU.MaxCurrent[_myPsuChannel - 1] + "A for channel " + _myPsuChannel + " is not supported by " + MyPSU.Name +
            ". Please set a current between " + MyPSU.MinCurrent[_myPsuChannel - 1] + "A and " + MyPSU.MaxCurrent[_myPsuChannel - 1] + "A for channel " + _myPsuChannel + ".", nameof(Current));
            Rules.Add(() => Current >= MyPSU.MinCurrent[_myPsuChannel - 1], () => "A current lower than " + MyPSU.MinCurrent[_myPsuChannel - 1] + "A for channel " + _myPsuChannel + " is not supported by " + MyPSU.Name +
            ". Please set a current between " + MyPSU.MinCurrent[_myPsuChannel - 1] + "A and " + MyPSU.MaxCurrent[_myPsuChannel - 1] + "A for channel " + _myPsuChannel + ".", nameof(Current));
        }

        public override void PrePlanRun()
        {
            base.PrePlanRun();
            // ToDo: Optionally add any setup code this step needs to run before the testplan starts
        }

        /// <summary>
        /// The actual test step. The power supply current will be set via Scpi command.
        /// The value will be read back to verify. If successful, test step passed. If not, test fails.
        /// </summary>
        public override void Run()
        {
            // Set the current.
            MyPSU.SetCurrent(_current, _myPsuChannel);

            // Read the set current back and verify if set correctly.
            if (MyPSU.GetCurrent(_myPsuChannel) == _current)
            {
                Log.Info("Power supply current of channel " + _myPsuChannel + " is set to " + _current + "A.");
                UpgradeVerdict(Verdict.Pass);
            }
            else
            {
                Log.Error("Failed to set power supply current of channel " + _myPsuChannel + " to " + _current + "A!");
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
