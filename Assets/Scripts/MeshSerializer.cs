using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Collections;

public class MeshSerializer {

    private static readonly byte[] marker = Encoding.ASCII.GetBytes("MESHDATA");

	public byte[] Serialize(Mesh mesh)
	{
		int vertexCount = mesh.vertexCount;

		int positionCount = vertexCount; // x, y, z tripplets
		int colorCount = vertexCount; // r, g, b tripplets
		int indexCount = vertexCount;

		int headerDataByteCount = 16;
		int positionDataByteCount = positionCount * 3 * 4;
		int colorDataByteCount = colorCount * 3;
		int indexDataByteCount = indexCount * 3 * 2;

		int packetSize = headerDataByteCount + positionDataByteCount + colorDataByteCount + indexDataByteCount;

		byte[] packet = new byte[packetSize];
		Array.Copy(marker, 0, packet, 0, marker.Length);

		int endOfMarker = marker.Length;
		putInt16(packet, endOfMarker, positionCount);
		putInt16(packet, endOfMarker + 2, colorCount);
		putInt16(packet, endOfMarker + 4, indexCount / 3);

		int positionDataStart = 16;
		int colorDataStart = positionDataStart + positionDataByteCount;
		int indexDataStart = colorDataStart + colorDataByteCount;

		for (int i = 0; i < mesh.vertexCount; i++)
		{ //<>//

			Vector3 position = mesh.vertices[i];
			Color vertexColor = mesh.colors[i];

			// encode the position data
			putFloat(packet, positionDataStart + (i * 12), position.x);
			putFloat(packet, positionDataStart + (i * 12) + 4, position.y);
			putFloat(packet, positionDataStart + (i * 12) + 8, position.z);

			byte r = (byte)(vertexColor.r);
			byte g = (byte)(vertexColor.g);
			byte b = (byte)(vertexColor.b);

			packet[colorDataStart + (i * 3)] = r;
			packet[colorDataStart + (i * 3) + 1] = g;
			packet[colorDataStart + (i * 3) + 2] = b;

			putInt16(packet, indexDataStart + (i * 2), i);
		}
		File.WriteAllBytes("./test.mesh", packet);

		return packet;
	}

	void putInt16(byte[] destination, int offset, int value)
	{
		byte firstByte = (byte)(value & 0xff);
		byte secondByte = (byte)((value >> 8) & 0xff);

		destination[offset] = firstByte;
		destination[offset + 1] = secondByte;
	}

	void putFloat(byte[] destination, int offset, float value)
	{

		byte[] bytes = BitConverter.GetBytes(value);

		destination[offset] = bytes[0];
		destination[offset + 1] = bytes[1];
		destination[offset + 2] = bytes[2];
		destination[offset + 3] = bytes[3];
	}
}
