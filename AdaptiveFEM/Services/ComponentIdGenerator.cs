namespace AdaptiveFEM.Services
{
    public static class ComponentIdGenerator
    {
        private static int _idBase = 96103722;

        public static int NewId()
        {
            return _idBase++;
        }
    }
}
