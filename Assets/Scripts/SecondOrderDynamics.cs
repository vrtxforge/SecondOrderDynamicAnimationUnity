using UnityEngine;

public class SecondOrderDynamics
{
    private Vector4 xp;
    private Vector4 y, yd;
    private readonly float k1, k2, k3;

    public SecondOrderDynamics(float f, float z, float r, Vector4 x0)
    {
        k1 = z / (Mathf.PI * f);
        k2 = 1 / ((2 * Mathf.PI * f) * (2 * Mathf.PI * f));
        k3 = r * z / (2 * Mathf.PI * f);

        xp = x0;
        y = x0;
        yd = Vector4.zero;
    }

    public Vector4 Update(float T, Vector4 x, bool disableSimulation, Vector4 xd = default)
    {
        if(!disableSimulation)
        {
            if (xd == default)
            {
                xd = (x - xp) / T;
                xp = x;
            }
            float k2_stable = Mathf.Max(k2, 1.1f * (T * T / 4 + T * k1 / 2));
            y += T * yd;
            yd += T * (x + k3 * xd - y - k1 * yd) / k2_stable;
        }
        else
        {
            yd = Vector4.zero;
            y = x;
        }

        return y;
    }
}