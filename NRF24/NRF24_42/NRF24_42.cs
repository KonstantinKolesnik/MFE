﻿using System;
using Microsoft.SPOT;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GTI = Gadgeteer.Interfaces;

namespace Gadgeteer.Modules.KKS
{
    /// <summary>
    /// A NRF24 module for Microsoft .NET Gadgeteer
    /// </summary>
    public class NRF24 : GTM.Module
    {
        // This example implements a driver in managed code for a simple Gadgeteer module.  This module uses a 
        // single GTI.InterruptInput to interact with a button that can be in either of two states: pressed or released.
        // The example code shows the recommended code pattern for exposing a property (IsPressed). 
        // The example also uses the recommended code pattern for exposing two events: Pressed and Released. 
        // The triple-slash "///" comments shown will be used in the build process to create an XML file named
        // GTM.KKSolutions.RF24. This file will provide IntelliSense and documentation for the
        // interface and make it easier for developers to use the RF24 module.        

        // -- CHANGE FOR MICRO FRAMEWORK 4.2 --
        // If you want to use Serial, SPI, or DaisyLink (which includes GTI.SoftwareI2C), you must do a few more steps
        // since these have been moved to separate assemblies for NETMF 4.2 (to reduce the minimum memory footprint of Gadgeteer)
        // 1) add a reference to the assembly (named Gadgeteer.[interfacename])
        // 2) in GadgeteerHardware.xml, uncomment the lines under <Assemblies> so that end user apps using this module also add a reference.

        #region Fields
        private GTI.SPI spi;
        private GTI.SPI.Configuration spiConfig;
        //private SPI.Configuration netMFSpiConfig;
        private GT.Socket socket;
        //private GTI.DigitalOutput pinReset;
        //private GTI.DigitalOutput pinBacklight;
        private GTI.DigitalOutput pinCSN;
        private uint spiSpeed = 12000; // kHz
        #endregion

        // Note: A constructor summary is auto-generated by the doc builder.
        /// <summary></summary>
        /// <param name="socketNumber">The socket that this module is plugged in to.</param>
        public NRF24(int socketNumber)
        {
            // This finds the Socket instance from the user-specified socket number.  
            // This will generate user-friendly error messages if the socket is invalid.
            // If there is more than one socket on this module, then instead of "null" for the last parameter, 
            // put text that identifies the socket to the user (e.g. "S" if there is a socket type S)
            socket = Socket.GetSocket(socketNumber, true, this, null);

            //socket.ReservePin(Socket.Pin.Three, this); // reset
            //socket.ReservePin(Socket.Pin.Four, this); // back light
            socket.ReservePin(Socket.Pin.Five, this); // CSN
            socket.ReservePin(Socket.Pin.Six, this); // CS

            socket.ReservePin(Socket.Pin.Seven, this); // MOSI
            socket.ReservePin(Socket.Pin.Eight, this); // MISO
            socket.ReservePin(Socket.Pin.Nine, this); // SCK

            socket.EnsureTypeIsSupported('S', this);
            /*
             * Serial peripheral interface (SPI).
             * Pin 7 is MOSI line, pin 8 is MISO line, pin 9 is SCK line.
             * In addition, pins 3, 4 and 5 are general-purpose input/outputs, with pin 3 supporting interrupt capabilities.
            */

            //pinReset = new GTI.DigitalOutput(socket, Socket.Pin.Three, false, this); // pin 3
            //pinBacklight = new GTI.DigitalOutput(socket, Socket.Pin.Four, false, this); // pin 4
            pinCSN = new GTI.DigitalOutput(socket, Socket.Pin.Five, false, this); // pin 5

            spiConfig = new GTI.SPI.Configuration(false, 0, 0, false, true, spiSpeed);
            //netMFSpiConfig = new SPI.Configuration(socket.CpuPins[6], spiConfig.ChipSelectActiveState, spiConfig.ChipSelectSetupTime, spiConfig.ChipSelectHoldTime, spiConfig.ClockIdleState, spiConfig.ClockEdge, spiConfig.ClockRateKHz, socket.SPIModule);
            spi = new GTI.SPI(socket, spiConfig, GTI.SPI.Sharing.Shared, socket, Socket.Pin.Six, this);
        }

        #region Private methods
        private void WriteCommand(byte command)
        {
            pinCSN.Write(false);
            spi.Write(new byte[1] { command });
        }
        private void WriteData(byte data)
        {
            WriteData(new byte[1] { data });
        }
        private void WriteData(byte[] data)
        {
            pinCSN.Write(true);
            spi.Write(data);
        }
        private void WriteData(ushort[] data)
        {
            pinCSN.Write(true);
            spi.Write(data);
        }
        #endregion
    }
}
