using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WpfSudoku.ViewModel
{
	public class ViewModel<DerivedType> : INotifyPropertyChanged where DerivedType : class
	{
		public ViewModel()
		{
			PropertyChanged += ViewModel_PropertyChanged;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void AddPropertyChangedHandler(string propertyName, Action handler)
		{
			GetHandlerList(propertyName).Add(new Handler(handler));
		}

		public void AddPropertyChangedHandler<TProp>(Expression<Func<TProp>> propertyExpression, Action<DerivedType> handler)
		{
			ExtractInstanceAndPropertyName(propertyExpression, out string propertyName, out DerivedType instance);
			GetHandlerList(propertyName).Add(new InstanceHandler(handler, instance));
		}

		public void AddPropertyChangedHandler<TProp>(Expression<Func<TProp>> propertyExpression, Action<DerivedType, TProp> handler)
		{
			ExtractInstanceAndPropertyName(propertyExpression, out string propertyName, out DerivedType instance);
			GetHandlerList(propertyName).Add(new OldValueHandler<TProp>(handler, instance));
		}

		protected void Set<Value>(ref Value backendStore, Value value, [CallerMemberName] string propertyName = "")
		{
			var oldValue = backendStore;
			backendStore = value;
			NotifyPropertyChanged(oldValue, propertyName);
		}

		protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		
		protected void NotifyPropertyChanged<T>(T oldValue, [CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedExtendedEventArgs<T>(propertyName, oldValue));

		private interface IHandler
		{
			void Invoke(PropertyChangedEventArgs args);
		}

		private class Handler : IHandler
		{
			private readonly Action _handler;

			public Handler(Action handler) => _handler = handler;

			public void Invoke(PropertyChangedEventArgs _) => _handler();
		}

		private class InstanceHandler : IHandler
		{
			private readonly Action<DerivedType> _handler;
			private readonly DerivedType _instance;

			public InstanceHandler(Action<DerivedType> handler, DerivedType instance)
			{
				_handler = handler;
				_instance = instance;
			}

			public void Invoke(PropertyChangedEventArgs _) => _handler(_instance);
		}

		private class OldValueHandler<Type> : IHandler
		{
			private readonly Action<DerivedType, Type> _handler;
			private readonly DerivedType _instance;

			public OldValueHandler(Action<DerivedType, Type> handler, DerivedType instance)
			{
				_handler = handler;
				_instance = instance;
			}

			public void Invoke(PropertyChangedEventArgs args)
			{
				if (args is PropertyChangedExtendedEventArgs<Type> exArgs)
				{
					_handler(_instance, exArgs.OldValue);
				}
			}
		}

		private readonly Dictionary<string, List<IHandler>> handlerData = new Dictionary<string, List<IHandler>>();

		private List<IHandler> GetHandlerList(string propertyName)
		{
			if (!handlerData.TryGetValue(propertyName, out var handlerList))
			{
				handlerList = new List<IHandler>();
				handlerData[propertyName] = handlerList;
			}
			return handlerList;
		}

		private static object? Evaluate(Expression e)
		{
			switch (e.NodeType)
			{
				case ExpressionType.Constant:
					return (e as ConstantExpression)?.Value;
				case ExpressionType.MemberAccess:
					{
						if (!(e is MemberExpression propertyExpression)) return null;
						var field = propertyExpression.Member as FieldInfo;
						var property = propertyExpression.Member as PropertyInfo;
						var container = propertyExpression.Expression == null ? null : Evaluate(propertyExpression.Expression);
						if (field != null)
							return field.GetValue(container);
						else if (property != null)
							return property.GetValue(container, null);
						else
							return null;
					}
				default:
					return null;
			}
		}

		private static void ExtractInstanceAndPropertyName<TProp>(Expression<Func<TProp>> propertyExpression, out string propertyName, out DerivedType instance)
		{
			if (propertyExpression.Body is MemberExpression memberExpression)
			{
				if (memberExpression?.Member is PropertyInfo propertyInfo)
				{
					if (Evaluate(memberExpression.Expression) is DerivedType instanceObject)
					{
						propertyName = propertyInfo.Name;
						instance = instanceObject;
						return;
					}
				}
			}
			throw new InvalidOperationException("Please provide a valid property expression, like '() => instance.PropertyName'.");
		}

		private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (handlerData.TryGetValue(e.PropertyName, out var handlerList))
			{
				foreach (var handlerData in handlerList)
				{
					handlerData.Invoke(e);
				}
			}
		}
	}
}
