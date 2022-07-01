using Acklann.Plaid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ProvenCfoUI.Comman
{
    public static class PlaidHelper
    {
		public static TRequest UseDefaultsWithNoAccessToken<TRequest>(this TRequest request,string ClientId,string Secret)
		{
			PropertyInfo[] properties = request.GetType().GetTypeInfo().GetRuntimeProperties().ToArray();
			setProperty(nameof(AuthorizedRequestBase.ClientId), ClientId);
			setProperty(nameof(AuthorizedRequestBase.Secret), Secret);
			return request;

			void setProperty(string name, object value)
			{
				PropertyInfo target = properties.FirstOrDefault(p => p.Name == name);
				if (target != null) target.SetValue(request, value);
			}
		}
	}
}