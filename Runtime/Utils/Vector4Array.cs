using System;
using UnityEngine;

// Used to work around some nodegraph limitations
public struct Vector4Array
{
	public Vector4[] value;

	public Vector4Array(Vector4[] value)
	{
		this.value = value ?? throw new ArgumentNullException(nameof(value));
	}
}