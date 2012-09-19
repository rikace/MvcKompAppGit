/* ****************************************************************************
 *
 * Copyright (c) Francesco Abbruzzese. All rights reserved.
 * francesco@dotnet-programming.com
 * http://www.dotnet-programming.com/
 * 
 * This software is subject to the the license at http://mvccontrolstoolkit.codeplex.com/license  
 * and included in the license.txt file of this distribution.
 * 
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MVCControlsToolkit.Controls.Bindings
{
    public interface IBindingsBuilderBase
    {
        IBindingsBuilder<N> ToType<N>();
            
    }
    public interface IBindingsBuilder<M> : IBindingsBuilderBase
    {
        IBindingsBuilder<M> Add(string binding);
        IBindingsBuilder<M> AddMethod(string name, string javaScriptCode);
        IBindingsBuilder<M> CSS<F>(
            string className,
            Expression<Func<M, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions);
        IBindingsBuilder<M> Style<F>(
            string stypePropertyName,
            Expression<Func<M, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions);
        IBindingsBuilder<M> Attr<F>(
            string attrName,
            Expression<Func<M, F>> expression,
            string format = null,
            params LambdaExpression[] otherExpressions);
        IBindingsBuilder<M> Event<F>(
            string eventName,
            Expression<Func<M, F>> expression,
            bool bubble = true,
            string format = null,
            params LambdaExpression[] otherExpressions);
        string GetFullName<F>(Expression<Func<M, F>> expression);
        string BindingPrefix { get; }
        string VerifyFieldsValid<F>(
                Expression<Func<M, F>> expression,
                params LambdaExpression[] otherExpressions);
        string VerifyFormValid();
        LambdaExpression L<F>(Expression<Func<M, F>> expression);
        MvcHtmlString Get();
        string ModelPrefix { get; }
        string ModelName { get; }
        string ValidationType { get; }
        void AddServerErrors(string prefix);
        MvcHtmlString HandleServerErrors();
        void SetHelper(HtmlHelper htmlHelper);
        HtmlHelper GetHelper();
        string GetFullBindingName(LambdaExpression expression);
        dynamic BuildExpression(string text);
        IAncestorBindingsBuilder<N, M> Parent<N>();
        IAncestorBindingsBuilder<N, M> Root<N>();
        IAncestorBindingsBuilder<N, M> Parents<N>(int i);
    }
    public interface IAncestorBindingsBuilder<M, A> : IBindingsBuilder<M>
    {
        IBindingsBuilder<A> Back();
    }
}
