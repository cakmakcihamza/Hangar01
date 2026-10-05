using UnityEngine;

public class HedefeGit : MonoBehaviour
{
    public Transform hedef;
    public float hiz = 14f;
    public float donusHizi = 60f;
    public float varisMesafesi = 8f;

    private Quaternion ucusRotasyonu;
    private float kanatYatisi;

    private void OnEnable()
    {
        ucusRotasyonu = Quaternion.LookRotation(transform.forward, Vector3.up);
        kanatYatisi = 0f;
    }

    private void Update()
    {
        if (hedef == null)
            return;

        Vector3 hedefVektoru = hedef.position - transform.position;
        if (hedefVektoru.magnitude <= varisMesafesi)
            return;

        Vector3 yon = hedefVektoru.normalized;
        float donusYonu = Vector3.Cross(transform.forward, yon).y;
        float istenenYatis = -Mathf.Clamp(donusYonu, -1f, 1f) * 45f;
        kanatYatisi = Mathf.MoveTowards(kanatYatisi, istenenYatis,
            donusHizi * Time.deltaTime);

        Quaternion hedefRotasyonu = Quaternion.LookRotation(yon, Vector3.up);
        ucusRotasyonu = Quaternion.RotateTowards(ucusRotasyonu, hedefRotasyonu,
            donusHizi * Time.deltaTime);
        transform.rotation = ucusRotasyonu * Quaternion.Euler(0f, 0f, kanatYatisi);
        transform.position += transform.forward * hiz * Time.deltaTime;
    }
}
