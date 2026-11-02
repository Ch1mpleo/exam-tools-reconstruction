using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace HashCodeProvider;

[Serializable]
public class IdentityHashCodeProvider : IHashCodeProvider
{
	int IHashCodeProvider.GetHashCode(object obj)
	{
		// Original IL calls Object.GetHashCode nonvirtually.
		return RuntimeHelpers.GetHashCode(obj);
	}
}
