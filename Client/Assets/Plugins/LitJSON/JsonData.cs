#region Header
/**
 * JsonData.cs
 *   Generic type to hold JSON data (objects, arrays, and so on). This is
 *   the default type returned by JsonMapper.ToObject().
 *
 * The authors disclaim copyright to this source code. For more details, see
 * the COPYING file included with this distribution.
 **/
#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using KeyNotFoundException = System.Collections.Generic.KeyNotFoundException;


namespace LitJson
{
    public class JsonData : IJsonWrapper, IEquatable<JsonData>, IEnumerable<JsonData.Pair>
    {
        [Serializable]
        public struct Pair
        {
            public readonly string   Key;
            public readonly JsonData Value;
            
            public Pair(string key, JsonData value)
            {
                Key   = key;
                Value = value;
            }
            
            public static implicit operator KeyValuePair<string, JsonData> (Pair pair)
            {
                return new KeyValuePair<string, JsonData> (pair.Key, pair.Value);
            }
            
            public static implicit operator Pair (KeyValuePair<string, JsonData> pair)
            {
                return new Pair (pair.Key, pair.Value);
            }
            
            public override string ToString ()
            {
                return string.Format("[{0}, {1}]", Key, Value);
            }
        }

        public List<string> TryGet(string rEWARD_CLAIMED, object p)
        {
            throw new NotImplementedException();
        }

        #region Fields
        private List<JsonData>               inst_array;
        private bool                         inst_boolean;
        private double                       inst_double;
        private int                          inst_int;
        private long                         inst_long;
        private Dictionary<string, JsonData> inst_object;
        private string                       inst_string;
        private string                       json;
        private JsonType                     type;

        // Used to implement the IOrderedDictionary interface
        private List<Pair>                   object_list;
        #endregion


        #region Properties
        public int Count {
            get { return EnsureCollection ().Count; }
        }
        public List<string> Keys
        {
            get
            {
                List<string> keys = new List<string>();
                EnsureDictionary();
                foreach (var item in inst_object.Keys)
                {
                    keys.Add(item.ToString());
                }
                return keys;
            }
        }
        public bool IsArray {
            get { return type == JsonType.Array; }
        }

        public bool IsBoolean {
            get { return type == JsonType.Boolean; }
        }

        public bool IsDouble {
            get { return type == JsonType.Double; }
        }

        public bool IsInt {
            get { return type == JsonType.Int; }
        }

        public bool IsLong {
            get { return type == JsonType.Long; }
        }

        public bool IsObject {
            get { return type == JsonType.Object; }
        }

        public bool IsString {
            get { return type == JsonType.String; }
        }
        #endregion


        #region IJsonWrapper Properties
        bool IJsonWrapper.IsArray {
            get { return IsArray; }
        }

        bool IJsonWrapper.IsBoolean {
            get { return IsBoolean; }
        }

        bool IJsonWrapper.IsDouble {
            get { return IsDouble; }
        }

        bool IJsonWrapper.IsInt {
            get { return IsInt; }
        }

        bool IJsonWrapper.IsLong {
            get { return IsLong; }
        }

        bool IJsonWrapper.IsObject {
            get { return IsObject; }
        }

        bool IJsonWrapper.IsString {
            get { return IsString; }
        }
        #endregion


        #region Public Indexers
        public bool HasValue (string prop_name)
        {
            return EnsureDictionary ().ContainsKey (prop_name);
        }

        public bool Contains (object value)
        {
            if (IsArray == false)
                return false;

			for (int idx = 0, count = Count; idx < count; ++idx)
			{
				JsonData other = this[idx];
				if (other == null && value != null)
					return false;
				if (other.EqualsData (value) == true)
					return true;
			}
			return false;
		}
		
        public JsonData this[string prop_name] {
            get {
                EnsureDictionary ();
                try {
                	return inst_object[prop_name];
                }
                catch (KeyNotFoundException) {
					// AX: unfortunately, constructor with the inner exception, doesn't work how we need...
					//throw new KeyNotFoundException(string.Format("The given key '{0}' was not present in the dictionary.", prop_name), e);                 				
					throw new KeyNotFoundException(string.Format("The given key '{0}' was not present in the dictionary.", prop_name));                 
                }
            }

            set {
                EnsureDictionary ();

                Pair entry = new Pair (prop_name, value);

                if (inst_object.ContainsKey (prop_name)) {
                    for (int i = 0; i < object_list.Count; i++) {
                        if (object_list[i].Key == prop_name) {
                            object_list[i] = entry;
                            break;
                        }
                    }
                } else
                    object_list.Add (entry);

                inst_object[prop_name] = value;

                json = null;
            }
        }

        public JsonData this[int index] {
            get {
                EnsureCollection ();

                if (type == JsonType.Array)
                    return inst_array[index];

                return object_list[index].Value;
            }

            set {
                EnsureCollection ();

                if (type == JsonType.Array)
                    inst_array[index] = value;
                else {
                    Pair entry = object_list[index];
                    Pair new_entry = new Pair (entry.Key, value);

                    object_list[index] = new_entry;
                    inst_object[entry.Key] = value;
                }

                json = null;
            }
        }
        #endregion


        #region Constructors
        public JsonData ()
        {
        }

		public JsonData (JsonData other)
		{
			inst_boolean = other.inst_boolean;
			inst_double  = other.inst_double;
			inst_int     = other.inst_int;
			inst_long    = other.inst_long;
            inst_string  = other.inst_string;
            json         = other.json;
            type         = other.type;
            
            if(other.inst_array  != null) inst_array   = new List<JsonData>(other.inst_array);
            if(other.inst_object != null) inst_object  = new Dictionary<string, JsonData>(other.inst_object);
            if(other.object_list != null) object_list  = new List<Pair>(other.object_list);
        }

        public JsonData (bool boolean)
        {
            type = JsonType.Boolean;
            inst_boolean = boolean;
        }

        public JsonData (float number)
        {
            type = JsonType.Double;
            inst_double = (double)number;
        }

        public JsonData (double number)
        {
            type = JsonType.Double;
            inst_double = number;
        }

        public JsonData (int number)
        {
            type = JsonType.Int;
            inst_int = number;
        }

        public JsonData (long number)
        {
            type = JsonType.Long;
            inst_long = number;
        }

        public JsonData (UnityEngine.Vector2 vector)
        {
            this["x"] = new JsonData (vector.x);
            this["y"] = new JsonData (vector.y);
        }
        
        public JsonData (UnityEngine.Vector3 vector)
        {
            this["x"] = new JsonData (vector.x);
            this["y"] = new JsonData (vector.y);
            this["z"] = new JsonData (vector.z);
        }
        
        public JsonData (UnityEngine.Color color)
        {
            this["r"] = new JsonData (color.r);
            this["g"] = new JsonData (color.g);
            this["b"] = new JsonData (color.b);
            this["a"] = new JsonData (color.a);
        }
        
        public JsonData (UnityEngine.Rect rect)
        {
            this["x"] = new JsonData (rect.x);
            this["y"] = new JsonData (rect.y);
            this["w"] = new JsonData (rect.width);
            this["h"] = new JsonData (rect.height);
        }
        
        public JsonData (JsonData[] list)
        {
            type = JsonType.Array;
            inst_array = new List<JsonData> (list.Length);
            for (int idx = 0, count = list.Length; idx < count; ++idx) {
                inst_array.Add (list[idx]);
            }
        }

        public JsonData (object obj)
        {
            if (obj is Boolean) {
                type = JsonType.Boolean;
                inst_boolean = (bool) obj;
                return;
            }

            if (obj is float) {
                type = JsonType.Double;
                inst_double = (double)(float) obj;
                return;
            }

            if (obj is Double) {
                type = JsonType.Double;
                inst_double = (double) obj;
                return;
            }

            if (obj is Int32) {
                type = JsonType.Int;
                inst_int = (int) obj;
                return;
            }

            if (obj is Int64) {
                type = JsonType.Long;
                inst_long = (long) obj;
                return;
            }

            if (obj is String) {
                type = JsonType.String;
                inst_string = (string) obj;
                return;
            }

            if (obj is Enum) {
                type = JsonType.String;
                inst_string = obj.ToString();
                return;
            }
			
			if (obj is DateTime) {
				type = JsonType.String;
                inst_string = Convert.ToString ((DateTime) obj, JsonMapper.datetime_format);
                return;
            }
            
            if (obj is UnityEngine.Vector2) {
                UnityEngine.Vector2 vector = (UnityEngine.Vector2) obj;
                this["x"] = new JsonData (vector.x);
                this["y"] = new JsonData (vector.y);
                return;
            }

            if (obj is UnityEngine.Vector3) {
                UnityEngine.Vector3 vector = (UnityEngine.Vector3) obj;
                this["x"] = new JsonData (vector.x);
                this["y"] = new JsonData (vector.y);
                this["z"] = new JsonData (vector.z);
                return;
            }
            
            if (obj is UnityEngine.Color) {
                UnityEngine.Color color = (UnityEngine.Color) obj;
                this["r"] = new JsonData (color.r);
                this["g"] = new JsonData (color.g);
                this["b"] = new JsonData (color.b);
                this["a"] = new JsonData (color.a);
                return;
            }
            
            if (obj is UnityEngine.Rect) {
                UnityEngine.Rect rect = (UnityEngine.Rect) obj;
                this["x"] = new JsonData (rect.x);
                this["y"] = new JsonData (rect.y);
                this["w"] = new JsonData (rect.width);
                this["h"] = new JsonData (rect.height);
                return;
            }

            if (obj is Array) {
                type = JsonType.Array;
                Array arr = (Array) obj;
                inst_array = new List<JsonData> (arr.Length);
                for (int idx = 0, count = arr.Length; idx < count; ++idx) {
                    inst_array.Add (new JsonData (arr.GetValue (idx)));
                }
                return;
            }
            
            if (obj is IList) {
                type = JsonType.Array;
                IList list = (IList) obj;
                inst_array = new List<JsonData> (list.Count);
                for (int idx = 0, count = list.Count; idx < count; ++idx) {
                    inst_array.Add (new JsonData (list[idx]));
                }
                return;
            }

            if (obj != null && typeof (IJsonExporter).IsAssignableFrom (obj.GetType ())) {
                type = JsonType.String;
				inst_string = ((IJsonExporter) obj).Stringify();
				return;
			}

            throw new ArgumentException (
                "Unable to wrap the given object with JsonData: " + obj);
        }

        public JsonData (string str)
        {
            type = JsonType.String;
            inst_string = str;
        }

        public JsonData (JsonType type)
        {
			SetJsonType(type);
        }
        #endregion
        
        #region Helper functions        
        static public JsonData FromObject (object obj)
        {
			return JsonMapper.FromObject (obj);
        }
        
		public object ApplyOnObject (object obj)
		{
			EnsureDictionary ();
			return JsonMapper.ApplyOnObject (obj, this);
		}
        
        #endregion

        #region Implicit Conversions
        public static implicit operator JsonData (Boolean data)
        {
            return new JsonData (data);
        }

		public static implicit operator JsonData (float data)
		{
			return new JsonData ((Double) data);
		}

        public static implicit operator JsonData (Double data)
        {
            return new JsonData (data);
        }

        public static implicit operator JsonData (Int32 data)
        {
            return new JsonData (data);
        }

		public static implicit operator JsonData (UInt32 data)
		{
			return new JsonData ((Int32) data);
		}

        public static implicit operator JsonData (Int64 data)
        {
            return new JsonData (data);
        }

		public static implicit operator JsonData (UInt64 data)
		{
			return new JsonData ((Int64) data);
        }

        public static implicit operator JsonData (String data)
        {
            return new JsonData (data);
        }

		public static implicit operator JsonData (UnityEngine.Vector2 data)
		{
			return new JsonData (data);
		}
		
		public static implicit operator JsonData (UnityEngine.Vector3 data)
		{
			return new JsonData (data);
		}
		
		public static implicit operator JsonData (UnityEngine.Color data)
		{
			return new JsonData (data);
		}
		
		public static implicit operator JsonData (UnityEngine.Rect data)
		{
			return new JsonData (data);
		}

		public static implicit operator JsonData (JsonData[] data)
		{
			return new JsonData (data);
		}
		#endregion


        #region Explicit Conversions
        public static explicit operator Boolean (JsonData data)
        {
            if (data.type != JsonType.Boolean)
                throw new InvalidCastException (
                    "Instance of JsonData doesn't hold a boolean");

            return data.inst_boolean;
        }

        public static explicit operator Double (JsonData data)
        {
            if (data.type != JsonType.Double)
                throw new InvalidCastException (
                    "Instance of JsonData doesn't hold a double");

            return data.inst_double;
        }

		public static explicit operator float (JsonData data)
		{
			if (data.type != JsonType.Double)
				throw new InvalidCastException (
					"Instance of JsonData doesn't hold a float");
			
			return (float) data.inst_double;
		}

        public static explicit operator Int32 (JsonData data)
        {
            if (data.type != JsonType.Int)
                throw new InvalidCastException (
                    "Instance of JsonData doesn't hold an int");

            return data.inst_int;
        }

		public static explicit operator UInt32 (JsonData data)
		{
			if (data.type != JsonType.Int)
				throw new InvalidCastException (
					"Instance of JsonData doesn't hold an int");
			
			return (UInt32) data.inst_int;
		}

        public static explicit operator Int64 (JsonData data)
        {
			if (data.type == JsonType.Int)
				return (Int64) data.inst_int;
            if (data.type != JsonType.Long)
                throw new InvalidCastException (
                    "Instance of JsonData doesn't hold an long");

            return data.inst_long;
        }

		public static explicit operator UInt64 (JsonData data)
		{
			if (data.type == JsonType.Int)
				return (UInt64) data.inst_int;
			if (data.type != JsonType.Long)
				throw new InvalidCastException (
					"Instance of JsonData doesn't hold an long");
			
			return (UInt64) data.inst_long;
        }

        public static explicit operator String (JsonData data)
        {
            if (data.type != JsonType.String)
                throw new InvalidCastException (
                    "Instance of JsonData doesn't hold a string " + data.type);

            return data.inst_string;
        }
		
		public static explicit operator UnityEngine.Vector2 (JsonData data)
		{
			if (data.type != JsonType.Object)
				throw new InvalidCastException (
					"Instance of JsonData doesn't hold a Vector2 " + data.type);
			if (data.inst_object.Count != 2)
				throw new InvalidCastException (
					"Instance of JsonData doesn't hold a Vector2 " + data.type);

			return new UnityEngine.Vector2((float)data["x"], (float)data["y"]);
		}
		
		public static explicit operator UnityEngine.Vector3 (JsonData data)
		{
			if (data.type != JsonType.Object)
				throw new InvalidCastException (
					"Instance of JsonData doesn't hold a Vector3 " + data.type);
			if (data.inst_object.Count != 3)
				throw new InvalidCastException (
					"Instance of JsonData doesn't hold a Vector3 " + data.type);
			
			return new UnityEngine.Vector3((float)data["x"], (float)data["y"], (float)data["z"]);
		}
		
		public static explicit operator UnityEngine.Color (JsonData data)
		{
			if (data.type != JsonType.Object)
				throw new InvalidCastException (
					"Instance of JsonData doesn't hold a Color " + data.type);
			if (data.inst_object.Count != 4)
				throw new InvalidCastException (
					"Instance of JsonData doesn't hold a Color " + data.type);
			
			return new UnityEngine.Color((float)data["r"], (float)data["g"], (float)data["b"], (float)data["a"]);
		}
		
		public static explicit operator UnityEngine.Rect (JsonData data)
		{
			if (data.type != JsonType.Object)
				throw new InvalidCastException (
					"Instance of JsonData doesn't hold a Rect " + data.type);
			if (data.inst_object.Count != 4)
				throw new InvalidCastException (
					"Instance of JsonData doesn't hold a Rect " + data.type);
            
            return new UnityEngine.Rect((float)data["x"], (float)data["y"], (float)data["w"], (float)data["h"]);
        }
        #endregion


        #region IEnumerable Methods
        IEnumerator<Pair> IEnumerable<Pair>.GetEnumerator ()
        {
            EnsureDictionary ();

            return object_list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator ()
        {
            EnsureDictionary ();

            return object_list.GetEnumerator();
        }
        #endregion


        #region IJsonWrapper Methods
        bool IJsonWrapper.GetBoolean ()
        {
            if (type != JsonType.Boolean)
                throw new InvalidOperationException (
                    "JsonData instance doesn't hold a boolean");

            return inst_boolean;
        }

        double IJsonWrapper.GetDouble ()
        {
            if (type != JsonType.Double)
                throw new InvalidOperationException (
                    "JsonData instance doesn't hold a double");

            return inst_double;
        }

        int IJsonWrapper.GetInt ()
        {
            if (type != JsonType.Int)
                throw new InvalidOperationException (
                    "JsonData instance doesn't hold an int");

            return inst_int;
        }

        long IJsonWrapper.GetLong ()
        {
			if( type == JsonType.Int )
			{
				return (long) inst_int;
			}
			
            if (type != JsonType.Long)
                throw new InvalidOperationException (
                    "JsonData instance doesn't hold a long");

            return inst_long;
        }

        string IJsonWrapper.GetString ()
        {
            if (type != JsonType.String)
                throw new InvalidOperationException (
                    "JsonData instance doesn't hold a string");

            return inst_string;
        }

        public void SetBoolean (bool val)
        {
            type = JsonType.Boolean;
            inst_boolean = val;
            json = null;
        }

        public void SetDouble (double val)
        {
            type = JsonType.Double;
            inst_double = val;
            json = null;
        }

        public void SetInt (int val)
        {
            type = JsonType.Int;
            inst_int = val;
            json = null;
        }

        public void SetLong (long val)
        {
            type = JsonType.Long;
            inst_long = val;
            json = null;
        }

        public void SetString (string val)
        {
            type = JsonType.String;
            inst_string = val;
            json = null;
        }

        string IJsonWrapper.ToJson ()
        {
            return ToJson ();
        }

        void IJsonWrapper.ToJson (JsonWriter writer)
        {
            ToJson (writer);
        }
        #endregion


        #region Private Methods
        private ICollection EnsureCollection ()
        {
            if (type == JsonType.Array)
                return (ICollection) inst_array;

            if (type == JsonType.Object)
                return (ICollection) inst_object;

            throw new InvalidOperationException (
                "The JsonData instance has to be initialized first");
        }

        private Dictionary<string, JsonData> EnsureDictionary ()
        {
            if (type == JsonType.Object)
                return inst_object;

            if (type != JsonType.None)
                throw new InvalidOperationException (
                    "Instance of JsonData is not a dictionary");

            type = JsonType.Object;
            inst_object = new Dictionary<string, JsonData> ();
            object_list = new List<Pair> ();

            return inst_object;
        }

        private List<JsonData> EnsureList ()
        {
            if (type == JsonType.Array)
                return inst_array;

            if (type != JsonType.None)
                throw new InvalidOperationException (
                    "Instance of JsonData is not a list");

            type = JsonType.Array;
            inst_array = new List<JsonData> ();

            return inst_array;
        }

        private JsonData ToJsonData (object obj)
        {
            if (obj == null)
                return null;

            if (obj is JsonData)
                return (JsonData) obj;

            return new JsonData (obj);
        }

        private static void WriteJson (JsonData obj, JsonWriter writer)
        {
            if (obj == null) {
                writer.Write (null);
                return;
            }

            switch (obj.GetJsonType()) {
            case JsonType.String: {
                writer.Write (((IJsonWrapper)obj).GetString ());
                return;
            }
            case JsonType.Boolean: {
                writer.Write (((IJsonWrapper)obj).GetBoolean ());
                return;
            }
            case JsonType.Double: {
                writer.Write (((IJsonWrapper)obj).GetDouble ());
                return;
            }
            case JsonType.Int: {
                writer.Write (((IJsonWrapper)obj).GetInt ());
                return;
            }
            case JsonType.Long: {
                writer.Write (((IJsonWrapper)obj).GetLong ());
                return;
            }
            case JsonType.Array: {
                writer.WriteArrayStart ();
                List<JsonData> list = obj.EnsureList ();
                int count = list.Count;
                for (int i = 0; i < count; ++i) {
                    WriteJson (list[i], writer);
                }
                writer.WriteArrayEnd ();

                return;
            }
            case JsonType.Object: {
                writer.WriteObjectStart ();
                Dictionary<string, JsonData> dict = obj.EnsureDictionary ();
                foreach (KeyValuePair<string, JsonData> entry in dict) {
                    writer.WritePropertyName (entry.Key);
                    WriteJson (entry.Value, writer);
                }
                writer.WriteObjectEnd ();

                return;
            }
            default: return;
            }
        }
        #endregion


        public int Add (object value)
        {
            List<JsonData> list = EnsureList ();

            json = null;

            int count = list.Count;
            
            list.Add (ToJsonData (value));
            
            return count;
        }

        public void Clear ()
        {
            if (IsObject) {
                inst_object.Clear ();
                object_list.Clear ();
                return;
            }

            if (IsArray) {
                inst_array.Clear ();
                return;
            }
        }

        public bool Equals (JsonData x)
        {
            if (x == null)
				return this.type == JsonType.None;

            if (x.type != this.type)
                return false;

			int count;

            switch (this.type) {
            case JsonType.None:
                return true;

            case JsonType.Object:
                if (this.inst_object.Equals (x.inst_object) == true)
					return true;
				count = this.object_list.Count;
				if (count != x.object_list.Count)
					return false;
				for (int idx = 0; idx < count; ++idx)
				{
					var pair = this.object_list[idx];
					JsonData data;
					if (x.inst_object.TryGetValue(pair.Key, out data) == false)
						return false;
					if (pair.Value != null) {
						if (pair.Value.Equals (data) == false)
							return false;
					} else if (data != null) {
						if (data.type != JsonType.None)
							return false;
					}
				}
				return true;

            case JsonType.Array:
				if (this.inst_array.Equals (x.inst_array) == true)
					return true;
				count = this.inst_array.Count;
				if (count != x.inst_array.Count)
					return false;
				for (int idx = 0; idx < count; ++idx)
				{
					JsonData data1 = this.inst_array[idx];
					JsonData data2 = x.inst_array[idx];
					if (data1 != null) {
						if (data1.Equals (data2) == false)
							return false;
					} else if (data2 != null) {
						if (data2.type != JsonType.None)
							return false;
					}
				}
				return true;

            case JsonType.String:
                return this.inst_string.Equals (x.inst_string);

            case JsonType.Int:
                return this.inst_int.Equals (x.inst_int);

            case JsonType.Long:
                return this.inst_long.Equals (x.inst_long);

            case JsonType.Double:
                return this.inst_double.Equals (x.inst_double);

            case JsonType.Boolean:
                return this.inst_boolean.Equals (x.inst_boolean);
            }

            return false;
        }

		private bool EqualsData (object x)
		{
			JsonData data = x as JsonData;
			if (data != null)
				return this.Equals (data);

			switch (this.type) {
			case JsonType.None:
				return x == null;
				
			case JsonType.Object:
				return this.inst_object.Equals (x);
				
			case JsonType.Array:
				return this.inst_array.Equals (x);
				
			case JsonType.String:
				return this.inst_string.Equals (x);
				
			case JsonType.Int:
				return this.inst_int.Equals (x);
				
			case JsonType.Long:
				return this.inst_long.Equals (x);
				
			case JsonType.Double:
				return this.inst_double.Equals (x);
				
			case JsonType.Boolean:
				return this.inst_boolean.Equals (x);
			}
			
			return false;
		}

		public JsonType GetJsonType ()
        {
            return type;
        }

        public void SetJsonType (JsonType type)
        {
            if (this.type == type)
                return;

            switch (type) {
            case JsonType.None:
                break;

            case JsonType.Object:
                inst_object = new Dictionary<string, JsonData> ();
                object_list = new List<Pair> ();
                break;

            case JsonType.Array:
                inst_array = new List<JsonData> ();
                break;

            case JsonType.String:
                inst_string = default (String);
                break;

            case JsonType.Int:
                inst_int = default (Int32);
                break;

            case JsonType.Long:
                inst_long = default (Int64);
                break;

            case JsonType.Double:
                inst_double = default (Double);
                break;

            case JsonType.Boolean:
                inst_boolean = default (Boolean);
                break;
            }

            this.type = type;
        }

        public string ToJson (bool pretty_print = false, bool regenerate = false)
        {
            if (json != null && regenerate == false)
                return json;

            StringWriter sw = new StringWriter ();
            JsonWriter writer = new JsonWriter (sw);
			writer.PrettyPrint = pretty_print;
            writer.Validate = false;

            WriteJson (this, writer);
            json = sw.ToString ();

            return json;
        }

        public string ToJsonSpecial(bool pretty_print = false, bool regenerate = false)
        {
            if (json != null && regenerate == false)
                return json;

            StringWriter sw = new StringWriter();
            JsonWriter writer = new JsonWriter(sw);
            writer.PrettyPrint = pretty_print;
            writer.Validate = false;

            WriteJson(this, writer);
            json = sw.ToString();

            return json.Replace("\"", "");
        }

        public void ToJson (JsonWriter writer)
        {
            bool old_validate = writer.Validate;

            writer.Validate = false;

            WriteJson (this, writer);

            writer.Validate = old_validate;
        }

        public override string ToString ()
        {
            switch (type) {
            case JsonType.Array:
                return string.Format ("JsonData array[{0}]", inst_array.Count);

            case JsonType.Boolean:
                return inst_boolean.ToString ();

            case JsonType.Double:
                return inst_double.ToString ();

            case JsonType.Int:
                return inst_int.ToString ();

            case JsonType.Long:
                return inst_long.ToString ();

            case JsonType.Object:
                return string.Format ("JsonData object[{0}]", inst_object.Count);

            case JsonType.String:
                return inst_string;
            }

            return "Uninitialized JsonData";
        }

        public T GetValue<T> ()
		{
			return (T) GetValue (typeof (T));
		}
		
		public T TryGet<T>(string prop_name)	
		{
			return TryGet(prop_name, default(T));
		}	

		public T TryGet<T>(string prop_name, T default_value)	
		{
            try 
            {
                if(HasValue(prop_name) == false)
                    return default_value;
            
                JsonData data = this[prop_name];
                T value = (T)data.GetValue(typeof(T));
                return value != null ? value : default_value;
            } 
            catch 
            {
                return default_value;
            }
        }    

		public JsonData TryGet(string prop_name, JsonType default_type)	
        {
        	try 
        	{
	        	if(HasValue(prop_name) == false)
					return new JsonData (default_type);
	
				JsonData data = this[prop_name];
				JsonType type = data.GetJsonType ();

				if(type == default_type) 
					return data;
				if(default_type == JsonType.Long && type == JsonType.Int)
					return data;

	        	return new JsonData (default_type);
			}
			catch
			{
				return new JsonData (default_type);
			}
        }	
		
        public object GetValue (System.Type valueType)
        {
            switch (type) {
            case JsonType.Array:
				return ExtractList (valueType);

            case JsonType.Boolean:
                if (valueType == typeof (bool))
					return inst_boolean;
				else
				if (valueType == typeof (string))
					return inst_boolean.ToString();
				else
					throw new System.InvalidCastException( string.Format("Cannot cast {0} -> {1}", type, valueType));

            case JsonType.Double:
                if (valueType == typeof (double))
	                return inst_double;
				else
                if (valueType == typeof (float))
	                return (float)inst_double;
				else
				if (valueType == typeof (string))
					return inst_double.ToString();
				else
					throw new System.InvalidCastException( string.Format("Cannot cast {0} -> {1}", type, valueType));

            case JsonType.Int:
                if (valueType == typeof (int))
	                return inst_int;
				else
				if (valueType == typeof (long))
					return (long)inst_int;
				else
				if (valueType == typeof (float))
					return (float)inst_int;
				else
				if (valueType.IsSubclassOf (typeof (Enum)))
					return Enum.ToObject(valueType, inst_int);
				else
				if (valueType == typeof (string))
					return inst_int.ToString();
				else
					throw new System.InvalidCastException( string.Format("Cannot cast {0} -> {1}", type, valueType));

            case JsonType.Long:
                if (valueType == typeof (long))
	                return inst_long;
				else
                if (valueType.IsSubclassOf (typeof (Enum)))
					return Enum.ToObject(valueType, inst_long);
				else
				if (valueType == typeof (string))
					return inst_long.ToString();
				else
					throw new System.InvalidCastException( string.Format("Cannot cast {0} -> {1}", type, valueType));

            case JsonType.Object:
                return ExtractObject (valueType);

            case JsonType.String:
                if (valueType == typeof (string))
	                return inst_string;
				else
				if (valueType == typeof (bool))
					return string.IsNullOrEmpty (inst_string) ? false : bool.Parse (inst_string);
				else
				if (valueType == typeof (int))
					return string.IsNullOrEmpty (inst_string) ? 0 : int.Parse (inst_string);
				else
				if (valueType == typeof (long))
					return string.IsNullOrEmpty (inst_string) ? 0 : long.Parse (inst_string);
				else
                if (valueType.IsSubclassOf (typeof (Enum)))
					return Enum.Parse (valueType, inst_string);
					/*return string.IsNullOrEmpty (inst_string) == false
							? Enum.Parse (valueType, inst_string)
							: {
								string[] values = Enum.GetValues (valueType); 
								return null;
							};*/
				else
					return JsonMapper.ExtractValue (valueType, inst_string ?? string.Empty);
			}

            return null;
        }

		private object ExtractList (System.Type valueType)
		{
			EnsureList ();
			
			if (typeof (IList).IsAssignableFrom (valueType) == false)
				throw new System.InvalidCastException ();
			
			if (valueType.IsArray == true)
			{
				Type argumentType = valueType.GetElementType ();
				if (argumentType == null)
					throw new System.InvalidCastException ();
				
				Array array = Array.CreateInstance (argumentType, inst_array.Count);
				for (int i = 0; i < inst_array.Count; ++i) {
					JsonData item = inst_array[i];
					if (item != null) {
						array.SetValue (inst_array[i].GetValue (argumentType), i);
					} else {
						array.SetValue (argumentType.IsValueType ? System.Activator.CreateInstance (argumentType) : null, i);
					}
				}
				
				return array;
			}
			else
			{
				var arguments = valueType.GetGenericArguments();
				if (arguments == null || arguments.Length != 1)
					throw new System.InvalidCastException ();
				
				Type argumentType = arguments[0];
				if (argumentType == null)
					throw new System.InvalidCastException ();
				
				IList list = (IList) System.Activator.CreateInstance (valueType, inst_array.Count);
				for (int i = 0; i < inst_array.Count; ++i) {
					JsonData item = inst_array[i];
					if (item != null) {
						list.Add (item.GetValue (argumentType));
					} else {
						list.Add (valueType.IsValueType ? System.Activator.CreateInstance (valueType) : null);
					}
				}
				
				return list;
			}
		}

		private object ExtractObject (System.Type valueType)
		{
			EnsureDictionary ();
			
			object obj = System.Activator.CreateInstance (valueType);
			
			for (int i = 0; i < object_list.Count; ++i) {
				var entry = object_list[i];
				
				if( obj is IDictionary )
				{
					IDictionary	dict			= obj as IDictionary;
					Type[]		arguments		= dict.GetType().GetGenericArguments();
					Type		dictKeyType		= arguments[0];
					Type		dictValueType	= arguments[1];
					
					if( true == entry.Key.GetType().Equals( dictKeyType ) )
					{
						dict[ entry.Key ] = entry.Value.GetValue( dictValueType );
					}
				}
				else
				{
					//TODO: this needs to be pre-cached !!!!
					System.Reflection.MemberInfo[] members = valueType.GetMember(entry.Key);
					if (members == null || members.Length != 1)
						continue;
					
					System.Reflection.MemberInfo member = members[0];
					if (member is System.Reflection.FieldInfo)
					{
						var field = (System.Reflection.FieldInfo) member;
						field.SetValue (obj, entry.Value != null ? entry.Value.GetValue (field.FieldType) : null);
					}
					else if (member is System.Reflection.PropertyInfo)
					{
						var property = (System.Reflection.PropertyInfo) member;
						if (property.GetSetMethod() != null)
							property.SetValue (obj, entry.Value != null ? entry.Value.GetValue (property.PropertyType) : null, null);
					}
				}
			}
			
			return obj;
		}

	}

}
