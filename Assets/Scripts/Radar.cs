using UnityEngine;

public class Radar : MonoBehaviour
{
    public Transform hedef;
    public float yarimAci = 35f;
    public float menzil = 120f;
    public bool Goruyor;
    public float YanTaraf;

    private void Update()
    {
        GorusHesapla();
    }

    private void GorusHesapla()
    {
        Goruyor = false;
        YanTaraf = 0f;
        if (hedef == null)
            return;

        Vector3 hedefVektoru = hedef.position - transform.position;
        Vector3 yon = hedefVektoru.normalized;
        float aciEsigi = Mathf.Cos(Mathf.Clamp(yarimAci, 0f, 180f) * Mathf.Deg2Rad);
        Goruyor = hedefVektoru.magnitude <= Mathf.Max(0f, menzil)
            && (hedefVektoru.sqrMagnitude == 0f
                || Vector3.Dot(transform.forward, yon) >= aciEsigi);
        // Pozitif: sag, negatif: sol, sifir: ayni dogrultu.
        YanTaraf = Vector3.Cross(transform.forward, yon).y;
    }

    private void OnDrawGizmos()
    {
        GorusHesapla();
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * Mathf.Max(0f, menzil));
        if (hedef == null)
            return;

        Gizmos.color = Goruyor ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, hedef.position);
        Gizmos.DrawWireSphere(hedef.position, 3f);
    }
}
