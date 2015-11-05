using System;

namespace XamlStyler.Service.Reorder
{
    public sealed class SortableStringAttribute: ISortableAttribute
    {
        public string Value { get; private set; }

        public SortableStringAttribute(string value)
        {
            this.Value = value;
        }

        public int CompareTo(ISortableAttribute other)
        {
            return String.Compare(Value, ((SortableStringAttribute) other).Value, StringComparison.Ordinal);
        }

#if DEBUG
        public override string ToString()
        {
            return Value;
        }
#endif
    }
}