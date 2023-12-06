using OpenTap;
using System;
using System.Collections.Generic;
using Creotronics.OpenTAP.Instruments.PSU.API;

namespace Creotronics.OpenTAP.Instruments.PSU.TestSteps
{
    /// <summary>
    /// This test step can be used to get the power supply output status.
    /// <br>Properties:</br>
    /// <br>    - Power supply instrument to be used</br>
    /// <br>    - Power supply channel</br>
    /// <br>    - Check the value?</br>
    /// <br>    - Expected output (1 or 0)</br>
    /// </summary>
    [Display("Get Output of Channel {Channel}", Group: "PSU", Description: "Get the power supply output status.")]
    public class GetOutput : TestStep
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

        private bool _outputCheck;
        /// <summary>
        /// Enable if the output must be verified.
        /// </summary>
        [Display(Group: "Output Checking", Name: "Output Check", Order: 2.1)]
        public bool OutputCheckEnabled
        {
            get => _outputCheck; 
            set => _outputCheck = value; 
        }

        private double _outputValue;
        /// <summary>
        /// The expected output value.
        /// </summary>
        [Display(Group: "Output Checking", Name: "Output", Order: 2.2,
            Description: "The expected output value.")]
        [EnabledIf("OutputCheckEnabled", true, HideIfDisabled = true)]
        public double OutputValue 
        { 
            get => _outputValue;
            set => _outputValue = value;
        }

        #endregion

        public GetOutput()
        {
            // Default power supply channel.
            Channel = 1;

            // Check if output check is 0 or 1
            Rules.Add(() => OutputValue == 0 || OutputValue == 1, "The expected output value should be 0 (output disabled) or 1 (output enabled)", "OutputValue");
        }

        public override void PrePlanRun()
        {
            base.PrePlanRun();
            // ToDo: Optionally add any setup code this step needs to run before the testplan starts
        }

        /// <summary>
        /// The actual test step. The power supply output state will be read via Scpi command.
        /// If value is as expected, test will pass. If not, test will fail.
        /// If no check of output is required, test will pass.
        /// </summary>
        public override void Run()
        {
            // Get the output state.
            bool readOutput = MyPSU.GetOutputState(_myPsuChannel);

            if (_outputCheck)
            {
                // Verify output state.
                if (_outputValue == 1 || _outputValue == 0)
                {
                    if (readOutput == Convert.ToBoolean(_outputValue))
                    {
                        // Value is as expected
                        Log.Info("Power supply output state of channel " + _myPsuChannel + " is " + readOutput + ". This is as expected.");
                        UpgradeVerdict(Verdict.Pass);
                    }
                    else
                    {
                        // Value is not as expected. Test fails.
                        Log.Error("Power supply output state of channel " + _myPsuChannel + " is " + readOutput + ". This is not as expected!");

                        UpgradeVerdict(Verdict.Fail);
                    }
                }
                else
                {
                    // Incorrect check given
                    Log.Error("Ppower supply output state can only be 1 or 0. Please correct the output check value.");

                    UpgradeVerdict(Verdict.Fail);
                }
            }
            else
            {
                // No need to verify the value. Test will pass.
                Log.Info("Power supply output state of channel " + _myPsuChannel + " is " + readOutput + ".");
                UpgradeVerdict(Verdict.Pass);
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
