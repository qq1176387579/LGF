#region Header
/**
 * JsonMapper.cs
 *   JSON to .Net object and object to JSON conversions.
 *
 * The authors disclaim copyright to this source code. For more details, see
 * the COPYING file included with this distribution.
 **/
#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Reflection;


//  TODO:: find better place for this generic attribute
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class NonSerializedProperty : System.Attribute
{}


namespace LitJson
{
	internal struct PropertyMetadata
	{
		public MemberInfo Info;
		public bool	      IsField;
		public Type	      Type;
	}


	internal struct ArrayMetadata
	{
		private Type element_type;
		private bool is_array;
		private bool is_list;


		public Type ElementType {
			get {
				if (element_type == null)
					return typeof (JsonData);

				return element_type;
			}

			set { element_type = value; }
		}

		public bool IsArray {
			get { return is_array; }
			set { is_array = value; }
		}

		public bool IsList {
			get { return is_list; }
			set { is_list = value; }
		}
	}


	internal struct ObjectMetadata
	{
		private Type element_type;
		private bool is_dictionary;

		private Dictionary<string, PropertyMetadata> properties;


		public Type ElementType {
			get {
				if (element_type == null)
					return typeof (JsonData);

				return element_type;
			}

			set { element_type = value; }
		}

		public bool IsDictionary {
			get { return is_dictionary; }
			set { is_dictionary = value; }
		}

		public Dictionary<string, PropertyMetadata> Properties {
			get { return properties; }
			set { properties = value; }
		}
	}


	internal delegate void ExporterFunc	(object obj, JsonWriter writer);
	public   delegate void ExporterFunc<T> (T obj, JsonWriter writer);

	internal delegate object ImporterFunc				(object input);
	public   delegate TValue ImporterFunc<TJson, TValue> (TJson input);

	public delegate IJsonWrapper WrapperFactory ();


	public interface IJsonExporter
	{
		string Stringify();
	}


	public class JsonMapper
	{
		#region Fields
		private static int max_nesting_depth;

		internal static IFormatProvider datetime_format;

		private static Dictionary<Type, ExporterFunc> base_exporters_table;
		private static Dictionary<Type, ExporterFunc> custom_exporters_table;

		private static Dictionary<Type,
				Dictionary<Type, ImporterFunc>> base_importers_table;
		private static Dictionary<Type,
				Dictionary<Type, ImporterFunc>> custom_importers_table;

		private static Dictionary<Type, ArrayMetadata> array_metadata;
		private static readonly object array_metadata_lock = new Object ();

		private static Dictionary<Type,
				Dictionary<Type, MethodInfo>> conv_ops;
		private static readonly object conv_ops_lock = new Object ();

		private static Dictionary<Type, ObjectMetadata> object_metadata;
		private static readonly object object_metadata_lock = new Object ();

		private static Dictionary<Type,
				List<PropertyMetadata>> type_properties;
		private static readonly object type_properties_lock = new Object ();

		private static JsonWriter	  static_writer;
		private static readonly object static_writer_lock = new Object ();
		#endregion


		#region Constructors
		static JsonMapper ()
		{
			max_nesting_depth = 100;

			array_metadata = new Dictionary<Type, ArrayMetadata> ();
			conv_ops = new Dictionary<Type, Dictionary<Type, MethodInfo>> ();
			object_metadata = new Dictionary<Type, ObjectMetadata> ();
			type_properties = new Dictionary<Type,
							List<PropertyMetadata>> ();

			static_writer = new JsonWriter ();

			datetime_format = DateTimeFormatInfo.InvariantInfo;

			base_exporters_table   = new Dictionary<Type, ExporterFunc> ();
			custom_exporters_table = new Dictionary<Type, ExporterFunc> ();

			base_importers_table = new Dictionary<Type,
								 Dictionary<Type, ImporterFunc>> ();
			custom_importers_table = new Dictionary<Type,
								   Dictionary<Type, ImporterFunc>> ();

			RegisterBaseExporters ();
			RegisterBaseImporters ();
		}
		#endregion


		#region Private Methods
		private static void AddArrayMetadata (Type type)
		{
			if (array_metadata.ContainsKey (type))
				return;

			ArrayMetadata data = new ArrayMetadata ();

			data.IsArray = type.IsArray;

			if (type.GetInterface ("System.Collections.IList") != null)
				data.IsList = true;

			foreach (PropertyInfo p_info in type.GetProperties ()) {
				if (p_info.Name != "Item")
					continue;

				ParameterInfo[] parameters = p_info.GetIndexParameters ();

				if (parameters.Length != 1)
					continue;

				if (parameters[0].ParameterType == typeof (int))
					data.ElementType = p_info.PropertyType;
			}

			lock (array_metadata_lock) {
				try {
					array_metadata.Add (type, data);
				} catch (ArgumentException) {
					return;
				}
			}
		}

		private static void AddObjectMetadata (Type type)
		{
			if (object_metadata.ContainsKey (type))
				return;

			ObjectMetadata data = new ObjectMetadata ();

			if (type.GetInterface ("System.Collections.IDictionary") != null)
				data.IsDictionary = true;

			data.Properties = new Dictionary<string, PropertyMetadata> ();

			bool isAnonymousType = type.IsGenericType == true && type.IsNotPublic == true
				? type.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)
				: false;
			
			const BindingFlags propertyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty;
			foreach (PropertyInfo p_info in type.GetProperties (propertyFlags)) {
				if (p_info.Name == "Item") {
					ParameterInfo[] parameters = p_info.GetIndexParameters ();

					if (parameters.Length != 1)
						continue;

					if (parameters[0].ParameterType == typeof (string))
						data.ElementType = p_info.PropertyType;

					continue;
				}
				
				if (p_info.CanRead == false)
					continue;
				if (p_info.CanWrite == false && isAnonymousType == false)
					continue;
				if( p_info.IsDefined( typeof(NonSerializedProperty), false ))
					continue;
					
				PropertyMetadata p_data = new PropertyMetadata ();
				p_data.Info = p_info;
				p_data.Type = p_info.PropertyType;

				data.Properties.Add (p_info.Name, p_data);
			}					

			const BindingFlags fieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.GetField;
			foreach (FieldInfo f_info in type.GetFields (fieldFlags)) {
				if (f_info.IsStatic == true)
					continue;
				if (f_info.IsInitOnly == true)
					continue;
				if (f_info.IsNotSerialized == true)
					continue;
#if !UNITY_WSA || UNITY_EDITOR			
				if (f_info.IsPublic == false && System.Attribute.IsDefined(f_info, typeof(UnityEngine.SerializeField)) == false)
					continue;
#else
				if (f_info.IsPublic == false && f_info.IsDefined(typeof(UnityEngine.SerializeField)) == false)
					continue;
#endif
					
				PropertyMetadata p_data = new PropertyMetadata ();
				p_data.Info = f_info;
				p_data.IsField = true;
				p_data.Type = f_info.FieldType;

				data.Properties.Add (f_info.Name, p_data);
			}					

			lock (object_metadata_lock) {
				try {
					object_metadata.Add (type, data);
				} catch (ArgumentException) {
					return;
				}
			}
		}

		private static void AddTypeProperties (Type type)
		{
			if (type_properties.ContainsKey (type))
				return;

			List<PropertyMetadata> props = new List<PropertyMetadata> ();

			bool isAnonymousType = type.IsGenericType == true && type.IsNotPublic == true
#if !UNITY_WSA || UNITY_EDITOR			
				? System.Attribute.IsDefined(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)
#else
				? type.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)
#endif							
				: false;
			
			const BindingFlags propertyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty;
			foreach (PropertyInfo p_info in type.GetProperties (propertyFlags)) {
				if (p_info.Name == "Item")
					continue;
				if (p_info.CanRead == false)
					continue;
				if (p_info.CanWrite == false && isAnonymousType == false)
					continue;
				if( p_info.IsDefined( typeof(NonSerializedProperty), false ))
					continue;

				PropertyMetadata p_data = new PropertyMetadata ();
				p_data.Info = p_info;
				p_data.IsField = false;
				props.Add (p_data);
			}

			const BindingFlags fieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.GetField;
			foreach (FieldInfo f_info in type.GetFields (fieldFlags)) {
				if (f_info.IsStatic == true)
					continue;
				if (f_info.IsInitOnly == true)
					continue;
				if (f_info.IsNotSerialized == true)
					continue;
#if !UNITY_WSA || UNITY_EDITOR				
				if (f_info.IsPublic == false && System.Attribute.IsDefined(f_info, typeof(UnityEngine.SerializeField)) == false)
					continue;
#else
				if (f_info.IsPublic == false && f_info.IsDefined(typeof(UnityEngine.SerializeField)) == false)
					continue;
#endif

				PropertyMetadata p_data = new PropertyMetadata ();
				p_data.Info = f_info;
				p_data.IsField = true;

				props.Add (p_data);
			}

			lock (type_properties_lock) {
				try {
					type_properties.Add (type, props);
				} catch (ArgumentException) {
					return;
				}
			}
		}

		private static MethodInfo GetConvOp (Type t1, Type t2)
		{
			lock (conv_ops_lock) {
				if (! conv_ops.ContainsKey (t1))
					conv_ops.Add (t1, new Dictionary<Type, MethodInfo> ());
			}

			MethodInfo op;
			
			if (conv_ops[t1].TryGetValue (t2, out op))
				return op;

			op = t1.GetMethod (
				"op_Implicit", new Type[] { t2 });

			lock (conv_ops_lock) {
				try {
					conv_ops[t1].Add (t2, op);
				} catch (ArgumentException) {
					return conv_ops[t1][t2];
				}
			}

			return op;
		}
		
		static bool SkipArray(JsonReader reader)
		{			
			if (reader.Token != JsonToken.ArrayStart)
			{
				return false;
			}

			while (true)
			{
				if (!reader.Read())
				{
					return false;
				}

				if (reader.Token == JsonToken.ArrayEnd)
				{
					return true;
				}
				else if (reader.Token == JsonToken.ArrayStart)
				{
					SkipArray(reader);
				}
			}
		}
		
		static bool SkipObject(JsonReader reader)
		{
			if (reader.Token != JsonToken.ObjectStart)
			{
				return false;
			}
			
			while (true)
			{
				if (!reader.Read())
				{
					return false;
				}
				
				if (reader.Token == JsonToken.ObjectEnd)
				{
					return true;					
				}
				else if (reader.Token == JsonToken.ObjectStart)
				{
					SkipObject(reader);
				}
			}
		}

		private static object ReadValue (Type inst_type, JsonReader reader)
		{
			reader.Read ();

			if (reader.Token == JsonToken.ArrayEnd)
				return null;

			if (reader.Token == JsonToken.Null) {

				if (! inst_type.IsClass)
					throw new JsonException (String.Format (
							"Can't assign null to an instance of type {0}",
							inst_type));

				return null;
			}

			if (reader.Token == JsonToken.Double ||
				reader.Token == JsonToken.Int ||
				reader.Token == JsonToken.Long ||
				reader.Token == JsonToken.String ||
				reader.Token == JsonToken.Boolean) {

				return ExtractValue (inst_type, reader.Value);
			}

			object instance = null;

			if (reader.Token == JsonToken.ArrayStart) {

				instance = ExtractArray (inst_type, reader);
						
			} else if (reader.Token == JsonToken.ObjectStart) {

				instance = ExtractObject (inst_type, reader);

			}

			return instance;
		}
				
		internal static object ExtractValue (Type inst_type, object value)
		{
			Type json_type = value.GetType ();

			if (inst_type.IsAssignableFrom (json_type))
				return value;

			Dictionary<Type, ImporterFunc> table;
			ImporterFunc importer;

			// If there's a custom importer that fits, use it
			if (custom_importers_table.TryGetValue (json_type, out table) &&
				table.TryGetValue (inst_type, out importer)) {

				return importer (value);
			}

			// Maybe there's a base importer that works
			if (base_importers_table.TryGetValue (json_type, out table) &&
				table.TryGetValue (inst_type, out importer)) {

				return importer (value);
			}

			// Maybe it's an enum
			if (inst_type.IsEnum) {
				if (value != null && value.GetType() == typeof(string))
					return Enum.Parse (inst_type, (string)value);
				else
					return Enum.ToObject (inst_type, value);
			}

			// Try using an implicit conversion operator
			MethodInfo conv_op = GetConvOp (inst_type, json_type);

			if (conv_op != null)
				return conv_op.Invoke (null,
									   new object[] { value });

			// No luck
			throw new JsonException (String.Format (
					"Can't assign value '{0}' (type {1}) to type {2}",
					value, json_type, inst_type));
		}

		private static object ExtractArray (Type inst_type, JsonReader reader)
		{
			object instance = null;

			AddArrayMetadata (inst_type);
			ArrayMetadata t_data = array_metadata[inst_type];

			if (! t_data.IsArray && ! t_data.IsList)
				throw new JsonException (String.Format (
						"Type {0} can't act as an array",
						inst_type));

			IList list;
			Type elem_type;

			if (! t_data.IsArray) {
				list = (IList) Activator.CreateInstance (inst_type);
				elem_type = t_data.ElementType;
			} else {
				list = new ArrayList ();
				elem_type = inst_type.GetElementType ();
			}

			while (true) {
				object item = ReadValue (elem_type, reader);
				if (item == null && reader.Token == JsonToken.ArrayEnd)
					break;

				list.Add (item);
			}

			if (t_data.IsArray) {
				int n = list.Count;
				instance = Array.CreateInstance (elem_type, n);

				for (int i = 0; i < n; i++)
					((Array) instance).SetValue (list[i], i);
			} else
				instance = list;

			return instance;
		}
			
		private static object ExtractObject (Type inst_type, JsonReader reader)
		{
			object instance = Activator.CreateInstance (inst_type);
			
			return ApplyOnObject (inst_type, instance, reader);
		}		
			
		private static object ApplyOnObject (Type inst_type, Object instance, JsonReader reader)
		{
			AddObjectMetadata (inst_type);
			ObjectMetadata t_data = object_metadata[inst_type];

			while (true) {
				reader.Read ();

				if (reader.Token == JsonToken.ObjectEnd)
					break;

				string property = (string) reader.Value;

				PropertyMetadata prop_data;
				if (t_data.Properties.TryGetValue (property, out prop_data)) 
				{
					if (prop_data.IsField) 
					{
						((FieldInfo) prop_data.Info).SetValue(instance, ReadValue (prop_data.Type, reader));
					} 
					else 
					{
						PropertyInfo p_info = (PropertyInfo) prop_data.Info;

						if (p_info.CanWrite)
							p_info.SetValue(instance,ReadValue (prop_data.Type, reader),null);
						else
							ReadValue (prop_data.Type, reader);
					}
				} 
				else 
				{
					if (! t_data.IsDictionary)
					{
//							throw new JsonException (String.Format ("The type {0} doesn't have the property '{1}'", inst_type, property));
						
						// We don't treat missing property as fatal error - just ignore it
						
						reader.Read();
						
						if (reader.Token == JsonToken.ArrayStart)
						{
							if (!SkipArray(reader))
							{
								throw new JsonException("Error skipping array");
							}
						}
						else if (reader.Token == JsonToken.ObjectStart)
						{
							if (!SkipObject(reader))
							{
								throw new JsonException("Error skipping object");
							}
						}
						
						continue;
					}
											   
					((IDictionary) instance).Add (property, ReadValue (t_data.ElementType, reader));
				}
			}

			return instance;
		}
		
		private static JsonData FromObject (Type inst_type, Object instance)
		{
			AddObjectMetadata (inst_type);
			ObjectMetadata t_data = object_metadata[inst_type];
			JsonData data = new JsonData (JsonType.Object);

			foreach(var pair in t_data.Properties)
			{
				PropertyMetadata prop_data = pair.Value;

				if (prop_data.IsField) 
				{
					FieldInfo f_info = prop_data.Info as FieldInfo;
					try {
						data[pair.Key] = new JsonData (f_info.GetValue (instance));
					} catch {
						data[pair.Key] = FromObject (f_info.GetValue (instance));
					}
				} 
				else 
				{
					PropertyInfo p_info = prop_data.Info as PropertyInfo;
					
					if (p_info.CanRead) {
						try {
							data[pair.Key] = new JsonData (p_info.GetValue (instance, null));
						} catch {
							data[pair.Key] = FromObject (p_info.GetValue (instance, null));
						}
					}
				}
			}

			return data;
		}

		private static object ApplyOnObject(Type inst_type, Object instance, JsonData jsonData)
		{
			AddObjectMetadata (inst_type);
			ObjectMetadata t_data = object_metadata[inst_type];
			
			if(jsonData.IsObject == false)
			{
				throw new JsonException("JsonData is not object type");			
			}
			
			foreach(JsonData.Pair item in jsonData)
			{
				PropertyMetadata prop_data;
				if (t_data.Properties.TryGetValue (item.Key, out prop_data)) 
				{
					if (prop_data.IsField) 
					{
						FieldInfo f_info = prop_data.Info as FieldInfo;
						f_info.SetValue(instance, item.Value.GetValue(f_info.FieldType));
					} 
					else 
					{
						PropertyInfo p_info = prop_data.Info as PropertyInfo;

						if (p_info.CanWrite)
							p_info.SetValue(instance, item.Value.GetValue(p_info.PropertyType), null);
					}
				} 
			}
		 
			return instance;   
		}
		

		private static IJsonWrapper ReadValue (WrapperFactory factory,
											   JsonReader reader)
		{
			reader.Read ();

			if (reader.Token == JsonToken.ArrayEnd ||
				reader.Token == JsonToken.Null)
				return null;

			JsonData instance = (JsonData)factory ();

			if (reader.Token == JsonToken.String) {
				instance.SetString ((string) reader.Value);
				return instance;
			}

			if (reader.Token == JsonToken.Double) {
				instance.SetDouble ((double) reader.Value);
				return instance;
			}

			if (reader.Token == JsonToken.Int) {
				instance.SetInt ((int) reader.Value);
				return instance;
			}

			if (reader.Token == JsonToken.Long) {
				instance.SetLong ((long) reader.Value);
				return instance;
			}

			if (reader.Token == JsonToken.Boolean) {
				instance.SetBoolean ((bool) reader.Value);
				return instance;
			}

			if (reader.Token == JsonToken.ArrayStart) {
				instance.SetJsonType (JsonType.Array);

				while (true) {
					IJsonWrapper item = ReadValue (factory, reader);
					if (item == null && reader.Token == JsonToken.ArrayEnd)
						break;

					instance.Add (item);
				}
			}
			else if (reader.Token == JsonToken.ObjectStart) {
				instance.SetJsonType (JsonType.Object);

				while (true) {
					reader.Read ();

					if (reader.Token == JsonToken.ObjectEnd)
						break;

					string property = (string) reader.Value;

					instance[property] = (JsonData)ReadValue (
						factory, reader);
				}

			}

			return instance;
		}

		private static void RegisterBaseExporters ()
		{
			base_exporters_table[typeof (IJsonExporter)] =
				delegate (object obj, JsonWriter writer) {
					writer.Write (((IJsonExporter) obj).Stringify ());
				};

			base_exporters_table[typeof (byte)] =
				delegate (object obj, JsonWriter writer) {
					writer.Write (Convert.ToInt32 ((byte) obj));
				};

			base_exporters_table[typeof (char)] =
				delegate (object obj, JsonWriter writer) {
					writer.Write (Convert.ToString ((char) obj));
				};

			base_exporters_table[typeof (DateTime)] =
				delegate (object obj, JsonWriter writer) {
					writer.Write (Convert.ToString ((DateTime) obj,
													datetime_format));
				};

			base_exporters_table[typeof (TimeSpan)] =
				delegate (object obj, JsonWriter writer) {
					writer.Write (Convert.ToString ((TimeSpan) obj));
				};
			
			base_exporters_table[typeof (decimal)] =
				delegate (object obj, JsonWriter writer) {
					writer.Write ((decimal) obj);
				};

			base_exporters_table[typeof (sbyte)] =
				delegate (object obj, JsonWriter writer) {
					writer.Write (Convert.ToInt32 ((sbyte) obj));
				};

			base_exporters_table[typeof (short)] =
				delegate (object obj, JsonWriter writer) {
					writer.Write (Convert.ToInt32 ((short) obj));
				};

			base_exporters_table[typeof (ushort)] =
				delegate (object obj, JsonWriter writer) {
					writer.Write (Convert.ToInt32 ((ushort) obj));
				};

			base_exporters_table[typeof (uint)] =
				delegate (object obj, JsonWriter writer) {
					writer.Write (Convert.ToUInt64 ((uint) obj));
				};

			base_exporters_table[typeof (int)] =
				delegate (object obj, JsonWriter writer) {
					writer.Write (Convert.ToInt64 ((int) obj));
				};
			
			base_exporters_table[typeof (ulong)] =
				delegate (object obj, JsonWriter writer) {
					writer.Write ((ulong) obj);
				};
			
			base_exporters_table[typeof (float)] =
				delegate (object obj, JsonWriter writer) {
					writer.Write ((float)obj);
				};
			
		}

		private static void RegisterBaseImporters ()
		{
			ImporterFunc importer;

			importer = delegate (object input) {
				return Convert.ToByte ((int) input);
			};
			RegisterImporter (base_importers_table, typeof (int),
							  typeof (byte), importer);

			importer = delegate (object input) {
				return Convert.ToUInt64 ((int) input);
			};
			RegisterImporter (base_importers_table, typeof (int),
							  typeof (ulong), importer);
			
			importer = delegate (object input) {
				return Convert.ToInt64 ((int) input);
			};
			RegisterImporter (base_importers_table, typeof (int),
							  typeof (long), importer);
						
			importer = delegate (object input) {
				return Convert.ToSByte ((int) input);
			};
			RegisterImporter (base_importers_table, typeof (int),
							  typeof (sbyte), importer);

			importer = delegate (object input) {
				return Convert.ToInt16 ((int) input);
			};
			RegisterImporter (base_importers_table, typeof (int),
							  typeof (short), importer);

			importer = delegate (object input) {
				return Convert.ToUInt16 ((int) input);
			};
			RegisterImporter (base_importers_table, typeof (int),
							  typeof (ushort), importer);

			importer = delegate (object input) {
				return Convert.ToUInt32 ((int) input);
			};
			RegisterImporter (base_importers_table, typeof (int),
							  typeof (uint), importer);

			importer = delegate (object input) {
				return Convert.ToSingle ((int) input);
			};
			RegisterImporter (base_importers_table, typeof (int),
							  typeof (float), importer);

			importer = delegate (object input) {
				return Convert.ToDouble ((int) input);
			};
			RegisterImporter (base_importers_table, typeof (int),
							  typeof (double), importer);

			importer = delegate (object input) {
				return Convert.ToDecimal ((double) input);
			};
			RegisterImporter (base_importers_table, typeof (double),
							  typeof (decimal), importer);


			importer = delegate (object input) {
				return Convert.ToUInt32 ((long) input);
			};
			RegisterImporter (base_importers_table, typeof (long),
							  typeof (uint), importer);

			importer = delegate (object input) {
				return Convert.ToChar ((string) input);
			};
			RegisterImporter (base_importers_table, typeof (string),
							  typeof (char), importer);

			importer = delegate (object input) {
				return string.IsNullOrEmpty ((string) input) ? default (DateTime) : Convert.ToDateTime ((string) input, datetime_format);
			};
			RegisterImporter (base_importers_table, typeof (string),
							  typeof (DateTime), importer);
			
			importer = delegate (object input) {
				TimeSpan result;
				//HACK: http://stackoverflow.com/questions/7429177/work-around-for-timespan-parsing-2400
				string str = (string) input;
				if (str == "24:00:00")
					str = "1.00:00:00";
				if (TimeSpan.TryParse(str, out result) == false)
					return default(TimeSpan);
				return result;
			};
			RegisterImporter (base_importers_table, typeof (string),
							  typeof (TimeSpan), importer);
			
			importer = delegate (object input) {
				return Convert.ToSingle((double) input);
			};
			
			RegisterImporter(base_importers_table, typeof(double), typeof(float), importer);
		}

		private static void RegisterImporter (
			Dictionary<Type, Dictionary<Type, ImporterFunc>> table,
			Type json_type, Type value_type, ImporterFunc importer)
		{
			if (! table.ContainsKey (json_type))
				table.Add (json_type, new Dictionary<Type, ImporterFunc> ());

			table[json_type][value_type] = importer;
		}

		private static void WriteValue (object obj, JsonWriter writer,
										bool writer_is_private,
										int depth)
		{
			if (depth > max_nesting_depth)
				throw new JsonException (
					String.Format ("Max allowed object depth reached while " +
								   "trying to export from type {0}",
								   obj.GetType ()));

			if (obj == null) {
				writer.Write (null);
				return;
			}

			if (obj is IJsonWrapper) {
				if (writer_is_private)
					writer.WriteJSONStr (((IJsonWrapper) obj).ToJson ());
				else
					((IJsonWrapper) obj).ToJson (writer);

				return;
			}

			if (obj is String) {
				writer.Write ((string) obj);
				return;
			}
			
			if (obj is Double) {
				writer.Write ((double) obj);
				return;
			}

			if (obj is Int32) {
				writer.Write ((int) obj);
				return;
			}

			if (obj is Boolean) {
				writer.Write ((bool) obj);
				return;
			}

			if (obj is Int64) {
				writer.Write ((long) obj);
				return;
			}

			if (obj is Array) {
				writer.WriteArrayStart ();

				Array arrayobj = (Array) obj;
				for (int i = 0; i < arrayobj.Length; ++i) {
					WriteValue (arrayobj.GetValue(i), writer, writer_is_private, depth + 1);
				}

				writer.WriteArrayEnd ();

				return;
			}

			if (obj is IList) {
				writer.WriteArrayStart ();
				IList listobj = (IList) obj;
				for (int i = 0; i < listobj.Count; ++i) {
					WriteValue (listobj[i], writer, writer_is_private, depth + 1);
				}
				writer.WriteArrayEnd ();

				return;
			}

			if (obj is IDictionary) {
				writer.WriteObjectStart ();
				foreach (DictionaryEntry entry in (IDictionary) obj) {
					writer.WritePropertyName ((string) entry.Key);
					WriteValue (entry.Value, writer, writer_is_private,
								depth + 1);
				}
				writer.WriteObjectEnd ();

				return;
			}

			Type obj_type = obj.GetType ();

			// See if there's a custom exporter for the object
			ExporterFunc exporter;
			if (custom_exporters_table.TryGetValue (obj_type, out exporter)) {
				exporter (obj, writer);

				return;
			}

			// If not, maybe there's a base exporter
			if (base_exporters_table.TryGetValue (obj_type, out exporter)) {
				exporter (obj, writer);

				return;
			}
			if (typeof (IJsonExporter).IsAssignableFrom (obj_type) && base_exporters_table.TryGetValue (typeof (IJsonExporter), out exporter)) {
				exporter (obj, writer);
				
				return;
			}

			// Last option, let's see if it's an enum
			if (obj is Enum) {
				Type e_type = Enum.GetUnderlyingType (obj_type);

				if (e_type == typeof (long)
					|| e_type == typeof (uint)
					|| e_type == typeof (ulong))
					writer.Write ((ulong) obj);
				else
					writer.Write ((int) obj);

				return;
			}

			// Okay, so it looks like the input should be exported as an
			// object
			AddTypeProperties (obj_type);
			List<PropertyMetadata> props = type_properties[obj_type];

			writer.WriteObjectStart ();
			for (int i = 0; i < props.Count; ++i) {
				if (props[i].IsField) {
					writer.WritePropertyName (props[i].Info.Name);
					WriteValue (((FieldInfo) props[i].Info).GetValue (obj),
								writer, writer_is_private, depth + 1);
				}
				else {
					PropertyInfo p_info = (PropertyInfo) props[i].Info;

					if (p_info.CanRead) {
						writer.WritePropertyName (props[i].Info.Name);
						WriteValue (p_info.GetValue (obj, null),
									writer, writer_is_private, depth + 1);
					}
				}
			}
			writer.WriteObjectEnd ();
		}
		#endregion


		public static string ToJson (object obj, bool pretty_print = false)
		{
			lock (static_writer_lock) {
				static_writer.Reset ();
				static_writer.PrettyPrint = pretty_print;

				WriteValue (obj, static_writer, true, 0);

				return static_writer.ToString ();
			}
		}

		public static void ToJson (object obj, JsonWriter writer)
		{
			WriteValue (obj, writer, false, 0);
		}

		public static JsonData ToObject (JsonReader reader)
		{
			return (JsonData) ToWrapper (
				delegate { return new JsonData (); }, reader);
		}

		public static JsonData ToObject (TextReader reader)
		{
			JsonReader json_reader = new JsonReader (reader);

			return (JsonData) ToWrapper (
				delegate { return new JsonData (); }, json_reader);
		}

		public static JsonData ToObject (string json)
		{
			return (JsonData) ToWrapper (
				delegate { return new JsonData (); }, json);
		}

		public static T ToObject<T> (JsonReader reader)
		{
			return (T) ReadValue (typeof (T), reader);
		}

		public static T ToObject<T> (TextReader reader)
		{
			JsonReader json_reader = new JsonReader (reader);

			return (T) ReadValue (typeof (T), json_reader);
		}

		public static T ToObject<T> (string json)
		{
			JsonReader reader = new JsonReader (json);

			return (T) ReadValue (typeof (T), reader);
		}
		
		public static T ApplyOnObject<T>(T instance, string json)
		{
			JsonReader reader = new JsonReader (json);
			
			reader.Read ();
			if (reader.Token != JsonToken.ObjectStart) 
				throw new JsonException( "Invalid input, input json is not a object");
			
		  	return (T) ApplyOnObject (typeof (T), instance, reader);
		}
		
		public static JsonData FromObject (object instance)
		{
			if (instance == null)
			{
				return new JsonData (JsonType.None);
			}
			else
			if (instance is JsonData)
			{
				return (JsonData)instance;
			}
			else
			if (instance is IList)
			{
				JsonData data = new JsonData (JsonType.Array);
				IList list = (IList)instance;
				
				for (int idx = 0, count = list.Count; idx < count; ++idx) {
					try {
						data.Add (list[idx]);
					} catch {
						data.Add (FromObject (list[idx]));
					}
				}
				
				return data;
			}
			else
			{
				return FromObject (instance.GetType(), instance);
			}
		}

		public static object ApplyOnObject(object instance, JsonData jsonData)
		{
		  	return ApplyOnObject (instance.GetType(), instance, jsonData);
		}

		public static IJsonWrapper ToWrapper (WrapperFactory factory,
											  JsonReader reader)
		{
			return ReadValue (factory, reader);
		}

		public static IJsonWrapper ToWrapper (WrapperFactory factory,
											  string json)
		{
			JsonReader reader = new JsonReader (json);

			return ReadValue (factory, reader);
		}

		public static void RegisterExporter<T> (ExporterFunc<T> exporter)
		{
			ExporterFunc exporter_wrapper =
				delegate (object obj, JsonWriter writer) {
					exporter ((T) obj, writer);
				};

			custom_exporters_table[typeof (T)] = exporter_wrapper;
		}

		public static void RegisterImporter<TJson, TValue> (
			ImporterFunc<TJson, TValue> importer)
		{
			ImporterFunc importer_wrapper =
				delegate (object input) {
					return importer ((TJson) input);
				};

			RegisterImporter (custom_importers_table, typeof (TJson),
							  typeof (TValue), importer_wrapper);
		}

		public static void UnregisterExporters ()
		{
			custom_exporters_table.Clear ();
		}

		public static void UnregisterImporters ()
		{
			custom_importers_table.Clear ();
		}
	}
}
