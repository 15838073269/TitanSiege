namespace Net.AI
{
    public class RenderTriangle
    {
        public Vector3[] m_Verts = new Vector3[3];
        public Color[] m_Colors = new Color[3] { Color.white, Color.white, Color.white };

        public RenderTriangle(Vector3 a, Vector3 b, Vector3 c, Color color)
        {
            m_Verts[0] = a;
            m_Verts[1] = b;
            m_Verts[2] = c;
            for (int i = 0; i < m_Colors.Length; ++i)
            {
                m_Colors[i] = color;
            }
        }
    }
}