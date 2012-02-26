/*
 * Encog(tm) Core v2.5 
 * http://www.heatonresearch.com/encog/
 * http://code.google.com/p/encog-java/
 * 
 * Copyright 2008-2010 by Heaton Research Inc.
 * 
 * Released under the LGPL.
 *
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 * 
 * Encog and Heaton Research are Trademarks of Heaton Research, Inc.
 * For information on Heaton Research trademarks, visit:
 * 
 * http://www.heatonresearch.com/copyright.html
 */

 namespace Encog.Engine {
	
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	
	/// <summary>
	/// General error class for Encog Engine.
	/// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
	public class EncogEngineError : Exception {
	
	
		/// <summary>
		/// Construct a message exception.
		/// </summary>
		///
		/// <param name="msg">The exception message.</param>
		public EncogEngineError(String msg) : base(msg) {
		}
	
		/// <summary>
		/// Construct an exception that holds another exception.
		/// </summary>
		///
		/// <param name="msg">A message.</param>
		/// <param name="t">The other exception.</param>
		public EncogEngineError(String msg, Exception t) : base(msg, t) {
		}
	
		/// <summary>
		/// Construct an exception that holds another exception.
		/// </summary>
		///
		/// <param name="t">The other exception.</param>
		public EncogEngineError(Exception t) : base(t.Message, t) {
		}
	}
}
