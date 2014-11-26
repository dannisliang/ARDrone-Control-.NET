using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LSL;
using Microsoft.DirectX.DirectInput;

namespace ARDrone.Input
{
    class LSLInput : GenericInput
    {
        private LSL.liblsl.StreamInfo streamInfo;
        private LSL.liblsl.StreamInlet streamInlet;

        private string expectedStreamName = "BCIstream";

        private string streamHost;


        private String deviceInstanceId = "none";
        public override string DeviceInstanceId
        {
            get
            {
                return this.deviceInstanceId;
            }
        }

        public override String DeviceName
        {
            get
            {
                return this.expectedStreamName;
            }
        }

        public static List<GenericInput> GetNewInputDevices(IntPtr windowHandle, List<GenericInput> currentDevices)
        {
            List<GenericInput> newDevices = new List<GenericInput>();
             
            if(!CheckIfDeviceExists(currentDevices))
                newDevices.Add(new LSLInput());

            return newDevices;
        }

        private static bool CheckIfDeviceExists(List<GenericInput> currentDevices)
        {
            for (int i = 0; i < currentDevices.Count; i++)
            {
                if (currentDevices[i].GetType().Equals(typeof(LSLInput)))
                    return true;
            }

            return false;
        }

        public override void InitDevice()
        {
            if (streamInlet == null) { 
            // wait until an EEG stream shows up
                liblsl.StreamInfo[] results = liblsl.resolve_stream("name", this.expectedStreamName);

                if (results.Length > 0) { 
                    // open an inlet and print some interesting info about the stream (meta-data, etc.)
                    streamInlet = new liblsl.StreamInlet(results[0]);
                    System.Console.Write(streamInlet.info().as_xml());
                    this.streamHost = streamInlet.info().hostname();
                }

            }
        }

        public override void Dispose()
        {
            if (streamInlet != null)
                streamInlet.close_stream();
        }

        public override string GetCurrentRawInput(out bool isAxis)
        {
            isAxis = false;
            return string.Empty;
        }

        public override Utils.InputState GetCurrentControlInput()
        {
            Utils.InputState currentState = new Utils.InputState();
            
            if (streamInlet != null)
            {
                float[] sample = new float[1];
   
                streamInlet.pull_sample(sample, 0.0);

                currentState.Pitch = sample.First();
            
                return currentState;
            }

            Console.Write("No information from BCI");

            return null;
        }

        public override void StartControlInput()
        {
            base.StartControlInput();
            System.Console.WriteLine("Start Control Input for LSL called");
        }

        public override void CancelEvents()
        {
            base.CancelEvents();
            System.Console.WriteLine("Cancel Events for LSL called");
        }

        public override void EndControlInput()
        {
            base.EndControlInput();
            System.Console.WriteLine("End Control Input for LSL called");
        }

        public override bool Cancellable
        {
            get
            {
                return true;
            }
        }

        public override bool IsDevicePresent
        {
            get { return streamInlet != null; }
        }
         
    }
}
