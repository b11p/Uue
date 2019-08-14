using System;

namespace UueLib
{
    public class UUEncoding
    {
        public static string ToUUEncodingString(byte[] inData, int offset, int length, bool insertLineBreaks, int lineBreakPosition = 80)
        {
            if (inData == null)
                throw new ArgumentNullException(nameof(inData));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Index out of range.");
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "Index out of range.");
            if (offset > inData.Length - length)
                throw new ArgumentOutOfRangeException(nameof(offset), "Index out of range.");
            if (insertLineBreaks && lineBreakPosition <= 0)
                throw new ArgumentOutOfRangeException(nameof(lineBreakPosition), "Break position must be greater than 0.");
            if (lineBreakPosition % 4 != 0)
                throw new ArgumentException("Position must be multi of 4.", nameof(lineBreakPosition));

            char[] outChars = new char[ToUUEncoding_CalculateAndValidateOutputLength(length, insertLineBreaks, lineBreakPosition)];
            int lengthmod3 = length % 3;
            int calcLength = offset + (length - lengthmod3);
            int j = 0;
            int charcount = 0;
            int i;
            for (i = offset; i < calcLength; i += 3)
            {
                if (insertLineBreaks)
                {
                    if (charcount == lineBreakPosition)
                    {
                        outChars[j++] = '\r';
                        outChars[j++] = '\n';
                        charcount = 0;
                    }
                    charcount += 4;
                }
                outChars[j] = (char)(((inData[i] & 0xfc) >> 2) + 33);
                outChars[j + 1] = (char)((((inData[i] & 0x03) << 4) | ((inData[i + 1] & 0xf0) >> 4)) + 33);
                outChars[j + 2] = (char)((((inData[i + 1] & 0x0f) << 2) | ((inData[i + 2] & 0xc0) >> 6)) + 33);
                outChars[j + 3] = (char)((inData[i + 2] & 0x3f) + 33);
                j += 4;
            }
            //Where we left off before
            i = calcLength;

            if (insertLineBreaks && lengthmod3 != 0 && charcount == lineBreakPosition)
            {
                outChars[j++] = '\r';
                outChars[j++] = '\n';
            }
            switch (lengthmod3)
            {
                case 2: //One character padding needed
                    outChars[j] = (char)(((inData[i] & 0xfc) >> 2) + 33);
                    outChars[j + 1] = (char)((((inData[i] & 0x03) << 4) | ((inData[i + 1] & 0xf0) >> 4)) + 33);
                    outChars[j + 2] = (char)(((inData[i + 1] & 0x0f) << 2) + 33);
                    outChars[j + 3] = (char)33; //Pad
                    j += 4;
                    break;
                case 1: // Two character padding needed
                    outChars[j] = (char)(((inData[i] & 0xfc) >> 2) + 33);
                    outChars[j + 1] = (char)(((inData[i] & 0x03) << 4) + 33);
                    outChars[j + 2] = (char)33; //Pad
                    outChars[j + 3] = (char)33; //Pad
                    j += 4;
                    break;
            }
            return new string(outChars);
        }

        private static int ToUUEncoding_CalculateAndValidateOutputLength(int inputLength, bool insertLineBreaks, int lineBreakPosition)
        {
            long outlen = (long)inputLength / 3 * 4;          // the base length - we want integer division here. 
            outlen += (inputLength % 3 != 0) ? 4 : 0;         // at most 4 more chars for the remainder

            if (outlen == 0)
                return 0;

            if (insertLineBreaks)
            {
                long newLines = outlen / lineBreakPosition;
                if (outlen % lineBreakPosition == 0)
                {
                    --newLines;
                }
                outlen += newLines * 2;              // the number of line break chars we'll add, "\r\n"
            }

            // If we overflow an int then we cannot allocate enough
            // memory to output the value so throw
            if (outlen > int.MaxValue)
                throw new OutOfMemoryException();

            return (int)outlen;
        }
    }
}
