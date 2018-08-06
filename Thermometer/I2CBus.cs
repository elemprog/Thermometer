﻿#define FT232H
//#define FT2232H
//#define FT4232H


namespace FTD2XX
{
    class I2CBus : D2XX
    {
        // I2C Library defines
        const byte I2C_Dir_SDAin_SCLin             = 0x00;
        const byte I2C_Dir_SDAin_SCLout            = 0x01;
        const byte I2C_Dir_SDAout_SCLout           = 0x03;
        const byte I2C_Dir_SDAout_SCLin            = 0x02;
        const byte I2C_Data_SDAhi_SCLhi            = 0x03;
        const byte I2C_Data_SDAlo_SCLhi            = 0x01;
        const byte I2C_Data_SDAlo_SCLlo            = 0x00;
        const byte I2C_Data_SDAhi_SCLlo            = 0x02;
        // MPSSE clocking commands
        const byte MSB_FALLING_EDGE_CLOCK_BYTE_IN  = 0x24;
        const byte MSB_RISING_EDGE_CLOCK_BYTE_IN   = 0x20;
        const byte MSB_FALLING_EDGE_CLOCK_BYTE_OUT = 0x11;
        const byte MSB_DOWN_EDGE_CLOCK_BIT_IN      = 0x26;
        const byte MSB_UP_EDGE_CLOCK_BYTE_IN       = 0x20;
        const byte MSB_UP_EDGE_CLOCK_BYTE_OUT      = 0x10;
        const byte MSB_RISING_EDGE_CLOCK_BIT_IN    = 0x22;
        const byte MSB_FALLING_EDGE_CLOCK_BIT_OUT  = 0x13;
        const uint ClockDivisor                    = 199; //  199 - clock 100KHz
        const uint NumBytesToRead                  = 2;

        protected static bool I2C_Ack              = false;
        private static byte I2C_Status             = 0;

        // GPIO
        private static byte GPIO_Low_Dat           = 0;
        private static byte GPIO_Low_Dir           = 1;



        public byte I2C_ConfigureMpsse()
        {
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            NumBytesToSend = 0;

            /***** Initial device configuration *****/

            ftStatus = FT_STATUS.FT_OK;
            ftStatus |= SetTimeouts(5000, 5000);
            ftStatus |= SetLatency(16);
            ftStatus |= SetFlowControl(FT_FLOW_CONTROL.FT_FLOW_RTS_CTS, 0x00, 0x00);
            ftStatus |= SetBitMode(0x00, 0x00);
            ftStatus |= SetBitMode(0x00, 0x02);         // MPSSE mode        

            if (ftStatus != FT_STATUS.FT_OK)
                return 1; // error();

            /***** Flush the buffer *****/
            I2C_Status = FlushBuffer();

            /***** Synchronize the MPSSE interface by sending bad command 0xAA *****/
            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0xAA;
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0) return 1; // error();
            I2C_Status = Receive_Data(NumBytesToRead);
            if (I2C_Status != 0) return 1; //error();

            if ((InputBuffer2[0] == 0xFA) && (InputBuffer2[1] == 0xAA))
            {
                //Console.WriteLine("Bad Command Echo successful");
            }
            else
            {
                return 1;            //error
            }

            /***** Synchronize the MPSSE interface by sending bad command 0xAB *****/
            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0xAB;
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0) return 1; // error();
            I2C_Status = Receive_Data(NumBytesToRead);
            if (I2C_Status != 0) return 1; //error();

            if ((InputBuffer2[0] == 0xFA) && (InputBuffer2[1] == 0xAB))
            {
                //Console.WriteLine("Bad Command Echo successful");
            }
            else
            {
                return 1;            //error
            }

            NumBytesToSend = 0;
            MPSSEbuffer[NumBytesToSend++] = 0x8A; 	// Disable clock divide by 5 for 60Mhz master clock
            MPSSEbuffer[NumBytesToSend++] = 0x97;	// Turn off adaptive clocking
            MPSSEbuffer[NumBytesToSend++] = 0x8C; 	// Enable 3 phase data clock, used by I2C to allow data on both clock edges
            // The SK clock frequency can be worked out by below algorithm with divide by 5 set as off
            // SK frequency  = 60MHz /((1 +  [(1 +0xValueH*256) OR 0xValueL])*2)
            MPSSEbuffer[NumBytesToSend++] = 0x86; 	//Command to set clock divisor
            MPSSEbuffer[NumBytesToSend++] = (byte)(ClockDivisor & 0x00FF);	//Set 0xValueL of clock divisor
            MPSSEbuffer[NumBytesToSend++] = (byte)((ClockDivisor >> 8) & 0x00FF);	//Set 0xValueH of clock divisor
            MPSSEbuffer[NumBytesToSend++] = 0x85; 			// loopback off

#if (FT232H)
            MPSSEbuffer[NumBytesToSend++] = 0x9E;       //Enable the FT232H's drive-zero mode with the following enable mask...
            MPSSEbuffer[NumBytesToSend++] = 0x07;       // ... Low byte (ADx) enables - bits 0, 1 and 2 and ... 
            MPSSEbuffer[NumBytesToSend++] = 0x00;       //...High byte (ACx) enables - all off

            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLhi | (GPIO_Low_Dat & 0xF8)); // SDA and SCL both output high (open drain)
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));
#else
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));  	// SDA and SCL set low but as input to mimic open drain
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLin | (GPIO_Low_Dir & 0xF8));	//

#endif

            MPSSEbuffer[NumBytesToSend++] = 0x80; 	//Command to set directions of lower 8 pins and force value on bits set as output 
            MPSSEbuffer[NumBytesToSend++] = (byte)(ADbusVal);
            MPSSEbuffer[NumBytesToSend++] = (byte)(ADbusDir);


            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
            {
                return 1;            //error
            }
            else
            {
                return 0;
            }
        }


        // Reads a byte over I2C 
        public byte I2C_ReadByte(bool ACK)
        {
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            NumBytesToSend = 0;

#if (FT232H)
            // Clock in one data byte
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BYTE_IN;      // Clock data byte in
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x00;                               // Data length of 0x0000 means 1 byte data to clock in

            // clock out one bit as ack/nak bit
            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BIT_OUT;     // Clock data bit out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                               // Length of 0 means 1 bit
            if (ACK == true)
                MPSSEbuffer[NumBytesToSend++] = 0x00;                           // Data bit to send is a '0'
            else
                MPSSEbuffer[NumBytesToSend++] = 0xFF;                           // Data bit to send is a '1'

            // I2C lines back to idle state 
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));
            MPSSEbuffer[NumBytesToSend++] = 0x80;                               //       ' Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                            //      ' Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                             //     ' Set the directions
#else
            // Ensure line is definitely an input since FT2232H and FT4232H don't have open drain
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8)); // make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions
            // Clock one byte of data in from the sensor
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BYTE_IN;      // Clock data byte in
            MPSSEbuffer[NumBytesToSend++] = 0x00;
            MPSSEbuffer[NumBytesToSend++] = 0x00;                               // Data length of 0x0000 means 1 byte data to clock in

            // Change direction back to output and clock out one bit. If ACK is true, we send bit as 0 as an acknowledge
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));    // back to output
            MPSSEbuffer[NumBytesToSend++] = 0x80;                               // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                           // set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                           // set the directions

            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BIT_OUT;    // Clock data bit out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                              // Length of 0 means 1 bit
            if (ACK == true)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x00;                          // Data bit to send is a '0'
            }
            else
            {
                MPSSEbuffer[NumBytesToSend++] = 0xFF;                          // Data bit to send is a '1'
            }

            // Put line states back to idle with SDA open drain high (set to input) 
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8));//make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                               //       ' Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                            //      ' Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                             //     ' Set the directions

#endif
            // This command then tells the MPSSE to send any results gathered back immediately
            MPSSEbuffer[NumBytesToSend++] = 0x87;                                  //    ' Send answer back immediate command

            // send commands to chip
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
            {
                return 1;
            }

            // get the byte which has been read from the driver's receive buffer
            I2C_Status = Receive_Data(1);
            if (I2C_Status != 0)
            {
                return 1;
            }

            // InputBuffer2[0] now contains the results

            return 0;
        }



        public byte I2C_SendDeviceAddrAndCheckACK(byte Address, bool Read)
        {
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            NumBytesToSend = 0;

            Address <<= 1;
            if (Read == true)
                Address |= 0x01;

#if (FT232H)
            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // 
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   //  Data length of 0x0000 means 1 byte data to clock in
            MPSSEbuffer[NumBytesToSend++] = Address;           //  Byte to send

            // Put line back to idle (data released, clock pulled low)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data bits in
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit
#else

            // Set directions of clock and data to output in preparation for clocking out a byte
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// back to output
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions
            // clock out one byte
            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // 
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Data length of 0x0000 means 1 byte data to clock in
            MPSSEbuffer[NumBytesToSend++] = Address;                         // Byte to send

            // Put line back to idle (data released, clock pulled low) so that sensor can drive data line
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8)); // make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data byte in
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit

#endif
            // This command then tells the MPSSE to send any results gathered (in this case the ack bit) back immediately
            MPSSEbuffer[NumBytesToSend++] = 0x87;                                //  ' Send answer back immediate command

            // send commands to chip
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
            {
                return 1;
            }

            // read back byte containing ack
            I2C_Status = Receive_Data(1);
            if (I2C_Status != 0)
            {
                return 1;            // can also check NumBytesRead
            }

            // if ack bit is 0 then sensor acked the transfer, otherwise it nak'd the transfer
            if ((InputBuffer2[0] & 0x01) == 0)
            {
                I2C_Ack = true;
            }
            else
            {
                I2C_Ack = false;
            }

            return 0;
        }


        // Writes one byte to the I2C bus
        public byte I2C_SendByteAndCheckACK(byte DataByteToSend)
        {
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            NumBytesToSend = 0;

#if (FT232H)
            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // 
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   //  Data length of 0x0000 means 1 byte data to clock in
            MPSSEbuffer[NumBytesToSend++] = DataByteToSend;// DataSend[0];          //  Byte to send

            // Put line back to idle (data released, clock pulled low)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data bits in
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit
#else

            // Set directions of clock and data to output in preparation for clocking out a byte
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));// back to output
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions
            // clock out one byte
            MPSSEbuffer[NumBytesToSend++] = MSB_FALLING_EDGE_CLOCK_BYTE_OUT;        // clock data byte out
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // 
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Data length of 0x0000 means 1 byte data to clock in
            MPSSEbuffer[NumBytesToSend++] = DataByteToSend;                         // Byte to send

            // Put line back to idle (data released, clock pulled low) so that sensor can drive data line
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8)); // make data input
            MPSSEbuffer[NumBytesToSend++] = 0x80;                                   // Command - set low byte
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;                               // Set the values
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;                               // Set the directions

            // CLOCK IN ACK
            MPSSEbuffer[NumBytesToSend++] = MSB_RISING_EDGE_CLOCK_BIT_IN;           // clock data byte in
            MPSSEbuffer[NumBytesToSend++] = 0x00;                                   // Length of 0 means 1 bit

#endif
            // This command then tells the MPSSE to send any results gathered (in this case the ack bit) back immediately
            MPSSEbuffer[NumBytesToSend++] = 0x87;                                //  ' Send answer back immediate command

            // send commands to chip
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
            {
                return 1;
            }

            // read back byte containing ack
            I2C_Status = Receive_Data(1);
            if (I2C_Status != 0)
            {
                return 1;            // can also check NumBytesRead
            }

            // if ack bit is 0 then sensor acked the transfer, otherwise it nak'd the transfer
            if ((InputBuffer2[0] & 0x01) == 0)
            {
                I2C_Ack = true;
            }
            else
            {
                I2C_Ack = false;
            }

            return 0;

        }


        // Sets I2C Start condition
        public byte I2C_SetStart()
        {
            byte Count = 0;
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            NumBytesToSend = 0;

#if (FT232H)
            // SDA high, SCL high
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLhi | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));    // on FT232H lines always output

            for (Count = 0; Count < 6; Count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA lo, SCL high
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLhi | (GPIO_Low_Dat & 0xF8));

            for (Count = 0; Count < 6; Count++)	// Repeat commands to ensure the minimum period of the start setup time ie 600ns is achieved
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA lo, SCL lo
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));

            for (Count = 0; Count < 6; Count++)	// Repeat commands to ensure the minimum period of the start setup time ie 600ns is achieved
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // Release SDA
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLlo | (GPIO_Low_Dat & 0xF8));

            MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction

# else

            // Both SDA and SCL high (setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLin | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA low, SCL high (setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLin | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)	// Repeat commands to ensure the minimum period of the start setup time
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA low, SCL low
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));//
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));//as above

            for (Count = 0; Count < 6; Count++)	// Repeat commands to ensure the minimum period of the start setup time
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // Release SDA (setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));//
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLout | (GPIO_Low_Dir & 0xF8));//as above

            MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
            MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
            MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction


# endif
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
                return 1;
            else
                return 0;
        }


        // Sets I2C Stop condition
        public byte I2C_SetStop()
        {
            byte Count = 0;
            byte ADbusVal = 0;
            byte ADbusDir = 0;
            NumBytesToSend = 0;

#if (FT232H)
            // SDA low, SCL low
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));    // on FT232H lines always output

            for (Count = 0; Count < 6; Count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA low, SCL high
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLhi | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));    // on FT232H lines always output

            for (Count = 0; Count < 6; Count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA high, SCL high
            ADbusVal = (byte)(0x00 | I2C_Data_SDAhi_SCLhi | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));        // on FT232H lines always output

            for (Count = 0; Count < 6; Count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

#else

            // SDA low, SCL low
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLout | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }


            // SDA low, SCL high (note: setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAout_SCLin | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }

            // SDA high, SCL high (note: setting to input simulates open drain high)
            ADbusVal = (byte)(0x00 | I2C_Data_SDAlo_SCLlo | (GPIO_Low_Dat & 0xF8));
            ADbusDir = (byte)(0x00 | I2C_Dir_SDAin_SCLin | (GPIO_Low_Dir & 0xF8));

            for (Count = 0; Count < 6; Count++)	// Repeat commands to hold states for longer time
            {
                MPSSEbuffer[NumBytesToSend++] = 0x80;	    // ADbus GPIO command
                MPSSEbuffer[NumBytesToSend++] = ADbusVal;   // Set data value
                MPSSEbuffer[NumBytesToSend++] = ADbusDir;	// Set direction
            }
#endif
            // send the buffer of commands to the chip 
            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
                return 1;
            else
                return 0;
        }


        // Sets GPIO values on high byte
        public byte I2C_SetGPIOValuesHigh(byte ACbusDir, byte ACbusVal)
        {
            NumBytesToSend = 0;

#if (FT4232H)

           return 1;
           
#else
            MPSSEbuffer[NumBytesToSend++] = 0x82;       // ACbus GPIO command
            MPSSEbuffer[NumBytesToSend++] = ACbusVal;   // Set data value
            MPSSEbuffer[NumBytesToSend++] = ACbusDir;   // Set direction

            I2C_Status = Send_Data(NumBytesToSend);
            if (I2C_Status != 0)
                return 1;
            else
                return 0;
#endif
        }
    }
}
