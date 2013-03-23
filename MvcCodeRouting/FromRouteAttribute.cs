﻿// Copyright 2011 Max Toro Q.
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;
using MvcCodeRouting.Controllers;

namespace MvcCodeRouting {
   
   /// <summary>
   /// Represents an attribute that is used to mark action method parameters and 
   /// controller properties, whose values must be bound using <see cref="RouteDataValueProvider"/>.
   /// It also instructs the route creation process to add token segments for each
   /// action method parameter after the {action} token, and for each controller property
   /// after the {controller} token.
   /// </summary>
   [Obsolete("Please use MvcCodeRouting.Web.Mvc.FromRouteAttribute instead.")]
   [EditorBrowsable(EditorBrowsableState.Never)]
   [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
   public class FromRouteAttribute : CustomModelBinderAttribute, IModelBinder, IFromRouteAttribute {

      /// <summary>
      /// The token name. The default name used is the parameter or property name.
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// The token name. The default name used is the parameter or property name.
      /// </summary>
      [Obsolete("Please use Name instead.")]
      public string TokenName { get { return Name; } }

      /// <summary>
      /// A regular expression that specify valid values for the decorated parameter or property.
      /// </summary>
      public string Constraint { get; set; }

      /// <summary>
      /// true if the parameter represents a catch-all token; otherwise, false.
      /// This setting is ignored when used on controller properties.
      /// </summary>
      [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "CatchAll", Justification = "Consistent with naming used in the .NET Framework.")]
      public bool CatchAll { get; set; }

      /// <summary>
      /// Gets or sets the type of the binder.
      /// </summary>
      public Type BinderType { get; set; }

      /// <summary>
      /// Initializes a new instance of the <see cref="FromRouteAttribute"/> class.
      /// </summary>
      public FromRouteAttribute() { }

      /// <summary>
      /// Initializes a new instance of the <see cref="FromRouteAttribute"/> class 
      /// using the specified token name.
      /// </summary>
      /// <param name="tokenName">The token name.</param>
      public FromRouteAttribute(string tokenName) {
         this.Name = tokenName;
      }

      /// <summary>
      /// Gets the model binder used to bind the decorated parameter.
      /// </summary>
      /// <returns>The model binder.</returns>
      public override IModelBinder GetBinder() {
         return this;
      }

      /// <summary>
      /// Binds the decorated parameter to a value by using the specified controller context and
      /// binding context.
      /// </summary>
      /// <param name="controllerContext">The controller context.</param>
      /// <param name="bindingContext">The binding context.</param>
      /// <returns>The bound value.</returns>
      public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {

         RouteValueDictionary values = controllerContext.RouteData.Values;

         string paramName = (this.Name != null
            && !String.Equals(bindingContext.ModelName, this.Name, StringComparison.OrdinalIgnoreCase)
            && !values.ContainsKey(bindingContext.ModelName)) ?
            bindingContext.ModelName
            : null;

         if (paramName != null) {

            values = new RouteValueDictionary(values) { 
               { paramName, values[this.Name] }
            };

            bindingContext.ValueProvider = new DictionaryValueProvider<object>(values, CultureInfo.InvariantCulture);
         
         } else {
            
            bindingContext.ValueProvider = new RouteDataValueProvider(controllerContext);
         }

         IModelBinder binder = GetRealBinder(bindingContext);

         return binder.BindModel(controllerContext, bindingContext);
      }

      IModelBinder GetRealBinder(ModelBindingContext bindingContext) {

         if (this.BinderType != null) {

            try {
               return (IModelBinder)Activator.CreateInstance(this.BinderType);

            } catch (Exception ex) {
               throw new InvalidOperationException("An error occurred when trying to create the IModelBinder '{0}'. Make sure that the binder has a public parameterless constructor.".FormatInvariant(this.BinderType.FullName), ex);
            }
         }

         return ModelBinders.Binders.GetBinder(bindingContext.ModelType, fallbackToDefault: true);
      }
   }
}
