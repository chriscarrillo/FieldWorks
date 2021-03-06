// Copyright (c) 2003-2013 SIL International
// This software is licensed under the LGPL, version 2.1 or later
// (http://www.gnu.org/licenses/lgpl-2.1.html)

// This class is a port of a C++ class.

using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sfm2Xml
{
	public class CRC
	{
		uint m_CRC32_POLYNOMIAL = 0xEDB88320;
		uint m_CRC32_STARTVALUE = 0xFFFFFFFF;
		static uint[] m_CRCTable;

		public CRC()
		{
			m_CRCTable = new uint[256];
			BuildCRCTable();
		}

		void BuildCRCTable()
		{
			for (uint i = 0; i <= 255; i++)
			{
				uint crc = i;
				for (int j = 8; j > 0; j--)
				{
					if ((crc & 1) == 1)
						crc = (crc >> 1) ^ m_CRC32_POLYNOMIAL;
					else
						crc >>= 1;
				}
				m_CRCTable[i] = crc;
			}
		}

		public uint CalculateCRC(byte[] buffer, int count)
		{
			uint crc = m_CRC32_STARTVALUE;
			byte b;	// unsigned char *p;

			//p = (unsigned char*) buffer;
			uint index = 0;
			while (index < count)
			{
				b = buffer[index++];
				uint temp1 = (crc >> 8) & 0x00FFFFFF;
				uint temp2 = m_CRCTable[((int)crc ^ b) & 0xff];
				crc = temp1 ^ temp2;
			}
			return (crc ^= m_CRC32_STARTVALUE);
		}

		public uint CalculateCRC_N(byte[] buffer, int count, uint lastValue, bool bFirst, bool bLast)
		{
			uint crc = m_CRC32_STARTVALUE;
			if (!bFirst)
				crc = lastValue;
			byte b;
			uint index = 0;
			while (index < count)
			{
				b = buffer[index++];
				uint temp1 = (crc >> 8) & 0x00FFFFFF;
				uint temp2 = m_CRCTable[((int)crc ^ b) & 0xff];
				crc = temp1 ^ temp2;
			}
			if (bLast)
				return (crc ^= m_CRC32_STARTVALUE);
			return crc;
		}

		public uint FileCRC(string fileName)
		{
			uint crc = 0;

			if (!System.IO.File.Exists(fileName))
				return 0;

			FileStream fs = null;
			try
			{
				fs = System.IO.File.Open(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read,
															System.IO.FileShare.Read);
			}
			catch (IOException ex)
			{
				//check if message is for a File IO
				// REVIEW: is ex.Message guaranteed to contain English text even when running on a localized Windows?
				if (ex.Message.ToString().Contains("being used by another process"))
				{
					MessageBox.Show("File " + fileName + "is in use.");
					return 0;
				}
			}
			try
			{
				using (var binReader = new System.IO.BinaryReader(fs))
				{
					int buffSize = 0x8000;
					// can process the file in just one pass of the crc generator
					if (fs.Length <= buffSize)
					{
						byte[] testArray = binReader.ReadBytes((int)fs.Length);
						crc = CalculateCRC(testArray, testArray.Length);
						return crc;
					}
					long bytesRead = 0;
					bool firstTime = true;
					while (bytesRead < fs.Length)
					{
						byte[] testArray = binReader.ReadBytes(buffSize);
						bytesRead += testArray.Length;
						crc = CalculateCRC_N(testArray, testArray.Length, crc, firstTime, bytesRead == fs.Length);
						firstTime = false;
					}
				}
			}
			catch
			{
			}
			finally
			{
				fs.Dispose();
			}
			return crc;
		}

	}
}
