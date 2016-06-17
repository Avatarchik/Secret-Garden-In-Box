using UnityEngine;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace OpenCVForUnity
{

		/// <summary>
		/// Represents a class which manages its own memory. 
		/// </summary>
		abstract public class DisposableObject : IDisposable
		{
       
				/// <summary>
				/// Default constructor
				/// </summary>
				protected DisposableObject ()
            : this(true)
				{
				}

				/// <summary>
				/// Constructor
				/// </summary>
				/// <param name="isEnabledDispose">true if you permit disposing this class by GC</param>
				protected DisposableObject (bool isEnabledDispose)
				{
						IsDisposed = false;
						IsEnabledDispose = isEnabledDispose;
				}


				/// <summary>
				/// Releases the resources
				/// </summary>
				public void Dispose ()
				{
						Dispose (true);
						GC.SuppressFinalize (this);
				}


				/// <summary>
				/// Releases the resources
				/// </summary>
				/// <param name="disposing">
				/// If disposing equals true, the method has been called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed.
				/// If false, the method has been called by the runtime from inside the finalizer and you should not reference other objects. Only unmanaged resources can be disposed.
				/// </param>
				protected virtual void Dispose (bool disposing)
				{
						if (!IsDisposed) {             
								// releases managed resources
								if (disposing) {
								}

								IsDisposed = true;
						}
				}


				/// <summary>
				/// Destructor
				/// </summary>
				~DisposableObject ()
				{
						Dispose (false);
				}

        

				/// <summary>
				/// Gets a value indicating whether this instance has been disposed.
				/// </summary>
				public bool IsDisposed { get; protected set; }

				/// <summary>
				/// Gets or sets a value indicating whether you permit disposing this instance.
				/// </summary>
				public bool IsEnabledDispose { get; set; }




				/// <summary>
				/// If this object is disposed, then ObjectDisposedException is thrown.
				/// </summary>
				public void ThrowIfDisposed ()
				{
						if (IsDisposed) 
								throw new ObjectDisposedException (GetType ().FullName);
				}
      
		}
}
