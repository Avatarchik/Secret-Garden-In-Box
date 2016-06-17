using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace OpenCVForUnity
{
		public static class Utils
		{


				/**
     * Copy OpenCV Mat Data to the Pixel Data IntPtr.
     * <p>
     * <br>This function copy the OpenCV Mat Data to the Pixel Data IntPtr.
     * <br>The Pixel Data has to be of the same byte size as the input Mat Data ([total() * elemSize()] byte).
     * <br>Because this method doesn't check bounds, is faster than Mat.get().
     *
     * @param mat The input Mat object
     * @param intPtr The Pixel Data has to be of the same byte size as the input Mat Data ([total() * elemSize()] byte).
	 */
				public static void copyFromMat (Mat mat, IntPtr intPtr)
				{
						if (mat != null)
								mat.ThrowIfDisposed ();
			
						if (mat == null)
								throw new ArgumentNullException ("mat == null");
						if (intPtr == IntPtr.Zero)
								throw new ArgumentNullException ("intPtr == IntPtr.Zero");
			
			
						#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR)
			
			OpenCVForUnity_MatDataToByteArray(mat.nativeObj, intPtr);
			
						#else
						return;
						#endif
			
			
				}
		
				/**
     * Copy the Pixel Data IntPtr to OpenCV Mat Data.
     * <p>
     * This function copy the Pixel Data IntPtr to the OpenCV Mat Data.
     * <br>The output Mat object has to be of the same byte size as the Pixel Data ([total() * elemSize()] byte).
     * <br>Because this method doesn't check bounds, is faster than Mat.put().
     * 
     * @param intPtr The Pixel Data IntPtr
     * @param mat The output Mat object has to be of the same byte size as the Pixel Data ([total() * elemSize()] byte).
     */
				public static void copyToMat (IntPtr intPtr, Mat mat)
				{
						if (mat != null)
								mat.ThrowIfDisposed ();
			
						if (intPtr == IntPtr.Zero)
								throw new ArgumentNullException ("intPtr == IntPtr.Zero");
						if (mat == null)
								throw new ArgumentNullException ("mat == null");
			
			
						#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR)
			
			
			OpenCVForUnity_ByteArrayToMatData( intPtr, mat.nativeObj);
			
						#else
						return;
						#endif
			
				}

				/**
     * Copy OpenCV Mat Data to the Pixel Data Array.
     * <p>
     * <br>This function copy the OpenCV Mat Data to the Pixel Data Array.
     * <br>The Pixel Data Array has to be of the same byte size as the input Mat Data ([total() * elemSize()] byte).
     * <br>Because this method doesn't check bounds, is faster than Mat.get().
     *
     * @param mat The input Mat object
     * @param array The Pixel Data Array has to be of the same byte size as the input Mat Data ([total() * elemSize()] byte).
	 */
				public static void copyFromMat<T> (Mat mat, IList<T> array)
				{
						if (mat != null)
								mat.ThrowIfDisposed ();
			
						if (mat == null)
								throw new ArgumentNullException ("mat == null");
						if (array == null)
								throw new ArgumentNullException ("array == null");
			
			
						#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR)
			GCHandle arrayHandle = GCHandle.Alloc(array,GCHandleType.Pinned);

			OpenCVForUnity_MatDataToByteArray(mat.nativeObj, arrayHandle.AddrOfPinnedObject());

			arrayHandle.Free ();
						#else
						return;
						#endif
			
			
				}
		
				/**
     * Copy the Pixel Data Array to OpenCV Mat Data.
     * <p>
     * This function copy the Pixel Data Array to the OpenCV Mat Data.
     * <br>The output Mat object has to be of the same byte size as the Pixel Data Array ([total() * elemSize()] byte).
     * <br>Because this method doesn't check bounds, is faster than Mat.put().
     * 
     * @param array The Pixel Data Array
     * @param mat The output Mat object has to be of the same byte size as the Pixel Data Array ([total() * elemSize()] byte).
     */
				public static void copyToMat<T> (IList<T> array, Mat mat)
				{
						if (mat != null)
								mat.ThrowIfDisposed ();
			
						if (array == null)
								throw new ArgumentNullException ("array == null");
						if (mat == null)
								throw new ArgumentNullException ("mat == null");
			
			
						#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR)
			GCHandle arrayHandle = GCHandle.Alloc(array,GCHandleType.Pinned);
			
			OpenCVForUnity_ByteArrayToMatData( arrayHandle.AddrOfPinnedObject(), mat.nativeObj);

			arrayHandle.Free ();

						#else
						return;
						#endif
			
				}
		
		
		
		
				/**
     * Converts OpenCV Mat to Unity Texture2D.
     * <p>
     * <br>This function converts the OpenCV Mat to the Unity Texture2D.
     * <br>The input Mat object has to be of the types 'CV_8UC4' (RGBA) , 'CV_8UC3' (RGB) or 'CV_8UC1' (GRAY).
     * <br>The output Texture2D object has to be of the TextureFormat 'RGBA32' or 'ARGB32'.(SetPixels32() must function.)
     * <br>The output Texture2D object has to be of the same size as the input Mat'(width * height).
     *
     * @param mat The input Mat object has to be of the types 'CV_8UC4' (RGBA) , 'CV_8UC3' (RGB) or 'CV_8UC1' (GRAY).
     * @param The output Texture2D object has to be of the TextureFormat 'RGBA32' or 'ARGB32'.(SetPixels32() must function.)texture2D The output Texture2D object has to be of the same size as the input Mat'(width * height).
     * @param bufferColors Optional array to receive pixel data.
     * You can optionally pass in an array of Color32s to use in colors to avoid allocating new memory each frame.
     * The array needs to be initialized to a length matching width * height of the texture.(http://docs.unity3d.com/ScriptReference/WebCamTexture.GetPixels32.html)
	 */
				public static void matToTexture2D (Mat mat, Texture2D texture2D, Color32[] bufferColors = null)
				{
						if (mat != null)
								mat.ThrowIfDisposed ();

						if (mat == null)
								throw new ArgumentNullException ("mat");
						if (texture2D == null)
								throw new ArgumentNullException ("texture2D");

						if (mat.cols () != texture2D.width || mat.rows () != texture2D.height)
								throw new ArgumentException ("The output Texture2D object has to be of the same size");



//						Core.flip (mat, mat, 0);
//
//						byte[] data = new byte[mat.cols () * mat.rows () * mat.channels ()];
//						mat.get (0, 0, data);
//
//						Core.flip (mat, mat, 0);
//
//						if (texture2D.format == TextureFormat.ARGB32 || texture2D.format == TextureFormat.BGRA32 || texture2D.format == TextureFormat.RGBA32) {
//
//								Color32[] colors = new Color32[mat.cols () * mat.rows ()];
//								
//										
//
//								if (mat.type () == CvType.CV_8UC1) {
//										for (int i = 0; i < colors.Length; i++) {
//												colors [i] = new Color32 (data [i], data [i], data [i], 255);
//										}
//								} else if (mat.type () == CvType.CV_8UC3) {
//										for (int i = 0; i < colors.Length; i++) {
//												colors [i] = new Color32 (data [(i * 3) + 0], data [(i * 3) + 1], data [(i * 3) + 2], 255);
//										}
//								} else if (mat.type () == CvType.CV_8UC4) {
//										for (int i = 0; i < colors.Length; i++) {
//												colors [i] = new Color32 (data [(i * 4) + 0], data [(i * 4) + 1], data [(i * 4) + 2], data [(i * 4) + 3]);
//										}
//								}
//										
//								
//								texture2D.SetPixels32 (colors);
//								
//						} else {
//								Color[] colors = new Color[mat.cols () * mat.rows ()];
//								
//								if (mat.type () == CvType.CV_8UC1) {
//										for (int i = 0; i < colors.Length; i++) {
//												colors [i] = new Color ((float)data [i] / 255.0f, data [i] / 255.0f, data [i] / 255.0f);
//										}
//								} else if (mat.type () == CvType.CV_8UC3) {
//										for (int i = 0; i < colors.Length; i++) {
//												colors [i] = new Color ((float)data [(i * 3) + 0] / 255.0f, (float)data [(i * 3) + 1] / 255.0f, (float)data [(i * 3) + 2] / 255.0f);
//										}
//								} else if (mat.type () == CvType.CV_8UC4) {
//										for (int i = 0; i < colors.Length; i++) {
//												colors [i] = new Color ((float)data [(i * 4) + 0] / 255.0f, (float)data [(i * 4) + 1] / 255.0f, (float)data [(i * 4) + 2] / 255.0f);
//										}
//								}
//					
//								
//								texture2D.SetPixels (colors);
//
//						}
//
//						texture2D.Apply ();



						#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR)



			GCHandle colorsHandle;

			if(bufferColors == null){
				Color32[] colors = texture2D.GetPixels32 ();
				
				colorsHandle = GCHandle.Alloc (colors, GCHandleType.Pinned);

				OpenCVForUnity_MatToTexture(mat.nativeObj, colorsHandle.AddrOfPinnedObject());
				
				texture2D.SetPixels32 (colors);
			}else{
				colorsHandle = GCHandle.Alloc (bufferColors, GCHandleType.Pinned);

				OpenCVForUnity_MatToTexture(mat.nativeObj, colorsHandle.AddrOfPinnedObject());
				
				texture2D.SetPixels32 (bufferColors);
			}


			texture2D.Apply ();
			
			colorsHandle.Free ();

						#else
						return;
						#endif


				}

				/**
     * Converts Unity Texture2D to OpenCV Mat.
     * <p>
     * This function converts an Unity Texture2D image to the OpenCV Mat.
     * <br>The output Mat object has to be of the same size as the input Texture2D'(width * height).
     * The output Mat object has to be of the types 'CV_8UC4' (RGBA) , 'CV_8UC3' (RGB) or 'CV_8UC1' (GRAY).
     * 
     * @param texture2D
     * @param mat The output Mat object has to be of the same size as the input Texture2D'(width * height).
     * The output Mat object has to be of the types 'CV_8UC4' (RGBA) , 'CV_8UC3' (RGB) or 'CV_8UC1' (GRAY).
     */
				public static void texture2DToMat (Texture2D texture2D, Mat mat)
				{
						if (mat != null)
								mat.ThrowIfDisposed ();

						if (texture2D == null)
								throw new ArgumentNullException ("texture2D == null");
						if (mat == null)
								throw new ArgumentNullException ("mat == null");

						if (mat.cols () != texture2D.width || mat.rows () != texture2D.height)
								throw new ArgumentException ("The output Mat object has to be of the same size");

//						byte[] data = new byte[mat.cols () * mat.rows () * mat.channels ()];
//
//						Color32[] colors = texture2D.GetPixels32 ();
//
//						if (mat.type () == CvType.CV_8UC1) {
//								for (int i = 0; i < colors.Length; i++) {
//										data [i] = colors [i].b;
//								}
//								mat.put (0, 0, data);
//								Core.flip (mat, mat, 0);
//						} else if (mat.type () == CvType.CV_8UC3) {
//								for (int i = 0; i < colors.Length; i++) {
//										data [(i * 3) + 0] = colors [i].b;
//										data [(i * 3) + 1] = colors [i].g;
//										data [(i * 3) + 2] = colors [i].r;
//								}
//								mat.put (0, 0, data);
//								Core.flip (mat, mat, 0);
//						} else if (mat.type () == CvType.CV_8UC4) {
//								for (int i = 0; i < colors.Length; i++) {
//										data [(i * 4) + 0] = colors [i].b;
//										data [(i * 4) + 1] = colors [i].g;
//										data [(i * 4) + 2] = colors [i].r;
//										data [(i * 4) + 3] = colors [i].a;
//								}
//								mat.put (0, 0, data);
//								Core.flip (mat, mat, 0);
//						}

						#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR)
			
			Color32[] colors = texture2D.GetPixels32 ();
			
			GCHandle colorsHandle = GCHandle.Alloc (colors, GCHandleType.Pinned);
			
			OpenCVForUnity_TextureToMat( colorsHandle.AddrOfPinnedObject(), mat.nativeObj);
			
			colorsHandle.Free ();
			
						#else
						return;
						#endif

				}

				/**
     * Converts Unity WebCamTexture to OpenCV Mat.
     * <p>
     * This function converts an Unity WebCamTexture image to the OpenCV Mat.
     * <br>The output Mat object has to be of the same size as the input WebCamTexture'(width * height).
     * The output Mat object has to be of the types 'CV_8UC4' (RGBA) , 'CV_8UC3' (RGB) or 'CV_8UC1' (GRAY).
     * 
     * @param webcamTexture
     * @param mat The output Mat object has to be of the same size as the input WebCamTexture'(width * height).
     * The output Mat object has to be of the types 'CV_8UC4' (RGBA) , 'CV_8UC3' (RGB) or 'CV_8UC1' (GRAY).
     * @param bufferColors Optional array to receive pixel data.
     * You can optionally pass in an array of Color32s to use in colors to avoid allocating new memory each frame.
     * The array needs to be initialized to a length matching width * height of the texture.(http://docs.unity3d.com/ScriptReference/WebCamTexture.GetPixels32.html)
     */
				public static void WebCamTextureToMat (WebCamTexture webCamTexture, Mat mat, Color32[] bufferColors = null)
				{
						if (mat != null)
								mat.ThrowIfDisposed ();
			
						if (webCamTexture == null)
								throw new ArgumentNullException ("webCamTexture == null");
						if (mat == null)
								throw new ArgumentNullException ("mat == null");
			
						if (mat.cols () != webCamTexture.width || mat.rows () != webCamTexture.height)
								throw new ArgumentException ("The output Mat object has to be of the same size");
			
//												byte[] data = new byte[mat.cols () * mat.rows () * mat.channels ()];
//						
//												Color32[] colors = webCamTexture.GetPixels32 ();
//						
//												if (mat.type () == CvType.CV_8UC1) {
//														for (int i = 0; i < colors.Length; i++) {
//																data [i] = colors [i].b;
//														}
//														mat.put (0, 0, data);
//														Core.flip (mat, mat, 0);
//												} else if (mat.type () == CvType.CV_8UC3) {
//														for (int i = 0; i < colors.Length; i++) {
//																data [(i * 3) + 0] = colors [i].r;
//																data [(i * 3) + 1] = colors [i].g;
//																data [(i * 3) + 2] = colors [i].b;
//														}
//														mat.put (0, 0, data);
//														Core.flip (mat, mat, 0);
//												} else if (mat.type () == CvType.CV_8UC4) {
//														for (int i = 0; i < colors.Length; i++) {
//																data [(i * 4) + 0] = colors [i].r;
//																data [(i * 4) + 1] = colors [i].g;
//																data [(i * 4) + 2] = colors [i].b;
//																data [(i * 4) + 3] = colors [i].a;
//														}
//														mat.put (0, 0, data);
//														Core.flip (mat, mat, 0);
//												}
			
						#if UNITY_PRO_LICENSE || ((UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR)
			GCHandle colorsHandle;
			if(bufferColors == null){

			Color32[] colors = webCamTexture.GetPixels32 ();
			
			colorsHandle = GCHandle.Alloc (colors, GCHandleType.Pinned);
			}else{
				webCamTexture.GetPixels32 (bufferColors);
				
				colorsHandle = GCHandle.Alloc (bufferColors, GCHandleType.Pinned);
			}
			
			OpenCVForUnity_TextureToMat( colorsHandle.AddrOfPinnedObject(), mat.nativeObj);
			
			colorsHandle.Free ();
			
						#else
						return;
						#endif
			
				}

				/// <summary>
				/// Gets the file path.
				/// [Android]Set the filename containing the folder of 'Assets/Plugin/Android/assets'.
				/// [iOS]Set the filename Added to Xcode by 'Build Phases > Copy Bundle Resources'
				/// </summary>
				/// <returns>The file path.</returns>
				/// <param name="filename">Filename.[Android]Set the filename containing the folder of 'Assets/Plugin/Android/assets'.[iOS]Set the filename Added to Xcode by 'Build Phases > Copy Bundle Resources'</param>
				public static string getFilePath (string filename)
				{

						
						#if (UNITY_ANDROID) && !UNITY_EDITOR
			               return Marshal.PtrToStringAnsi (OpenCVForUnity_GetFilePath (filename));
						#else
						return System.IO.Path.Combine (Application.streamingAssetsPath, filename);
						#endif
				}

				/// <summary>
				/// URs the shift.
				/// </summary>
				/// <returns>The shift.</returns>
				/// <param name="number">Number.</param>
				/// <param name="bits">Bits.</param>
				internal static int URShift (int number, int bits)
				{
						if (number >= 0)
								return number >> bits;
						else
								return (number >> bits) + (2 << ~bits);
				}
		
				/// <summary>
				/// URs the shift.
				/// </summary>
				/// <returns>The shift.</returns>
				/// <param name="number">Number.</param>
				/// <param name="bits">Bits.</param>
				internal static long URShift (long number, int bits)//TODO:@check
				{
						if (number >= 0)
								return number >> bits;
						else
								return (number >> bits) + (2 << ~bits);
				}
		
				/// <summary>
				/// Determines if hash contents the specified enumerable.
				/// </summary>
				/// <returns><c>true</c> if hash contents the specified enumerable; otherwise, <c>false</c>.</returns>
				/// <param name="enumerable">Enumerable.</param>
				/// <typeparam name="T">The 1st type parameter.</typeparam>
				internal static int HashContents<T> (this IEnumerable<T> enumerable)//TODO:@check
				{
						int hash = 0x218A9B2C;
						foreach (var item in enumerable) {
								int thisHash = item.GetHashCode ();
								//mix up the bits.
								hash = thisHash ^ ((hash << 5) + hash);
						}
						return hash;
				}


		#if UNITY_IPHONE && !UNITY_EDITOR
		[DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr OpenCVForUnity_GetFilePath (string filename);

		[DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
		private static extern void OpenCVForUnity_MatToTexture (IntPtr mat, IntPtr textureColors);
		
		[DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
		private static extern void OpenCVForUnity_TextureToMat (IntPtr textureColors, IntPtr Mat);

		[DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
		private static extern void OpenCVForUnity_MatDataToByteArray (IntPtr mat, IntPtr byteArray);
		
		[DllImport("__Internal", CallingConvention = CallingConvention.Cdecl)]
		private static extern void OpenCVForUnity_ByteArrayToMatData (IntPtr byteArray, IntPtr Mat);

#else
				[DllImport("opencvforunity", CallingConvention = CallingConvention.Cdecl)]
				private static extern IntPtr OpenCVForUnity_GetFilePath (string filename);
		
				[DllImport("opencvforunity", CallingConvention = CallingConvention.Cdecl)]
				private static extern void OpenCVForUnity_MatToTexture (IntPtr mat, IntPtr textureColors);
		
				[DllImport("opencvforunity", CallingConvention = CallingConvention.Cdecl)]
				private static extern void OpenCVForUnity_TextureToMat (IntPtr textureColors, IntPtr Mat);

				[DllImport("opencvforunity", CallingConvention = CallingConvention.Cdecl)]
				private static extern void OpenCVForUnity_MatDataToByteArray (IntPtr mat, IntPtr byteArray);
		
				[DllImport("opencvforunity", CallingConvention = CallingConvention.Cdecl)]
				private static extern void OpenCVForUnity_ByteArrayToMatData (IntPtr byteArray, IntPtr Mat);
		#endif
		}
}
