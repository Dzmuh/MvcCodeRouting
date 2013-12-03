﻿// Copyright 2013 Max Toro Q.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvcCodeRouting.ParameterBinding {
   
   /// <summary>
   /// 
   /// </summary>
   /// <remarks>
   /// Implementations should be thread-safe.
   /// </remarks>
   public abstract class ParameterBinder {

      readonly Type _ParameterType;

      public Type ParameterType {
         get { return _ParameterType; }
      }

      internal static ParameterBinder CreateInstance(Type binderType) {

         try {
            return (ParameterBinder)Activator.CreateInstance(binderType);

         } catch (Exception ex) {
            throw new InvalidOperationException("An error occurred when trying to create the ParameterBinder '{0}'. Make sure that the binder has a public parameterless constructor.".FormatInvariant(binderType.FullName), ex);
         }
      }

      protected ParameterBinder(Type parameterType) {

         if (parameterType == null) throw new ArgumentNullException("parameterType");

         _ParameterType = parameterType;
      }

      public abstract bool TryBind(string value, IFormatProvider provider, out object result);
   }
}
