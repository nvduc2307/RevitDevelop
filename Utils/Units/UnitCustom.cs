namespace Utils.Units
{
    public class UnitCustom
    {
        public UnitEnum Type { get; }
        public string Name { get; }
        public UnitCustom(UnitEnum type)
        {
            Type = type;
            Name = GetName(type);
        }
        public UnitCustom(UnitEnum type, string name)
        {
            Type = type;
            Name = $"{name}({GetName(type)})";
        }
        private string GetName(UnitEnum type)
        {
            var value = UnitEnum.mm.ToString();
            switch (type)
            {
                case UnitEnum.mm:
                    value = UnitEnum.mm.ToString();
                    break;
                case UnitEnum.cm:
                    value = UnitEnum.cm.ToString();
                    break;
                case UnitEnum.m:
                    value = UnitEnum.m.ToString();
                    break;
                case UnitEnum.ft:
                    value = UnitEnum.ft.ToString();
                    break;
            }
            return value;
        }
        public static double GetRatio(UnitEnum typeSelected, UnitEnum currentType)
        {
            var result = 1.0;
            switch (typeSelected)
            {
                case UnitEnum.mm:
                    switch (currentType)
                    {
                        case UnitEnum.mm:
                            result = 1;
                            break;
                        case UnitEnum.cm:
                            result = 10;
                            break;
                        case UnitEnum.m:
                            result = 1000;
                            break;
                        case UnitEnum.ft:
                            result = 1;
                            break;
                    }
                    break;
                case UnitEnum.cm:
                    switch (currentType)
                    {
                        case UnitEnum.mm:
                            result = 0.1;
                            break;
                        case UnitEnum.cm:
                            result = 1;
                            break;
                        case UnitEnum.m:
                            result = 100;
                            break;
                        case UnitEnum.ft:
                            result = 1;
                            break;
                    }
                    break;
                case UnitEnum.m:
                    switch (currentType)
                    {
                        case UnitEnum.mm:
                            result = 0.001;
                            break;
                        case UnitEnum.cm:
                            result = 0.01;
                            break;
                        case UnitEnum.m:
                            result = 1;
                            break;
                        case UnitEnum.ft:
                            result = 1;
                            break;
                    }
                    break;
                case UnitEnum.ft:
                    switch (currentType)
                    {
                        case UnitEnum.mm:
                            result = 1;
                            break;
                        case UnitEnum.cm:
                            result = 1;
                            break;
                        case UnitEnum.m:
                            result = 1;
                            break;
                        case UnitEnum.ft:
                            result = 1;
                            break;
                    }
                    break;
            }
            return result;
        }
    }
    public enum UnitEnum
    {
        mm = 0,
        cm = 1,
        m = 2,
        ft = 3,
    }
}
