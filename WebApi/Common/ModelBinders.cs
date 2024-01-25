using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Audiobooks.WebApi
{
	public class CommaSeparatedArrayModelBinder : IModelBinder
	{
		public Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException(nameof(bindingContext));
			}

			if (!bindingContext.ModelType.IsArray)
			{
				throw new InvalidOperationException();
			}

			var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
			if (valueProviderResult == ValueProviderResult.None)
			{
				return Task.CompletedTask;
			}

			bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

			try
			{
				var elementType = bindingContext.ModelType.GetElementType();
				var converter = TypeDescriptor.GetConverter(elementType);
				var stringArray = valueProviderResult.FirstValue.Split(',');
				var model = Array.CreateInstance(elementType, stringArray.Length);

				for (var i = 0; i < stringArray.Length; i++)
				{
					model.SetValue(converter.ConvertFromString(stringArray[i]), i);
				}

				bindingContext.Result = ModelBindingResult.Success(model);
			}
			catch (Exception exception)
			{
				if (!(exception is FormatException) && exception.InnerException != null)
				{
					exception = ExceptionDispatchInfo.Capture(exception.InnerException).SourceException;
				}

				bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, exception, bindingContext.ModelMetadata);
			}

			return Task.CompletedTask;
		}
	}
}