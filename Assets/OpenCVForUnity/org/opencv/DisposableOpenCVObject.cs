using UnityEngine;

using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCVForUnity
{

		/// <summary>
		/// DisposableObject + ICvPtrHolder
		/// </summary>
		abstract public class DisposableOpenCVObject : DisposableObject
		{
				/// <summary>
				/// Data pointer
				/// </summary>
				internal IntPtr nativeObj;

       

				/// <summary>
				/// Default constructor
				/// </summary>
				protected DisposableOpenCVObject ()
            : this(true)
				{
				}


				/// <summary>
				/// 
				/// </summary>
				/// <param name="ptr"></param>
				protected DisposableOpenCVObject (IntPtr ptr)
            : this(ptr, true)
				{
				}


				/// <summary>
				///  
				/// </summary>
				/// <param name="isEnabledDispose"></param>
				protected DisposableOpenCVObject (bool isEnabledDispose)
            : this(IntPtr.Zero, isEnabledDispose)
				{
				}


				/// <summary>
				/// 
				/// </summary>
				/// <param name="ptr"></param>
				/// <param name="isEnabledDispose"></param>
				protected DisposableOpenCVObject (IntPtr ptr, bool isEnabledDispose)
            : base(isEnabledDispose)
				{
						this.nativeObj = ptr;
				}


				/// <summary>
				/// Clean up any resources being used.
				/// </summary>
				/// <param name="disposing">
				/// If disposing equals true, the method has been called directly or indirectly by a user's code. Managed and unmanaged resources can be disposed.
				/// If false, the method has been called by the runtime from inside the finalizer and you should not reference other objects. Only unmanaged resources can be disposed.
				/// </param>
				protected override void Dispose (bool disposing)
				{


						try {
								if (disposing) {
								}
								nativeObj = IntPtr.Zero;

						} finally {
								base.Dispose (disposing);
						}

				}
     
		}
}
