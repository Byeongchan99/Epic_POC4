using UnityEngine;

/// <summary>
/// 마법진을 끊을 때 나타나는 효과
/// </summary>
public class CutEffect : MonoBehaviour
{
    public float lifetime = 1f;
    public int particleCount = 20;
    public float explosionForce = 3f;
    public Color particleColor = new Color(1f, 0.5f, 0f);

    void Start()
    {
        CreateParticles();
        Destroy(gameObject, lifetime);
    }

    void CreateParticles()
    {
        for (int i = 0; i < particleCount; i++)
        {
            GameObject particle = new GameObject("Particle");
            particle.transform.position = transform.position;

            // LineRenderer로 작은 선 입자 생성
            LineRenderer lr = particle.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = particleColor;
            lr.endColor = new Color(particleColor.r, particleColor.g, particleColor.b, 0f);
            lr.startWidth = 0.05f;
            lr.endWidth = 0.02f;
            lr.positionCount = 2;
            lr.useWorldSpace = true;
            lr.sortingOrder = 5;

            // 랜덤 방향으로 날아가기
            Vector2 direction = Random.insideUnitCircle.normalized;
            float speed = Random.Range(explosionForce * 0.5f, explosionForce);

            ParticleMover mover = particle.AddComponent<ParticleMover>();
            mover.velocity = direction * speed;
            mover.lifetime = lifetime;
            mover.lineRenderer = lr;

            particle.transform.parent = transform;
        }
    }
}

/// <summary>
/// 입자 움직임 처리
/// </summary>
public class ParticleMover : MonoBehaviour
{
    public Vector2 velocity;
    public float lifetime;
    public LineRenderer lineRenderer;

    private float elapsed = 0f;
    private Vector2 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / lifetime;

        // 감속
        velocity *= 0.95f;

        // 이동
        transform.position += (Vector3)velocity * Time.deltaTime;

        // LineRenderer로 꼬리 표현
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, startPos);
            lineRenderer.SetPosition(1, transform.position);

            // 페이드 아웃
            Color c = lineRenderer.startColor;
            c.a = 1f - t;
            lineRenderer.startColor = c;
        }

        // 수명 다하면 제거
        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
