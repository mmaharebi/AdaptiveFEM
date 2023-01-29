namespace AdaptiveFEM.Services
{
    /// <summary>
    /// <field name="_baseIndex">
    /// Index zero is always belonged to Domain component
    /// </field>
    /// </summary>
    public static class ComponentLayerIndexGenerator
    {
        private static int _baseIndex = 0;

        public static int NewLayerIndex()
        {
            return _baseIndex++;
        }
    }
}
